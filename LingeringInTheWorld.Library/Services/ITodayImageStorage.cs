using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public interface ITodayImageStorage {
    Task<TodayImage> GetTodayImageAsync(bool isIncludingImageStream);

    Task SaveTodayImageAsync(TodayImage todayImage, bool isSavingExpiresAtOnly);
}