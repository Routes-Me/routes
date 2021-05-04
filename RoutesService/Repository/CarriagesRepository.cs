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
    public class CarriagesRepository : ICarriagesRepository
    {
        private readonly AppSettings _appSettings;
        private readonly RoutesServiceContext _context;
        public CarriagesRepository(IOptions<AppSettings> appSettings, RoutesServiceContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public dynamic GetCarriages(string routeId, string include, Pagination pageInfo)
        {
            List<Carriages> carriages = new List<Carriages>();
            int recordsCount = 1;
 
            if (!string.IsNullOrEmpty(routeId))
                carriages = _context.Carriages.Include(c => c.Route).Where(c => c.RouteId == Obfuscation.Decode(routeId)).ToList();
            else
            {
                carriages = _context.Carriages.Include(c => c.Route).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();
                recordsCount = _context.Carriages.Count();
            }

            var page = new Pagination
            {
                offset = pageInfo.offset,
                limit = pageInfo.limit,
                total = recordsCount
            };

            dynamic carriagesData = carriages.Select(c => new CarriagesDto {
                    RouteId = Obfuscation.Encode(c.RouteId),
                    VehicleId = Obfuscation.Encode(c.VehicleId),
                    CreatedAt = c.CreatedAt,
                    Route = !string.IsNullOrEmpty(include) && include.ToLower() == "routes" ? new RoutesDto {
                        Title = c.Route.Title,
                        Subtitle = c.Route.Subtitle,
                        CreatedAt = c.CreatedAt
                    } : null
                }).ToList();       

            return new GetResponse
            {
                data = JArray.Parse(JsonConvert.SerializeObject(carriagesData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })),
                pagination = page,
            };
        }

        public Carriages PostCarriages(CarriagesDto carriagesDto)
        {
            if (carriagesDto == null)
                throw new ArgumentNullException(CommonMessage.InvalidData);
            
            return new Carriages
            {
                RouteId = Obfuscation.Decode(carriagesDto.RouteId),
                VehicleId = Obfuscation.Decode(carriagesDto.VehicleId),
                CreatedAt = DateTime.Now
            };
        }

        public Carriages DeleteCarriages(string vehicleId)
        {
            if (string.IsNullOrEmpty(vehicleId))
                throw new ArgumentNullException(CommonMessage.InvalidData);

            int vehicleIdDecrypted = Obfuscation.Decode(vehicleId);
            Carriages carriage = _context.Carriages.Where(c => c.VehicleId == vehicleIdDecrypted).FirstOrDefault();
            if (carriage == null)
                throw new KeyNotFoundException(CommonMessage.CarriageNotFound);

            return carriage;
        }
    }
}
