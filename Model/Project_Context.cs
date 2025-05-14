using Microsoft.EntityFrameworkCore;

namespace Service2.Model
{
    public class Project_Context:DbContext
    {
       public Project_Context(DbContextOptions<Project_Context> options) : base(options)
        {
        }
        public DbSet<Transactions> Transactions { get; set; }
    }
}
