using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace LingeringInTheWorld.Library.Models;

// [SQLite.Table("todos")]
public class ToDo :ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Title { get; set; }=string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime? DeadLine { get; set; }
}