using System;
using System.Collections.Generic;
using System.Linq;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Service for handling transaction management operations
    /// </summary>
    public class TransactionService
    {
        private static readonly Dictionary<string, Transaction> _transactions = new Dictionary<string, Transaction>();
        private static readonly Dictionary<string, TransferRequest> _pendingTransfers = new Dictionary<string, TransferRequest>();
        private static int _nextTransactionId = 1001;

        private readonly AccountService _accountService;

        public TransactionService()
        {
            _accountService = new AccountService();
            InitializeSampleTransactions();
        }

        /// <summary>
        /// Initializes sample transactions for testing
        /// </summary>
        private void InitializeSampleTransactions()
        {
            // Sample transactions for admin customer
            CreateSampleTransaction("1001", TransactionType.Deposit, 50000.00m, "Initial deposit", "REF001");
            CreateSampleTransaction("1002", TransactionType.Deposit, 15000.00m, "Initial deposit", "REF002");
            CreateSampleTransaction("1002", TransactionType.Withdrawal, 500.00m, "ATM withdrawal", "REF003");
            CreateSampleTransaction("1002", TransactionType.Transfer, 1000.00m, "Transfer to savings", "REF004", "1001");

            // Sample transactions for regular customer
            CreateSampleTransaction("1003", TransactionType.Deposit, 25000.00m, "Initial deposit", "REF005");
            CreateSampleTransaction("1004", TransactionType.Deposit, 5000.00m, "Initial deposit", "REF006");
            CreateSampleTransaction("1004", TransactionType.Withdrawal, 200.00m, "ATM withdrawal", "REF007");
            CreateSampleTransaction("1004", TransactionType.BillPayment, 150.00m, "Electricity bill payment", "REF008");
        }

        /// <summary>
        /// Gets transaction history for an account
        /// </summary>
        /// <param name="accountNumber">Account number</param>
        /// <param name="customerId">Customer ID for authorization</param>
        /// <param name="fromDate">Start date</param>
        /// <param name="toDate">End date</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Transaction history result</returns>
        public TransactionHistoryResult GetTransactionHistory(string accountNumber, string customerId, DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 20)
        {
            var result = new TransactionHistoryResult();

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
                // Verify account ownership
                var account = _accountService.GetAccount(accountNumber);
                if (account == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Account not found.";
                    return result;
                }

                if (account.CustomerId != customerId)
                {
                    result.Success = false;
                    result.ErrorMessage = "Access denied: You can only view your own transaction history.";
                    return result;
                }

                // Get transactions for the account within date range
                var transactions = _transactions.Values
                    .Where(t => t.AccountNumber == accountNumber &&
                               t.TransactionDate >= fromDate &&
                               t.TransactionDate <= toDate)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();

                // Apply pagination
                var totalCount = transactions.Count;
                var skip = (pageNumber - 1) * pageSize;
                var pagedTransactions = transactions.Skip(skip).Take(pageSize).ToList();

                result.Success = true;
                result.Transactions = pagedTransactions;
                result.TotalCount = totalCount;
                result.PageNumber = pageNumber;
                result.PageSize = pageSize;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Failed to get transaction history: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Transfers money between accounts
        /// </summary>
        /// <param name="transferRequest">Transfer request</param>
        /// <param name="customerId">Customer ID for authorization</param>
        /// <returns>Transfer result</returns>
        public TransferResult TransferMoney(TransferRequest transferRequest, string customerId)
        {
            var result = new TransferResult();

            // Validate input
            var validationErrors = ValidateTransferRequest(transferRequest, customerId);
            if (validationErrors.Count > 0)
            {
                result.Success = false;
                result.Message = "Transfer validation failed.";
                result.ValidationErrors = validationErrors;
                return result;
            }

            try
            {
                // Check if source account has sufficient funds
                if (!_accountService.HasSufficientFunds(transferRequest.FromAccountNumber, transferRequest.Amount))
                {
                    result.Success = false;
                    result.Message = "Insufficient funds in source account.";
                    result.ValidationErrors.Add("Insufficient funds in source account.");
                    return result;
                }

                // Check if destination account exists
                var destinationAccount = _accountService.GetAccount(transferRequest.ToAccountNumber);
                if (destinationAccount == null)
                {
                    result.Success = false;
                    result.Message = "Destination account not found.";
                    result.ValidationErrors.Add("Destination account not found.");
                    return result;
                }

                // Generate transaction ID
                var transactionId = GenerateTransactionId();

                // Create debit transaction
                var debitTransaction = new Transaction
                {
                    TransactionId = transactionId + "_DEBIT",
                    AccountNumber = transferRequest.FromAccountNumber,
                    Type = TransactionType.Transfer,
                    Amount = transferRequest.Amount,
                    Currency = "USD",
                    Description = $"Transfer to {transferRequest.ToAccountNumber}: {transferRequest.Description}",
                    ReferenceNumber = transferRequest.ReferenceNumber,
                    Status = TransactionStatus.Completed,
                    TransactionDate = DateTime.Now,
                    RelatedAccountNumber = transferRequest.ToAccountNumber
                };

                // Create credit transaction
                var creditTransaction = new Transaction
                {
                    TransactionId = transactionId + "_CREDIT",
                    AccountNumber = transferRequest.ToAccountNumber,
                    Type = TransactionType.Transfer,
                    Amount = transferRequest.Amount,
                    Currency = "USD",
                    Description = $"Transfer from {transferRequest.FromAccountNumber}: {transferRequest.Description}",
                    ReferenceNumber = transferRequest.ReferenceNumber,
                    Status = TransactionStatus.Completed,
                    TransactionDate = DateTime.Now,
                    RelatedAccountNumber = transferRequest.FromAccountNumber
                };

                // Update account balances
                var debitSuccess = _accountService.UpdateAccountBalance(transferRequest.FromAccountNumber, transferRequest.Amount, TransactionType.Transfer);
                var creditSuccess = _accountService.UpdateAccountBalance(transferRequest.ToAccountNumber, transferRequest.Amount, TransactionType.Transfer);

                if (debitSuccess && creditSuccess)
                {
                    // Store transactions
                    _transactions[debitTransaction.TransactionId] = debitTransaction;
                    _transactions[creditTransaction.TransactionId] = creditTransaction;

                    // Store pending transfer for status tracking
                    _pendingTransfers[transactionId] = transferRequest;

                    result.Success = true;
                    result.TransactionId = transactionId;
                    result.Message = "Transfer completed successfully.";
                    result.TransferDate = DateTime.Now;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Transfer failed due to account update error.";
                    result.ValidationErrors.Add("Transfer failed due to account update error.");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Transfer failed: {ex.Message}";
                result.ValidationErrors.Add(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets transfer status
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <param name="customerId">Customer ID for authorization</param>
        /// <returns>Transfer status result</returns>
        public TransferStatusResult GetTransferStatus(string transactionId, string customerId)
        {
            var result = new TransferStatusResult();

            if (string.IsNullOrEmpty(transactionId))
            {
                result.Success = false;
                result.StatusMessage = "Transaction ID is required.";
                return result;
            }

            if (string.IsNullOrEmpty(customerId))
            {
                result.Success = false;
                result.StatusMessage = "Customer ID is required.";
                return result;
            }

            try
            {
                // Find transactions related to this transfer
                var relatedTransactions = _transactions.Values
                    .Where(t => t.TransactionId.StartsWith(transactionId))
                    .ToList();

                if (relatedTransactions.Count == 0)
                {
                    result.Success = false;
                    result.StatusMessage = "Transfer not found.";
                    return result;
                }

                // Check if customer owns any of the accounts involved
                var customerOwnsTransfer = relatedTransactions.Any(t => 
                {
                    var account = _accountService.GetAccount(t.AccountNumber);
                    return account != null && account.CustomerId == customerId;
                });

                if (!customerOwnsTransfer)
                {
                    result.Success = false;
                    result.StatusMessage = "Access denied: You can only view your own transfers.";
                    return result;
                }

                // Get the main transaction (debit transaction)
                var mainTransaction = relatedTransactions.FirstOrDefault(t => t.TransactionId.EndsWith("_DEBIT"));
                if (mainTransaction != null)
                {
                    result.Success = true;
                    result.TransactionId = transactionId;
                    result.Status = mainTransaction.Status;
                    result.StatusMessage = GetStatusMessage(mainTransaction.Status);
                    result.LastUpdated = mainTransaction.TransactionDate;
                }
                else
                {
                    result.Success = false;
                    result.StatusMessage = "Transfer status not available.";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.StatusMessage = $"Failed to get transfer status: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Validates transfer request
        /// </summary>
        /// <param name="transferRequest">Transfer request</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>List of validation errors</returns>
        private List<string> ValidateTransferRequest(TransferRequest transferRequest, string customerId)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(transferRequest.FromAccountNumber))
                errors.Add("Source account number is required.");

            if (string.IsNullOrEmpty(transferRequest.ToAccountNumber))
                errors.Add("Destination account number is required.");

            if (transferRequest.Amount <= 0)
                errors.Add("Transfer amount must be greater than zero.");

            if (transferRequest.FromAccountNumber == transferRequest.ToAccountNumber)
                errors.Add("Cannot transfer to the same account.");

            // Check if source account belongs to customer
            var sourceAccount = _accountService.GetAccount(transferRequest.FromAccountNumber);
            if (sourceAccount == null)
            {
                errors.Add("Source account not found.");
            }
            else if (sourceAccount.CustomerId != customerId)
            {
                errors.Add("You can only transfer from your own accounts.");
            }

            return errors;
        }

        /// <summary>
        /// Gets status message for transaction status
        /// </summary>
        /// <param name="status">Transaction status</param>
        /// <returns>Status message</returns>
        private string GetStatusMessage(TransactionStatus status)
        {
            switch (status)
            {
                case TransactionStatus.Pending:
                    return "Transfer is pending processing.";
                case TransactionStatus.Completed:
                    return "Transfer completed successfully.";
                case TransactionStatus.Failed:
                    return "Transfer failed.";
                case TransactionStatus.Cancelled:
                    return "Transfer was cancelled.";
                case TransactionStatus.Reversed:
                    return "Transfer was reversed.";
                default:
                    return "Unknown status.";
            }
        }

        /// <summary>
        /// Creates a sample transaction for testing
        /// </summary>
        /// <param name="accountNumber">Account number</param>
        /// <param name="type">Transaction type</param>
        /// <param name="amount">Amount</param>
        /// <param name="description">Description</param>
        /// <param name="referenceNumber">Reference number</param>
        /// <param name="relatedAccountNumber">Related account number</param>
        private void CreateSampleTransaction(string accountNumber, TransactionType type, decimal amount, string description, string referenceNumber, string relatedAccountNumber = null)
        {
            var transaction = new Transaction
            {
                TransactionId = GenerateTransactionId(),
                AccountNumber = accountNumber,
                Type = type,
                Amount = amount,
                Currency = "USD",
                Description = description,
                ReferenceNumber = referenceNumber,
                Status = TransactionStatus.Completed,
                TransactionDate = DateTime.Now.AddDays(-new Random().Next(1, 30)),
                RelatedAccountNumber = relatedAccountNumber
            };

            _transactions[transaction.TransactionId] = transaction;
        }

        /// <summary>
        /// Generates a unique transaction ID
        /// </summary>
        /// <returns>Transaction ID</returns>
        private string GenerateTransactionId()
        {
            return $"TXN{_nextTransactionId++:D6}";
        }
    }
}
