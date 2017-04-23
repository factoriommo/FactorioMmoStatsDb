using System;

namespace FactorioMmoStatsDb.Database
{
	public class Session
	{
		public int SessionId { get; set; }
		public int GameId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; internal set; }

		public virtual Game Game { get; set; }
	}
}