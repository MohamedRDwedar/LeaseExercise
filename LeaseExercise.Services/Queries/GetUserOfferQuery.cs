using CSharpFunctionalExtensions;
using LeaseExercise.Common.Extensions;
using LeaseExercise.Common.Interfaces;
using LeaseExercise.Domain.Enums;
using LeaseExercise.Domain.Interfaces;
using LeaseExercise.Services.Validators;
using LeaseExercise.Services.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using LeaseExercise.Common.Attributes;
using Serilog;

namespace LeaseExercise.Services.Queries
{
    public sealed class GetUserOfferQuery : IQuery<OfferInformation>
    {
        public GetUserOfferQuery(int maxNumberOfPeople, decimal minMonthlyIncome, int leasePeriod, string email)
        {
            MaxNumberOfPeople = maxNumberOfPeople;
            MinMonthlyIncome = minMonthlyIncome;
            LeasePeriod = leasePeriod;
            Email = email;
        }

        public int MaxNumberOfPeople { get; }

        public decimal MinMonthlyIncome { get; }

        public int LeasePeriod { get; }

        public string Email { get; }
    }

    public sealed class GetUserOfferQueryHandler : IQueryHandler<GetUserOfferQuery, OfferInformation>
    {
        private readonly IVehicleQueryRepository _vehicleQueryRepository;
        private readonly IOfferGenerator _offerGenerator;

        public GetUserOfferQueryHandler(IVehicleQueryRepository vehicleQueryRepository, IOfferGenerator offerGenerator)
        {
            _vehicleQueryRepository = vehicleQueryRepository;
            _offerGenerator = offerGenerator;
        }

        /// <summary>
        /// combine the business log to get the suitable vehicle and get the user offer id 
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Result of OfferInformation</returns>
        [LoggingAspect]
        public async Task<Result<OfferInformation>> HandleAsync(GetUserOfferQuery query)
        {
            try
            {
                var validator = new UserOfferQueryValidator();
                Log.Information("Start validating the GetUserOfferQuery object");
                var validateResult = await validator.ValidateAsync(query);
                var result = validateResult.IsValid ? Result.Success() : Result.Failure(string.Join(',', validateResult.GetErrors()));

                Log.Information("Start getting suitable vehicle based on the user MaxNumberOfPeople, MinMonthlyIncome and LeasePeriod. ");
                var returnValue = await result
                    .Bind(() =>
                    {
                        var value = _vehicleQueryRepository.GetVehicle(query.MaxNumberOfPeople, query.MinMonthlyIncome,
                                query.LeasePeriod);
                        return value;
                    })
                    .Bind(async vehicle =>
                    {
                        Log.Information("Start getting the user offer id. ");
                        return await _offerGenerator.GenerateNewOffer(vehicle.GetFriendlyName(), vehicle.GetMonthlyPrice(), query.Email)
                            .Bind(r =>
                            {
                                Log.Information("Return OfferInformation view model object to combine both vehicle and offer data. ");
                                return Result.Success(new OfferInformation(r, vehicle.GetFriendlyName(), vehicle.GetMonthlyPrice()));
                            })
                            .OnFailure(error => throw new Exception(error));
                    })
                    .OnFailure(error => throw new Exception(error));
                return returnValue;
            }
            catch (Exception exception)
            {
                return exception.ToFailure<OfferInformation>();
            }
        }
    }
}
