using System.Collections.Generic;
using FluentValidation;
using LeaseExercise.Services.Queries;

namespace LeaseExercise.Services.Validators
{
    class UserOfferQueryValidator : AbstractValidator<GetUserOfferQuery>
    {
        public UserOfferQueryValidator()
        {
            When(x => x != null, () =>
            {
                var leasePeriods = new List<int>{ 1, 3, 5 };
                RuleFor(c => c.MaxNumberOfPeople).GreaterThan(0);
                RuleFor(c => c.MinMonthlyIncome).GreaterThanOrEqualTo(2000);
                RuleFor(c => c.LeasePeriod).Must(x => leasePeriods.Contains(x));
                RuleFor(c => c.Email).EmailAddress();
            });
        }
    }
}

