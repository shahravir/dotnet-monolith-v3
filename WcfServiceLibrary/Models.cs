using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WcfServiceLibrary
{
    #region Authentication Models

    /// <summary>
    /// Result of a login operation
    /// </summary>
    [DataContract]
    public class LoginResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public CustomerProfile Customer { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public DateTime LoginTime { get; set; }
    }

    /// <summary>
    /// Result of a logout operation
    /// </summary>
    [DataContract]
    public class LogoutResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public DateTime LogoutTime { get; set; }
    }

    /// <summary>
    /// Result of token validation
    /// </summary>
    [DataContract]
    public class TokenValidationResult
    {
        [DataMember]
        public bool IsValid { get; set; }

        [DataMember]
        public CustomerProfile Customer { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public DateTime ValidationTime { get; set; }
    }

    #endregion

    #region Customer Models

    /// <summary>
    /// Customer profile information
    /// </summary>
    [DataContract]
    public class CustomerProfile
    {
        [DataMember]
        public string CustomerId { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public CustomerStatus Status { get; set; }

        [DataMember]
        public DateTime CreatedAt { get; set; }

        [DataMember]
        public DateTime LastLoginAt { get; set; }
    }

    /// <summary>
    /// Customer registration request
    /// </summary>
    [DataContract]
    public class CustomerRegistrationRequest
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public string Country { get; set; }
    }

    /// <summary>
    /// Customer registration result
    /// </summary>
    [DataContract]
    public class CustomerRegistrationResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string CustomerId { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Customer profile result
    /// </summary>
    [DataContract]
    public class CustomerProfileResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public CustomerProfile Customer { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Customer update result
    /// </summary>
    [DataContract]
    public class CustomerUpdateResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Password change request
    /// </summary>
    [DataContract]
    public class PasswordChangeRequest
    {
        [DataMember]
        public string CurrentPassword { get; set; }

        [DataMember]
        public string NewPassword { get; set; }

        [DataMember]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Password change result
    /// </summary>
    [DataContract]
    public class PasswordChangeResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }
    }

    /// <summary>
    /// Customer status enumeration
    /// </summary>
    [DataContract]
    public enum CustomerStatus
    {
        [EnumMember]
        Active = 1,
        [EnumMember]
        Inactive = 2,
        [EnumMember]
        Suspended = 3,
        [EnumMember]
        Pending = 4
    }

    #endregion

    #region Account Models

    /// <summary>
    /// Bank account information
    /// </summary>
    [DataContract]
    public class BankAccount
    {
        [DataMember]
        public string AccountNumber { get; set; }

        [DataMember]
        public string CustomerId { get; set; }

        [DataMember]
        public string AccountType { get; set; }

        [DataMember]
        public decimal Balance { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public AccountStatus Status { get; set; }

        [DataMember]
        public DateTime OpenedDate { get; set; }

        [DataMember]
        public DateTime LastTransactionDate { get; set; }

        [DataMember]
        public decimal AvailableBalance { get; set; }

        [DataMember]
        public decimal HoldAmount { get; set; }
    }

    /// <summary>
    /// Account list result
    /// </summary>
    [DataContract]
    public class AccountListResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public List<BankAccount> Accounts { get; set; } = new List<BankAccount>();

        [DataMember]
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Account detail result
    /// </summary>
    [DataContract]
    public class AccountDetailResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public BankAccount Account { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Account balance result
    /// </summary>
    [DataContract]
    public class AccountBalanceResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string AccountNumber { get; set; }

        [DataMember]
        public decimal Balance { get; set; }

        [DataMember]
        public decimal AvailableBalance { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public DateTime LastUpdated { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Account status enumeration
    /// </summary>
    [DataContract]
    public enum AccountStatus
    {
        [EnumMember]
        Active = 1,
        [EnumMember]
        Inactive = 2,
        [EnumMember]
        Frozen = 3,
        [EnumMember]
        Closed = 4
    }

    #endregion

    #region Transaction Models

    /// <summary>
    /// Bank transaction
    /// </summary>
    [DataContract]
    public class Transaction
    {
        [DataMember]
        public string TransactionId { get; set; }

        [DataMember]
        public string AccountNumber { get; set; }

        [DataMember]
        public TransactionType Type { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ReferenceNumber { get; set; }

        [DataMember]
        public TransactionStatus Status { get; set; }

        [DataMember]
        public DateTime TransactionDate { get; set; }

        [DataMember]
        public decimal BalanceAfterTransaction { get; set; }

        [DataMember]
        public string RelatedAccountNumber { get; set; }
    }

    /// <summary>
    /// Transaction history result
    /// </summary>
    [DataContract]
    public class TransactionHistoryResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Transfer request
    /// </summary>
    [DataContract]
    public class TransferRequest
    {
        [DataMember]
        public string FromAccountNumber { get; set; }

        [DataMember]
        public string ToAccountNumber { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ReferenceNumber { get; set; }
    }

    /// <summary>
    /// Transfer result
    /// </summary>
    [DataContract]
    public class TransferResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string TransactionId { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public DateTime TransferDate { get; set; }

        [DataMember]
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Transfer status result
    /// </summary>
    [DataContract]
    public class TransferStatusResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string TransactionId { get; set; }

        [DataMember]
        public TransactionStatus Status { get; set; }

        [DataMember]
        public string StatusMessage { get; set; }

        [DataMember]
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Transaction type enumeration
    /// </summary>
    [DataContract]
    public enum TransactionType
    {
        [EnumMember]
        Deposit = 1,
        [EnumMember]
        Withdrawal = 2,
        [EnumMember]
        Transfer = 3,
        [EnumMember]
        BillPayment = 4,
        [EnumMember]
        Fee = 5,
        [EnumMember]
        Interest = 6
    }

    /// <summary>
    /// Transaction status enumeration
    /// </summary>
    [DataContract]
    public enum TransactionStatus
    {
        [EnumMember]
        Pending = 1,
        [EnumMember]
        Completed = 2,
        [EnumMember]
        Failed = 3,
        [EnumMember]
        Cancelled = 4,
        [EnumMember]
        Reversed = 5
    }

    #endregion

    #region Bill Payment Models

    /// <summary>
    /// Biller category
    /// </summary>
    [DataContract]
    public class BillerCategory
    {
        [DataMember]
        public string CategoryId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string IconUrl { get; set; }
    }

    /// <summary>
    /// Biller information
    /// </summary>
    [DataContract]
    public class Biller
    {
        [DataMember]
        public string BillerId { get; set; }

        [DataMember]
        public string CategoryId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string AccountNumberFormat { get; set; }

        [DataMember]
        public decimal MinimumAmount { get; set; }

        [DataMember]
        public decimal MaximumAmount { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Bill payment request
    /// </summary>
    [DataContract]
    public class BillPaymentRequest
    {
        [DataMember]
        public string FromAccountNumber { get; set; }

        [DataMember]
        public string BillerId { get; set; }

        [DataMember]
        public string AccountNumber { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ReferenceNumber { get; set; }
    }

    /// <summary>
    /// Bill payment result
    /// </summary>
    [DataContract]
    public class BillPaymentResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string TransactionId { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public DateTime PaymentDate { get; set; }

        [DataMember]
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Biller category result
    /// </summary>
    [DataContract]
    public class BillerCategoryResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public List<BillerCategory> Categories { get; set; } = new List<BillerCategory>();

        [DataMember]
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Biller list result
    /// </summary>
    [DataContract]
    public class BillerListResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public List<Biller> Billers { get; set; } = new List<Biller>();

        [DataMember]
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Bill payment history result
    /// </summary>
    [DataContract]
    public class BillPaymentHistoryResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public List<Transaction> Payments { get; set; } = new List<Transaction>();

        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }

    #endregion

    #region System Models

    /// <summary>
    /// System status information
    /// </summary>
    [DataContract]
    public class SystemStatusResult
    {
        [DataMember]
        public bool IsHealthy { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public DateTime ServerTime { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public List<string> Warnings { get; set; } = new List<string>();

        [DataMember]
        public List<string> Errors { get; set; } = new List<string>();
    }

    #endregion
}
