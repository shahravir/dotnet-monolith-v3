using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Service for handling customer management operations
    /// </summary>
    public class CustomerService
    {
        private static readonly Dictionary<string, CustomerProfile> _customers = new Dictionary<string, CustomerProfile>();
        private static readonly Dictionary<string, string> _customerPasswords = new Dictionary<string, string>();
        private static int _nextCustomerId = 3; // Start after sample customers

        static CustomerService()
        {
            // Initialize with sample customers (same as AuthenticationService)
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

            _customers["CUST001"] = adminCustomer;
            _customers["CUST002"] = regularCustomer;

            // Store hashed passwords
            _customerPasswords["CUST001"] = HashPassword("admin123");
            _customerPasswords["CUST002"] = HashPassword("customer123");
        }

        /// <summary>
        /// Registers a new customer
        /// </summary>
        /// <param name="request">Customer registration request</param>
        /// <returns>Registration result</returns>
        public CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest request)
        {
            var result = new CustomerRegistrationResult();

            // Validate input
            var validationErrors = ValidateRegistrationRequest(request);
            if (validationErrors.Count > 0)
            {
                result.Success = false;
                result.Message = "Registration validation failed.";
                result.ValidationErrors = validationErrors;
                return result;
            }

            // Check if username already exists
            if (IsUsernameTaken(request.Username))
            {
                result.Success = false;
                result.Message = "Username is already taken.";
                result.ValidationErrors.Add("Username is already taken.");
                return result;
            }

            // Check if email already exists
            if (IsEmailTaken(request.Email))
            {
                result.Success = false;
                result.Message = "Email is already registered.";
                result.ValidationErrors.Add("Email is already registered.");
                return result;
            }

            try
            {
                // Create new customer
                var customerId = GenerateCustomerId();
                var customer = new CustomerProfile
                {
                    CustomerId = customerId,
                    Username = request.Username,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Address = request.Address,
                    City = request.City,
                    State = request.State,
                    PostalCode = request.PostalCode,
                    Country = request.Country,
                    Status = CustomerStatus.Pending,
                    CreatedAt = DateTime.Now,
                    LastLoginAt = DateTime.Now
                };

                // Store customer and password
                _customers[customerId] = customer;
                _customerPasswords[customerId] = HashPassword(request.Password);

                result.Success = true;
                result.CustomerId = customerId;
                result.Message = "Customer registered successfully. Account is pending activation.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Registration failed: {ex.Message}";
                result.ValidationErrors.Add(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Updates customer profile
        /// </summary>
        /// <param name="customerProfile">Updated customer profile</param>
        /// <returns>Update result</returns>
        public CustomerUpdateResult UpdateCustomerProfile(CustomerProfile customerProfile)
        {
            var result = new CustomerUpdateResult();

            // Validate input
            if (string.IsNullOrEmpty(customerProfile.CustomerId))
            {
                result.Success = false;
                result.Message = "Customer ID is required.";
                result.ValidationErrors.Add("Customer ID is required.");
                return result;
            }

            if (!_customers.ContainsKey(customerProfile.CustomerId))
            {
                result.Success = false;
                result.Message = "Customer not found.";
                result.ValidationErrors.Add("Customer not found.");
                return result;
            }

            try
            {
                var existingCustomer = _customers[customerProfile.CustomerId];
                
                // Update allowed fields (don't allow changing critical fields)
                existingCustomer.FirstName = customerProfile.FirstName;
                existingCustomer.LastName = customerProfile.LastName;
                existingCustomer.Email = customerProfile.Email;
                existingCustomer.PhoneNumber = customerProfile.PhoneNumber;
                existingCustomer.Address = customerProfile.Address;
                existingCustomer.City = customerProfile.City;
                existingCustomer.State = customerProfile.State;
                existingCustomer.PostalCode = customerProfile.PostalCode;
                existingCustomer.Country = customerProfile.Country;

                result.Success = true;
                result.Message = "Customer profile updated successfully.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Update failed: {ex.Message}";
                result.ValidationErrors.Add(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Changes customer password
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="passwordChange">Password change request</param>
        /// <returns>Password change result</returns>
        public PasswordChangeResult ChangePassword(string customerId, PasswordChangeRequest passwordChange)
        {
            var result = new PasswordChangeResult();

            // Validate input
            if (string.IsNullOrEmpty(customerId) || !_customers.ContainsKey(customerId))
            {
                result.Success = false;
                result.Message = "Invalid customer ID.";
                return result;
            }

            if (string.IsNullOrEmpty(passwordChange.CurrentPassword) ||
                string.IsNullOrEmpty(passwordChange.NewPassword) ||
                string.IsNullOrEmpty(passwordChange.ConfirmPassword))
            {
                result.Success = false;
                result.Message = "All password fields are required.";
                return result;
            }

            if (passwordChange.NewPassword != passwordChange.ConfirmPassword)
            {
                result.Success = false;
                result.Message = "New password and confirmation password do not match.";
                return result;
            }

            if (passwordChange.NewPassword.Length < 6)
            {
                result.Success = false;
                result.Message = "New password must be at least 6 characters long.";
                return result;
            }

            try
            {
                // Verify current password
                var currentHashedPassword = _customerPasswords[customerId];
                var providedHashedPassword = HashPassword(passwordChange.CurrentPassword);

                if (currentHashedPassword != providedHashedPassword)
                {
                    result.Success = false;
                    result.Message = "Current password is incorrect.";
                    return result;
                }

                // Update password
                _customerPasswords[customerId] = HashPassword(passwordChange.NewPassword);

                result.Success = true;
                result.Message = "Password changed successfully.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Password change failed: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Validates registration request
        /// </summary>
        /// <param name="request">Registration request</param>
        /// <returns>List of validation errors</returns>
        private List<string> ValidateRegistrationRequest(CustomerRegistrationRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(request.Username) || request.Username.Length < 3)
                errors.Add("Username must be at least 3 characters long.");

            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 6)
                errors.Add("Password must be at least 6 characters long.");

            if (string.IsNullOrEmpty(request.FirstName))
                errors.Add("First name is required.");

            if (string.IsNullOrEmpty(request.LastName))
                errors.Add("Last name is required.");

            if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
                errors.Add("Valid email address is required.");

            if (string.IsNullOrEmpty(request.PhoneNumber))
                errors.Add("Phone number is required.");

            if (request.DateOfBirth >= DateTime.Now.AddYears(-18))
                errors.Add("Customer must be at least 18 years old.");

            if (string.IsNullOrEmpty(request.Address))
                errors.Add("Address is required.");

            if (string.IsNullOrEmpty(request.City))
                errors.Add("City is required.");

            if (string.IsNullOrEmpty(request.Country))
                errors.Add("Country is required.");

            return errors;
        }

        /// <summary>
        /// Checks if username is already taken
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>True if username is taken</returns>
        private bool IsUsernameTaken(string username)
        {
            foreach (var customer in _customers.Values)
            {
                if (customer.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if email is already taken
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <returns>True if email is taken</returns>
        private bool IsEmailTaken(string email)
        {
            foreach (var customer in _customers.Values)
            {
                if (customer.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Validates email format
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <returns>True if email is valid</returns>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a unique customer ID
        /// </summary>
        /// <returns>Customer ID</returns>
        private string GenerateCustomerId()
        {
            return $"CUST{_nextCustomerId++:D3}";
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
    }
}
