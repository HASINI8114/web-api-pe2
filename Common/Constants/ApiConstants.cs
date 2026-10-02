namespace GDB.Api.Common.Constants
{
    public static class ApiConstants
    {
        #region ApiVersion
        public const string Version1 = "1.0";
        public const string Version2 = "2.0";
        #endregion

        #region BaseRoutes
        // "v{version:apiVersion}" lets Asp.Versioning read the version from the URL: /api/v1/..., /api/v2/...
        public const string BaseApi = "api/v{version:apiVersion}";
        public const string BaseAccounts = BaseApi + "/accounts";
        public const string BaseTransactions = BaseApi + "/transactions";
        public const string BaseTemplates = BaseApi + "/templates";
        #endregion

        #region AccountRoutes (relative to BaseAccounts)
        // GET    /api/v1/accounts                -> view all accounts (use [HttpGet] with no template)
        // POST   /api/v1/accounts                -> create account (use [HttpPost] with no template)
        // GET    /api/v1/accounts/{accNo}        -> get account
        public const string AccountByNumber = "{accNo}";
        // GET    /api/v1/accounts/{accNo}/balance -> view balance
        public const string AccountBalance = "{accNo}/balance";
        // GET    /api/v1/accounts/{accNo}/view   -> view account details
        public const string AccountView = "{accNo}/view";
        // POST   /api/v1/accounts/close          -> close account
        public const string CloseAccount = "close";
        #endregion

        #region TransactionRoutes (relative to BaseTransactions)
        // POST   /api/v1/transactions/deposit    -> deposit
        public const string Deposit = "deposit";
        // POST   /api/v1/transactions/withdraw   -> withdraw
        public const string Withdraw = "withdraw";
        // POST   /api/v1/transactions/transfer   -> transfer funds
        public const string Transfer = "transfer";
        #endregion

        #region TemplateRoutes (relative to BaseTemplates)
        public const string TemplateById = "{id:int}";
        public const string TemplateItem = "{id:int}/items/{itemId:guid}";
        public const string TemplateAction = "[action]";
        // "~/" ignores the controller-level route, so this one has no version segment
        public const string TemplatePing = "~/api/templates/ping";
        #endregion
    }
}
