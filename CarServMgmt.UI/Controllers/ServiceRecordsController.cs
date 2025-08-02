using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarServMgmt.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace CarServMgmt.UI.Controllers
{
    [Authorize]
    public class ServiceRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Mechanic")]
        public async Task<IActionResult> Index()
        {
            IQueryable<ServiceRecord> serviceRecords = _context.ServiceRecords.Include(s => s.Car).ThenInclude(c => c.Customer).Include(s => s.Mechanic);
            if (User.IsInRole("Mechanic") && !User.IsInRole("Admin"))
            {
                serviceRecords = serviceRecords.Where(sr => sr.Mechanic != null && sr.Mechanic.Email == User.Identity.Name);
            }
            return View(await serviceRecords.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var serviceRecord = await _context.ServiceRecords
                .Include(s => s.Car).ThenInclude(c => c.Customer)
                .Include(s => s.Mechanic)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceRecord == null) return NotFound();
            return View(serviceRecord);
        }

        [Authorize(Roles = "Admin,Mechanic")]
        public IActionResult Create()
        {
            ViewData["CarId"] = new SelectList(_context.Cars.Include(c => c.Customer).ToList(), "Id", "RegistrationNumber");
            ViewData["MechanicId"] = new SelectList(_context.Mechanics, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mechanic")]
        public async Task<IActionResult> Create([Bind("CarId,MechanicId,Description,Hours,DateBroughtIn")] ServiceRecord serviceRecord)
        {
            if (ModelState.IsValid)
            {
                serviceRecord.IsComplete = false;
                _context.Add(serviceRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarId"] = new SelectList(_context.Cars.Include(c => c.Customer).ToList(), "Id", "RegistrationNumber", serviceRecord.CarId);
            ViewData["MechanicId"] = new SelectList(_context.Mechanics, "Id", "Name", serviceRecord.MechanicId);
            return View(serviceRecord);
        }

        [Authorize(Roles = "Admin,Mechanic")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var serviceRecord = await _context.ServiceRecords.FindAsync(id);
            if (serviceRecord == null) return NotFound();
            ViewData["CarId"] = new SelectList(_context.Cars.Include(c => c.Customer).ToList(), "Id", "RegistrationNumber", serviceRecord.CarId);
            ViewData["MechanicId"] = new SelectList(_context.Mechanics, "Id", "Name", serviceRecord.MechanicId);
            return View(serviceRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mechanic")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CarId,MechanicId,Description,Hours,IsComplete,DateBroughtIn,DateCompleted")] ServiceRecord serviceRecord)
        {
            if (id != serviceRecord.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    if (serviceRecord.DateCompleted != null)
                        serviceRecord.IsComplete = true;
                    
                    if (serviceRecord.IsComplete && serviceRecord.DateCompleted == null)
                        serviceRecord.DateCompleted = System.DateTime.Now;
                    _context.Update(serviceRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRecordExists(serviceRecord.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarId"] = new SelectList(_context.Cars.Include(c => c.Customer).ToList(), "Id", "RegistrationNumber", serviceRecord.CarId);
            ViewData["MechanicId"] = new SelectList(_context.Mechanics, "Id", "Name", serviceRecord.MechanicId);
            return View(serviceRecord);
        }

        [Authorize(Roles = "Admin,Mechanic")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var serviceRecord = await _context.ServiceRecords
                .Include(s => s.Car).ThenInclude(c => c.Customer)
                .Include(s => s.Mechanic)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceRecord == null) return NotFound();
            return View(serviceRecord);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mechanic")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRecord = await _context.ServiceRecords.FindAsync(id);
            if (serviceRecord != null)
            {
                _context.ServiceRecords.Remove(serviceRecord);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRecordExists(int id)
        {
            return _context.ServiceRecords.Any(e => e.Id == id);
        }
    }
} 