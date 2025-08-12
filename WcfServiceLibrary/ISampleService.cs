using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Service contract for the Mobile Banking Gateway
    /// </summary>
    [ServiceContract]
    public interface IBankingGatewayService
    {
        #region Authentication & Security

        /// <summary>
        /// Authenticates a customer with username and password
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>Login result with token and customer info</returns>
        [OperationContract]
        LoginResult Login(string username, string password);

        /// <summary>
        /// Logs out a customer and invalidates their token
        /// </summary>
        /// <param name="token">The authentication token</param>
        /// <returns>Logout result</returns>
        [OperationContract]
        LogoutResult Logout(string token);

        /// <summary>
        /// Validates a customer's authentication token
        /// </summary>
        /// <param name="token">The authentication token</param>
        /// <returns>Token validation result</returns>
        [OperationContract]
        TokenValidationResult ValidateToken(string token);

        #endregion

        #region Customer Management

        /// <summary>
        /// Registers a new customer
        /// </summary>
        /// <param name="customerRegistration">Customer registration data</param>
        /// <returns>Registration result</returns>
        [OperationContract]
        CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest customerRegistration);

        /// <summary>
        /// Gets customer profile information
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Customer profile</returns>
        [OperationContract]
        CustomerProfileResult GetCustomerProfile(string token, string customerId);

        /// <summary>
        /// Updates customer profile information
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="customerProfile">Updated customer profile</param>
        /// <returns>Update result</returns>
        [OperationContract]
        CustomerUpdateResult UpdateCustomerProfile(string token, CustomerProfile customerProfile);

        /// <summary>
        /// Changes customer password
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="passwordChange">Password change request</param>
        /// <returns>Password change result</returns>
        [OperationContract]
        PasswordChangeResult ChangePassword(string token, PasswordChangeRequest passwordChange);

        #endregion

        #region Account Management

        /// <summary>
        /// Gets all accounts for a customer
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>List of customer accounts</returns>
        [OperationContract]
        AccountListResult GetCustomerAccounts(string token, string customerId);

        /// <summary>
        /// Gets account details by account number
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="accountNumber">Account number</param>
        /// <returns>Account details</returns>
        [OperationContract]
        AccountDetailResult GetAccountDetails(string token, string accountNumber);

        /// <summary>
        /// Gets account balance
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="accountNumber">Account number</param>
        /// <returns>Account balance</returns>
        [OperationContract]
        AccountBalanceResult GetAccountBalance(string token, string accountNumber);

        #endregion

        #region Transaction Management

        /// <summary>
        /// Gets transaction history for an account
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="accountNumber">Account number</param>
        /// <param name="fromDate">Start date</param>
        /// <param name="toDate">End date</param>
        /// <param name="pageNumber">Page number for pagination</param>
        /// <param name="pageSize">Page size for pagination</param>
        /// <returns>Transaction history</returns>
        [OperationContract]
        TransactionHistoryResult GetTransactionHistory(string token, string accountNumber, DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 20);

        /// <summary>
        /// Transfers money between accounts
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="transferRequest">Transfer request</param>
        /// <returns>Transfer result</returns>
        [OperationContract]
        TransferResult TransferMoney(string token, TransferRequest transferRequest);

        /// <summary>
        /// Gets transfer status
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Transfer status</returns>
        [OperationContract]
        TransferStatusResult GetTransferStatus(string token, string transactionId);

        #endregion

        #region Bill Payments

        /// <summary>
        /// Gets biller categories
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <returns>List of biller categories</returns>
        [OperationContract]
        BillerCategoryResult GetBillerCategories(string token);

        /// <summary>
        /// Gets billers by category
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of billers</returns>
        [OperationContract]
        BillerListResult GetBillersByCategory(string token, string categoryId);

        /// <summary>
        /// Pays a bill
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="billPayment">Bill payment request</param>
        /// <returns>Bill payment result</returns>
        [OperationContract]
        BillPaymentResult PayBill(string token, BillPaymentRequest billPayment);

        /// <summary>
        /// Gets bill payment history
        /// </summary>
        /// <param name="token">Authentication token</param>
        /// <param name="customerId">Customer ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Bill payment history</returns>
        [OperationContract]
        BillPaymentHistoryResult GetBillPaymentHistory(string token, string customerId, int pageNumber = 1, int pageSize = 20);

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the current server time
        /// </summary>
        /// <returns>Current server time</returns>
        [OperationContract]
        DateTime GetServerTime();

        /// <summary>
        /// Gets system status and health information
        /// </summary>
        /// <returns>System status</returns>
        [OperationContract]
        SystemStatusResult GetSystemStatus();

        #endregion
    }
}
