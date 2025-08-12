using System;
using System.Collections.Generic;
using System.Linq;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Service for handling account management operations
    /// </summary>
    public class AccountService
    {
        private static readonly Dictionary<string, BankAccount> _accounts = new Dictionary<string, BankAccount>();
        private static int _nextAccountNumber = 1001; // Start account numbers

        static AccountService()
        {
            // Initialize with sample accounts
            InitializeSampleAccounts();
        }

        /// <summary>
        /// Initializes sample accounts for testing
        /// </summary>
        private static void InitializeSampleAccounts()
        {
            // Admin customer accounts
            var adminSavings = new BankAccount
            {
                AccountNumber = "1001",
                CustomerId = "CUST001",
                AccountType = "Savings",
                Balance = 50000.00m,
                Currency = "USD",
                Status = AccountStatus.Active,
                OpenedDate = DateTime.Now.AddDays(-30),
                LastTransactionDate = DateTime.Now.AddDays(-1),
                AvailableBalance = 50000.00m,
                HoldAmount = 0.00m
            };

            var adminChecking = new BankAccount
            {
                AccountNumber = "1002",
                CustomerId = "CUST001",
                AccountType = "Checking",
                Balance = 15000.00m,
                Currency = "USD",
                Status = AccountStatus.Active,
                OpenedDate = DateTime.Now.AddDays(-25),
                LastTransactionDate = DateTime.Now,
                AvailableBalance = 15000.00m,
                HoldAmount = 0.00m
            };

            // Regular customer accounts
            var customerSavings = new BankAccount
            {
                AccountNumber = "1003",
                CustomerId = "CUST002",
                AccountType = "Savings",
                Balance = 25000.00m,
                Currency = "USD",
                Status = AccountStatus.Active,
                OpenedDate = DateTime.Now.AddDays(-15),
                LastTransactionDate = DateTime.Now.AddDays(-2),
                AvailableBalance = 25000.00m,
                HoldAmount = 0.00m
            };

            var customerChecking = new BankAccount
            {
                AccountNumber = "1004",
                CustomerId = "CUST002",
                AccountType = "Checking",
                Balance = 5000.00m,
                Currency = "USD",
                Status = AccountStatus.Active,
                OpenedDate = DateTime.Now.AddDays(-10),
                LastTransactionDate = DateTime.Now.AddDays(-1),
                AvailableBalance = 5000.00m,
                HoldAmount = 0.00m
            };

            _accounts["1001"] = adminSavings;
            _accounts["1002"] = adminChecking;
            _accounts["1003"] = customerSavings;
            _accounts["1004"] = customerChecking;
        }

        /// <summary>
        /// Gets all accounts for a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Account list result</returns>
        public AccountListResult GetCustomerAccounts(string customerId)
        {
            var result = new AccountListResult();

            if (string.IsNullOrEmpty(customerId))
            {
                result.Success = false;
                result.ErrorMessage = "Customer ID is required.";
                return result;
            }

            try
            {
                var customerAccounts = _accounts.Values
                    .Where(a => a.CustomerId == customerId && a.Status == AccountStatus.Active)
                    .ToList();

                result.Success = true;
                result.Accounts = customerAccounts;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Failed to get customer accounts: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Gets account details by account number
        /// </summary>
        /// <param name="accountNumber">Account number</param>
        /// <param name="customerId">Customer ID for authorization</param>
        /// <returns>Account detail result</returns>
        public AccountDetailResult GetAccountDetails(string accountNumber, string customerId)
        {
            var result = new AccountDetailResult();

            if (string.IsNullOrEmpty(accountNumber))
            {
                result.Success = false;
                result.ErrorMessage = "Account number is required.";
                return result;
            }

            if (string.IsNullOrEmpty(customerId))
            {
                result.Success = false;
                result.ErrorMessage = "Customer ID is required.";
                return result;
            }

            try
            {
                if (!_accounts.ContainsKey(accountNumber))
                {
                    result.Success = false;
                    result.ErrorMessage = "Account not found.";
                    return result;
                }

                var account = _accounts[accountNumber];

                // Check if customer owns this account
                if (account.CustomerId != customerId)
                {
                    result.Success = false;
                    result.ErrorMessage = "Access denied: You can only view your own accounts.";
                    return result;
                }

                result.Success = true;
                result.Account = account;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Failed to get account details: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Gets account balance
        /// </summary>
        /// <param name="accountNumber">Account number</param>
        /// <param name="customerId">Customer ID for authorization</param>
        /// <returns>Account balance result</returns>
        public AccountBalanceResult GetAccountBalance(string accountNumber, string customerId)
        {
            var result = new AccountBalanceResult();

            if (string.IsNullOrEmpty(accountNumber))
            {
                result.Success = false;
                result.ErrorMessage = "Account number is required.";
                return result;
            }

            if (string.IsNullOrEmpty(customerId))
            {
                result.Success = false;
                result.ErrorMessage = "Customer ID is required.";
                return result;
            }

            try
            {
                if (!_accounts.ContainsKey(accountNumber))
                {
                    result.Success = false;
                    result.ErrorMessage = "Account not found.";
                    return result;
                }

                var account = _accounts[accountNumber];

                // Check if customer owns this account
                if (account.CustomerId != customerId)
                {
                    result.Success = false;
                    result.ErrorMessage = "Access denied: You can only view your own account balances.";
                    return result;
                }

                result.Success = true;
                result.AccountNumber = accountNumber;
                result.Balance = account.Balance;
                result.AvailableBalance = account.AvailableBalance;
                result.Currency = account.Currency;
                result.LastUpdated = DateTime.Now;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Failed to get account balance: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Updates account balance (used by transaction service)
        /// </summary>
        /// <param name="accountNumber">Account number</param>
        /// <param name="amount">Amount to add/subtract</param>
        /// <param name="transactionType">Type of transaction</param>
        /// <returns>True if successful</returns>
        public bool UpdateAccountBalance(string accountNumber, decimal amount, TransactionType transactionType)
        {
            if (!_accounts.ContainsKey(accountNumber))
                return false;

            var account = _accounts[accountNumber];

            switch (transactionType)
            {
                case TransactionType.Deposit:
                case TransactionType.Interest:
                    account.Balance += amount;
                    account.AvailableBalance += amount;
                    break;

                case TransactionType.Withdrawal:
                case TransactionType.Transfer:
                case TransactionType.BillPayment:
                case TransactionType.Fee:
                    if (account.AvailableBalance >= amount)
                    {
                        account.Balance -= amount;
                        account.AvailableBalance -= amount;
                    }
                    else
                    {
                        return false; // Insufficient funds
                    }
                    break;
            }

            account.LastTransactionDate = DateTime.Now;
            return true;
        }

        /// <summary>
        /// Checks if account has sufficient funds
        /// </summary>
        /// <param name="accountNumber">Account number</param>
        /// <param name="amount">Amount to check</param>
        /// <returns>True if sufficient funds</returns>
        public bool HasSufficientFunds(string accountNumber, decimal amount)
        {
            if (!_accounts.ContainsKey(accountNumber))
                return false;

            var account = _accounts[accountNumber];
            return account.AvailableBalance >= amount && account.Status == AccountStatus.Active;
        }

        /// <summary>
        /// Gets account by account number (internal use)
        /// </summary>
        /// <param name="accountNumber">Account number</param>
        /// <returns>Bank account if found</returns>
        public BankAccount GetAccount(string accountNumber)
        {
            return _accounts.ContainsKey(accountNumber) ? _accounts[accountNumber] : null;
        }

        /// <summary>
        /// Creates a new account
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="accountType">Account type</param>
        /// <param name="initialDeposit">Initial deposit amount</param>
        /// <returns>New account number</returns>
        public string CreateAccount(string customerId, string accountType, decimal initialDeposit)
        {
            var accountNumber = GenerateAccountNumber();
            var account = new BankAccount
            {
                AccountNumber = accountNumber,
                CustomerId = customerId,
                AccountType = accountType,
                Balance = initialDeposit,
                AvailableBalance = initialDeposit,
                Currency = "USD",
                Status = AccountStatus.Active,
                OpenedDate = DateTime.Now,
                LastTransactionDate = DateTime.Now,
                HoldAmount = 0.00m
            };

            _accounts[accountNumber] = account;
            return accountNumber;
        }

        /// <summary>
        /// Generates a unique account number
        /// </summary>
        /// <returns>Account number</returns>
        private string GenerateAccountNumber()
        {
            return (_nextAccountNumber++).ToString();
        }
    }
}
