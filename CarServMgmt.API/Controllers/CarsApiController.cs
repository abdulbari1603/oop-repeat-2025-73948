using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarServMgmt.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CarServMgmt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CarsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

         [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            return await _context.Cars.Include(c => c.Customer).ToListAsync();
        }
    }
} 