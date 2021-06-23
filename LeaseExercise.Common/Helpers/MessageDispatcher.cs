using CSharpFunctionalExtensions;
using LeaseExercise.Common.Extensions;
using LeaseExercise.Common.Interfaces;
using LeaseExercise.Common.Models;
using System;
using System.Threading.Tasks;

namespace LeaseExercise.Common.Helpers
{
    public class MessageDispatcher: IMessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        public MessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Result<T>> DispatchAsync<T>(IQuery<T> query)
        {
            try
            {
                var type = typeof(IQueryHandler<,>);
                Type[] typeArgs = { query.GetType(), typeof(T) };
                var handlerType = type.MakeGenericType(typeArgs);

                dynamic handler = _serviceProvider.GetService(handlerType);
                Result<T> result = await handler.HandleAsync((dynamic)query);
                return result;
            }
            catch (Exception exception)
            {
                return exception.ToFailure<T>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="failureEvent"></param>
        /// <returns></returns>
        public async Task<Result> RaiseFailureAsync(FailureEvent failureEvent)
        {
            try
            {
                var type = typeof(IEventHandler<>);
                Type[] typeArgs = { failureEvent.GetType() };
                var handlerType = type.MakeGenericType(typeArgs);

                dynamic handler = _serviceProvider.GetService(handlerType);
                Result result = await handler.HandleAsync((dynamic)failureEvent);
                return result;
            }
            catch (Exception exception)
            {
                return exception.ToFailure();
            }
        }
    }
}
