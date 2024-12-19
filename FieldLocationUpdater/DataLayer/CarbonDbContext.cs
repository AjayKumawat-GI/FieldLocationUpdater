using FieldLocationUpdater.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace FieldLocationUpdater.DataLayer
{
    public class CarbonDbContext : DbContext
    {
        public CarbonDbContext(DbContextOptions<CarbonDbContext> options) : base(options)
        {
        }

        public DbSet<FieldDetail> FieldDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FieldDetail>().ToTable("Farm_FarmerDataTagging");
            base.OnModelCreating(modelBuilder);
        }
    }
}
