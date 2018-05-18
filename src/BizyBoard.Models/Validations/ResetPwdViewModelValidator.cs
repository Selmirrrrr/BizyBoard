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
}