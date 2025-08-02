using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection; 

namespace CarServMgmt.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public class UserWithRole
        {
            public IdentityUser User { get; set; }
            public string Role { get; set; }
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userWithRoles = new List<UserWithRole>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userWithRoles.Add(new UserWithRole
                {
                    User = user,
                    Role = roles.FirstOrDefault() ?? ""
                });
            }
            return View(userWithRoles);
        }

      
      
      
      
      
      
      
      
      
      
      
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = allRoles;
            return View(user);
        }

       
       
       
       
       
       
       
       
       
       
       
       
       
       
       
       
       
       
       
       
         [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string email, string userName, string newPassword, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            user.Email = email;
            user.UserName = userName;
            var result = await _userManager.UpdateAsync(user);
            if (!string.IsNullOrEmpty(newPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!passResult.Succeeded)
                {
                    foreach (var error in passResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
                    ViewBag.UserRoles = userRoles;
                    ViewBag.AllRoles = allRoles;
                    return View(user);
                }
            }
            if (!string.IsNullOrEmpty(selectedRole))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, selectedRole);

              
              
              
              
              
              
              
              
              
              
              
              
              
              
              
              
              
              
              
              
                 var db = HttpContext.RequestServices.GetService(typeof(CarServMgmt.Domain.ApplicationDbContext)) as CarServMgmt.Domain.ApplicationDbContext;
                if (db != null)
                {
                    var userEmail = user.Email;
                    if (selectedRole == "Customer")
                    {
                       
                       
                       
                       
                       
                       
                       
                       
                       
                        if (!db.Customers.Any(c => c.Email == userEmail))
                        {
                            db.Customers.Add(new CarServMgmt.Domain.Customer { Name = userEmail, Email = userEmail });
                        }
                       
                       
                       
                       
                       
                       
                       
                       
                       
                       
                       
                       
                       
                       
                         var mechanic = db.Mechanics.FirstOrDefault(m => m.Email == userEmail);
                        if (mechanic != null)
                        {
                            db.Mechanics.Remove(mechanic);
                        }
                    }
                    else if (selectedRole == "Mechanic")
                    {
                      
                      
                      
                      
                      
                      
                      
                      
                      
                      
                      
                        if (!db.Mechanics.Any(m => m.Email == userEmail))
                        {
                            db.Mechanics.Add(new CarServMgmt.Domain.Mechanic { Name = userEmail, Email = userEmail });
                        }
                      
                      
                      
                      
                      
                      
                      
                      
                      
                      
                      
                      
                      
                                      var customer = db.Customers.FirstOrDefault(c => c.Email == userEmail);
                        if (customer != null)
                        {
                            db.Customers.Remove(customer);
                        }
                    }
                    else 
                    




                    {
                        
                        var customer = db.Customers.FirstOrDefault(c => c.Email == userEmail);
                        if (customer != null)
                        {
                            db.Customers.Remove(customer);
                        }
                        var mechanic = db.Mechanics.FirstOrDefault(m => m.Email == userEmail);
                        if (mechanic != null)
                        {
                            db.Mechanics.Remove(mechanic);
                        }
                    }
                    await db.SaveChangesAsync();
                }
            }
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            var userRoles2 = await _userManager.GetRolesAsync(user);
            var allRoles2 = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            ViewBag.UserRoles = userRoles2;
            ViewBag.AllRoles = allRoles2;
            return View(user);
        }

      
      
      
      
       
       
       
       
       
                          [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var allRoles = await _roleManager.Roles
                .Where(r => r.Name == "Customer")
                .Select(r => r.Name)
                .ToListAsync();
            ViewBag.AllRoles = allRoles;
            return View();
        }

                            
                            
                            
                            
                            
                              [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(string email, string userName, string password, string selectedRole)
        {
            var user = new IdentityUser { Email = email, UserName = userName };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(selectedRole))
                {
                    await _userManager.AddToRoleAsync(user, selectedRole);
                    var db = HttpContext.RequestServices.GetService(typeof(CarServMgmt.Domain.ApplicationDbContext)) as CarServMgmt.Domain.ApplicationDbContext;
                    if (db != null)
                    {
                        if (selectedRole == "Customer")
                        {
                            if (!db.Customers.Any(c => c.Email == email))
                                db.Customers.Add(new CarServMgmt.Domain.Customer { Name = email, Email = email });
                        }
                        await db.SaveChangesAsync();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var allRoles = await _roleManager.Roles
                .Where(r => r.Name == "Customer")
                .Select(r => r.Name)
                .ToListAsync();
            ViewBag.AllRoles = allRoles;
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.FirstOrDefault() != "Customer")
            {
                return Forbid();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.FirstOrDefault() != "Customer")
            {
                return Forbid();
            }
            await _userManager.DeleteAsync(user);
            var db = HttpContext.RequestServices.GetService(typeof(CarServMgmt.Domain.ApplicationDbContext)) as CarServMgmt.Domain.ApplicationDbContext;
            if (db != null)
            {
                var customer = db.Customers.FirstOrDefault(c => c.Email == user.Email);
                if (customer != null)
                {
                    db.Customers.Remove(customer);
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 