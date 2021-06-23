using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using LeaseExercise.Common.Attributes;
using LeaseExercise.Common.Extensions;
using LeaseExercise.Domain.DataModels;
using LeaseExercise.Domain.Interfaces;
using RestSharp;

namespace LeaseExercise.Infrastructure.External
{
    public class OfferGenerator : IOfferGenerator
    {
        private readonly IRestClient _restClient;
        public OfferGenerator(IRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Send the new offer to the external system and return back with the user offer id
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="price"></param>
        /// <param name="email"></param>
        /// <returns>Result of string</returns>
        [LoggingAspect]
        public async Task<Result<string>> GenerateNewOffer(string vehicle, decimal price, string email)
        {
            if (!string.IsNullOrWhiteSpace(vehicle) && price != default && !string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    var request = new RestRequest("offer?vehicle=" + vehicle + "&price=" + price + "&email=" + email, DataFormat.Json);
                    var offerDataModel = await _restClient.GetAsync<OfferDataModel>(request);
                    return Result.Success(offerDataModel.Id);
                }
                catch (Exception exception)
                {
                    return exception.ToFailure<string>();
                }
            }
            return Result.Failure<string>("Missing correct parameters");
        }
    }
}
