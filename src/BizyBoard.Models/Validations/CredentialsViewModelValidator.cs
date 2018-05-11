namespace BizyBoard.Models.Validations
{
    using FluentValidation;
    using FluentValidation.Attributes;
    using ViewModels;

    [Validator(typeof(CredentialsViewModelValidator))]
    public class CredentialsViewModelValidator : AbstractValidator<CredentialsViewModel>
    {
        public CredentialsViewModelValidator()
        {
            RuleFor(vm => vm.Email).EmailAddress().WithMessage("Email address is not valid");
            RuleFor(vm => vm.Email).NotEmpty().WithMessage("Email cannot be empty");
            RuleFor(vm => vm.Password).Length(8).WithMessage("Password must exceed  8 characters");
        }
    }
}
