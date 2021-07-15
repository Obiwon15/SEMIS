using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EIMS.Domain.Interfaces;
using EIMS.Infrastructure.Interfaces;
using EIMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EIMS.Controllers
{
	[Authorize]
	public class HomeController : BaseController
	{
		private readonly ISchoolService _schoolServices;
		private readonly ITeacherService _teacherService;
		private readonly ILocalGovernmentRepo _localGovernmentRepo;
		private readonly IStudentServices _studentServices;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ITeacherLoggerRepo _loggerRepo;
		public HomeController(ISchoolService schoolServices, ITeacherService teacherService, 
			ILocalGovernmentRepo localGovernmentRepo, IHttpContextAccessor httpContextAccessor,
			ITeacherLoggerRepo loggerRepo, IStudentServices studentServices)
		{
			_schoolServices = schoolServices;
			_teacherService = teacherService;
			_localGovernmentRepo = localGovernmentRepo;
			_httpContextAccessor = httpContextAccessor;
			_loggerRepo = loggerRepo;
			_studentServices= studentServices;

		}
		[Route("Admin")]
		public IActionResult Index()
		{
			if (User.IsInRole("SchoolAdmin"))
			{
				return RedirectToAction("SchoolIndex");
			}
			if (User.IsInRole("Teacher"))
			{
				return RedirectToAction("TeacherIndex");
			}

			var model = new AdminViewModel() {
				LocalGovernments = _localGovernmentRepo.GetAll(),
				Schools = _schoolServices.GetSchools(),
				Teachers = _teacherService.GetTeachers(),
				Students = _studentServices.GetAllStudents("")
			};
			return View(model);
		}

		[Route("School")]
		public IActionResult SchoolIndex()
		{
			var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
			var model = _schoolServices.GetSchools().FirstOrDefault(s => s.Email == email);
			return View(model);
		}

		[Route("Teacher")]
		public IActionResult TeacherIndex()
		{
			var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			var model = _teacherService.GetAssignTeachers().FirstOrDefault(t => t.UserId==userId);
			var logs = _loggerRepo.GetAll().Where(log => log.TeacherId == model.Id).OrderByDescending(l=>l.CreatedAt);
			model.TeacherLogs = logs;
			return View(model);
		}
	}
}
