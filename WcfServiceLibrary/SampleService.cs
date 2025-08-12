using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WcfServiceLibrary
{
    /// <summary>
    /// Implementation of the Mobile Banking Gateway Service
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class BankingGatewayService : IBankingGatewayService
    {
        private readonly AuthenticationService _authService;
        private readonly CustomerService _customerService;
        private readonly AccountService _accountService;
        private readonly TransactionService _transactionService;
        private readonly BillPaymentService _billPaymentService;

        public BankingGatewayService()
        {
            _authService = new AuthenticationService();
            _customerService = new CustomerService();
            _accountService = new AccountService();
            _transactionService = new TransactionService();
            _billPaymentService = new BillPaymentService();
        }

        #region Authentication & Security

        public LoginResult Login(string username, string password)
        {
            try
            {
                return _authService.AuthenticateCustomer(username, password);
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = $"Login failed: {ex.Message}",
                    LoginTime = DateTime.Now
                };
            }
        }

        public LogoutResult Logout(string token)
        {
            try
            {
                return _authService.LogoutCustomer(token);
            }
            catch (Exception ex)
            {
                return new LogoutResult
                {
                    Success = false,
                    Message = $"Logout failed: {ex.Message}",
                    LogoutTime = DateTime.Now
                };
            }
        }

        public TokenValidationResult ValidateToken(string token)
        {
            try
            {
                return _authService.ValidateCustomerToken(token);
            }
            catch (Exception ex)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Token validation failed: {ex.Message}",
                    ValidationTime = DateTime.Now
                };
            }
        }

        #endregion

        #region Customer Management

        public CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest customerRegistration)
        {
            try
            {
                return _customerService.RegisterCustomer(customerRegistration);
            }
            catch (Exception ex)
            {
                return new CustomerRegistrationResult
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public CustomerProfileResult GetCustomerProfile(string token, string customerId)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new CustomerProfileResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid authentication token"
                    };
                }

                if (customer.CustomerId != customerId)
                {
                    return new CustomerProfileResult
                    {
                        Success = false,
                        ErrorMessage = "Access denied: Customer ID mismatch"
                    };
                }

                return new CustomerProfileResult
                {
                    Success = true,
                    Customer = customer
                };
            }
            catch (Exception ex)
            {
                return new CustomerProfileResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to get customer profile: {ex.Message}"
                };
            }
        }

        public CustomerUpdateResult UpdateCustomerProfile(string token, CustomerProfile customerProfile)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new CustomerUpdateResult
                    {
                        Success = false,
                        Message = "Invalid authentication token",
                        ValidationErrors = new List<string> { "Invalid authentication token" }
                    };
                }

                return _customerService.UpdateCustomerProfile(customerProfile);
            }
            catch (Exception ex)
            {
                return new CustomerUpdateResult
                {
                    Success = false,
                    Message = $"Update failed: {ex.Message}",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public PasswordChangeResult ChangePassword(string token, PasswordChangeRequest passwordChange)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new PasswordChangeResult
                    {
                        Success = false,
                        Message = "Invalid authentication token"
                    };
                }

                return _customerService.ChangePassword(customer.CustomerId, passwordChange);
            }
            catch (Exception ex)
            {
                return new PasswordChangeResult
                {
                    Success = false,
                    Message = $"Password change failed: {ex.Message}"
                };
            }
        }

        #endregion

        #region Account Management

        public AccountListResult GetCustomerAccounts(string token, string customerId)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new AccountListResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid authentication token"
                    };
                }

                if (customer.CustomerId != customerId)
                {
                    return new AccountListResult
                    {
                        Success = false,
                        ErrorMessage = "Access denied: Customer ID mismatch"
                    };
                }

                return _accountService.GetCustomerAccounts(customerId);
            }
            catch (Exception ex)
            {
                return new AccountListResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to get accounts: {ex.Message}"
                };
            }
        }

        public AccountDetailResult GetAccountDetails(string token, string accountNumber)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new AccountDetailResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid authentication token"
                    };
                }

                return _accountService.GetAccountDetails(accountNumber, customer.CustomerId);
            }
            catch (Exception ex)
            {
                return new AccountDetailResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to get account details: {ex.Message}"
                };
            }
        }

        public AccountBalanceResult GetAccountBalance(string token, string accountNumber)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new AccountBalanceResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid authentication token"
                    };
                }

                return _accountService.GetAccountBalance(accountNumber, customer.CustomerId);
            }
            catch (Exception ex)
            {
                return new AccountBalanceResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to get account balance: {ex.Message}"
                };
            }
        }

        #endregion

        #region Transaction Management

        public TransactionHistoryResult GetTransactionHistory(string token, string accountNumber, DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new TransactionHistoryResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid authentication token"
                    };
                }

                return _transactionService.GetTransactionHistory(accountNumber, customer.CustomerId, fromDate, toDate, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                return new TransactionHistoryResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to get transaction history: {ex.Message}"
                };
            }
        }

        public TransferResult TransferMoney(string token, TransferRequest transferRequest)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new TransferResult
                    {
                        Success = false,
                        Message = "Invalid authentication token",
                        ValidationErrors = new List<string> { "Invalid authentication token" }
                    };
                }

                return _transactionService.TransferMoney(transferRequest, customer.CustomerId);
            }
            catch (Exception ex)
            {
                return new TransferResult
                {
                    Success = false,
                    Message = $"Transfer failed: {ex.Message}",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public TransferStatusResult GetTransferStatus(string token, string transactionId)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new TransferStatusResult
                    {
                        Success = false,
                        StatusMessage = "Invalid authentication token"
                    };
                }

                return _transactionService.GetTransferStatus(transactionId, customer.CustomerId);
            }
            catch (Exception ex)
            {
                return new TransferStatusResult
                {
                    Success = false,
                    StatusMessage = $"Failed to get transfer status: {ex.Message}"
                };
            }
        }

        #endregion

        #region Bill Payments

        public BillerCategoryResult GetBillerCategories(string token)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new BillerCategoryResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid authentication token"
                    };
                }

                return _billPaymentService.GetBillerCategories();
            }
            catch (Exception ex)
            {
                return new BillerCategoryResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to get biller categories: {ex.Message}"
                };
            }
        }

        public BillerListResult GetBillersByCategory(string token, string categoryId)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new BillerListResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid authentication token"
                    };
                }

                return _billPaymentService.GetBillersByCategory(categoryId);
            }
            catch (Exception ex)
            {
                return new BillerListResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to get billers: {ex.Message}"
                };
            }
        }

        public BillPaymentResult PayBill(string token, BillPaymentRequest billPayment)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new BillPaymentResult
                    {
                        Success = false,
                        Message = "Invalid authentication token",
                        ValidationErrors = new List<string> { "Invalid authentication token" }
                    };
                }

                return _billPaymentService.PayBill(billPayment, customer.CustomerId);
            }
            catch (Exception ex)
            {
                return new BillPaymentResult
                {
                    Success = false,
                    Message = $"Bill payment failed: {ex.Message}",
                    ValidationErrors = new List<string> { ex.Message }
                };
            }
        }

        public BillPaymentHistoryResult GetBillPaymentHistory(string token, string customerId, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var customer = _authService.GetCustomerByToken(token);
                if (customer == null)
                {
                    return new BillPaymentHistoryResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid authentication token"
                    };
                }

                if (customer.CustomerId != customerId)
                {
                    return new BillPaymentHistoryResult
                    {
                        Success = false,
                        ErrorMessage = "Access denied: Customer ID mismatch"
                    };
                }

                return _billPaymentService.GetBillPaymentHistory(customerId, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                return new BillPaymentHistoryResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to get bill payment history: {ex.Message}"
                };
            }
        }

        #endregion

        #region Utility Methods

        public DateTime GetServerTime()
        {
            return DateTime.Now;
        }

        public SystemStatusResult GetSystemStatus()
        {
            return new SystemStatusResult
            {
                IsHealthy = true,
                Version = "1.0.0",
                ServerTime = DateTime.Now,
                Status = "Operational",
                Warnings = new List<string>(),
                Errors = new List<string>()
            };
        }

        #endregion
    }
}
