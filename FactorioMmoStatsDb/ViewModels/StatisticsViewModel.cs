using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace FactorioMmoStatsDb.ViewModels
{
	[DataContract]
	public class StatisticsViewModel
	{
		[DataMember]
		public int Tick { get; set; }
		[DataMember]
		public int Speed { get; set; }
		[DataMember]
		public int PlayersOnline { get; set; }

		[DataMember(Name = "entities_produced")]
		public IList<DataPointViewModel> EntitiesProduced { get; set; }
		[DataMember(Name = "entities_killed")]
		public IList<DataPointViewModel> EntitiesKilled { get; set; }
		[DataMember(Name = "entities_placed")]
		public IList<DataPointViewModel> EntitiesPlaced { get; set; }
	}
}
