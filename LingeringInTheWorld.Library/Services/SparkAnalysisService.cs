using Newtonsoft.Json;
using System.Text;
using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public class SparkAnalysisService : IAIAnalysisService
{
    private readonly IAlertService _alertService;
    private const string Server = "讯飞星火智能分析服务";
    
    private const string ApiUrl = "https://spark-api-open.xf-yun.com/v1/chat/completions";
    private const string ApiKey = "Bearer pGJFhuoTEdcbirGlUyVF:pwtZNbdfLOCJOqWCHjnE";
    
    public SparkAnalysisService(IAlertService alertService)
    {
        _alertService = alertService;
    }

    public async Task<DiaryAnalysisResponse> AnalyzeDiaryAsync(string date, string content)
    {
        var data = new
        {
            max_tokens = 4096,
            top_k = 4,
            temperature = 0.5,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "角色名称：智能日记分析助手\n\n角色属性：\n你是一名专业的人工智能助手，擅长处理日常生活中的各种信息，包括账单、代办事项和个人日记内容的提取和整理。\n你能够精准地从用户提供的日记日期和文本中提取账单信息（类型，时间，金额，类型，备注）以及代办事项（标题、内容、完成状态、截止日期）。\n你具备高效的自然语言处理能力，能够准确识别并分析日记中的各类数据，确保生成的内容符合用户的需求，并能为用户提供清晰的结构化数据。\n你能根据用户提供的日记文本内容，识别并提取其中的账单信息和代办事项，并生成结构化的数据。返回结构化的 JSON 格式数据。JSON 应包含以下字段：\n\n账单（Accounting)：识别文本中的账单信息，并提取以下字段：\n1.消费类型（Category）：收入的类别包括：生活费、生意、工资、奖金、其他人情、收红包、退款、收转账、其他。支出的类别包括：餐饮、交通、服饰、购物、服务、教育、娱乐、运动、旅行、医疗。\n2.账单类别（Type）：收入或支出。\n3.金额（Amount）：提取数字金额，若货币单位不是“元”则将其换算为以“元”为单位的金额。\n4.内容（Content）：账单的详细描述或备注。\n5.时间（Time）：账单发生的日期和时间，应符合 ISO 8601 格式（yyyy-MM-ddTHH:mm:ss）。\n\n代办事项(ToDo)：识别文本中的代办事项，并提取以下字段：\n标题（Title）：代办事项的简短描述。\n内容（Content）：代办事项的详细内容。\n完成状态（Status）：判断代办事项的完成状态，false表示未完成，true表示已完成。\n截止日期（DeadLine）：如果存在，提取截止日期，应符合 ISO 8601 格式（yyyy-MM-ddTHH:mm:ss）。\n\nJSON 输出格式示例：\n{\n  \"Accountings\": [\n    {\n      \"Category\": \"餐饮\",\n      \"Type\": \"支出\",\n      \"Amount\": \"50\",\n      \"Content\": \"午餐费用\",\n      \"Time\": \"2024-12-07T12:30:00\"\n    }\n  ],\n  \"ToDos\": [\n    {\n      \"Title\": \"项目报告提交\",\n      \"Content\": \"提交项目报告\",\n      \"Status\": \n     \\\"DeadLine\\\": \\\"2024-12-15T23:59:59\\\",\\n     }\n  ]\n}\n\n指令要求：\n准确性：提取的账单和代办事项字段应准确无误，格式统一，时间应符合 ISO 8601 格式（yyyy-MM-ddTHH:mm:ss）。\n支持复杂文本解析：对于复杂或模糊的文本，应尽量推测和确认信息，但在必要时标注“无法识别”。\n返回格式化 JSON：确保返回的数据遵循 JSON 标准格式，并能方便用户解析或存储。\n字段要求：字段的名称和类型准确匹配所需的格式。\n\n"
                },
                new
                {
                    role = "user",
                    content = date + content
                }
            },
            model = "4.0Ultra"
        };

        try
        {
            using var client = new HttpClient();
            var requestContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            // 设置 Authorization Header
            client.DefaultRequestHeaders.Add("Authorization", ApiKey);

            var json = await client.PostAsync(ApiUrl, requestContent);
            
            if (json.IsSuccessStatusCode)
            {
                var str = await json.Content.ReadAsStringAsync();
                Console.WriteLine("json--------"+str);
                var response = JsonConvert.DeserializeObject<Response>(str);
                var contentStr  = response.Choices[0].Message.Content;
                contentStr = contentStr.Replace("```json", "").Replace("```", "").Replace("\n", "").Trim();
                var result = JsonConvert.DeserializeObject<DiaryAnalysisResponse>(contentStr);
                return result;
            }
        }
        catch (Exception ex)
        {
            await _alertService.AlertAsync(Server, $"智能分析服务调用失败: {ex.Message}");
            Console.WriteLine("智能分析服务调用失败: {0}", ex.Message);
        }

        return null;
    }
}

// 响应模型，表示 API 返回的数据
public class DiaryAnalysisResponse
{
    public List<Accounting> Accountings { get; set; } = new List<Accounting>();
    public List<ToDo> ToDos { get; set; } = new List<ToDo>();
}

public class Response
{
    public int Code { get; set; }
    public string Message { get; set; }
    public string Sid { get; set; }
    public Choice[] Choices { get; set; }
    public Usage Usage { get; set; }
}

public class Choice
{
    public Message Message { get; set; }
    public int Index { get; set; }
}

public class Message
{
    public string Role { get; set; }
    public string Content { get; set; }
}

public class Usage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}

