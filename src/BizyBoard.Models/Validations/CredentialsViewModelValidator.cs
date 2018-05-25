namespace BizyBoard.Models.Validations
{
    using FluentValidation;
    using ViewModels;

    public class CredentialsViewModelValidator : AbstractValidator<CredentialsViewModel>
    {
        public CredentialsViewModelValidator()
        {
            RuleFor(vm => vm.Email).NotEmpty().WithMessage("L'email ne peut être vide.");
            RuleFor(vm => vm.Email).EmailAddress().WithMessage("L'email n'est pas au bon format.");
            RuleFor(vm => vm.Password).MinimumLength(8).WithMessage("Password must exceed 8 characters");
        }
    }
}