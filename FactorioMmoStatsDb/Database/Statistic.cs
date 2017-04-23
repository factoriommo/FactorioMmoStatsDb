using System;

namespace FactorioMmoStatsDb.Database
{
	public class Statistic
	{
		public long StatisticId { get; set; }
		public int GameId { get; set; }
		public int SessionId { get; set; }
		public int Tick { get; set; }
		public DateTime Timestamp { get; set; }
		public int? PlayerId { get; set; }
		public string Type { get; set; }
		public decimal Value { get; set; }

		public Game Game { get; set; }
		public Session Session { get; set; }
		public Player Player { get; set; }
	}
}