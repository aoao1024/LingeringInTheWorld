using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public interface IAppStorage
{
    bool IsInitialized { get; }//判断数据库是否部署
    
    Task InitializeAsync();
    
    Task InsertAsync(Diary diary);
    
    Task<IList<Diary>> ListAsync();
    
    Task<IList<Diary>> QueryAsync(string keyword);
}