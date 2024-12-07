namespace LingeringInTheWorld.Library.Services;

public interface IAIAnalysisService
{
    Task<DiaryAnalysisResponse> AnalyzeDiaryAsync(string date, string content);
}