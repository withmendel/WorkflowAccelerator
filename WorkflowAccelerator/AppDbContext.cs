using Microsoft.EntityFrameworkCore;

namespace WorkflowAccelerator
{
    // 1. Define the data model
    public class ProcessedRecord
    {
        public int Id { get; set; }
        public string OriginalData { get; set; }
        public string ProcessedResult { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

    // 2. Define the Database Context
    public class AppDbContext : DbContext
    {
        public DbSet<ProcessedRecord> Records { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Connect to PostgreSQL
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=WorkflowDB;Username=postgres;Password=admin");
        }
    }
}using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowAccelerator
{
    internal class AppDbContext
    {
    }
}
