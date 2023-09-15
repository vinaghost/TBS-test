using FluentValidation;
using WPFUI.Models.Input;

namespace WPFUI.Models.Validators
{
    public class ResourceBuildInputValidator : AbstractValidator<ResourceBuildInput>
    {
        public ResourceBuildInputValidator()
        {
            RuleFor(x => x.Level)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}