using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace LeaseExercise.Common.Interfaces
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task<Result> HandleAsync(TEvent @event);
    }
}
