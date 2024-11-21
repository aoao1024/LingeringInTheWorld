
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LingeringInTheWorld.Library.Services
{
    public interface IAIService
    {
        Task<ExtractedAccounting> ExtractAccounting(string inputText);
    }

    public class AIService : IAIService
    {
        private const string ApiUrl = "https://audit.iflyosil.com/audit/v2/syncText";
        private const string AppId = "a777f55f";
        private const string ApiKey = "47d4ca5d6b1429463afb5d27d6241012";
        private const string ApiSecret = "ODhiNDE4OTg4Y2RkZTA1ZmU0ZTBmMzIz";

        public async Task<ExtractedAccounting> ExtractAccounting(string inputText)
        {
            // Step 1: 生成时间戳和签名
            var time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var signatureOrigin = $"{ApiKey}{time}{ApiSecret}";
            var signature = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(signatureOrigin)));

            // Step 2: 构造请求头
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("appid", AppId);
            client.DefaultRequestHeaders.Add("utc", time);
            client.DefaultRequestHeaders.Add("signature", signature);

            // Step 3: 构造请求体
            var requestBody = new
            {
                text = inputText
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Step 4: 发送请求并解析响应
            var response = await client.PostAsync(ApiUrl, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // 解析返回结果
            var result = JsonConvert.DeserializeObject<AIResponse>(jsonResponse);
            return MapToAccounting(result);
        }

        private ExtractedAccounting MapToAccounting(AIResponse response)
        {
            // 根据返回数据映射到 ExtractedAccounting 模型
            // 这里只是示例，需要根据实际的 API 响应结构调整
            return new ExtractedAccounting
            {
                Type = response.Data.Type,
                Category = response.Data.Category,
                Amount = response.Data.Amount,
                Date = response.Data.Date
            };
        }
    }

    // 辅助类，用于解析 API 返回的数据
    public class AIResponse
    {
        public AIResponseData Data { get; set; }
    }

    public class AIResponseData
    {
        public string Type { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class ExtractedAccounting
    {
        public string Type { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
