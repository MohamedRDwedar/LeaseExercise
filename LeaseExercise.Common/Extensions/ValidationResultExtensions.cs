using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace LeaseExercise.Common.Extensions
{
    public static class ValidationResultExtensions
    {
        public static List<string> GetErrors(this ValidationResult validationResult)
        {
            return validationResult.Errors.Select(c => $"{c.PropertyName} : {c.ErrorMessage}").ToList();
        }
    }
}
