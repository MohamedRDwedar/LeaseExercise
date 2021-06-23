namespace LeaseExercise.Services.ViewModels
{
   public class OfferInformation
    {
        public OfferInformation(string offerId, string vehicle, decimal price)
        {
            OfferId = offerId;
            Vehicle = vehicle;
            Price = price;
        }

        public string OfferId { get; }

        public string Vehicle { get; }

        public decimal Price { get; }
    }
}
