using FluentValidation;
using WPFUI.Models.Input;

namespace WPFUI.Models.Validators
{
    public class NormalBuildInputValidator : AbstractValidator<NormalBuildInput>
    {
        public NormalBuildInputValidator()
        {
            RuleFor(x => x.Level)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}