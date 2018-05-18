namespace BizyBoard.Models.ViewModels
{
    using FluentValidation.Attributes;

    [Validator(typeof(ResetPwdUpdateViewModel))]
    public class ResetPwdUpdateViewModel
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string PasswordConfirmation { get; set; }
        public string Token { get; set; }
    }
}