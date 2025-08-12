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

                    // Test Login functionality
                    Console.WriteLine("5. Testing Login:");
                    
                    // Test successful login with admin
                    Console.WriteLine("   Testing admin login:");
                    LoginResult adminLogin = client.Login("admin", "admin123");
                    if (adminLogin.Success)
                    {
                        Console.WriteLine($"   Success! Token: {adminLogin.Token}");
                        Console.WriteLine($"   User: {adminLogin.User.FullName} ({adminLogin.User.Role})");
                        Console.WriteLine($"   Email: {adminLogin.User.Email}");
                    }
                    else
                    {
                        Console.WriteLine($"   Failed: {adminLogin.ErrorMessage}");
                    }

                    // Test successful login with regular user
                    Console.WriteLine("   Testing user login:");
                    LoginResult userLogin = client.Login("user", "user123");
                    if (userLogin.Success)
                    {
                        Console.WriteLine($"   Success! Token: {userLogin.Token}");
                        Console.WriteLine($"   User: {userLogin.User.FullName} ({userLogin.User.Role})");
                        Console.WriteLine($"   Email: {userLogin.User.Email}");
                    }
                    else
                    {
                        Console.WriteLine($"   Failed: {userLogin.ErrorMessage}");
                    }

                    // Test failed login
                    Console.WriteLine("   Testing failed login:");
                    LoginResult failedLogin = client.Login("admin", "wrongpassword");
                    if (!failedLogin.Success)
                    {
                        Console.WriteLine($"   Expected failure: {failedLogin.ErrorMessage}");
                    }

                    // Test invalid credentials
                    Console.WriteLine("   Testing invalid credentials:");
                    LoginResult invalidLogin = client.Login("nonexistent", "password");
                    if (!invalidLogin.Success)
                    {
                        Console.WriteLine($"   Expected failure: {invalidLogin.ErrorMessage}");
                    }

                    Console.WriteLine();

                    // Test error handling
                    Console.WriteLine("6. Testing error handling:");
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
