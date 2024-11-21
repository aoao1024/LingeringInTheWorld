using LingeringInTheWorld.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LingeringInTheWorld.Library.Services
{
    public class MonthStatisticsService : IMonthStatisticsService
    {
        private readonly IAccountingStorage _accountingStorage;
        public MonthStatisticsService(IAccountingStorage accountingStorage)
        {
            _accountingStorage = accountingStorage;
        }

        public async Task<MonthStatistics> GetMonthStatisticsAsync(DateTime date)
        {
            var accountingList = await _accountingStorage.GetMonthAccountingAsync(date);
            var monthStatistics = new MonthStatistics();
            monthStatistics.Year = date.Year;
            monthStatistics.Month = date.Month;
            monthStatistics.TotalIncome = accountingList.Where(x => x.Type == "收入").Sum(x => x.Amount ?? 0);
            monthStatistics.TotalOutcome = accountingList.Where(x => x.Type == "支出").Sum(x => x.Amount ?? 0);

            return monthStatistics;
        }
       

    }
}
