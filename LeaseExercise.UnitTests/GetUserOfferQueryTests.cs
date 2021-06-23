using CSharpFunctionalExtensions;
using LeaseExercise.Domain.Enums;
using LeaseExercise.Domain.Interfaces;
using LeaseExercise.Domain.Models;
using LeaseExercise.Services.Queries;
using Moq;
using NUnit.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaseExercise.UnitTests
{
    [TestFixture]
    public class GetUserOfferQueryTests
    {
        public class Tests
        {
            private Mock<IVehicleQueryRepository> _vehicleQueryRepository;
            private Mock<IOfferGenerator> _offerGenerator;
            private Vehicle _vehicle;
            private GetUserOfferQuery _getUserOfferQuery;
            private string _email;
            private string _offerId;

            [SetUp]
            public void GetUserOfferTests()
            {
                _vehicle = new Vehicle(VehicleTypeEnum.Motorcycle, 2, 2000, new List<MonthlyPrice>
                {
                    new MonthlyPrice(1, 200)
                });
                _email = "m@m.com";
                _offerId = "12345";
                _getUserOfferQuery = new GetUserOfferQuery(_vehicle.MaxNumberOfPeople, _vehicle.MinMonthlyIncome, 1, _email);

                _vehicleQueryRepository = new Mock<IVehicleQueryRepository>();
                _offerGenerator = new Mock<IOfferGenerator>();

                Log.Logger = new LoggerConfiguration().CreateLogger();
            }

            [TestCase(0, 2000, 1, "m@m.com")]
            [TestCase(2, 1000, 1, "m@m.com")]
            [TestCase(2, 2000, 2, "m@m.com")]
            [TestCase(2, 2000, 1, "mm")]
            public async Task GetUserOfferQuery_ValidationFailed_ShouldNotGetVehicle(int maxNumberOfPeople, decimal minMonthlyIncome, int leasePeriod, string email)
            {
                // Arrange 
                var getUserOfferQueryHandler = new GetUserOfferQueryHandler(_vehicleQueryRepository.Object, _offerGenerator.Object);
                _getUserOfferQuery = new GetUserOfferQuery(maxNumberOfPeople, minMonthlyIncome, leasePeriod, email);

                // Act
                var result = await getUserOfferQueryHandler.HandleAsync(_getUserOfferQuery);

                // Assert
                _vehicleQueryRepository.Verify(c => c.GetVehicle(_getUserOfferQuery.MaxNumberOfPeople, _getUserOfferQuery.MinMonthlyIncome, _getUserOfferQuery.LeasePeriod), Times.Never);
                Assert.AreEqual(result.IsFailure, true);
                Assert.That(result.Error, Is.Not.Empty);
            }

            [Test]
            public async Task GetUserOfferQuery_GetVehicleInCorrectParameters_ShouldNotGenerateNewOffer()
            {
                // Arrange 
                var getUserOfferQueryHandler = new GetUserOfferQueryHandler(_vehicleQueryRepository.Object, _offerGenerator.Object);
                _vehicleQueryRepository
                    .Setup(c => c.GetVehicle(_getUserOfferQuery.MaxNumberOfPeople, _getUserOfferQuery.MinMonthlyIncome, _getUserOfferQuery.LeasePeriod))
                    .Returns(Result.Failure<Vehicle>("Missing correct parameters"));

                // Act
                var result = await getUserOfferQueryHandler.HandleAsync(_getUserOfferQuery);

                // Assert
                _offerGenerator.Verify(c => c.GenerateNewOffer(_vehicle.GetFriendlyName(), _vehicle.GetMonthlyPrice(), _email), Times.Never);
                Assert.AreEqual(result.IsFailure, true);
                Assert.That(result.Error, Is.Not.Empty);
            }

            [Test]
            public async Task GetUserOfferQuery_DidNotGetSuitableVehicle_ShouldNotGenerateNewOffer()
            {
                // Arrange 
                var getUserOfferQueryHandler = new GetUserOfferQueryHandler(_vehicleQueryRepository.Object, _offerGenerator.Object);
                _vehicleQueryRepository
                    .Setup(c => c.GetVehicle(_getUserOfferQuery.MaxNumberOfPeople, _getUserOfferQuery.MinMonthlyIncome, _getUserOfferQuery.LeasePeriod))
                    .Returns(Result.Failure<Vehicle>("did not find suitable vehicle"));

                // Act
                var result = await getUserOfferQueryHandler.HandleAsync(_getUserOfferQuery);

                // Assert
                _offerGenerator.Verify(c => c.GenerateNewOffer(_vehicle.GetFriendlyName(), _vehicle.GetMonthlyPrice(), _email), Times.Never);
                Assert.AreEqual(result.IsFailure, true);
                Assert.That(result.Error, Is.Not.Empty);
            }

            [Test]
            public async Task GetUserOfferQuery_GenerateNewOfferInCorrectParameters_ReturnErrorMessage()
            {
                // Arrange 
                _vehicleQueryRepository
                    .Setup(c => c.GetVehicle(_getUserOfferQuery.MaxNumberOfPeople, _getUserOfferQuery.MinMonthlyIncome, _getUserOfferQuery.LeasePeriod))
                    .Returns(Result.Success(_vehicle));

                _offerGenerator
                    .Setup(c => c.GenerateNewOffer(_vehicle.GetFriendlyName(), _vehicle.GetMonthlyPrice(), _email))
                    .ReturnsAsync(Result.Failure<string>("Missing correct parameters"));

                var getUserOfferQueryHandler = new GetUserOfferQueryHandler(_vehicleQueryRepository.Object, _offerGenerator.Object);

                // Act
                var result = await getUserOfferQueryHandler.HandleAsync(_getUserOfferQuery);

                // Assert
                Assert.AreEqual(result.IsFailure, true);
                Assert.That(result.Error, Is.Not.Empty);
            }

            [Test]
            public async Task GetUserOfferQuery_GenerateNewOffer_ThrowException()
            {
                // Arrange 
                _vehicleQueryRepository
                    .Setup(c => c.GetVehicle(_getUserOfferQuery.MaxNumberOfPeople, _getUserOfferQuery.MinMonthlyIncome, _getUserOfferQuery.LeasePeriod))
                    .Returns(Result.Success(_vehicle));


                _offerGenerator
                    .Setup(c => c.GenerateNewOffer(_vehicle.GetFriendlyName(), _vehicle.GetMonthlyPrice(), _email))
                    .Throws(new Exception("Exception"));
                var getUserOfferQueryHandler = new GetUserOfferQueryHandler(_vehicleQueryRepository.Object, _offerGenerator.Object);

                // Act
                var result = await getUserOfferQueryHandler.HandleAsync(_getUserOfferQuery);

                // Assert
                Assert.AreEqual(result.IsFailure, true);
                Assert.That(result.Error, Is.Not.Empty);
            }


            [Test]
            public async Task GetUserOfferQuery_WhenCalled_GetOfferInformation()
            {
                // Arrange 
                _vehicleQueryRepository
                    .Setup(c => c.GetVehicle(_getUserOfferQuery.MaxNumberOfPeople, _getUserOfferQuery.MinMonthlyIncome, _getUserOfferQuery.LeasePeriod))
                    .Returns(Result.Success(_vehicle));
                _offerGenerator
                    .Setup(c => c.GenerateNewOffer(_vehicle.GetFriendlyName(), _vehicle.GetMonthlyPrice(), _email))
                    .ReturnsAsync(_offerId);
                var getUserOfferQueryHandler = new GetUserOfferQueryHandler(_vehicleQueryRepository.Object, _offerGenerator.Object);

                // Act
                var result = await getUserOfferQueryHandler.HandleAsync(_getUserOfferQuery);


                // Assert
                Assert.AreEqual(result.IsSuccess, true);
                Assert.That(result.Value.Vehicle, Is.EqualTo(_vehicle.GetFriendlyName()));
                Assert.That(result.Value.Price, Is.EqualTo(_vehicle.GetMonthlyPrice()));
                Assert.That(result.Value.OfferId, Is.EqualTo(_offerId));
            }
        }
    }
}
