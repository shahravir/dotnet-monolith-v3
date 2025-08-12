using System;
using System.ServiceModel;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Service contract for the sample WCF service
    /// </summary>
    [ServiceContract]
    public interface ISampleService
    {
        /// <summary>
        /// Gets a greeting message
        /// </summary>
        /// <param name="name">The name to greet</param>
        /// <returns>A greeting message</returns>
        [OperationContract]
        string GetGreeting(string name);

        /// <summary>
        /// Performs a simple calculation
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <param name="operation">Operation to perform (add, subtract, multiply, divide)</param>
        /// <returns>The result of the calculation</returns>
        [OperationContract]
        double Calculate(double a, double b, string operation);

        /// <summary>
        /// Gets the current server time
        /// </summary>
        /// <returns>Current server time</returns>
        [OperationContract]
        DateTime GetServerTime();

        /// <summary>
        /// Echoes back the input string
        /// </summary>
        /// <param name="message">Message to echo</param>
        /// <returns>The echoed message</returns>
        [OperationContract]
        string Echo(string message);

        /// <summary>
        /// Authenticates a user with username and password
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>Login result with token and user info</returns>
        [OperationContract]
        LoginResult Login(string username, string password);
    }
}
