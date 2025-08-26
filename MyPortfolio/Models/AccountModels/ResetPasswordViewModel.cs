namespace MyPortfolio.Models.AccountModels
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; } = null!;
        [DataType(DataType.Password), Compare(nameof(Password))]
        [Required(ErrorMessage = "Confirmed Password Is Required")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
