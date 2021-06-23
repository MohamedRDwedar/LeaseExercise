using System;
using System.Collections.Generic;
using System.Linq;
using LeaseExercise.Domain.Enums;

namespace LeaseExercise.Domain.Models
{
    public class Vehicle
    {
        public Vehicle(VehicleTypeEnum vehicleType, int maxNumberOfPeople, decimal minMonthlyIncome, List<MonthlyPrice> monthlyPrices)
        {
            VehicleType = vehicleType;
            MaxNumberOfPeople = maxNumberOfPeople;
            MinMonthlyIncome = minMonthlyIncome;
            MonthlyPrices = monthlyPrices;
        }

        public VehicleTypeEnum VehicleType { get; }

        public int MaxNumberOfPeople { get; }

        public decimal MinMonthlyIncome { get; }

        public List<MonthlyPrice> MonthlyPrices { get; }

        public string GetFriendlyName()
        {
            return Enum.GetName(typeof(VehicleTypeEnum), VehicleType);
        }

        public decimal GetMonthlyPrice()
        {
            var price = MonthlyPrices?.FirstOrDefault()?.Price;
            return price.GetValueOrDefault();
        }
    }
}
