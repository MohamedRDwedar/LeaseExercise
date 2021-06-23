using CSharpFunctionalExtensions;
using LeaseExercise.Common.Models;
using System.Threading.Tasks;

namespace LeaseExercise.Common.Interfaces
{
    public interface IMessageDispatcher
    {
        Task<Result<T>> DispatchAsync<T>(IQuery<T> query);

        Task<Result> RaiseFailureAsync(FailureEvent @failureEvent);
    }
}
