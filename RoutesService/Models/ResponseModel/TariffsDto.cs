using System;

namespace RoutesService.Models.ResponseModel
{
    public class TariffsDto
    {
        public string RouteId { get; set; }
        public string TicketId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
