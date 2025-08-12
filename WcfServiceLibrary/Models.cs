using System;
using System.Runtime.Serialization;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Result of a login operation
    /// </summary>
    [DataContract]
    public class LoginResult
    {
        /// <summary>
        /// Indicates if the login was successful
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// Authentication token for successful login
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        /// <summary>
        /// User information for successful login
        /// </summary>
        [DataMember]
        public User User { get; set; }

        /// <summary>
        /// Error message for failed login
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Timestamp when the login occurred
        /// </summary>
        [DataMember]
        public DateTime LoginTime { get; set; }
    }

    /// <summary>
    /// User information
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// User's full name
        /// </summary>
        [DataMember]
        public string FullName { get; set; }

        /// <summary>
        /// User's email address
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// User's role in the system
        /// </summary>
        [DataMember]
        public string Role { get; set; }

        /// <summary>
        /// When the user was created
        /// </summary>
        [DataMember]
        public DateTime CreatedAt { get; set; }
    }
}
