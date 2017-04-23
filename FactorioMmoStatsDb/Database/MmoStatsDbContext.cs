using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace FactorioMmoStatsDb.Database
{
	public class MmoStatsDbContext : DbContext
	{
		public MmoStatsDbContext(DbContextOptions options)
			: base(options) { }

		public DbSet<Game> Games { get; set; }
		public DbSet<Session> Sessions { get; set; }
		public DbSet<Player> Players { get; set; }
		public DbSet<Death> Deaths { get; set; }
		public DbSet<Statistic> Statistics { get; set; }
	}
}
