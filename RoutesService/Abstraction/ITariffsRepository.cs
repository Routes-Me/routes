using RoutesService.Models.DBModels;
using RoutesService.Models.ResponseModel;

using RoutesService.Models;

namespace RoutesService.Abstraction
{
    public interface ITariffsRepository
    {
        dynamic GetTariffs(string routeId, Pagination pageInfo);
        Tariffs PostTariffs(TariffsDto tariffsDto);
        Tariffs DeleteTariffs(string ticketId);
    }
}
