using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace LeaseExercise.Common.Interfaces
{
    public interface IQuery<TResult>
    {
    }

    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<Result<TResult>> HandleAsync(TQuery query);
    }
}
