using System;
using System.ServiceModel;
using WcfServiceLibrary;
using System.Linq; // Added for .Take()

namespace WcfServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Mobile Banking Gateway Client");
            Console.WriteLine("=============================");

            try
            {
                // Create the channel factory
                using (ChannelFactory<IBankingGatewayService> factory = new ChannelFactory<IBankingGatewayService>("BasicHttpBinding_IBankingGatewayService"))
                {
                    // Create the channel
                    IBankingGatewayService client = factory.CreateChannel();

                    Console.WriteLine("Testing Mobile Banking Gateway Service...\n");

                    // Test Authentication
                    Console.WriteLine("1. Testing Authentication:");
                    TestAuthentication(client);

                    // Test Customer Management
                    Console.WriteLine("\n2. Testing Customer Management:");
                    TestCustomerManagement(client);

                    // Test Account Management
                    Console.WriteLine("\n3. Testing Account Management:");
                    TestAccountManagement(client);

                    // Test Transaction Management
                    Console.WriteLine("\n4. Testing Transaction Management:");
                    TestTransactionManagement(client);

                    // Test Bill Payments
                    Console.WriteLine("\n5. Testing Bill Payments:");
                    TestBillPayments(client);

                    // Test System Status
                    Console.WriteLine("\n6. Testing System Status:");
                    TestSystemStatus(client);

                    // Close the channel
                    ((IClientChannel)client).Close();
                }

                Console.WriteLine("\nAll tests completed successfully!");
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

        static void TestAuthentication(IBankingGatewayService client)
        {
            // Test admin login
            Console.WriteLine("   Testing admin login:");
            var adminLogin = client.Login("admin", "admin123");
            if (adminLogin.Success)
            {
                Console.WriteLine($"   Success! Token: {adminLogin.Token}");
                Console.WriteLine($"   Customer: {adminLogin.Customer.FirstName} {adminLogin.Customer.LastName}");
                Console.WriteLine($"   Email: {adminLogin.Customer.Email}");
            }
            else
            {
                Console.WriteLine($"   Failed: {adminLogin.ErrorMessage}");
            }

            // Test customer login
            Console.WriteLine("   Testing customer login:");
            var customerLogin = client.Login("customer", "customer123");
            if (customerLogin.Success)
            {
                Console.WriteLine($"   Success! Token: {customerLogin.Token}");
                Console.WriteLine($"   Customer: {customerLogin.Customer.FirstName} {customerLogin.Customer.LastName}");
                Console.WriteLine($"   Email: {customerLogin.Customer.Email}");
            }
            else
            {
                Console.WriteLine($"   Failed: {customerLogin.ErrorMessage}");
            }

            // Test failed login
            Console.WriteLine("   Testing failed login:");
            var failedLogin = client.Login("admin", "wrongpassword");
            if (!failedLogin.Success)
            {
                Console.WriteLine($"   Expected failure: {failedLogin.ErrorMessage}");
            }

            // Test token validation
            if (adminLogin.Success)
            {
                Console.WriteLine("   Testing token validation:");
                var tokenValidation = client.ValidateToken(adminLogin.Token);
                if (tokenValidation.IsValid)
                {
                    Console.WriteLine($"   Token is valid for: {tokenValidation.Customer.FirstName} {tokenValidation.Customer.LastName}");
                }
                else
                {
                    Console.WriteLine($"   Token validation failed: {tokenValidation.ErrorMessage}");
                }
            }
        }

        static void TestCustomerManagement(IBankingGatewayService client)
        {
            // Login as admin
            var login = client.Login("admin", "admin123");
            if (!login.Success)
            {
                Console.WriteLine("   Failed to login for customer management tests");
                return;
            }

            // Test customer registration
            Console.WriteLine("   Testing customer registration:");
            var registration = new CustomerRegistrationRequest
            {
                Username = "newcustomer",
                Password = "password123",
                FirstName = "New",
                LastName = "Customer",
                Email = "newcustomer@email.com",
                PhoneNumber = "+1555123456",
                DateOfBirth = new DateTime(1995, 3, 15),
                Address = "789 Pine St",
                City = "Chicago",
                State = "IL",
                PostalCode = "60601",
                Country = "USA"
            };

            var registrationResult = client.RegisterCustomer(registration);
            if (registrationResult.Success)
            {
                Console.WriteLine($"   Registration successful! Customer ID: {registrationResult.CustomerId}");
            }
            else
            {
                Console.WriteLine($"   Registration failed: {registrationResult.Message}");
                foreach (var error in registrationResult.ValidationErrors)
                {
                    Console.WriteLine($"     - {error}");
                }
            }

            // Test get customer profile
            Console.WriteLine("   Testing get customer profile:");
            var profileResult = client.GetCustomerProfile(login.Token, login.Customer.CustomerId);
            if (profileResult.Success)
            {
                Console.WriteLine($"   Profile retrieved: {profileResult.Customer.FirstName} {profileResult.Customer.LastName}");
                Console.WriteLine($"   Email: {profileResult.Customer.Email}");
                Console.WriteLine($"   Status: {profileResult.Customer.Status}");
            }
            else
            {
                Console.WriteLine($"   Failed to get profile: {profileResult.ErrorMessage}");
            }
        }

        static void TestAccountManagement(IBankingGatewayService client)
        {
            // Login as customer
            var login = client.Login("customer", "customer123");
            if (!login.Success)
            {
                Console.WriteLine("   Failed to login for account management tests");
                return;
            }

            // Test get customer accounts
            Console.WriteLine("   Testing get customer accounts:");
            var accountsResult = client.GetCustomerAccounts(login.Token, login.Customer.CustomerId);
            if (accountsResult.Success)
            {
                Console.WriteLine($"   Found {accountsResult.Accounts.Count} accounts:");
                foreach (var account in accountsResult.Accounts)
                {
                    Console.WriteLine($"     - {account.AccountType}: {account.AccountNumber} (Balance: {account.Balance:C})");
                }
            }
            else
            {
                Console.WriteLine($"   Failed to get accounts: {accountsResult.ErrorMessage}");
            }

            // Test get account details
            if (accountsResult.Success && accountsResult.Accounts.Count > 0)
            {
                var firstAccount = accountsResult.Accounts[0];
                Console.WriteLine("   Testing get account details:");
                var accountDetails = client.GetAccountDetails(login.Token, firstAccount.AccountNumber);
                if (accountDetails.Success)
                {
                    Console.WriteLine($"   Account: {accountDetails.Account.AccountNumber}");
                    Console.WriteLine($"   Type: {accountDetails.Account.AccountType}");
                    Console.WriteLine($"   Balance: {accountDetails.Account.Balance:C}");
                    Console.WriteLine($"   Available: {accountDetails.Account.AvailableBalance:C}");
                }
                else
                {
                    Console.WriteLine($"   Failed to get account details: {accountDetails.ErrorMessage}");
                }

                // Test get account balance
                Console.WriteLine("   Testing get account balance:");
                var balanceResult = client.GetAccountBalance(login.Token, firstAccount.AccountNumber);
                if (balanceResult.Success)
                {
                    Console.WriteLine($"   Balance: {balanceResult.Balance:C}");
                    Console.WriteLine($"   Available: {balanceResult.AvailableBalance:C}");
                    Console.WriteLine($"   Currency: {balanceResult.Currency}");
                }
                else
                {
                    Console.WriteLine($"   Failed to get balance: {balanceResult.ErrorMessage}");
                }
            }
        }

        static void TestTransactionManagement(IBankingGatewayService client)
        {
            // Login as customer
            var login = client.Login("customer", "customer123");
            if (!login.Success)
            {
                Console.WriteLine("   Failed to login for transaction management tests");
                return;
            }

            // Get customer accounts
            var accountsResult = client.GetCustomerAccounts(login.Token, login.Customer.CustomerId);
            if (!accountsResult.Success || accountsResult.Accounts.Count < 2)
            {
                Console.WriteLine("   Need at least 2 accounts for transfer tests");
                return;
            }

            var fromAccount = accountsResult.Accounts[0];
            var toAccount = accountsResult.Accounts[1];

            // Test transfer money
            Console.WriteLine("   Testing money transfer:");
            var transferRequest = new TransferRequest
            {
                FromAccountNumber = fromAccount.AccountNumber,
                ToAccountNumber = toAccount.AccountNumber,
                Amount = 100.00m,
                Description = "Test transfer",
                ReferenceNumber = "REF_TEST_001"
            };

            var transferResult = client.TransferMoney(login.Token, transferRequest);
            if (transferResult.Success)
            {
                Console.WriteLine($"   Transfer successful! Transaction ID: {transferResult.TransactionId}");
                Console.WriteLine($"   Amount: {transferRequest.Amount:C}");
                Console.WriteLine($"   Message: {transferResult.Message}");
            }
            else
            {
                Console.WriteLine($"   Transfer failed: {transferResult.Message}");
                foreach (var error in transferResult.ValidationErrors)
                {
                    Console.WriteLine($"     - {error}");
                }
            }

            // Test get transaction history
            Console.WriteLine("   Testing get transaction history:");
            var fromDate = DateTime.Now.AddDays(-30);
            var toDate = DateTime.Now;
            var historyResult = client.GetTransactionHistory(login.Token, fromAccount.AccountNumber, fromDate, toDate, 1, 10);
            if (historyResult.Success)
            {
                Console.WriteLine($"   Found {historyResult.Transactions.Count} transactions (Total: {historyResult.TotalCount})");
                foreach (var transaction in historyResult.Transactions.Take(3))
                {
                    Console.WriteLine($"     - {transaction.Type}: {transaction.Amount:C} ({transaction.Description})");
                }
            }
            else
            {
                Console.WriteLine($"   Failed to get transaction history: {historyResult.ErrorMessage}");
            }

            // Test get transfer status
            if (transferResult.Success)
            {
                Console.WriteLine("   Testing get transfer status:");
                var statusResult = client.GetTransferStatus(login.Token, transferResult.TransactionId);
                if (statusResult.Success)
                {
                    Console.WriteLine($"   Transfer status: {statusResult.Status}");
                    Console.WriteLine($"   Status message: {statusResult.StatusMessage}");
                }
                else
                {
                    Console.WriteLine($"   Failed to get transfer status: {statusResult.StatusMessage}");
                }
            }
        }

        static void TestBillPayments(IBankingGatewayService client)
        {
            // Login as customer
            var login = client.Login("customer", "customer123");
            if (!login.Success)
            {
                Console.WriteLine("   Failed to login for bill payment tests");
                return;
            }

            // Test get biller categories
            Console.WriteLine("   Testing get biller categories:");
            var categoriesResult = client.GetBillerCategories(login.Token);
            if (categoriesResult.Success)
            {
                Console.WriteLine($"   Found {categoriesResult.Categories.Count} biller categories:");
                foreach (var category in categoriesResult.Categories)
                {
                    Console.WriteLine($"     - {category.Name}: {category.Description}");
                }
            }
            else
            {
                Console.WriteLine($"   Failed to get biller categories: {categoriesResult.ErrorMessage}");
            }

            // Test get billers by category
            if (categoriesResult.Success && categoriesResult.Categories.Count > 0)
            {
                var firstCategory = categoriesResult.Categories[0];
                Console.WriteLine("   Testing get billers by category:");
                var billersResult = client.GetBillersByCategory(login.Token, firstCategory.CategoryId);
                if (billersResult.Success)
                {
                    Console.WriteLine($"   Found {billersResult.Billers.Count} billers in {firstCategory.Name}:");
                    foreach (var biller in billersResult.Billers)
                    {
                        Console.WriteLine($"     - {biller.Name}: {biller.Description}");
                    }
                }
                else
                {
                    Console.WriteLine($"   Failed to get billers: {billersResult.ErrorMessage}");
                }

                // Test bill payment
                if (billersResult.Success && billersResult.Billers.Count > 0)
                {
                    var firstBiller = billersResult.Billers[0];
                    var accountsResult = client.GetCustomerAccounts(login.Token, login.Customer.CustomerId);
                    if (accountsResult.Success && accountsResult.Accounts.Count > 0)
                    {
                        Console.WriteLine("   Testing bill payment:");
                        var billPayment = new BillPaymentRequest
                        {
                            FromAccountNumber = accountsResult.Accounts[0].AccountNumber,
                            BillerId = firstBiller.BillerId,
                            AccountNumber = "123456789",
                            Amount = 50.00m,
                            Description = "Test bill payment",
                            ReferenceNumber = "BILL_TEST_001"
                        };

                        var paymentResult = client.PayBill(login.Token, billPayment);
                        if (paymentResult.Success)
                        {
                            Console.WriteLine($"   Bill payment successful! Transaction ID: {paymentResult.TransactionId}");
                            Console.WriteLine($"   Amount: {billPayment.Amount:C}");
                            Console.WriteLine($"   Message: {paymentResult.Message}");
                        }
                        else
                        {
                            Console.WriteLine($"   Bill payment failed: {paymentResult.Message}");
                            foreach (var error in paymentResult.ValidationErrors)
                            {
                                Console.WriteLine($"     - {error}");
                            }
                        }
                    }
                }
            }

            // Test get bill payment history
            Console.WriteLine("   Testing get bill payment history:");
            var historyResult = client.GetBillPaymentHistory(login.Token, login.Customer.CustomerId, 1, 10);
            if (historyResult.Success)
            {
                Console.WriteLine($"   Found {historyResult.Payments.Count} bill payments (Total: {historyResult.TotalCount})");
                foreach (var payment in historyResult.Payments.Take(3))
                {
                    Console.WriteLine($"     - {payment.Amount:C} ({payment.Description})");
                }
            }
            else
            {
                Console.WriteLine($"   Failed to get bill payment history: {historyResult.ErrorMessage}");
            }
        }

        static void TestSystemStatus(IBankingGatewayService client)
        {
            // Test get server time
            Console.WriteLine("   Testing get server time:");
            var serverTime = client.GetServerTime();
            Console.WriteLine($"   Server time: {serverTime}");

            // Test get system status
            Console.WriteLine("   Testing get system status:");
            var systemStatus = client.GetSystemStatus();
            if (systemStatus.IsHealthy)
            {
                Console.WriteLine($"   System is healthy - Version: {systemStatus.Version}");
                Console.WriteLine($"   Status: {systemStatus.Status}");
            }
            else
            {
                Console.WriteLine($"   System is not healthy: {systemStatus.Status}");
            }
        }
    }
}
