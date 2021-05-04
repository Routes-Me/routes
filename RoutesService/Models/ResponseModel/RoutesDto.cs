using System;
using System.Collections.Generic;

namespace RoutesService.Models.ResponseModel
{
    public class RoutesDto
    {
        public string RouteId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TariffsDto> Tarrifs { get; set; }
    }
}
