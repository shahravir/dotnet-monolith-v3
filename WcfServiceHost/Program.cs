using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using WcfServiceLibrary;

namespace WcfServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Mobile Banking Gateway Service...");
            Console.WriteLine("Press any key to stop the service.");

            try
            {
                // Create the service host
                using (ServiceHost host = new ServiceHost(typeof(BankingGatewayService)))
                {
                    // Start the service
                    host.Open();

                    Console.WriteLine("Mobile Banking Gateway Service is running at:");
                    foreach (var endpoint in host.Description.Endpoints)
                    {
                        Console.WriteLine($"  {endpoint.Address}");
                    }
                    Console.WriteLine();
                    Console.WriteLine("Available endpoints:");
                    Console.WriteLine("  - Authentication & Security");
                    Console.WriteLine("  - Customer Management");
                    Console.WriteLine("  - Account Management");
                    Console.WriteLine("  - Transaction Management");
                    Console.WriteLine("  - Bill Payments");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to stop the service...");
                    Console.ReadKey();

                    // Stop the service
                    host.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting service: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
