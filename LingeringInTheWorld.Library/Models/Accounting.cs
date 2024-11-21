using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingeringInTheWorld.Library.Models
{
    [SQLite.Table("accounting")]
    public class Accounting
    {
        [SQLite.Column("id")]
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        //消费类型
        [SQLite.Column("category")] public string Category { get; set; } = string.Empty;
        //账单类别 （收入/支出）
        [SQLite.Column("type")]
        public string Type { get; set; } = "支出";
        [SQLite.Column("amount")]
        public decimal? Amount { get; set; }
        [SQLite.Column("content")]
        public string Content { get; set; } = string.Empty;
        [SQLite.Column("time")]
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
