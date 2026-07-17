using GymSystem.BLL.Services.Classes;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MembersViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberServices memberServices;

        public MemberController(IMemberServices memberServices)
        {
            this.memberServices = memberServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct) 
        {
            var Members = await memberServices.GetAllMemberAsync(ct);
            return View(Members);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create),model);
            var Result = await memberServices.CreateMemberAsync(model,ct);
            if(Result.Success)
                TempData["Success"] = "Member Created Successfully";
            else
                TempData["Failed"] = "Failed to Create Member";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id ,CancellationToken ct)
        {
            var member = await memberServices.GetMemberDetailsAsync(id, ct);
            if (member is null)
            {
                TempData["Error"] = "Member Not Found";
                return View(nameof(Index));
            }
            return View(member);
        }
        [HttpGet]
        public async Task <IActionResult> HealthRecordDetails(int id ,CancellationToken ct)
        {
            var healthRecord = await memberServices.GetMemberHealthRecordAsync(id, ct);
            if (healthRecord is null)
            {
                TempData["Error"] = "Health Record Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken ct)
        {
            var member = await memberServices.GetMemberToUpdateAsync(id, ct);
            if (member is null)
            {
                TempData["Error"] = "Member Can not be Edit";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var Result = await memberServices.UpdateMemberDetailsAsync(id, model, ct);

            if (Result.Success)
            {
                TempData["Success"] = "Member Updated Succefully";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = Result.Error;

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {

            var member = await memberServices.GetMenberById(id, ct);

            if (member is null)
            {
                TempData["Error"] = "Member not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }
        [HttpPost]

        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {

            var Result = await memberServices.DeleteMemberAsync(id, ct);

            TempData[Result.Success ? "Success" : "Error"] = Result.Success ? "Member Deleted Successfully!" : Result.Error;
            return RedirectToAction(nameof(Index));
        }
    }
}
