using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FactorioMmoStatsDb.Database;
using FactorioMmoStatsDb.ViewModels;
using Microsoft.EntityFrameworkCore;

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
		public async Task<SuccessViewModel> SaveDeath([FromBody]DeathViewModel deathModel)
		{
			var session = await GetSession(deathModel.SessionId);
			var player = await GetPlayer(deathModel.Player, session);

			var death = new Death
			{
				Game = session.Game,
				Session = session,
				Tick = deathModel.Tick,
				Timestamp = deathModel.Timestamp,
				Player = player,
				Cause = deathModel.Cause,
			};
			_context.Deaths.Add(death);
			await _context.SaveChangesAsync();

			return new SuccessViewModel { Success = true, };
		}

		[Route("session/{sessionId:int}/data")]
		[HttpPut]
		public async Task<SuccessViewModel> SaveData([FromBody]StatisticsViewModel statsModel)
		{
			try
			{
				var session = await GetSession(statsModel.SessionId);

				foreach (var stat in statsModel.Statistics)
				{
					var dataPoint = new Statistic
					{
						Game = session.Game,
						Session = session,
						Tick = statsModel.Tick,
						Timestamp = statsModel.Timestamp,
						Type = stat.Type,
						Value = stat.Value,
					};
					if (!string.IsNullOrWhiteSpace(stat.Player))
						dataPoint.Player = await GetPlayer(stat.Player, session);

					_context.Statistics.Add(dataPoint);
				}

				await _context.SaveChangesAsync();
				return new SuccessViewModel { Success = true, };

			}
			catch (Exception ex)
			{

				throw;
			}		}

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
