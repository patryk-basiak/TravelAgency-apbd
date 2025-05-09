using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }
        

        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetTrip(int id)
        {
            var trips = await _clientsService.GetClientTrips(id);
            return Ok(trips);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientsTripDTO client)
        {
            if (string.IsNullOrEmpty(client.Name) || string.IsNullOrEmpty(client.Surname))
            {
                return BadRequest("Name and Surname are blank.");
            }

            bool result = await _clientsService.CreateClient(client);

            return ??;
        }
        
    }
}
