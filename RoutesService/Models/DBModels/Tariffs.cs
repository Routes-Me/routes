using System;

namespace RoutesService.Models.DBModels
{
    public partial class Tariffs
    {
        public int RouteId { get; set; }
        public int TicketId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Routes Route { get; set; }
    }
}
