using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarServMgmt.Domain;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic; 
using System.Linq; 

namespace CarServMgmt.UI.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Customer,Mechanic,Receptionist")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Customer") && !User.IsInRole("Receptionist"))
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == User.Identity.Name);
                if (customer != null)
                {
                    return View(new List<Customer> { customer });
                }
                else
                {
                    return View(new List<Customer>()); 
                }
            }
            else if (User.IsInRole("Mechanic") && !User.IsInRole("Receptionist"))
            {
                var mechanicEmail = User.Identity.Name;
                var mechanic = await _context.Mechanics.FirstOrDefaultAsync(m => m.Email == mechanicEmail);
                if (mechanic == null)
                {
                    return View(new List<Customer>());
                }
                var customerIds = await _context.ServiceRecords
                    .Where(sr => sr.MechanicId == mechanic.Id)
                    .Select(sr => sr.Car.CustomerId)
                    .Distinct()
                    .ToListAsync();
                var customers = await _context.Customers.Where(c => customerIds.Contains(c.Id)).ToListAsync();
                return View(customers);
            }
            return View(await _context.Customers.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,PhoneNumber")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
           
            if (!User.IsInRole("Admin") && customer.Email != User.Identity.Name)
            {
                return Forbid();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,PhoneNumber")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }
           
            if (!User.IsInRole("Admin") && customer.Email != User.Identity.Name)
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
           
            if (!User.IsInRole("Admin") && customer.Email != User.Identity.Name)
            {
                return Forbid();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
           
            if (!User.IsInRole("Admin") && customer.Email != User.Identity.Name)
            {
                return Forbid();
            }
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
} 