using System;

namespace RoutesService.Models.ResponseModel
{
    public class CarriagesDto
    {
        public string RouteId { get; set; }
        public string VehicleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public RoutesDto Route { get; set; }
    }
}
