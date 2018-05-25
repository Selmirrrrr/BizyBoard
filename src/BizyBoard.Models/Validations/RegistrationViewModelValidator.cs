namespace BizyBoard.Models.Validations
{
    using FluentValidation;
    using ViewModels;

    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {
        public RegistrationViewModelValidator()
        {
            RuleFor(vm => vm.Email).NotEmpty().WithMessage("L'email ne peut être vide.");
            RuleFor(vm => vm.Email).EmailAddress().WithMessage("L'email n'est pas au bon format.");
            RuleFor(vm => vm.Password).NotEmpty().WithMessage("Le mot de passe ne peut être vide.");
            RuleFor(vm => vm.Password).MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères.");
            RuleFor(vm => vm.Company).NotEmpty().WithMessage("L'entreprise ne peut être vide.");
            RuleFor(vm => vm.WinBizPassword).NotEmpty().WithMessage("Le mot de passe WinBIZ ne peut être vide.");
            RuleFor(vm => vm.WinBizUsername).NotEmpty().WithMessage("Le not d'utilisateur WinBIZ ne peut être vide.");
        }
    }
}