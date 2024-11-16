using CommunityToolkit.Mvvm.ComponentModel;

namespace LingeringInTheWorld.Library.Models;

// [SQLite.Table("todos")]
public class ToDo :ObservableObject
{
    // [SQLite.Column("id")] public int Id { get; set; }
    // [SQLite.Column("title")] public string Title { get; set; }=string.Empty;
    // [SQLite.Column("content")]
    // public string Content { get; set; } = string.Empty;
    // [SQLite.Column("status")] public int Status { get; set; }
    // [SQLite.Column("deadline")] public DateTime DeadLine { get; set; }
    
    //1115
     public int Id { get; set; }
     public string Title { get; set; }=string.Empty;
    
    public string Content { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime DeadLine { get; set; }
}