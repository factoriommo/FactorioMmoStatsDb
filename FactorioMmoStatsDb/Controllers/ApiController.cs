using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FactorioMmoStatsDb.Database;
using FactorioMmoStatsDb.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.ApplicationInsights;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FactorioMmoStatsDb.Controllers
{
	[Route("api")]
	public class ApiController : Controller
	{
		private MmoStatsDbContext _context;

		public ApiController(MmoStatsDbContext context)
		{
			this._context = context;
		}

		[Route("game/new")]
		[HttpPost]
		public async Task<GameViewModel> NewGame([FromBody]NewGameViewModel newGame)
		{
			var game = new Game { Name = string.IsNullOrWhiteSpace(newGame.Name) ? null : newGame.Name, IsPatreon = newGame.IsPatreon, };
			_context.Games.Add(game);
			var session = new Session { Game = game, StartTime = DateTime.UtcNow, };
			_context.Sessions.Add(session);
			await _context.SaveChangesAsync();
			return new GameViewModel() { GameId = game.GameId, SessionId = session.SessionId, };
		}

		[Route("game/{gameId:int}/new_session")]
		[HttpPost]
		public async Task<GameViewModel> NewSession(int gameId)
		{
			var session = new Session { GameId = gameId, StartTime = DateTime.UtcNow, };
			_context.Sessions.Add(session);
			await _context.SaveChangesAsync();
			return new GameViewModel() { GameId = gameId, SessionId = session.SessionId, };
		}

		[Route("session/{sessionId:int}/end")]
		[HttpPost]
		public async Task<SuccessViewModel> EndSession(int sessionId)
		{
			var session = _context.Sessions.Find(sessionId);
			session.EndTime = DateTime.UtcNow;
			await _context.SaveChangesAsync();
			return new SuccessViewModel { Success = true, };
		}

		[Route("game/{gameId:int}/end")]
		[HttpPost]
		public async Task<SuccessViewModel> EndGame(int sessionId)
		{
			return new SuccessViewModel { Success = true, };
		}

		[Route("session/{sessionId:int}/death")]
		[HttpPut]
		public async Task<SuccessViewModel> SaveDeath(int sessionId, [FromQuery]DateTime timestamp, [FromBody]DeathViewModel deathModel)
		{
			try
			{
				var session = await GetSession(sessionId);
				var player = await GetPlayer(deathModel.Player, session);

				var death = new Death
				{
					Game = session.Game,
					Session = session,
					Tick = deathModel.Tick,
					Timestamp = timestamp,
					Player = player,
					Cause = deathModel.Cause,
				};
				_context.Deaths.Add(death);
				await _context.SaveChangesAsync();

				return new SuccessViewModel { Success = true, };

			}
			catch (Exception ex)
			{
				new TelemetryClient().TrackException(ex);
				Response.StatusCode = (int)HttpStatusCode.BadRequest;
				return new SuccessViewModel { Success = false, };
			}
		}

		[Route("session/{sessionId:int}/data")]
		[HttpPut]
		public async Task<SuccessViewModel> SaveData(int sessionId, [FromQuery]DateTime timestamp, [FromBody]StatisticsViewModel statsModel)
		{
			try
			{
				var session = await GetSession(sessionId);

				SaveGameStats(timestamp, statsModel, session);
				await SaveEntityStats(timestamp, statsModel, session);

				await _context.SaveChangesAsync();
				return new SuccessViewModel { Success = true, };

			}
			catch (Exception ex)
			{
				new TelemetryClient().TrackException(ex);
				Response.StatusCode = (int)HttpStatusCode.BadRequest;
				return new SuccessViewModel { Success = false, };
			}
		}

		private void SaveGameStats(DateTime timestamp, StatisticsViewModel statsModel, Session session)
		{
			{
				var dataPoint = new Statistic
				{
					Game = session.Game,
					Session = session,
					Tick = statsModel.Tick,
					Timestamp = timestamp,
					StatisticType = StatisticType.Game,
					EntityName = "speed",
					Value = statsModel.Speed,
				};
				_context.Statistics.Add(dataPoint);
			}

			{
				var dataPoint = new Statistic
				{
					Game = session.Game,
					Session = session,
					Tick = statsModel.Tick,
					Timestamp = timestamp,
					StatisticType = StatisticType.Game,
					EntityName = "players-online",
					Value = statsModel.PlayersOnline,
				};
				_context.Statistics.Add(dataPoint);
			}
		}

		private async Task SaveEntityStats(DateTime timestamp, StatisticsViewModel statsModel, Session session)
		{
			var allStats = statsModel.EntitiesProduced.Select(p => new { Type = StatisticType.Produced, Stat = p, })
				.Concat(statsModel.EntitiesKilled.Select(p => new { Type = StatisticType.Killed, Stat = p }))
				.Concat(statsModel.EntitiesPlaced.Select(p => new { Type = StatisticType.Placed, Stat = p }));

			foreach (var stat in allStats)
			{
				var dataPoint = new Statistic
				{
					Game = session.Game,
					Session = session,
					Tick = statsModel.Tick,
					Timestamp = timestamp,
					StatisticType = stat.Type,
					EntityName = stat.Stat.EntityName,
					Value = stat.Stat.Value,
				};
				if (!string.IsNullOrWhiteSpace(stat.Stat.Player))
					dataPoint.Player = await GetPlayer(stat.Stat.Player, session);

				_context.Statistics.Add(dataPoint);
			}
		}

		private async Task<Session> GetSession(int sessionId) =>
			await _context.Sessions
				.Include(s => s.Game)
				.SingleAsync(s => s.SessionId == sessionId);

		private async Task<Player> GetPlayer(string playerName, Session session)
		{
			var player = await _context.Players.SingleOrDefaultAsync(p => p.Name == playerName);
			if (player == null)
			{
				player = new Player { Name = playerName, };
				_context.Players.Add(player);
			}
			player.IsPatreon = player.IsPatreon || session.Game.IsPatreon;
			return player;
		}
	}
}
