using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.UnitTest.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LingeringInTheWorld.UnitTest.Services
{

    public class ExpectedExpensesStorageTest
    {

        [Fact]
        public async Task SaveExpectedExpensesAsync_GetExpectedExpensesAsync_Default()
        {
            var updated = false;
            ExpectedExpenses updatedExpectedExpenses = null;

            var ExpectedExpensesStorage = new ExpectedExpensesStorage();
            ExpectedExpensesStorage.Updated += (_, args) => {
                updated = true;
                updatedExpectedExpenses = args.UpdatedExpectedExpenses;
            };
            await ExpectedExpensesStorage.InitializeAsync();

            var ExpectedExpensesToSave = new ExpectedExpenses
            {
                Year = 2024,
                Month = 11,
                Value = 2000
            };
            await ExpectedExpensesStorage.SaveExpectedExpensesAsync(ExpectedExpensesToSave);

            var ExpectedExpenses =
                await ExpectedExpensesStorage.GetExpectedExpenses(ExpectedExpensesToSave.Id);
            Assert.Equal(ExpectedExpensesToSave.Id, ExpectedExpenses.Id);
            Assert.Equal(ExpectedExpensesToSave.Month, ExpectedExpenses.Month);
            Assert.Equal(ExpectedExpensesToSave.Year, ExpectedExpenses.Year);
            Assert.Equal(ExpectedExpensesToSave.Value, ExpectedExpenses.Value);

            Assert.True(updated);
            Assert.Same(ExpectedExpensesToSave, updatedExpectedExpenses);

            await ExpectedExpensesStorage.CloseAsync();
        }

        [Fact]
        public async Task GetExpectedExpensessAsync_Default()
        {
            var ExpectedExpensesStorage = new ExpectedExpensesStorage();
            await ExpectedExpensesStorage.InitializeAsync();

            var ExpectedExpensesListToSave = new List<ExpectedExpenses>();
            var random = new Random();
            for (var i = 0; i < 5; i++)
            {
                ExpectedExpensesListToSave.Add(new ExpectedExpenses
                {
                    Year = 2024,
                    Month = i + 1,
                    Value = 2000
                });
                await Task.Delay(10);
            }

            foreach (var ExpectedExpensesToSave in ExpectedExpensesListToSave)
            {
                await ExpectedExpensesStorage.SaveExpectedExpensesAsync(ExpectedExpensesToSave);
            }

            var list = await ExpectedExpensesStorage.GetExpectedExpensesAsync(
            Expression.Lambda<Func<ExpectedExpenses, bool>>(Expression.Constant(true),
                Expression.Parameter(typeof(ExpectedExpenses), "p")), 0, int.MaxValue);
            Assert.True(list.Count() > 0);
            await ExpectedExpensesStorage.CloseAsync();
        }

        [Fact]
        public async Task GetMonthExpectedExpensesAsync_Default()
        {
            var ExpectedExpensesStorage = new ExpectedExpensesStorage();
            var model = await ExpectedExpensesStorage.GetMonthExpectedExpensesAsync(2024, 1);
            Assert.True(model != null);
            Assert.Equal(1, model.Month);
            Assert.Equal(2024, model.Year);

            await ExpectedExpensesStorage.CloseAsync();
        }

        private static IPreferenceStorage GetEmptyPreferenceStorage() =>
            new Mock<IPreferenceStorage>().Object;
    }
}
