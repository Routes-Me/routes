using RoutesService.Abstraction;
using RoutesService.Models;
using RoutesService.Models.Common;
using RoutesService.Models.DBModels;
using RoutesService.Models.ResponseModel;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using RoutesSecurity;
using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;

namespace RoutesService.Repository
{
    public class CarriersRepository : ICarriersRepository
    {
        private readonly AppSettings _appSettings;
        private readonly Dependencies _dependencies;
        private readonly RoutesServiceContext _context;
        public CarriersRepository(IOptions<AppSettings> appSettings, IOptions<Dependencies> dependencies, RoutesServiceContext context)
        {
            _appSettings = appSettings.Value;
            _dependencies = dependencies.Value;
            _context = context;
        }

        public dynamic GetCarriers(string vehicleId, string include)
        {
            Carriages carriage = ValidateCarriage(vehicleId);

            List<TicketsDto> ticketsDtosList = JsonConvert.DeserializeObject<GetResponse>(GetAPI(_dependencies.TicketsUrl + Obfuscation.Encode(carriage.Route.Tariffs.FirstOrDefault().TicketId)).Content).data.ToObject<List<TicketsDto>>();

            if (!string.IsNullOrEmpty(include) && include.Equals("currencies"))
                ticketsDtosList.ForEach(ticketDto =>
                    ticketDto.Currencies = JsonConvert.DeserializeObject<GetResponse>(GetAPI(_dependencies.CurrenciesUrl + ticketDto.CurrencyId).Content).data.ToObject<List<CurrenciesDto>>()
            );

            CarriersDto carriersDto = new CarriersDto {
                RouteNumber = carriage.Route.Title,
                Destination = carriage.Route.Subtitle,
                Tickets = ticketsDtosList
            };

            return new CarriersGetResponse {
                data = JObject.Parse(JsonConvert.SerializeObject(carriersDto, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })),
            };
        }

        private Carriages ValidateCarriage(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId))
                throw new ArgumentNullException(CommonMessage.InvalidData);

            Carriages carriage = _context.Carriages.Include(c => c.Route.Tariffs).Where(c => c.VehicleId == Obfuscation.Decode(vehicleId)).FirstOrDefault();
            if (carriage == null)
                throw new KeyNotFoundException(CommonMessage.CarriageNotFound);

            return carriage;
        }

        private dynamic GetAPI(string url, string query = "")
        {
            UriBuilder uriBuilder = new UriBuilder(_appSettings.Host + url);
            uriBuilder = AppendQueryToUrl(uriBuilder, query);
            var client = new RestClient(uriBuilder.Uri);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == 0)
                throw new HttpListenerException(400, CommonMessage.ConnectionFailure);

            if (!response.IsSuccessful)
                throw new HttpListenerException((int)response.StatusCode, response.Content);

            return response;
        }

        private UriBuilder AppendQueryToUrl(UriBuilder baseUri, string queryToAppend)
        {
            if (baseUri.Query != null && baseUri.Query.Length > 1)
                baseUri.Query = baseUri.Query.Substring(1) + "&" + queryToAppend;
            else
                baseUri.Query = queryToAppend;
            return baseUri;
        }
    }
}
