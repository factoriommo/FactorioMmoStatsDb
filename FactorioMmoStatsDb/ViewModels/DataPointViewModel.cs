using System.Runtime.Serialization;

namespace FactorioMmoStatsDb.ViewModels
{
	[DataContract]
	public class DataPointViewModel
	{
		[DataMember]
		public string Player { get; set; }
		[DataMember(Name = "entity_name")]
		public string EntityName { get; set; }
		[DataMember]
		public decimal Value { get; set; }
	}
}