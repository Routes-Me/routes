using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RoutesService.Abstraction;
using RoutesService.Models;
using RoutesService.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RoutesService.Controllers
{
    [ApiController]
    [ApiVersion( "1.0" )]
    [Route("v{version:apiVersion}/")]
    public class CarriersController : ControllerBase
    {
        private readonly ICarriersRepository _CarriersRepository;
        private readonly RoutesServiceContext _context;
        public CarriersController(ICarriersRepository CarriersRepository, RoutesServiceContext context)
        {
            _CarriersRepository = CarriersRepository;
            _context = context;
        }

        [HttpGet]
        [Route("carriers/{vehicleId}")]
        public IActionResult GetCarriers(string vehicleId, string include)
        {
            CarriersGetResponse response = new CarriersGetResponse();
            try
            {
                response = _CarriersRepository.GetCarriers(vehicleId, include);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponse{ error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponse{ error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse{ error = ex.Message });
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}
