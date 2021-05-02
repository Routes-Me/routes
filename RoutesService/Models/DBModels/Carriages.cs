using System;

namespace RoutesService.Models.DBModels
{
    public partial class Carriages
    {
        public int RouteId { get; set; }
        public int VehicleId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Routes Route { get; set; }
    }
}
