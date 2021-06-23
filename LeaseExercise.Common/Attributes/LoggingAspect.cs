using Newtonsoft.Json;
using PostSharp.Aspects;
using PostSharp.Serialization;
using Serilog;
using System.Linq;
using System.Reflection;

namespace LeaseExercise.Common.Attributes
{
    [PSerializable]
    public class LoggingAspect : OnMethodBoundaryAspect
    {
        private string methodName;

        public LoggingAspect()
        {
            SemanticallyAdvisedMethodKinds = SemanticallyAdvisedMethodKinds.None;
        }

        public override bool CompileTimeValidate(MethodBase method)
        {
            // Don't apply the aspect to constructors, property getters, and so on.
            return !method.IsSpecialName;
        }

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            if (method.DeclaringType != null) methodName = method.DeclaringType.Name + "." + method.Name;
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            var methodParameters = args.Method.GetParameters();
            var argumentsArr = methodParameters.Select((a, i) => new
            {
                name = a.Name,
                type = a.ParameterType.Name,
                value = args.Arguments.GetArgument(i)
            });
            var dictionary = argumentsArr.ToDictionary(argument => args.Method.Name + "." + argument.name,
                argument => new { argument.type, argument.value });
            var parametersJson = JsonConvert.SerializeObject(dictionary);
            var parameters = JsonConvert.DeserializeObject<dynamic>(parametersJson);
            if (methodParameters.Any())
            {
                Log.Information("The {MethodName} method has been started  with {@Parameters}", methodName, parameters);
            }
            else
            {
                Log.Information("The {MethodName} method has been started.", methodName);
            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            dynamic returnObject = null;
            var returnValue = args.ReturnValue;
            if (returnValue != null)
            {
                var returnType = args.ReturnValue.GetType().Name;
                returnObject = new { type = returnType, value = returnValue };
            }

            Log.ForContext(methodName + ".returnValue", returnObject, true)
                .Information("The {MethodName} method executed successfully ", methodName);
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Log.ForContext("exceptionSource", args.Exception.Source)
            .ForContext("exceptionStackTrace", args.Exception.StackTrace)
            .Error("An exception was thrown in {MethodName}, with message {ExceptionMessage}", methodName, args.Exception.Message);
        }
    }
}
