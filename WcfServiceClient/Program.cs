using System;
using System.ServiceModel;
using WcfServiceLibrary;

namespace WcfServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WCF Service Client");
            Console.WriteLine("==================");

            try
            {
                // Create the channel factory
                using (ChannelFactory<ISampleService> factory = new ChannelFactory<ISampleService>("BasicHttpBinding_ISampleService"))
                {
                    // Create the channel
                    ISampleService client = factory.CreateChannel();

                    Console.WriteLine("Testing service methods...\n");

                    // Test GetGreeting
                    Console.WriteLine("1. Testing GetGreeting:");
                    string greeting = client.GetGreeting("World");
                    Console.WriteLine($"   Result: {greeting}\n");

                    // Test Calculate
                    Console.WriteLine("2. Testing Calculate:");
                    double result1 = client.Calculate(10, 5, "add");
                    Console.WriteLine($"   10 + 5 = {result1}");
                    double result2 = client.Calculate(10, 5, "multiply");
                    Console.WriteLine($"   10 * 5 = {result2}");
                    double result3 = client.Calculate(10, 5, "divide");
                    Console.WriteLine($"   10 / 5 = {result3}\n");

                    // Test GetServerTime
                    Console.WriteLine("3. Testing GetServerTime:");
                    DateTime serverTime = client.GetServerTime();
                    Console.WriteLine($"   Server time: {serverTime}\n");

                    // Test Echo
                    Console.WriteLine("4. Testing Echo:");
                    string echo = client.Echo("Hello WCF Service!");
                    Console.WriteLine($"   Result: {echo}\n");

                    // Test error handling
                    Console.WriteLine("5. Testing error handling:");
                    try
                    {
                        double invalidResult = client.Calculate(10, 0, "divide");
                    }
                    catch (FaultException ex)
                    {
                        Console.WriteLine($"   Expected error caught: {ex.Message}");
                    }

                    // Close the channel
                    ((IClientChannel)client).Close();
                }

                Console.WriteLine("All tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
