using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace LeaseExercise.Common.Extensions
{
    public static partial class ExceptionExtensions
    {

        /// <summary>
        /// Returns a list of all the exception messages from the top-level
        /// exception down through all the inner exceptions. Useful for making
        /// logs and error pages easier to read when dealing with exceptions.
        /// Usage: Exception.Messages()
        /// </summary>
        private static IEnumerable<string> Messages(this Exception exception)
        {
            // return an empty sequence if the provided exception is null
            if (exception == null) { yield break; }
            // first return THIS exception's message at the beginning of the list
            yield return exception.Message;
            // then get all the lower-level exception messages recursively (if any)
            IEnumerable<Exception> innerExceptions = Enumerable.Empty<Exception>();

            if (exception is AggregateException aggregateException && aggregateException.InnerExceptions.Any())
            {
                innerExceptions = aggregateException.InnerExceptions;
            }
            else if (exception.InnerException != null)
            {
                innerExceptions = new Exception[] { exception.InnerException };
            }

            foreach (var innerException in innerExceptions)
            {
                foreach (string message in innerException.Messages())
                {
                    yield return message;
                }
            }
        }

        public static string Message(this Exception exception)
        {
            if (exception != null)
            {
                if (exception.InnerException != null)
                {
                    return exception.InnerException.Message;
                }
                else
                {
                    return exception.Message;
                }
            }
            return string.Empty;
        }

        public static Result<T> ToFailure<T>(this Exception exception)
        {
            return Result.Failure<T>(string.Join(",", exception.Messages()));
        }

        public static Result ToFailure(this Exception exception)
        {
            return Result.Failure(string.Join(",", exception.Messages()));
        }
    }
}
