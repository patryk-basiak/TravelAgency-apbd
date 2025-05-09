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
        public async Task<IActionResult> AddClient([FromBody] ClientDTO client)
        {
            if (client.Email == null && client.Name == null)
            {
                return BadRequest();
            }
            var result = _clientsService.AddClient(client);
            return CreatedAtAction(nameof(AddClient), new { }, client);
        }

        [HttpPut("{id}/trips/{tripId}")]
        public async Task<IActionResult> AssignClientToTrip(int id, int trip)
        {
            var result = _clientsService.AssignClientToTrip(id, trip);
            if (result.Result)
            {
                return Ok("Client has been assigned");
            }
            return BadRequest();
            
        }

        [HttpDelete("{id}/trips/{tripId}")]
        public async Task<IActionResult> DeleteClientFromTrip(int id, int tripId)
        {
            var result = _clientsService.DeleteClientFromTrip(id, tripId);
            if (result.Result)
            {
                return Ok("Client has been deleted from trip");
            }
            return BadRequest();
        }
        
    }
}
