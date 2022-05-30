using ConditionalDI.Interfaces;
using ConditionalDI.Strategies;
using Microsoft.Extensions.DependencyInjection;
namespace ConditionalDI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddSingleton<AustraliaTaxCalculator>();
            services.AddSingleton<EuropeTaxCalculator>();

            services.AddSingleton<Func<Locations, ITaxCalculator>>(
                ServiceProvider => key =>
                {
                    switch (key) {
                        case Locations.Australia:
                            return ServiceProvider.GetService<AustraliaTaxCalculator>();
                        case Locations.Europe:
                            return ServiceProvider.GetService<EuropeTaxCalculator>();
                        default:
                            return null;
                    }
                });


            services.AddSingleton<Purchase>();
            var serviceProvider = services.BuildServiceProvider();
            var purchase = serviceProvider.GetService<Purchase>();

            Console.WriteLine($"Australia purchase: {purchase.CheckOut(Locations.Australia)}");
            Console.WriteLine($"Europe purchase: {purchase.CheckOut(Locations.Europe)}");
        }
    }

    public enum Locations {
        Europe,
        Australia
    }
}