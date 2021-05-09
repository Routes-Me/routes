using System.Collections.Generic;

namespace RoutesService.Models.ResponseModel
{
    public class CarriersDto
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public List<TicketsDto> Tickets { get; set; }
    }
}
