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
    public class TariffsController : ControllerBase
    {
        private readonly ITariffsRepository _TariffsRepository;
        private readonly RoutesServiceContext _context;
        public TariffsController(ITariffsRepository TariffsRepository, RoutesServiceContext context)
        {
            _TariffsRepository = TariffsRepository;
            _context = context;
        }

        [HttpGet]
        [Route("tariffs/{routeId?}")]
        public IActionResult GetTariffs(string routeId, [FromQuery] Pagination pageInfo)
        {
            GetResponse response = new GetResponse();
            try
            {
                response = _TariffsRepository.GetTariffs(routeId, pageInfo);
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
        [Route("tariffs")]
        public async Task<IActionResult> PostTariffs(TariffsDto tariffsDto)
        {
            try
            {
                Tariffs tariff = _TariffsRepository.PostTariffs(tariffsDto);
                await _context.Tariffs.AddAsync(tariff);
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
            return StatusCode(StatusCodes.Status201Created, new SuccessResponse{ message = CommonMessage.TariffsInserted});
        }

        [HttpDelete]
        [Route("tariffs/{ticketId}")]
        public async Task<IActionResult> DeleteTariffs(string ticketId)
        {
            try
            {
                Tariffs tariff = _TariffsRepository.DeleteTariffs(ticketId);
                _context.Tariffs.Remove(tariff);
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
