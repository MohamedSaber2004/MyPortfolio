namespace MyPortfolio.Models.AccountModels
{
    public class ForgetPasswordViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email Field Is Required")]
        public string Email { get; set; } = null!;
    }
}
