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
                    _snippet = Content.Length > 150 ? Content.Substring(0, 150)+"......" : Content;
                }
                return _snippet;
            }
        }
        
        // 用于设置标签集合
        public void SetTags(List<string> tags)
        {
            Tags = string.Join(" | ", tags);
        }
        
        // 返回标签的 List<string>
        [SQLite.Ignore]
        public List<string> TagList =>
            string.IsNullOrEmpty(Tags) ? new List<string>() : Tags.Split('|').ToList();
        
        
        // 将字节数组集合存储为字符串（例如，存储为以逗号分隔的Base64字符串）
        public void SetImageBytes(List<byte[]> imageBytes)
        {
            if (imageBytes != null && imageBytes.Any())
            {
                // 将每个字节数组转换为Base64字符串并存储
                var base64Strings = imageBytes.Select(image => Convert.ToBase64String(image)).ToList();
                Images = string.Join(",", base64Strings);
            }
            else
            {
                Images = string.Empty;
            }
        }

        // 返回图片的 List<byte[]>，将 Base64 字符串转换回字节数组
        public List<byte[]> ImageBytesList()
        {
            if (string.IsNullOrEmpty(Images)) return new List<byte[]>();
            
            return Images.Split(',').Select(base64 => Convert.FromBase64String(base64)).ToList();
        }

        
    }
}
