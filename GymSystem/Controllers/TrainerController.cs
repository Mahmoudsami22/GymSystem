using GymSystem.BLL.Services.Classes;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MembersViewModels;
using GymSystem.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.Controllers
{
    [Authorize]
    public class TrainerController : Controller
    {
        private readonly ITrainerServices trainerServices;

        public TrainerController(ITrainerServices trainerServices)
        {
            this.trainerServices = trainerServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Trainers = await trainerServices.GetAllTrainerAsync(ct);
            return View(Trainers);
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateTrainer(CreateTrainerViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);
            var Result = await trainerServices.CreateTrainerAsync(model, ct);
            if (Result.Success)
                TempData["Success"] = "Trainer Created Successfully";
            else
                TempData["Failed"] = "Failed to Create Trainer";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> TrainerDetails(int id, CancellationToken ct)
        {
            var trainer = await trainerServices.GetTrainerDetailsAsync(id, ct);
            if (trainer is null)
            {
                TempData["Error"] = "Trainer Not Found";
                return View(nameof(Index));
            }
            return View(trainer);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var trainer = await trainerServices.GetTrainerToUpdateAsync(id, ct);
            if (trainer is null)
            {
                TempData["Error"] = "Trainer Can not be Edit";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var Result = await trainerServices.UpdateTrainerDetailsAsync(id, model, ct);

            if (Result.Success)
            {
                TempData["Success"] = "Trainer Updated Succefully";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = Result.Error;

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {

            var trainer = await trainerServices.GetTrainerById(id, ct);

            if (trainer is null)
            {
                TempData["Error"] = "Trainer not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }
        [HttpPost]

        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {

            var Result = await trainerServices.DeleteTrainerAsync(id, ct);

            TempData[Result.Success ? "Success" : "Error"] = Result.Success ? "Trainer Deleted Successfully!" : Result.Error;
            return RedirectToAction(nameof(Index));
        }
    }
}
