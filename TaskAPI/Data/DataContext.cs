using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TaskAPI.Models;
using Task = TaskAPI.Models.Task;

namespace TaskAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(e => e.Tasks)
                .WithOne(e => e.Assignee)
                .HasForeignKey(e => e.UserId);
        }


    }
}
