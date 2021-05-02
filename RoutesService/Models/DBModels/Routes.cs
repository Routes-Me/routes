using System;
using System.Collections.Generic;

namespace RoutesService.Models.DBModels
{
    public partial class Routes
    {
        public int RouteId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Tariffs> Tariffs { get; set; }
        public virtual ICollection<Carriages> Carriages { get; set; }
    }
}
