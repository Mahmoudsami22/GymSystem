using GymSystem.BLL.Services.Classes;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MembersViewModels;
using GymSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Classes;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymSystem.Controllers
{
    [Authorize]
    public class PlanController : Controller
    {
        private readonly IPlanServices planServices;

        public PlanController(IPlanServices planServices)
        {
            this.planServices = planServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await planServices.GetAllPlanAsync(ct);
            return View(plans);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await planServices.GetPlanDetailsAsync(id, ct); 
            if (plan == null)
            {
                RedirectToAction(nameof(Index));
            }

            return View(plan);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await planServices.GetPlanToUpdateAsync(id, ct);
            if (plan is null)
            {
                TempData["Error"] = "Plan Can not be Edit";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var Result = await planServices.UpdatePlanDetailsAsync(id, model, ct);

            if (Result.Success)
            {
                TempData["Success"] = "Plan Updated Succefully";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = Result.Error;

            return View(model);
        }
    }
}
