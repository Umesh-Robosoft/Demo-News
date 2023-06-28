using DataEntities;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    /// <summary>
    /// Application db context class 
    /// </summary>
    public class NewsDbContext : DbContext
    {
        /// <summary>
        /// constructor 
        /// </summary>
        public NewsDbContext()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="options"></param>
        public NewsDbContext(DbContextOptions<NewsDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// virtual property for map sql table to entity framwork entity class
        /// </summary>
        public virtual DbSet<News> News { get; set; }

        /// <summary>
        /// method on onconfiguring
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseCosmos(
                    "https://news-db.documents.azure.com:443/",
                    "A6lchasI3ny5FWKJiAdM4CZc10SI2QE91TnMPMLRUP7ffThdzRWCwflWoAY7ISygyiZV8NiOkxwaACDbBVwcSQ==",
                    "NewsDb");
            }
        }

        /// <summary>
        /// method onmodelcreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<News>().ToContainer("News").HasPartitionKey(news => news.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
