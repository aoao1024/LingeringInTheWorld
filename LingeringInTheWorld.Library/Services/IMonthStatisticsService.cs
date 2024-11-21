using LingeringInTheWorld.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingeringInTheWorld.Library.Services
{
    public interface IMonthStatisticsService
    {
        Task<MonthStatistics> GetMonthStatisticsAsync(DateTime date);
    }
}
