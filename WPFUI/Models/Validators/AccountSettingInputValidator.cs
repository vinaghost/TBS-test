﻿using FluentValidation;
using WPFUI.Models.Input;

namespace WPFUI.Models.Validators
{
    public class AccountSettingInputValidator : AbstractValidator<AccountSettingInput>
    {
        public AccountSettingInputValidator()
        {
            RuleFor(x => x.ClickDelay.Min)
                .LessThanOrEqualTo(x => x.ClickDelay.Max)
                .WithMessage("Minimum click delay ({PropertyValue}) should be less than maximum click delay ({ComparisonValue})");
            RuleFor(x => x.ClickDelay.Min)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum click delay ({PropertyValue}) should be positive number");
            RuleFor(x => x.TaskDelay.Min)
                .LessThanOrEqualTo(x => x.TaskDelay.Max)
                .WithMessage("Minimum task delay ({PropertyValue}) should be less than maximum task delay ({ComparisonValue})");
            RuleFor(x => x.TaskDelay.Min)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum task delay ({PropertyValue}) should be positive number");
        }
    }
}