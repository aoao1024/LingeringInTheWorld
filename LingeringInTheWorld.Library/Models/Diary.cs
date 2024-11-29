using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LingeringInTheWorld.Library.Models
{
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
        public string Images { get; set; }     // 图片的Base64编码字符串（逗号分隔的字符串）

        

        // 日记前200个字，用于显示预览
        private string _snippet;
        [SQLite.Ignore]
        public string Snippet
        {
            get
            {
                if (_snippet == null)
                {
                    // 去掉所有的空格和换行符
                    string cleanContent = Content.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace(" ", "");
                    // 获取前100个字符，并加上省略号
                    _snippet = cleanContent.Length > 100 ? cleanContent.Substring(0, 100) + "..." : cleanContent;
                }
                return _snippet;
            }
        }
        
        // 返回标签的 List<string>，将"|"分隔的字符串转换为 List<string>
        public List<string> TagsList()
        {
            if (string.IsNullOrEmpty(Tags)) return new List<string>();
            
            return Tags.Replace(" ", "").Split('|').ToList();
        }
        // 返回图片的 List<byte[]>，将 Base64 字符串转换回字节数组
        public List<byte[]> ImageBytesList()
        {
            if (string.IsNullOrEmpty(Images)) return new List<byte[]>();
            
            return Images.Split(',').Select(base64 => Convert.FromBase64String(base64)).ToList();
        }

        
    }
}
