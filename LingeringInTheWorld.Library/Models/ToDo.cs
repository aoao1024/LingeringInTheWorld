namespace LingeringInTheWorld.Library.Models;


public class ToDo
{
    [SQLite.Column("id")] public int Id { get; set; }
    [SQLite.Column("title")] public string Title { get; set; }=string.Empty;
    [SQLite.Column("content")]
    public string Content { get; set; } = string.Empty;
    [SQLite.Column("status")] public int Status { get; set; }
    [SQLite.Column("deadline")] public DateTime DeadLine { get; set; }
}