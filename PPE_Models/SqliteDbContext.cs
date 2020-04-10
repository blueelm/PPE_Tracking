using Microsoft.EntityFrameworkCore;

namespace PPE_Models
{
    public class SQLiteDBContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<PPE_Models.Stock> Stocks { get; set; }
        public DbSet<PPE_Models.StockHistory> StockHistory { get; set; }
        public DbSet<PPE_Models.TrackedStock> TrackedStocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=ppe_tracking.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // modelBuilder.Entity<PPE_Models.Stock>().ToTable("Stocks");
            modelBuilder.Entity<PPE_Models.StockHistory>().HasKey(c => new { c.StockID, c.DateTime });
        }
    }
}