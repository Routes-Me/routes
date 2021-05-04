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
    public class RoutesRepository : IRoutesRepository
    {
        private readonly AppSettings _appSettings;
        private readonly RoutesServiceContext _context;
        public RoutesRepository(IOptions<AppSettings> appSettings, RoutesServiceContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public dynamic GetRoutes(string routeId, string include, Pagination pageInfo)
        {
            List<Routes> routes = new List<Routes>();
            int recordsCount = 1;
 
            if (!string.IsNullOrEmpty(routeId))
                routes = _context.Routes.Include(r => r.Tariffs).Where(r => r.RouteId == Obfuscation.Decode(routeId)).ToList();
            else
            {
                routes = _context.Routes.Include(r => r.Tariffs).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();
                recordsCount = _context.Routes.Count();
            }

            var page = new Pagination
            {
                offset = pageInfo.offset,
                limit = pageInfo.limit,
                total = recordsCount
            };

            dynamic routesData = routes.Select(r => new RoutesDto {
                    RouteId = Obfuscation.Encode(r.RouteId),
                    Title = r.Title,
                    Subtitle = r.Subtitle,
                    CreatedAt = r.CreatedAt,
                    Tarrifs = !string.IsNullOrEmpty(include) && include.ToLower() == "tariffs" ? r.Tariffs.Select(t => new TariffsDto {
                        TicketId = Obfuscation.Encode(t.TicketId),
                        CreatedAt = t.CreatedAt
                    }).ToList() : null
                }).ToList();       

            return new GetResponse
            {
                data = JArray.Parse(JsonConvert.SerializeObject(routesData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })),
                pagination = page,
            };
        }

        public Routes PostRoutes(RoutesDto routesDto)
        {
            if (routesDto == null)
                throw new ArgumentNullException(CommonMessage.InvalidData);
            
            return new Routes
            {
                Title = routesDto.Title,
                Subtitle = routesDto.Subtitle,
                CreatedAt = DateTime.Now
            };
        }

        public Routes UpdateRoutes(string routeId, RoutesDto routesDto)
        {
            if (routesDto == null || string.IsNullOrEmpty(routeId))
                throw new ArgumentNullException(CommonMessage.InvalidData);

            Routes route = _context.Routes.Where(r => r.RouteId == Obfuscation.Decode(routeId)).FirstOrDefault();
            if (route == null)
                throw new ArgumentException(CommonMessage.RoutesNotFound);

            route.Title = routesDto.Title ?? route.Title;
            route.Subtitle = routesDto.Subtitle ?? route.Subtitle;
            return route;
        }

        public Routes DeleteRoutes(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
                throw new ArgumentNullException(CommonMessage.InvalidData);

            int routeIdDecrypted = Obfuscation.Decode(routeId);
            Routes route = _context.Routes.Include(r => r.Tariffs).Include(r => r.Carriages).Where(r => r.RouteId == routeIdDecrypted).FirstOrDefault();
            if (route == null)
                throw new KeyNotFoundException(CommonMessage.RoutesNotFound);

            return route;
        }
    }
}
