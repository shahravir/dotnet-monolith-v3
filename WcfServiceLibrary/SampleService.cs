using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Implementation of the sample WCF service
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SampleService : ISampleService
    {
        /// <summary>
        /// Gets a greeting message
        /// </summary>
        /// <param name="name">The name to greet</param>
        /// <returns>A greeting message</returns>
        public string GetGreeting(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "Hello, Anonymous!";
            }
            
            return $"Hello, {name}! Welcome to the WCF Service.";
        }

        /// <summary>
        /// Performs a simple calculation
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <param name="operation">Operation to perform (add, subtract, multiply, divide)</param>
        /// <returns>The result of the calculation</returns>
        public double Calculate(double a, double b, string operation)
        {
            switch (operation?.ToLower())
            {
                case "add":
                    return a + b;
                case "subtract":
                    return a - b;
                case "multiply":
                    return a * b;
                case "divide":
                    if (b == 0)
                    {
                        throw new FaultException("Cannot divide by zero.");
                    }
                    return a / b;
                default:
                    throw new FaultException($"Unknown operation: {operation}. Supported operations: add, subtract, multiply, divide.");
            }
        }

        /// <summary>
        /// Gets the current server time
        /// </summary>
        /// <returns>Current server time</returns>
        public DateTime GetServerTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Echoes back the input string
        /// </summary>
        /// <param name="message">Message to echo</param>
        /// <returns>The echoed message</returns>
        public string Echo(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return "Echo: (empty message)";
            }
            
            return $"Echo: {message}";
        }
    }
}
