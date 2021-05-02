using RoutesService.Models.DBModels;
using RoutesService.Models.ResponseModel;

using RoutesService.Models;

namespace RoutesService.Abstraction
{
    public interface IRoutesRepository
    {
        dynamic GetRoutes(string routeId, string include, Pagination pageInfo);
        Routes PostRoutes(RoutesDto routesDto);
        Routes UpdateRoutes(RoutesDto routesDto);
        Routes DeleteRoutes(string routeId);
    }
}
