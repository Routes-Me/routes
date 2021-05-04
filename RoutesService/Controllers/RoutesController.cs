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
    public class RoutesController : ControllerBase
    {
        private readonly IRoutesRepository _RoutesRepository;
        private readonly RoutesServiceContext _context;
        public RoutesController(IRoutesRepository RoutesRepository, RoutesServiceContext context)
        {
            _RoutesRepository = RoutesRepository;
            _context = context;
        }

        [HttpGet]
        [Route("routes/{routeId?}")]
        public IActionResult GetRoutes(string routeId, string include, [FromQuery] Pagination pageInfo)
        {
            GetResponse response = new GetResponse();
            try
            {
                response = _RoutesRepository.GetRoutes(routeId, include, pageInfo);
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
        [Route("routes")]
        public async Task<IActionResult> PostRoutes(RoutesDto routesDto)
        {
            try
            {
                Routes route = _RoutesRepository.PostRoutes(routesDto);
                await _context.Routes.AddAsync(route);
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
            return StatusCode(StatusCodes.Status201Created, new SuccessResponse{ message = CommonMessage.RoutesInserted});
        }

        [HttpPut]
        [Route("routes/{routeId}")]
        public async Task<IActionResult> UpdateRoutes(string routeId, RoutesDto routesDto)
        {
            try
            {
                Routes route = _RoutesRepository.UpdateRoutes(routeId, routesDto);
                _context.Routes.Update(route);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponse{ error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponse{ error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResponse{ error = ex.Message });
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpDelete]
        [Route("routes/{routeId}")]
        public async Task<IActionResult> DeleteRoutes(string routeId)
        {
            try
            {
                Routes route = _RoutesRepository.DeleteRoutes(routeId);
                _context.Routes.Remove(route);
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
