using SQLite;

namespace LingeringInTheWorld.Library.Models;

public class Diary
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }                // 日记ID
    public string Title { get; set; }          // 日记标题
    public string Content { get; set; }        // 日记正文
    public DateTime DateTime { get; set; }     // 创建时间
    public string Weather { get; set; }        // 天气描述
    public string Location { get; set; }       // 定位信息
    public string Tags { get; set; }           // 标签（逗号分隔的字符串）
    public string ImagePaths { get; set; }     // 图片路径（逗号分隔的字符串）
    
    private string _snippet;                   // 用于显示预览的字段
    //日记第一句话，用于显示预览
    [SQLite.Ignore]
    public string Snippet =>
        _snippet ??= Content.Split('。')[0].Replace("\r\n", " ");
    
    // 返回图片路径的 List<string>，如果数据库存储的是逗号分隔字符串
    [SQLite.Ignore]
    public List<string> ImagePathList =>
        string.IsNullOrEmpty(ImagePaths) ? new List<string>() : ImagePaths.Split(',').ToList();
    
    // 用于设置图片路径集合
    public void SetImagePaths(List<string> imagePaths)
    {
        ImagePaths = string.Join(",", imagePaths);
    }
}