using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarServMgmt.Domain;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;

namespace CarServMgmt.UI.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Customer,Receptionist,Mechanic")]
        public async Task<IActionResult> Index()
        {
            IQueryable<Car> cars = _context.Cars.Include(c => c.Customer);
            if (User.IsInRole("Customer") && !User.IsInRole("Admin") && !User.IsInRole("Receptionist") && !User.IsInRole("Mechanic"))
            {
                cars = cars.Where(c => c.Customer != null && c.Customer.Email == User.Identity.Name);
            }
            else if (User.IsInRole("Mechanic") && !User.IsInRole("Admin") && !User.IsInRole("Receptionist"))
            {
                var mechanicEmail = User.Identity.Name;
                var mechanic = await _context.Mechanics.FirstOrDefaultAsync(m => m.Email == mechanicEmail);
                if (mechanic != null)
                {
                    var carIds = await _context.ServiceRecords
                        .Where(sr => sr.MechanicId == mechanic.Id)
                        .Select(sr => sr.CarId)
                        .Distinct()
                        .ToListAsync();
                    cars = cars.Where(c => carIds.Contains(c.Id));
                }
                else
                {
                    cars = cars.Where(c => false);
                }
            }
            return View(await cars.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var car = await _context.Cars
                .Include(c => c.Customer)
                .Include(c => c.ServiceRecords)
                    .ThenInclude(sr => sr.Mechanic)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null) return NotFound();
          
            if (!User.IsInRole("Admin") && !User.IsInRole("Receptionist") && (car.Customer == null || car.Customer.Email != User.Identity.Name))
            {
                return Forbid();
            }
            return View(car);
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Customers, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create([Bind("RegistrationNumber,CustomerId")] Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Customers, "Id", "Name", car.CustomerId);
            return View(car);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();
            ViewData["CustomerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Customers, "Id", "Name", car.CustomerId);
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RegistrationNumber,CustomerId")] Car car)
        {
            if (id != car.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Customers, "Id", "Name", car.CustomerId);
            return View(car);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var car = await _context.Cars
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null) return NotFound();
            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
} 