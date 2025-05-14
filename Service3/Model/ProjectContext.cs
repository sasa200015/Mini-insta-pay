using Microsoft.EntityFrameworkCore;

namespace Service3.Model
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {
        }
        public DbSet<Report> Report { get; set; }

     }

}
