using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace LeaseExercise.Domain.Interfaces
{
    public interface IOfferGenerator
    {
        /// <summary>
        /// Send the new offer to the external system and return back with the user offer id
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="price"></param>
        /// <param name="email"></param>
        Task<Result<string>> GenerateNewOffer(string vehicle, decimal price, string email);
    }
}
