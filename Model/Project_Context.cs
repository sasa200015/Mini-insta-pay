using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Service1.Model
{
    public class Project_Context : IdentityDbContext<Users>
    {
       public Project_Context(DbContextOptions<Project_Context> options) : base(options)
        {

        }
        public DbSet<Users> Users { get; set; }
    }
}
