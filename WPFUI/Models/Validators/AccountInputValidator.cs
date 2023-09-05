using FluentValidation;
using WPFUI.Models.Input;

namespace WPFUI.Models.Validators
{
    public class AccountInputValidator : AbstractValidator<AccountInput>
    {
        public AccountInputValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Server).NotEmpty();
            RuleFor(x => x.Accesses).NotEmpty();
        }
    }
}