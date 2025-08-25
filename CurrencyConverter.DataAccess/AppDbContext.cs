using CurrencyConverter.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<CurrencyConversion> Conversions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyRate>().HasKey(c => c.CurrencyCode);
            modelBuilder.Entity<CurrencyRate>().Property(c => c.Rate).HasPrecision(18, 6);

            modelBuilder.Entity<CurrencyConversion>().HasKey(c => c.Id);
            modelBuilder.Entity<CurrencyConversion>().Property(c => c.InputAmount).HasPrecision(18, 6);
            modelBuilder.Entity<CurrencyConversion>().Property(c => c.ConvertedAmount).HasPrecision(18, 6);
        }
    }
}
