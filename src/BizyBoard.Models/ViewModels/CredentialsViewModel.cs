namespace BizyBoard.Models.ViewModels
{
    using FluentValidation.Attributes;
    using Validations;

    [Validator(typeof(CredentialsViewModelValidator))]
    public class CredentialsViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}