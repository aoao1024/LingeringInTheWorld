using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingeringInTheWorld.Library.Models
{
    /// <summary>
    /// 每月预期消费表
    /// </summary>
    [SQLite.Table("expected_expenses")]
    public class ExpectedExpenses
    {
        [SQLite.Column("id")] [PrimaryKey, AutoIncrement] public int Id { get; set; }
        [SQLite.Column("year")] public int Year { get; set; }
        [SQLite.Column("month")] public int Month { get; set; }
        [SQLite.Column("value")] public decimal Value { get; set; }
    }
}
