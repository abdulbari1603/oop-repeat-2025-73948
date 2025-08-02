using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CarServiceManagement.UI.Models;
using CarServMgmt.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarServiceManagement.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            ViewBag.TotalCars = await _context.Cars.CountAsync();
            ViewBag.TotalCustomers = await _context.Customers.CountAsync();
            ViewBag.TotalServiceRecords = await _context.ServiceRecords.CountAsync();
            ViewBag.TotalMechanics = await _context.Mechanics.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard statistics");
            ViewBag.TotalCars = 0;
            ViewBag.TotalCustomers = 0;
            ViewBag.TotalServiceRecords = 0;
            ViewBag.TotalMechanics = 0;
        }
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
