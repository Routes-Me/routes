using System.Collections.Generic;

namespace RoutesService.Models.ResponseModel
{
    public class TicketsDto
    {
        public string TicketId { get; set; }
        public double Price { get; set; }
        public string CurrencyId { get; set; }
        public List<CurrenciesDto> Currencies { get; set; }
    }
}
