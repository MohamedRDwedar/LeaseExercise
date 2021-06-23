namespace LeaseExercise.Domain.Models
{
    public class MonthlyPrice
    {
        public MonthlyPrice(int leasePeriod, int price)
        {
            LeasePeriod = leasePeriod;
            Price = price;
        }

        public int LeasePeriod { get; }

        public decimal Price { get; }
    }
}
