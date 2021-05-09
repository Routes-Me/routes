using RoutesService.Models;

namespace RoutesService.Abstraction
{
    public interface ICarriersRepository
    {
        dynamic GetCarriers(string vehicleId, string include);
    }
}
