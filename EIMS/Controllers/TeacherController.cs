using System;
using System.Linq;
using System.Threading.Tasks;
using EIMS.Domain.Entities.Enum;
using EIMS.Domain.Interfaces;
using EIMS.Infrastructure.Interfaces;
using EIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EIMS.Controllers
{
	[Authorize]
	public class TeacherController : BaseController
	{
		private readonly ITeacherService _teacherService;

		public TeacherController(ITeacherService teacherService)
		{
			_teacherService = teacherService;

		}
		[Authorize(Roles = "Admin")]
		public IActionResult AddTeacher()
		{
			return View();
		}

		[Authorize(Roles = "Admin")]

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddTeacher(CreateTeacherViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				var result = await _teacherService.CreateTeacher(viewModel);
				if (result == "Success")
				{
					SweetAlert("Success", $"{viewModel.Name} has been added", NotificationType.success);
					return RedirectToAction("Index", "Home");
				}

				ModelState.AddModelError("", result);
				return View(viewModel);
			}
			ModelState.AddModelError("", "Invalid data");
			return View(viewModel);
		}


		[Authorize(Roles = "Admin")]
		public IActionResult UploadTeachers()	
		{
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UploadTeachers(IFormFile file)
		{
			if (file != null)
			{
				var res = await _teacherService.UploadTeachers(file);
				if (res == "Success")
				{
					SweetAlert(file.FileName, "Upload Successful", NotificationType.success);
					return RedirectToAction("AllTeachers", "Teacher");
				}
				TempData["UploadError"] = res;
				return View(file);
			}
			TempData["UploadError"] = "Select a file";
			return View();
		}
		
		public IActionResult AllTeachers(string type = "active")
		{
			TempData["type"] = type == "active" ? "inactive" : "active";

			var teachers = _teacherService.GetTeachers(type);
			return View(teachers);
		}

		[Authorize(Roles = "Admin")]
		public IActionResult ChangeTeacherStatus(int id)
		{
			var change = _teacherService.ChangeStatus(id);
			var stat = Enum.GetName(typeof(Status), change.Status);

			SweetAlert(stat.ToUpper(), "Teacher status is " + stat, NotificationType.info);
			return RedirectToAction("AllTeachers");
		}

		[Authorize(Roles = "Admin")]
		public IActionResult DeleteTeacher(int id)
		{
			_teacherService.DeleteTeacherAsync(id);
			SweetAlert("Deleted", "Teacher deleted successful", NotificationType.error);

			return RedirectToAction("AllTeachers");
		}

		[Authorize(Roles = "Admin")]
		public IActionResult AssignView(string type = "assign")
		{
			TempData["typeA"] = type == "assign" ? "unassign" : "assign";

			var teachers = _teacherService.GetAssignTeachers(type);
			return View(teachers);
		}


		[Authorize(Roles = "Admin")]
		public IActionResult AssignTeacher(int id)
		{
			var model = _teacherService.GetTeacher(id);
			return View(model);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult AssignTeacher(AssignTeacherViewModel assignTeacherViewModel)
		{
			if (assignTeacherViewModel.SchoolId != 0 && 
                assignTeacherViewModel.LocalGovernmentId != 0)
			{
				var res = _teacherService.AssignTeacher(assignTeacherViewModel);
				if (res == "Success")
				{
					SweetAlert("Assigned", assignTeacherViewModel.Name + " has been assigned a school", NotificationType.success);
					return RedirectToAction("AssignView");
				}
				TempData["AssignError"] = res;
				return RedirectToAction("AssignTeacher", new { id = assignTeacherViewModel.Id });
			}
			TempData["AssignError"] = "Please Choose Local government and School";
			return RedirectToAction("AssignTeacher", new { id = assignTeacherViewModel.Id });
		}
		
		[Authorize(Roles = "Admin")]
		public IActionResult UnassignTeacher(int id)
		{
			
				var res = _teacherService.UnassignTeacher(id);
				if (res == "Success")
				{
					SweetAlert("Unassigned",  "Teacher has been unassigned", NotificationType.info);
					return RedirectToAction("AssignView");
				}
				SweetAlert("Error", res, NotificationType.error);
				return RedirectToAction("AssignView");

		}

		public JsonResult GetSchoolsByLga(string id)
		{
			return Json(_teacherService.GetSchoolByLGA(Convert.ToInt32(id))
                .OrderBy(school => school.SchoolName).ToList());
		}

		public IActionResult TransferTeacher( int id)
		{
			var transferTeacherViewModelModel = _teacherService.GetTransferTeacehrViewModel(id);

			return View(transferTeacherViewModelModel);
		}
		[Authorize(Roles = "Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken] public IActionResult TransferTeacher(TransferTeacehrViewModel   transferTeacherViewModel)
		{
			if (transferTeacherViewModel.SchoolIdTo != 0 && transferTeacherViewModel.LocalGovernmentIdTo != 0)
			{
				var res = _teacherService.TransferTeacher(transferTeacherViewModel);
				if (res == "Success")
				{
					SweetAlert("Transfer", transferTeacherViewModel.Name + " has been transferred",
						NotificationType.success);
					return RedirectToAction("AssignView");
				}

				SweetAlert("Error", res, NotificationType.error);
				return RedirectToAction("TransferTeacher", new {id = transferTeacherViewModel.Id});
			}
			SweetAlert("Error", "Please Choose Transfer Local government and School", NotificationType.error);

			return RedirectToAction("TransferTeacher", new { id = transferTeacherViewModel.Id });
		}
	}
}
