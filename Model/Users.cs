using Microsoft.AspNetCore.Identity;

namespace Service1.Model
{
    public class Users:IdentityUser
    {
        public decimal balance { get; set; }
        public DateTime createdAt { get; set; }   
    }
}
