using FluentValidation;
using WPFUI.Models.Input;

namespace WPFUI.Models.Validators
{
    public class AccessInputValidator : AbstractValidator<AccessInput>
    {
        public AccessInputValidator()
        {
            RuleFor(x => x.Password).NotEmpty();

            RuleFor(x => x.ProxyHost).NotEmpty().When(x => x.ProxyPort != 0);
            RuleFor(x => x.ProxyPort).NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.ProxyHost));
            RuleFor(x => x.ProxyUsername).NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.ProxyPassword));
            RuleFor(x => x.ProxyPassword).NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.ProxyUsername));
        }
    }
}