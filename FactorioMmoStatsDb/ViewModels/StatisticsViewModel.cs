using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FactorioMmoStatsDb.ViewModels
{
	public class StatisticsViewModel
	{
		public int GameId { get; set; }
		public int SessionId { get; set; }
		public int Tick { get; set; }
		public DateTime Timestamp { get; set; }

		public IList<DataPointViewModel> Statistics { get; set; }
	}
}
