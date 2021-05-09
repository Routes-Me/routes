using Newtonsoft.Json.Linq;

namespace RoutesService.Models
{
    public class ErrorResponse
    {
        public string error { get; set; }
    }
    public class SuccessResponse
    {
        public string message { get; set; }
    }
    public class GetResponse
    {
        public Pagination pagination { get; set; }
        public JArray data { get; set; }
    }
    public class CarriersGetResponse
    {
        public JObject data { get; set; }
    }
}
