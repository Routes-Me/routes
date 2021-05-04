using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RoutesService.Abstraction;
using RoutesService.Models;
using RoutesService.Models.DBModels;
using RoutesService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RoutesService.Controllers
{
    [ApiController]
    [ApiVersion( "1.0" )]
    [Route("v{version:apiVersion}/")]
    public class CarriagesController : ControllerBase
    {
        private readonly ICarriagesRepository _CarriagesRepository;
        private readonly RoutesServiceContext _context;
        public CarriagesController(ICarriagesRepository CarriagesRepository, RoutesServiceContext context)
        {
            _CarriagesRepository = CarriagesRepository;
            _context = context;
        }

        [HttpGet]
        [Route("carriages/{routeId?}")]
        public IActionResult GetCarriages(string routeId, string include, [FromQuery] Pagination pageInfo)
        {
            GetResponse response = new GetResponse();
            try
            {
                response = _CarriagesRepository.GetCarriages(routeId, include, pageInfo);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponse{ error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse{ error = ex.Message });
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        [Route("carriages")]
        public async Task<IActionResult> PostCarriages(CarriagesDto carriagesDto)
        {
            try
            {
                Carriages carriage = _CarriagesRepository.PostCarriages(carriagesDto);
                await _context.Carriages.AddAsync(carriage);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponse{ error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse{ error = ex.Message });
            }
            return StatusCode(StatusCodes.Status201Created, new SuccessResponse{ message = CommonMessage.CarriagesInserted});
        }

        [HttpDelete]
        [Route("carriages/{vehicleId}")]
        public async Task<IActionResult> DeleteCarriages(string vehicleId)
        {
            try
            {
                Carriages carriage = _CarriagesRepository.DeleteCarriages(vehicleId);
                _context.Carriages.Remove(carriage);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse{ error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponse{ error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse{ error = ex.Message });
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
