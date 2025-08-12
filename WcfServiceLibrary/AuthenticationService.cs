using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Service for handling user authentication
    /// </summary>
    public class AuthenticationService
    {
        private static readonly Dictionary<string, User> _users = new Dictionary<string, User>();
        private static readonly Dictionary<string, string> _userPasswords = new Dictionary<string, string>();

        static AuthenticationService()
        {
            // Initialize with some sample users
            InitializeSampleUsers();
        }

        /// <summary>
        /// Initializes sample users for testing
        /// </summary>
        private static void InitializeSampleUsers()
        {
            var adminUser = new User
            {
                Id = 1,
                Username = "admin",
                FullName = "Administrator",
                Email = "admin@example.com",
                Role = "Admin",
                CreatedAt = DateTime.Now.AddDays(-30)
            };

            var regularUser = new User
            {
                Id = 2,
                Username = "user",
                FullName = "Regular User",
                Email = "user@example.com",
                Role = "User",
                CreatedAt = DateTime.Now.AddDays(-15)
            };

            _users["admin"] = adminUser;
            _users["user"] = regularUser;

            // Store hashed passwords (in production, use proper password hashing)
            _userPasswords["admin"] = HashPassword("admin123");
            _userPasswords["user"] = HashPassword("user123");
        }

        /// <summary>
        /// Authenticates a user with username and password
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>Login result</returns>
        public LoginResult AuthenticateUser(string username, string password)
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

            // Check if user exists
            if (!_users.ContainsKey(username))
            {
                result.Success = false;
                result.ErrorMessage = "Invalid username or password.";
                return result;
            }

            // Verify password
            var hashedPassword = HashPassword(password);
            if (_userPasswords[username] != hashedPassword)
            {
                result.Success = false;
                result.ErrorMessage = "Invalid username or password.";
                return result;
            }

            // Generate token
            var token = GenerateToken(username);

            // Return successful result
            result.Success = true;
            result.Token = token;
            result.User = _users[username];
            result.ErrorMessage = null;

            return result;
        }

        /// <summary>
        /// Validates a token
        /// </summary>
        /// <param name="token">The token to validate</param>
        /// <returns>True if token is valid, false otherwise</returns>
        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            // Simple token validation - in production, use proper JWT or similar
            try
            {
                var decodedToken = DecodeToken(token);
                return _users.ContainsKey(decodedToken);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets user by token
        /// </summary>
        /// <param name="token">The authentication token</param>
        /// <returns>User if token is valid, null otherwise</returns>
        public User GetUserByToken(string token)
        {
            if (!ValidateToken(token))
                return null;

            var username = DecodeToken(token);
            return _users.ContainsKey(username) ? _users[username] : null;
        }

        /// <summary>
        /// Hashes a password using SHA256
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>Hashed password</returns>
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Generates a simple token for the user
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>Generated token</returns>
        private string GenerateToken(string username)
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
        private string DecodeToken(string token)
        {
            var tokenBytes = Convert.FromBase64String(token);
            var tokenString = Encoding.UTF8.GetString(tokenBytes);
            return tokenString.Split(':')[0];
        }
    }
}
