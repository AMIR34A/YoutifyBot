using System.ComponentModel.DataAnnotations;

namespace YoutifyBot.Models.ViewModels
{
    public class LogInViewModel
    {
        [Required(ErrorMessage ="Please Enter the Username!")]
        [Display(Name ="Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please Enter the Password!")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
