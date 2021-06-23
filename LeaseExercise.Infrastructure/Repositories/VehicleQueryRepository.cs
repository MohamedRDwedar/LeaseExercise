using LeaseExercise.Domain.Enums;
using LeaseExercise.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using LeaseExercise.Common.Attributes;
using LeaseExercise.Domain.Interfaces;

namespace LeaseExercise.Infrastructure.Repositories
{
    public class VehicleQueryRepository : IVehicleQueryRepository
    {
        /// <summary>
        /// static list for the vehicles in real world example will get this list from database 
        /// </summary>
        /// <returns></returns>
        [LoggingAspect]
        private static IEnumerable<Vehicle> GetVehicles()
        {
            var vehicles = new List<Vehicle>
            {
                new Vehicle(VehicleTypeEnum.Motorcycle, 2, 2000,
                    new List<MonthlyPrice>
                    {
                        new MonthlyPrice(1, 200),
                        new MonthlyPrice(2, 180),
                        new MonthlyPrice(3, 160)
                    }),
                new Vehicle(VehicleTypeEnum.Car, 5, 4000,
                    new List<MonthlyPrice>
                    {
                        new MonthlyPrice(1, 500),
                        new MonthlyPrice(2, 480),
                        new MonthlyPrice(3, 460)
                    }),
                new Vehicle(VehicleTypeEnum.Limousine, 18, 7000,
                    new List<MonthlyPrice>
                    {
                        new MonthlyPrice(1, 900),
                        new MonthlyPrice(2, 880),
                        new MonthlyPrice(3, 860)
                    })
            };
            return vehicles;
        }

        /// <summary>
        /// Get the suitable vehicle based on the user input for the method parameters 
        /// </summary>
        /// <param name="maxNumberOfPeople"></param>
        /// <param name="minMonthlyIncome"></param>
        /// <param name="leasePeriod"></param>
        /// <returns>Result of Vehicle</returns>
        [LoggingAspect]
        public Result<Vehicle> GetVehicle(int maxNumberOfPeople, decimal minMonthlyIncome, int leasePeriod)
        {
            if (maxNumberOfPeople != default && minMonthlyIncome != default && leasePeriod != default)
            {
                var vehicle = GetVehicles()
                    .Where(c => maxNumberOfPeople <= c.MaxNumberOfPeople && minMonthlyIncome >= c.MinMonthlyIncome
                                                                          && c.MonthlyPrices.Any(a => a.LeasePeriod == leasePeriod))
                    .Select(m => new Vehicle(m.VehicleType, m.MaxNumberOfPeople, m.MinMonthlyIncome,
                        m.MonthlyPrices.Where(r => r.LeasePeriod == leasePeriod).ToList()))
                    .OrderBy(n => n.MaxNumberOfPeople).ThenBy(c => c.MinMonthlyIncome)
                    .FirstOrDefault();
                return vehicle != null ? Result.Success(vehicle) : Result.Failure<Vehicle>("did not find suitable vehicle");
            }
            return Result.Failure<Vehicle>("Missing correct parameters");
        }
    }
}
