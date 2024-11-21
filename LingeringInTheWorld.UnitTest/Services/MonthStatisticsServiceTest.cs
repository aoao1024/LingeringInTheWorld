using LingeringInTheWorld.Library.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingeringInTheWorld.UnitTest.Services
{
    public class MonthStatisticsServiceTest
    {
        [Fact]
        public async Task GetMonthStatisticsAsync_Default()
        {
            var preferenceStorageMock = new Mock<IPreferenceStorage>();
            var mockPreferenceStorage = preferenceStorageMock.Object;
            var AccountingStorage = new AccountingStorage(mockPreferenceStorage);

            var service = new MonthStatisticsService(AccountingStorage);
            var today = DateTime.Now;
            var data = await service.GetMonthStatisticsAsync(DateTime.Now);

            Assert.True(data != null);
            Assert.Equal(today.Month, data.Month);
            Assert.Equal(today.Year, data.Year);
            await AccountingStorage.CloseAsync();
        }
    }
}
