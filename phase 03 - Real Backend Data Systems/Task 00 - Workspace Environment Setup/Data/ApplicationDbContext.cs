using Microsoft.EntityFrameworkCore;
using Task_00___Workspace_Environment_Setup.Entities;

namespace Task_00___Workspace_Environment_Setup.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
           
        }
        DbSet<Student> Students { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
