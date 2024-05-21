
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAPI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace TravelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly TravelDbContext _context;

        public TripsController(TravelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrips(int page = 1, int pageSize = 10)
        {
            var trips = await _context.Trips
                .OrderByDescending(t => t.DateFrom)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalTrips = await _context.Trips.CountAsync();

            return Ok(new
            {
                pageNum = page,
                pageSize,
                allPages = (int)Math.Ceiling(totalTrips / (double)pageSize),
                trips
            });
        }

        [HttpDelete("clients/{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            var client = await _context.Clients.Include(c => c.Trips).FirstOrDefaultAsync(c => c.Id == idClient);

            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            if (client.Trips.Any())
            {
                return BadRequest(new { message = "Client cannot be deleted as they are assigned to one or more trips" });
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("trips/{idTrip}/clients")]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] ClientDto clientDto)
        {
            var trip = await _context.Trips.FindAsync(idTrip);
            if (trip == null || trip.DateFrom <= DateTime.Now)
            {
                return BadRequest(new { message = "Trip does not exist or has already occurred" });
            }

            var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == clientDto.Pesel);
            if (existingClient != null)
            {
                if (existingClient.Trips.Any(t => t.Id == idTrip))
                {
                    return BadRequest(new { message = "Client is already registered for this trip" });
                }
            }

            var client = existingClient ?? new Client
            {
                FirstName = clientDto.FirstName,
                LastName = clientDto.LastName,
                Email = clientDto.Email,
                Telephone = clientDto.Telephone,
                Pesel = clientDto.Pesel
            };

            trip.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class ClientDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Pesel { get; set; }
    }
}
