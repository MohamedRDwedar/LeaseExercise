using System.Collections.Generic;

namespace LeaseExercise.Common.Models
{
    public abstract class FailureEvent 
    {
        internal FailureEvent()
        {

        }

        protected FailureEvent(string eventName, List<string> errors, long userId)
        {
            EventName = eventName;
            Errors = new List<string>();
            AddErrors(errors);
        }

        protected FailureEvent(string eventName, string error, long userId)
        {
            EventName = eventName;
            Errors = new List<string>();
            AddError(error);
        }

        private void AddError(string error)
        {
            Errors.Add(error);
        }

        private void AddErrors(List<string> errors)
        {
            Errors.AddRange(errors);
        }

        private string EventName { get; set; }

        private List<string> Errors { get; set; }
    }
}
