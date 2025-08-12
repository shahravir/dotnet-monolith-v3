using System;
using System.Collections.Generic;
using System.Linq;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Service for handling bill payment operations
    /// </summary>
    public class BillPaymentService
    {
        private static readonly Dictionary<string, BillerCategory> _billerCategories = new Dictionary<string, BillerCategory>();
        private static readonly Dictionary<string, Biller> _billers = new Dictionary<string, Biller>();
        private static readonly Dictionary<string, Transaction> _billPayments = new Dictionary<string, Transaction>();
        private static int _nextPaymentId = 1001;

        private readonly AccountService _accountService;
        private readonly TransactionService _transactionService;

        public BillPaymentService()
        {
            _accountService = new AccountService();
            _transactionService = new TransactionService();
            InitializeSampleBillers();
        }

        /// <summary>
        /// Initializes sample billers and categories for testing
        /// </summary>
        private void InitializeSampleBillers()
        {
            // Initialize biller categories
            var utilitiesCategory = new BillerCategory
            {
                CategoryId = "UTILITIES",
                Name = "Utilities",
                Description = "Electricity, water, gas, and other utility bills",
                IconUrl = "/icons/utilities.png"
            };

            var telecomCategory = new BillerCategory
            {
                CategoryId = "TELECOM",
                Name = "Telecommunications",
                Description = "Phone, internet, and cable bills",
                IconUrl = "/icons/telecom.png"
            };

            var insuranceCategory = new BillerCategory
            {
                CategoryId = "INSURANCE",
                Name = "Insurance",
                Description = "Health, auto, and home insurance payments",
                IconUrl = "/icons/insurance.png"
            };

            var creditCardCategory = new BillerCategory
            {
                CategoryId = "CREDIT_CARD",
                Name = "Credit Cards",
                Description = "Credit card payments",
                IconUrl = "/icons/credit-card.png"
            };

            _billerCategories["UTILITIES"] = utilitiesCategory;
            _billerCategories["TELECOM"] = telecomCategory;
            _billerCategories["INSURANCE"] = insuranceCategory;
            _billerCategories["CREDIT_CARD"] = creditCardCategory;

            // Initialize billers
            var electricityBiller = new Biller
            {
                BillerId = "ELEC001",
                CategoryId = "UTILITIES",
                Name = "City Power Company",
                Description = "Electricity bill payments",
                AccountNumberFormat = "XXXXXXXXXX",
                MinimumAmount = 10.00m,
                MaximumAmount = 10000.00m,
                Currency = "USD",
                IsActive = true
            };

            var waterBiller = new Biller
            {
                BillerId = "WATER001",
                CategoryId = "UTILITIES",
                Name = "Metro Water Services",
                Description = "Water and sewer bill payments",
                AccountNumberFormat = "XXXXXXXXX",
                MinimumAmount = 5.00m,
                MaximumAmount = 5000.00m,
                Currency = "USD",
                IsActive = true
            };

            var internetBiller = new Biller
            {
                BillerId = "INTERNET001",
                CategoryId = "TELECOM",
                Name = "FastNet Internet",
                Description = "Internet service payments",
                AccountNumberFormat = "XXXXXXXX",
                MinimumAmount = 20.00m,
                MaximumAmount = 2000.00m,
                Currency = "USD",
                IsActive = true
            };

            var phoneBiller = new Biller
            {
                BillerId = "PHONE001",
                CategoryId = "TELECOM",
                Name = "MobileConnect",
                Description = "Mobile phone bill payments",
                AccountNumberFormat = "XXXXXXXXXX",
                MinimumAmount = 15.00m,
                MaximumAmount = 1500.00m,
                Currency = "USD",
                IsActive = true
            };

            var healthInsuranceBiller = new Biller
            {
                BillerId = "HEALTH001",
                CategoryId = "INSURANCE",
                Name = "HealthFirst Insurance",
                Description = "Health insurance premium payments",
                AccountNumberFormat = "XXXXXXXXX",
                MinimumAmount = 50.00m,
                MaximumAmount = 5000.00m,
                Currency = "USD",
                IsActive = true
            };

            var creditCardBiller = new Biller
            {
                BillerId = "CC001",
                CategoryId = "CREDIT_CARD",
                Name = "Global Credit Card",
                Description = "Credit card payments",
                AccountNumberFormat = "XXXXXXXXXXXXXXXX",
                MinimumAmount = 10.00m,
                MaximumAmount = 50000.00m,
                Currency = "USD",
                IsActive = true
            };

            _billers["ELEC001"] = electricityBiller;
            _billers["WATER001"] = waterBiller;
            _billers["INTERNET001"] = internetBiller;
            _billers["PHONE001"] = phoneBiller;
            _billers["HEALTH001"] = healthInsuranceBiller;
            _billers["CC001"] = creditCardBiller;
        }

        /// <summary>
        /// Gets biller categories
        /// </summary>
        /// <returns>Biller category result</returns>
        public BillerCategoryResult GetBillerCategories()
        {
            var result = new BillerCategoryResult();

            try
            {
                result.Success = true;
                result.Categories = _billerCategories.Values.ToList();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Failed to get biller categories: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Gets billers by category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>Biller list result</returns>
        public BillerListResult GetBillersByCategory(string categoryId)
        {
            var result = new BillerListResult();

            if (string.IsNullOrEmpty(categoryId))
            {
                result.Success = false;
                result.ErrorMessage = "Category ID is required.";
                return result;
            }

            try
            {
                if (!_billerCategories.ContainsKey(categoryId))
                {
                    result.Success = false;
                    result.ErrorMessage = "Category not found.";
                    return result;
                }

                var billers = _billers.Values
                    .Where(b => b.CategoryId == categoryId && b.IsActive)
                    .ToList();

                result.Success = true;
                result.Billers = billers;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Failed to get billers: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Pays a bill
        /// </summary>
        /// <param name="billPayment">Bill payment request</param>
        /// <param name="customerId">Customer ID for authorization</param>
        /// <returns>Bill payment result</returns>
        public BillPaymentResult PayBill(BillPaymentRequest billPayment, string customerId)
        {
            var result = new BillPaymentResult();

            // Validate input
            var validationErrors = ValidateBillPaymentRequest(billPayment, customerId);
            if (validationErrors.Count > 0)
            {
                result.Success = false;
                result.Message = "Bill payment validation failed.";
                result.ValidationErrors = validationErrors;
                return result;
            }

            try
            {
                // Check if source account has sufficient funds
                if (!_accountService.HasSufficientFunds(billPayment.FromAccountNumber, billPayment.Amount))
                {
                    result.Success = false;
                    result.Message = "Insufficient funds in source account.";
                    result.ValidationErrors.Add("Insufficient funds in source account.");
                    return result;
                }

                // Get biller information
                var biller = _billers[billPayment.BillerId];

                // Generate transaction ID
                var transactionId = GeneratePaymentId();

                // Create bill payment transaction
                var paymentTransaction = new Transaction
                {
                    TransactionId = transactionId,
                    AccountNumber = billPayment.FromAccountNumber,
                    Type = TransactionType.BillPayment,
                    Amount = billPayment.Amount,
                    Currency = "USD",
                    Description = $"Bill payment to {biller.Name} - {billPayment.Description}",
                    ReferenceNumber = billPayment.ReferenceNumber,
                    Status = TransactionStatus.Completed,
                    TransactionDate = DateTime.Now,
                    RelatedAccountNumber = billPayment.AccountNumber
                };

                // Update account balance
                var balanceUpdateSuccess = _accountService.UpdateAccountBalance(billPayment.FromAccountNumber, billPayment.Amount, TransactionType.BillPayment);

                if (balanceUpdateSuccess)
                {
                    // Store transaction
                    _billPayments[transactionId] = paymentTransaction;

                    result.Success = true;
                    result.TransactionId = transactionId;
                    result.Message = $"Bill payment to {biller.Name} completed successfully.";
                    result.PaymentDate = DateTime.Now;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Bill payment failed due to account update error.";
                    result.ValidationErrors.Add("Bill payment failed due to account update error.");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Bill payment failed: {ex.Message}";
                result.ValidationErrors.Add(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets bill payment history for a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Bill payment history result</returns>
        public BillPaymentHistoryResult GetBillPaymentHistory(string customerId, int pageNumber = 1, int pageSize = 20)
        {
            var result = new BillPaymentHistoryResult();

            if (string.IsNullOrEmpty(customerId))
            {
                result.Success = false;
                result.ErrorMessage = "Customer ID is required.";
                return result;
            }

            try
            {
                // Get customer's accounts
                var customerAccounts = _accountService.GetCustomerAccounts(customerId);
                if (!customerAccounts.Success)
                {
                    result.Success = false;
                    result.ErrorMessage = "Failed to get customer accounts.";
                    return result;
                }

                var accountNumbers = customerAccounts.Accounts.Select(a => a.AccountNumber).ToList();

                // Get bill payments for customer's accounts
                var payments = _billPayments.Values
                    .Where(p => accountNumbers.Contains(p.AccountNumber))
                    .OrderByDescending(p => p.TransactionDate)
                    .ToList();

                // Apply pagination
                var totalCount = payments.Count;
                var skip = (pageNumber - 1) * pageSize;
                var pagedPayments = payments.Skip(skip).Take(pageSize).ToList();

                result.Success = true;
                result.Payments = pagedPayments;
                result.TotalCount = totalCount;
                result.PageNumber = pageNumber;
                result.PageSize = pageSize;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Failed to get bill payment history: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Validates bill payment request
        /// </summary>
        /// <param name="billPayment">Bill payment request</param>
        /// <param name="customerId">Customer ID</param>
        /// <returns>List of validation errors</returns>
        private List<string> ValidateBillPaymentRequest(BillPaymentRequest billPayment, string customerId)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(billPayment.FromAccountNumber))
                errors.Add("Source account number is required.");

            if (string.IsNullOrEmpty(billPayment.BillerId))
                errors.Add("Biller ID is required.");

            if (string.IsNullOrEmpty(billPayment.AccountNumber))
                errors.Add("Biller account number is required.");

            if (billPayment.Amount <= 0)
                errors.Add("Payment amount must be greater than zero.");

            // Check if source account belongs to customer
            var sourceAccount = _accountService.GetAccount(billPayment.FromAccountNumber);
            if (sourceAccount == null)
            {
                errors.Add("Source account not found.");
            }
            else if (sourceAccount.CustomerId != customerId)
            {
                errors.Add("You can only pay bills from your own accounts.");
            }

            // Check if biller exists and is active
            if (!_billers.ContainsKey(billPayment.BillerId))
            {
                errors.Add("Biller not found.");
            }
            else
            {
                var biller = _billers[billPayment.BillerId];
                if (!biller.IsActive)
                {
                    errors.Add("Biller is not active.");
                }
                else
                {
                    // Validate amount limits
                    if (billPayment.Amount < biller.MinimumAmount)
                        errors.Add($"Payment amount must be at least {biller.MinimumAmount:C}.");

                    if (billPayment.Amount > biller.MaximumAmount)
                        errors.Add($"Payment amount cannot exceed {biller.MaximumAmount:C}.");
                }
            }

            return errors;
        }

        /// <summary>
        /// Generates a unique payment ID
        /// </summary>
        /// <returns>Payment ID</returns>
        private string GeneratePaymentId()
        {
            return $"PAY{_nextPaymentId++:D6}";
        }
    }
}
