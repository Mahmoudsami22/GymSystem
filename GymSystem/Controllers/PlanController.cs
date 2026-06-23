using GymSystem.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymSystem.Controllers
{
    public class PlanController : Controller
    {
        private readonly GymDbContext _context = new GymDbContext();
        public async Task<IActionResult> Index()
        {
            var plans = await _context.Plans.ToListAsync();
            return View(plans);
        }

        public async Task<IActionResult> Details(int id)
        {
            var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
            {
                RedirectToAction(nameof(Index));
            }

            return View(plan);
        }
    }
}
