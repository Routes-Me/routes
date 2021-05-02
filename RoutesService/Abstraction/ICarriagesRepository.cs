using RoutesService.Models.DBModels;
using RoutesService.Models.ResponseModel;

using RoutesService.Models;

namespace RoutesService.Abstraction
{
    public interface ICarriagesRepository
    {
        dynamic GetCarriages(string routeId, string include, Pagination pageInfo);
        Carriages PostCarriages(CarriagesDto carriagesDto);
        Carriages DeleteCarriages(string vehicleId);
    }
}
