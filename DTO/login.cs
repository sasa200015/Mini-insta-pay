using System.ComponentModel.DataAnnotations;

namespace Service1.DTO
{
    public class login
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
