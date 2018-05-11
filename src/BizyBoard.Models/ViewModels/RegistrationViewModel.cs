namespace BizyBoard.Models.ViewModels
{
    using FluentValidation.Attributes;
    using Validations;

    [Validator(typeof(RegistrationViewModelValidator))]
    public class RegistrationViewModel
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string WinBizUsername { get; set; }

        public string WinBizPassword { get; set; }

        public string WinBizCompanyName { get; set; }

        public string Company { get; set; }
    }
}