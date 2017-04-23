using System;

namespace FactorioMmoStatsDb.ViewModels
{
	public class DeathViewModel
	{
		public int GameId { get; set; }
		public int SessionId { get; set; }
		public int Tick { get; set; }
		public DateTime Timestamp { get; set; }
		public string Player { get; set; }
		public string Cause { get; set; }
	}
}