namespace BizyBoard.Models.ViewModels
{
    using FluentValidation.Attributes;
    using Validations;

    [Validator(typeof(ResetPwdViewModel))]
    public class ResetPwdViewModel
    {
        public string Email { get; set; }
    }
}