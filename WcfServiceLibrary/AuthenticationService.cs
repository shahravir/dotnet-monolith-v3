using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Service for handling customer authentication
    /// </summary>
    public class AuthenticationService
    {
        private static readonly Dictionary<string, CustomerProfile> _customers = new Dictionary<string, CustomerProfile>();
        private static readonly Dictionary<string, string> _customerPasswords = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _activeTokens = new Dictionary<string, string>(); // token -> customerId

        static AuthenticationService()
        {
            // Initialize with some sample customers
            InitializeSampleCustomers();
        }

        /// <summary>
        /// Initializes sample customers for testing
        /// </summary>
        private static void InitializeSampleCustomers()
        {
            var adminCustomer = new CustomerProfile
            {
                CustomerId = "CUST001",
                Username = "admin",
                FirstName = "John",
                LastName = "Admin",
                Email = "admin@bank.com",
                PhoneNumber = "+1234567890",
                DateOfBirth = new DateTime(1985, 5, 15),
                Address = "123 Main St",
                City = "New York",
                State = "NY",
                PostalCode = "10001",
                Country = "USA",
                Status = CustomerStatus.Active,
                CreatedAt = DateTime.Now.AddDays(-30),
                LastLoginAt = DateTime.Now
            };

            var regularCustomer = new CustomerProfile
            {
                CustomerId = "CUST002",
                Username = "customer",
                FirstName = "Jane",
                LastName = "Customer",
                Email = "jane@email.com",
                PhoneNumber = "+1987654321",
                DateOfBirth = new DateTime(1990, 8, 22),
                Address = "456 Oak Ave",
                City = "Los Angeles",
                State = "CA",
                PostalCode = "90210",
                Country = "USA",
                Status = CustomerStatus.Active,
                CreatedAt = DateTime.Now.AddDays(-15),
                LastLoginAt = DateTime.Now
            };

            _customers["admin"] = adminCustomer;
            _customers["customer"] = regularCustomer;

            // Store hashed passwords (in production, use proper password hashing)
            _customerPasswords["admin"] = HashPassword("admin123");
            _customerPasswords["customer"] = HashPassword("customer123");
        }

        /// <summary>
        /// Authenticates a customer with username and password
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>Login result</returns>
        public LoginResult AuthenticateCustomer(string username, string password)
        {
            var result = new LoginResult
            {
                LoginTime = DateTime.Now
            };

            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                result.Success = false;
                result.ErrorMessage = "Username and password are required.";
                return result;
            }

            // Check if customer exists
            if (!_customers.ContainsKey(username))
            {
                result.Success = false;
                result.ErrorMessage = "Invalid username or password.";
                return result;
            }

            // Verify password
            var hashedPassword = HashPassword(password);
            if (_customerPasswords[username] != hashedPassword)
            {
                result.Success = false;
                result.ErrorMessage = "Invalid username or password.";
                return result;
            }

            // Check if customer is active
            var customer = _customers[username];
            if (customer.Status != CustomerStatus.Active)
            {
                result.Success = false;
                result.ErrorMessage = "Account is not active. Please contact support.";
                return result;
            }

            // Generate token
            var token = GenerateToken(username);

            // Store active token
            _activeTokens[token] = customer.CustomerId;

            // Update last login time
            customer.LastLoginAt = DateTime.Now;

            // Return successful result
            result.Success = true;
            result.Token = token;
            result.Customer = customer;
            result.ErrorMessage = null;

            return result;
        }

        /// <summary>
        /// Logs out a customer and invalidates their token
        /// </summary>
        /// <param name="token">The authentication token</param>
        /// <returns>Logout result</returns>
        public LogoutResult LogoutCustomer(string token)
        {
            var result = new LogoutResult
            {
                LogoutTime = DateTime.Now
            };

            if (string.IsNullOrEmpty(token))
            {
                result.Success = false;
                result.Message = "Token is required.";
                return result;
            }

            if (_activeTokens.ContainsKey(token))
            {
                _activeTokens.Remove(token);
                result.Success = true;
                result.Message = "Successfully logged out.";
            }
            else
            {
                result.Success = false;
                result.Message = "Invalid or expired token.";
            }

            return result;
        }

        /// <summary>
        /// Validates a customer's authentication token
        /// </summary>
        /// <param name="token">The authentication token</param>
        /// <returns>Token validation result</returns>
        public TokenValidationResult ValidateCustomerToken(string token)
        {
            var result = new TokenValidationResult
            {
                ValidationTime = DateTime.Now
            };

            if (string.IsNullOrEmpty(token))
            {
                result.IsValid = false;
                result.ErrorMessage = "Token is required.";
                return result;
            }

            if (_activeTokens.ContainsKey(token))
            {
                var customerId = _activeTokens[token];
                var customer = GetCustomerById(customerId);
                
                if (customer != null && customer.Status == CustomerStatus.Active)
                {
                    result.IsValid = true;
                    result.Customer = customer;
                }
                else
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Customer account is not active.";
                    _activeTokens.Remove(token);
                }
            }
            else
            {
                result.IsValid = false;
                result.ErrorMessage = "Invalid or expired token.";
            }

            return result;
        }

        /// <summary>
        /// Gets customer by token
        /// </summary>
        /// <param name="token">The authentication token</param>
        /// <returns>Customer if token is valid, null otherwise</returns>
        public CustomerProfile GetCustomerByToken(string token)
        {
            if (string.IsNullOrEmpty(token) || !_activeTokens.ContainsKey(token))
                return null;

            var customerId = _activeTokens[token];
            return GetCustomerById(customerId);
        }

        /// <summary>
        /// Gets customer by ID
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <returns>Customer if found, null otherwise</returns>
        public CustomerProfile GetCustomerById(string customerId)
        {
            foreach (var customer in _customers.Values)
            {
                if (customer.CustomerId == customerId)
                    return customer;
            }
            return null;
        }

        /// <summary>
        /// Hashes a password using SHA256
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>Hashed password</returns>
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Generates a simple token for the customer
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>Generated token</returns>
        private static string GenerateToken(string username)
        {
            var tokenData = $"{username}:{DateTime.UtcNow.Ticks}";
            var tokenBytes = Encoding.UTF8.GetBytes(tokenData);
            return Convert.ToBase64String(tokenBytes);
        }

        /// <summary>
        /// Decodes a token to extract username
        /// </summary>
        /// <param name="token">The token to decode</param>
        /// <returns>Username from token</returns>
        private static string DecodeToken(string token)
        {
            var tokenBytes = Convert.FromBase64String(token);
            var tokenString = Encoding.UTF8.GetString(tokenBytes);
            return tokenString.Split(':')[0];
        }
    }
}
