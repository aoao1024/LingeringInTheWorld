using System.Linq.Expressions;
using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public interface IAppStorage
{
    bool IsInitialized { get; }//判断数据库是否部署
    
    Task InitializeAsync();
    
    //插入日记
    Task InsertDiaryAsync(Diary diary);
    
    //删除日记
    Task DeleteDiaryAsync(int id);
    
    //更新日记
    Task UpdateDiaryAsync(Diary diary);
    
    //列出所有日记
    Task<IList<Diary>> ListDiaryAsync();
    
    //根据ID查询日记
    Task<Diary> QueryDiaryByIdAsync(int id);
    
    //根据标题查询日记
    Task<IList<Diary>> QueryDiaryByTitleAsync(string title);
    
    Task<List<Diary>> GetDiariesAsync(
        Expression<Func<Diary, bool>> where, // 查询条件
        int skip, // 跳过的记录数
        int take // 每次获取的记录数
    );
    
    event EventHandler<Diary> Updated;
}