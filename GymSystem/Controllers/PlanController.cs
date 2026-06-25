using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using GymSystem.DAL.Repositories.Interfaces;
using GymSystem.DAL.Repositories.Classes;

namespace GymSystem.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanRepository _planRepositors;
        public PlanController(IPlanRepository planRepository)
        {
            _planRepositors = planRepository;
        }
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var plans = await _planRepositors.GetAll(false, token);
            return View(plans);
        }

        public async Task<IActionResult> Details(int id, CancellationToken token)
        {
            var plan = await _planRepositors.GetById(id,token); 
            if (plan == null)
            {
                RedirectToAction(nameof(Index));
            }

            return View(plan);
        }
    }
}
