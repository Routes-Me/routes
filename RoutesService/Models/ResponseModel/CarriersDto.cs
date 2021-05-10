using System.Collections.Generic;

namespace RoutesService.Models.ResponseModel
{
    public class CarriersDto
    {
        public string RouteNumber { get; set; }
        public string Destination { get; set; }
        public List<TicketsDto> Tickets { get; set; }
    }
}
