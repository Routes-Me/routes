using RoutesService.Abstraction;
using RoutesService.Models;
using RoutesService.Models.Common;
using RoutesService.Models.DBModels;
using RoutesService.Models.ResponseModel;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RoutesSecurity;
using System;
using System.Linq;
using System.Collections.Generic;

namespace RoutesService.Repository
{
    public class TariffsRepository : ITariffsRepository
    {
        private readonly AppSettings _appSettings;
        private readonly RoutesServiceContext _context;
        public TariffsRepository(IOptions<AppSettings> appSettings, RoutesServiceContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public dynamic GetTariffs(string routeId, Pagination pageInfo)
        {
            List<Tariffs> tariffs = new List<Tariffs>();
            int recordsCount = 1;
 
            if (!string.IsNullOrEmpty(routeId))
                tariffs = _context.Tariffs.Where(t => t.RouteId == Obfuscation.Decode(routeId)).ToList();
            else
            {
                tariffs = _context.Tariffs.Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();
                recordsCount = _context.Tariffs.Count();
            }

            var page = new Pagination
            {
                offset = pageInfo.offset,
                limit = pageInfo.limit,
                total = recordsCount
            };

            dynamic tariffsData = tariffs.Select(t => new TariffsDto {
                    RouteId = Obfuscation.Encode(t.RouteId),
                    TicketId = Obfuscation.Encode(t.TicketId),
                    CreatedAt = t.CreatedAt
                }).ToList();       

            return new GetResponse
            {
                data = JArray.Parse(JsonConvert.SerializeObject(tariffsData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })),
                pagination = page,
            };
        }

        public Tariffs PostTariffs(TariffsDto tariffsDto)
        {
            if (tariffsDto == null)
                throw new ArgumentNullException(CommonMessage.InvalidData);
            
            return new Tariffs
            {
                RouteId = Obfuscation.Decode(tariffsDto.RouteId),
                TicketId = Obfuscation.Decode(tariffsDto.TicketId),
                CreatedAt = DateTime.Now
            };
        }

        public Tariffs DeleteTariffs(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
                throw new ArgumentNullException(CommonMessage.InvalidData);

            Tariffs tariff = _context.Tariffs.Where(r => r.TicketId == Obfuscation.Decode(ticketId)).FirstOrDefault();
            if (tariff == null)
                throw new KeyNotFoundException(CommonMessage.TariffsNotFound);

            return tariff;
        }
    }
}
