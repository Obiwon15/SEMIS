using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AutoMapper;
using EIMS.Domain.Entities;
using EIMS.Domain.Entities.Enum;
using EIMS.Domain.Interfaces;
using EIMS.Infrastructure.PageHelpers;
using EIMS.Models;

namespace EIMS.Controllers
{
    public class WorkExperienceController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IWorkExperienceRepo _workExpRepo;

        public WorkExperienceController(IMapper mapper, IWorkExperienceRepo workExperienceRepo)
        {
            _mapper = mapper;
            _workExpRepo = workExperienceRepo;
        }
        public IActionResult GetWorkExperience(int teacherId)
        {
            var experiences = _workExpRepo.Find(we => we.TeacherId == teacherId).ToList();
            return PartialView("_WorkXp", experiences);
        }

        public IActionResult AddorEditExperience(int teacherId, int id = 0)
        {
            if (id != 0)
            {
                var workExpModel = _mapper.Map<WorkExperienceViewModel>(_workExpRepo.GetById(id));
                return View(workExpModel);
            }
            var addExpModel = new WorkExperienceViewModel() { TeacherId = teacherId };
            return View(addExpModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddorEditExperience(int id, WorkExperienceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var workExperience = _mapper.Map<WorkExperience>(viewModel);
                var successMessage = "";
                if (id == 0)
                {
                    _workExpRepo.Insert(workExperience);
                    successMessage = "Added work experience";
                }
                else
                {
                    _workExpRepo.Update(workExperience);
                    successMessage = "Changes have been saved";
                }

                var result = _workExpRepo.Save();
                if (result > 0)
                {
                    SweetAlert("Success", $"{successMessage}", NotificationType.success);
                    return new RedirectResult(Url.Action("EditProfile", "Profile")+"#profile1");
                }
            }
            SweetAlert("Error", $"Please ensure you fill all required fields", NotificationType.error);
            return new RedirectResult(Url.Action("EditProfile", "Profile") + "#profile1");
        }

        public IActionResult DeleteExperience(int id)
        {
            var experience = _workExpRepo.GetById(id);
            if (experience != null)
            {
                _workExpRepo.Delete(id);
                var result = _workExpRepo.Save();
                if (result > 0)
                {
                    SweetAlert("Deleted", "Successfully deleted experience", NotificationType.info);
                    return new RedirectResult(Url.Action("EditProfile", "Profile") + "#profile1");
                }
            }
            SweetAlert("Error", "Could not delete experience", NotificationType.error);
            return RedirectToAction("EditProfile", "Profile");
        }
    }
}
