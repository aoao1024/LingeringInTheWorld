namespace LingeringInTheWorld.Library.Services;

public interface ILocationService
{
    Task<(double Latitude, double Longitude ,string LocationName)> GetCurrentLocationAsync();
}