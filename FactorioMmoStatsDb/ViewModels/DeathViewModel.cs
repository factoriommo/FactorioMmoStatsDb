using System;

namespace FactorioMmoStatsDb.ViewModels
{
	public class DeathViewModel
	{
		public int Tick { get; set; }
		public string Player { get; set; }
		public string Cause { get; set; }
	}
}