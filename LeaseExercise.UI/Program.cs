using CSharpFunctionalExtensions;
using LeaseExercise.Common.Helpers;
using LeaseExercise.Common.Interfaces;
using LeaseExercise.Domain.Interfaces;
using LeaseExercise.Infrastructure.Repositories;
using LeaseExercise.Services.Queries;
using LeaseExercise.Services.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using Destructurama;
using LeaseExercise.Common.Attributes;
using LeaseExercise.Common.Extensions;
using LeaseExercise.Infrastructure.External;
using RestSharp;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;

namespace LeaseExercise.UI
{
    static class Program
    {
        private static IServiceProvider _serviceProvider;

        [LoggingAspect]
        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            // Build configuration

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();

            Log.Information("Adding services");

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IRestClient>(new RestClient(configuration["BaseAddress"]));

            services.AddScoped<IMessageDispatcher, MessageDispatcher>();
            services.AddScoped<IOfferGenerator, OfferGenerator>();
            services.AddScoped<IVehicleQueryRepository, VehicleQueryRepository>();
            services.AddScoped<IQueryHandler<GetUserOfferQuery, OfferInformation>, GetUserOfferQueryHandler>();

            Log.Information("Building service provider");
            _serviceProvider = services.BuildServiceProvider();
        }

        [LoggingAspect]
        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        [LoggingAspect]
        static async Task MainAsync()
        {
            RegisterServices();

            Console.WriteLine("LEASE OFFER APPLICATION");
            Console.WriteLine("=======================");
            var running = true;
            while (running)
            {
                Console.WriteLine("How many people would you want to fit in the vehicle?");
                var maxNumberOfPeople = int.Parse(Console.ReadLine() ?? string.Empty);

                Console.WriteLine("What is your monthly income (euro)?");
                var minMonthlyIncome = decimal.Parse(Console.ReadLine() ?? string.Empty);

                Console.WriteLine("How many years do you want to lease the vehicle for?");
                var leasePeriod = int.Parse(Console.ReadLine() ?? string.Empty);

                Console.WriteLine("What is your email address?");
                var email = Console.ReadLine();

                var messageDispatcher = _serviceProvider.GetService<IMessageDispatcher>();
                if (messageDispatcher != null)
                {
                    await messageDispatcher
                        .DispatchAsync(new GetUserOfferQuery(maxNumberOfPeople, minMonthlyIncome, leasePeriod, email))
                        .Tap(c =>
                        {
                            Console.WriteLine();
                            Console.WriteLine("The vehicle we offer you is a " + c.Vehicle +
                                              " for the lease price of " + c.Price +
                                              " euro per month. Your offer id is " + c.OfferId);
                            running = false;
                        })
                        .OnFailure(error =>
                        {
                            Console.WriteLine();
                            Console.WriteLine($"An error occurred while getting your offer : {error} .");
                            Console.WriteLine("Please click any key to try again.");
                            Console.ReadKey();
                            running = true;
                        });
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("An error occurred while getting your offer please try again.");
                    Console.WriteLine("Please click any key to try again.");
                    Console.ReadKey();
                    running = true;
                }
            }
            Console.ReadKey();
            DisposeServices();
        }

        static int Main()
        {
            // Initialize serilog logger
            // the real world example of the logging will be to async service 
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Destructure.JsonNetTypes()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithRootName("Exception"))
                .WriteTo.Async(a => a.Console())
                .CreateLogger();
            try
            {
                // Start!
                MainAsync().Wait();
                return 0;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message());
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
