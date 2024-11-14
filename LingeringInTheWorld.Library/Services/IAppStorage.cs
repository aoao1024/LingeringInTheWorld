using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public interface IAppStorage
{
    Task InitializeAsync();
    
    Task InsertAsync(Diary diary);
    
    Task<IList<Diary>> ListAsync();
    
    Task<IList<Diary>> QueryAsync(string keyword);
}