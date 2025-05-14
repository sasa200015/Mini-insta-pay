using System.ComponentModel.DataAnnotations;

namespace Service1.DTO
{
    public class register
    {
        [Required]
        public string User_Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
         public string Password { get; set; }
        [Required]
        public string Phone_Number { get; set; }
    }
}
