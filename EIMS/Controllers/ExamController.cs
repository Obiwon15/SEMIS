using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using EIMS.Domain.Entities.Enum;
using EIMS.Infrastructure.Interfaces;
using EIMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace EIMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExamController : BaseController
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddExam()
        {
            return View();
        }

        public JsonResult GetExams()
        {
            try
            {

                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                string searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                // int recordsTotal = 0;


                var exams = _examService.GetAllExams();

                /*if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                }*/

                if (!string.IsNullOrEmpty(searchValue))
                {
                    exams = exams.Where(ex => ex.ExaminationName.Contains(searchValue));
                }
                var count = exams.Count();

                var data= exams.Skip(skip).Take(pageSize).ToList();

                // Paging Length 10,20  
                return Json(new { draw, recordsFiltered = count, recordsTotal = count, data });
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddExam(AddExamViewModel examViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _examService.AddExam(examViewModel);
                if (result > 0)
                {
                    SweetAlert("Success", $"{examViewModel.ExamCategory} was successfully created", NotificationType.success);
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Failed to add exam, please try again");
            }

            return View(examViewModel);
        }

        public IActionResult EditExam(int id)
        {
            try
            {
                var editExam = _examService.GetExam(id);
                return View(editExam);
            }
            catch (Exception error)
            {
                SweetAlert("Error", error.Message, NotificationType.error);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditExam(ExamViewModel examViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _examService.UpdateExam(examViewModel);
                if (result > 0)
                {
                    SweetAlert("Updated", $"{examViewModel.ExamCategory} has been updated", NotificationType.success);
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError("", "Failed to update user");
            return View(examViewModel);
        }

        public IActionResult DeleteExam(int id)
        {
            try
            {
                var delExam = _examService.DeleteExam(id);
                SweetAlert("Deleted!", $"{delExam.ExaminationName} ({delExam.ExaminationCode}) has been removed",
                    NotificationType.info);
                return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                SweetAlert("Error!", error.Message, NotificationType.error);
                return RedirectToAction("Index");
            }
        }

        public IActionResult ActivateExam(int id)
        {
            try
            {
                _examService.ActivateOrDeactivateExam(id, Status.Active);
                return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                SweetAlert("Failed", error.Message, NotificationType.error);
                return RedirectToAction("Index");
            }
            
        }

        public IActionResult DeactivateExam(int id)
        {
            try
            {
                _examService.ActivateOrDeactivateExam(id, Status.Inactive);
                return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                SweetAlert("Failed", error.Message, NotificationType.error);
                return RedirectToAction("Index");
            }

        }
    }
}
