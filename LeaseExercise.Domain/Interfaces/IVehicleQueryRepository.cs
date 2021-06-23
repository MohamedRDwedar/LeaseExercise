using LeaseExercise.Domain.Models;
using CSharpFunctionalExtensions;

namespace LeaseExercise.Domain.Interfaces
{
    public interface IVehicleQueryRepository
    {
        /// <summary>
        /// Get the suitable vehicle based on the user input for the method parameters 
        /// </summary>
        /// <param name="maxNumberOfPeople"></param>
        /// <param name="minMonthlyIncome"></param>
        /// <param name="leasePeriod"></param>
        /// <returns></returns>
        Result<Vehicle> GetVehicle(int maxNumberOfPeople, decimal minMonthlyIncome, int leasePeriod);
    }
}
