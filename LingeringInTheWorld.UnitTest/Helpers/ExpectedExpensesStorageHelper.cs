using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingeringInTheWorld.UnitTest.Helpers
{
    public class ExpectedExpensesStorageHelper
    {
        public static async Task<ExpectedExpensesStorage> GetInitializedExpectedExpensesStorage()
        {
            var accountingStorage = new ExpectedExpensesStorage();
            await accountingStorage.InitializeAsync();
            return accountingStorage;
        }
    }
}
