namespace BizyBoard.Models.Validations
{
    using FluentValidation;
    using ViewModels;

    public class ResetPwdViewModelValidator : AbstractValidator<ResetPwdViewModel>
    {
        public ResetPwdViewModelValidator()
        {
            RuleFor(vm => vm.Email).NotEmpty().WithMessage("L'email ne peut être vide.");
            RuleFor(vm => vm.Email).EmailAddress().WithMessage("L'email n'est pas au bon format.");
        }
    }

    public class ResetPwdUpdateViewModelValidator : AbstractValidator<ResetPwdUpdateViewModel>
    {
        public ResetPwdUpdateViewModelValidator()
        {
            RuleFor(vm => vm.Token).NotEmpty().WithMessage("Le token ne peut être vide.");
            RuleFor(vm => vm.Email).NotEmpty().WithMessage("L'email ne peut être vide.");
            RuleFor(vm => vm.Email).EmailAddress().WithMessage("L'email n'est pas au bon format.");
            RuleFor(vm => vm.NewPassword).NotEmpty().WithMessage("Le mot de passe ne peut être vide.");
            RuleFor(vm => vm.NewPassword).MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères.");
            RuleFor(vm => vm.NewPassword).Equal(d => d.PasswordConfirmation).WithMessage("Les deux mot de passes doivent être identiques.");
        }
    }
}