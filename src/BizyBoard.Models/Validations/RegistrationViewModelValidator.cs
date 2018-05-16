namespace BizyBoard.Models.Validations
{
    using FluentValidation;
    using ViewModels;

    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {
        public RegistrationViewModelValidator()
        {
            RuleFor(vm => vm.Email).NotEmpty().WithMessage("Email cannot be empty");
            RuleFor(vm => vm.Email).EmailAddress().WithMessage("Email format is incorrect");
            RuleFor(vm => vm.Password).NotEmpty().WithMessage("Password cannot be empty");
            RuleFor(vm => vm.Password).MinimumLength(8).WithMessage("Password must be at least 8 chars long");
            RuleFor(vm => vm.Company).NotEmpty().WithMessage("Company cannot be empty");
            RuleFor(vm => vm.WinBizPassword).NotEmpty().WithMessage("WinBizPassword cannot be empty");
            RuleFor(vm => vm.WinBizUsername).NotEmpty().WithMessage("WinBizUsername cannot be empty");
        }
    }
}