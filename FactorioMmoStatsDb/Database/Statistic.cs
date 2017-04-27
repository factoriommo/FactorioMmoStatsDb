using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FactorioMmoStatsDb.Database
{
	public enum StatisticType : byte
	{
		Produced = 1,
		Killed = 2,
		Placed = 3,
		Game = 4,
	}

	public class Statistic
	{
		public long StatisticId { get; set; }
		public int GameId { get; set; }
		public int SessionId { get; set; }
		public int Tick { get; set; }
		public DateTime Timestamp { get; set; }
		public int? PlayerId { get; set; }
		public byte StatisticTypeId { get; set; }
		public string EntityName { get; set; }
		public decimal Value { get; set; }

		[NotMapped]
		public StatisticType StatisticType
		{
			get => (StatisticType)StatisticTypeId;
			set => StatisticTypeId = (byte)value;
		}
		public Game Game { get; set; }
		public Session Session { get; set; }
		public Player Player { get; set; }
	}
}