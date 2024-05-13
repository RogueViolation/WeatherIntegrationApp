using DataAccess.DTO;

namespace DataAccess
{
    public interface IWeatherAccess
    {
        T CallWeatherApi<T>(string method, string argument);
    }
}
