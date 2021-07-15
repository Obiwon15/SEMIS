﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EIMS.Domain.Entities;
using EIMS.Domain.Entities.Enum;
using EIMS.Domain.Identity;
using EIMS.Domain.Interfaces;
using EIMS.Infrastructure.Interfaces;
using EIMS.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace EIMS.Infrastructure.Services
{
	public class TeacherService : ITeacherService
	{
		private readonly IMapper _mapper;
		private readonly ITeacherRepo _teacherRepo;
		private readonly IProfileRepo _profileRepo;
		private readonly IAccountServices _accountServices;
		private readonly IRoleServices _roleServices;
		private readonly ITeacherLoggerRepo _LogAction;

		public TeacherService(IMapper mapper,
			ITeacherRepo teacherRepo,
			IRoleServices roleServices,
			IProfileRepo profileRepo,
			IAccountServices accountServices,
			ITeacherLoggerRepo logAction)
		{
			_mapper = mapper;
			_teacherRepo = teacherRepo;
			_profileRepo = profileRepo;
			_roleServices = roleServices;
			_accountServices = accountServices;
			_LogAction = logAction;
		}

		public async Task<string> CreateTeacher(CreateTeacherViewModel teacherViewModel)
		{
			var user = _mapper.Map<AppUser>(teacherViewModel);
			user.UserName = teacherViewModel.Email;
			var errorMessage = "";
			try
			{
				var result = await _accountServices.CreateUser(user);
				if (result.Succeeded)
				{
					//Assign Teacher role to user
					var role = await _roleServices.FindRoleByNameAsync("Teacher");
					if (role != null)
					{
						await _roleServices.AssignRoleAsync(user, role.Id);

                        var (saved, profile) = await _profileRepo.CreateProfile(user.Id);
						if (saved > 0)
                        {
							var teacher = new Teacher() { UserId = user.Id, ProfileId = profile.Id };
							_teacherRepo.Insert(teacher);
							var success = _teacherRepo.Save();
							if (success > 0)
							{
								return "Success";
							}
						}

					}

				}

				return result.Errors.Aggregate(errorMessage, (current, error) => current + $"{error.Description} \n");
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

		public IEnumerable<TeacherViewModel> GetTeachers(string query = null)
		{
			var teachers = _teacherRepo.AllInclude(t => t.User);
			var teachersViewModels = _mapper.Map<IEnumerable<TeacherViewModel>>(teachers);
			return query switch
			{
				"active" => teachersViewModels.Where(t => t.Status == Status.Active),
				"inactive" => teachersViewModels.Where(t => t.Status == Status.Inactive),
				_ => teachersViewModels
			};
		}

		public async Task<string> UploadTeachers(IFormFile file)
		{
			try
			{
				var error = "";
				var (createTeacherViewModels, item2) = await ExtractTeachersTasks(file);
				if (item2== "Success")
				{
					if (createTeacherViewModels.Count > 0)
					{
						var count = 0;
						foreach (var teacher in createTeacherViewModels)
						{
							count++;
						var res = await CreateTeacher(teacher);
						if (res != "Success")
						{
							error += $"\n{count}. " + res + " teacher name: " + teacher.Name + "\n";
						}
						} 
						return error != "" ? error : "Success";		
					}
				    return "No data found";
				}

				return item2;
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

		public static async Task<(List<CreateTeacherViewModel>,string)> ExtractTeachersTasks(IFormFile file)
		{
			var teachers = new List<CreateTeacherViewModel>();
			try
			{
				var stream = new MemoryStream();
				await file.CopyToAsync(stream);

				var package = new ExcelPackage(stream);
				ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
				var rowCount = worksheet.Dimension.Rows;
				for (int row = 2; row <= rowCount; row++)
				{
					teachers.Add(new CreateTeacherViewModel()
					{
						Title = worksheet.Cells[row, 1].Value.ToString(),
						Name = worksheet.Cells[row, 2].Value.ToString(),
						Email = worksheet.Cells[row, 3].Value.ToString(),
						PhoneNumber = worksheet.Cells[row, 4].Value.ToString()
					});
				}
				return (teachers,"Success");

			}
			catch (Exception e)
			{
				return (teachers, e.Message);

			}

		}

		public TeacherViewModel ChangeStatus(int id)
		{
			var teacher = _teacherRepo.GetById(id);
			teacher.Status = teacher.Status == Status.Active ? Status.Inactive : Status.Active;
			_teacherRepo.Update(teacher);
			_teacherRepo.Save();
			_LogAction.Insert(_mapper.Map<TeacherLog>(teacher));
			_LogAction.Save();
			return _mapper.Map<TeacherViewModel>(teacher);
		}
		public void DeleteTeacherAsync(int id)
		{
			try
			{
				_teacherRepo.DeleteTeachers(id);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

		}
		public IEnumerable<TeacherViewModel> GetAssignTeachers(string query = null)
		{
			var teachers = _teacherRepo.AllInclude(t => t.User, t => t.LocalGovernment, t => t.School);
			var teachersViewModels = _mapper.Map<IEnumerable<TeacherViewModel>>(teachers);
			return query switch
			{
				"assign" => teachersViewModels.Where(t => t.LocalGovernmentId != null && t.SchoolId != null),
				"unassign" => teachersViewModels.Where(t => t.LocalGovernmentId == null && t.SchoolId == null),
				_ => teachersViewModels
			};
		}
		public AssignTeacherViewModel GetTeacher(int id)
		{
			var AssignModel = _teacherRepo.AllInclude(t => t.User, t => t.LocalGovernment, t => t.School)
				.FirstOrDefault(teacher => teacher.Id == id);
			var model = _mapper.Map<AssignTeacherViewModel>(AssignModel);
			model.LocalGovernments = _teacherRepo.RetLGA().ToList();
			return model;
		}

		public IEnumerable<School> GetSchoolByLGA(int id)
		{
			var schools = _teacherRepo.RetSchools().Where(school => school.LocalGovernmentId == id);
			return schools;
		}

		public string AssignTeacher(AssignTeacherViewModel assignTeacherViewModel)
		{
			try
			{
				var teacher = _teacherRepo.AllInclude(u => u.User, l => l.LocalGovernment, s => s.School).FirstOrDefault(t => t.Id == assignTeacherViewModel.Id);
				teacher.LocalGovernmentId = assignTeacherViewModel.LocalGovernmentId;
				teacher.SchoolId = assignTeacherViewModel.SchoolId;

				_teacherRepo.Update(teacher);
				_teacherRepo.Save();

				var log = _mapper.Map<TeacherLog>(teacher);
				log.LogType = LogType.Assign;
				log.Details = teacher.User.Name + " was assigned to LGA: " + _teacherRepo.RetLGA().FirstOrDefault(l => l.Id == assignTeacherViewModel.LocalGovernmentId).Name +
				              "and School: " + _teacherRepo.RetSchools().FirstOrDefault(l => l.Id == assignTeacherViewModel.SchoolId).SchoolName;
				log.CreatedAt = DateTime.Now;

				_LogAction.Insert(log);
				_LogAction.Save();
				return "Success";
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}
		public string UnassignTeacher(int id)
		{
			try
			{
				var teacher = _teacherRepo.AllInclude(u => u.User, l => l.LocalGovernment, s => s.School).FirstOrDefault(t => t.Id == id);

				var log = _mapper.Map<TeacherLog>(teacher);
				log.LogType = LogType.Unassign;
				log.Details = teacher.User.Name + " was unassigned from LGA: " + teacher.LocalGovernment.Name +
							  "and School: " + teacher.School.SchoolName;
				log.CreatedAt = DateTime.Now;
				
				_LogAction.Insert(log);
				_LogAction.Save();

				teacher.LocalGovernmentId = null;
				teacher.LocalGovernment = null;
				teacher.School = null;
				teacher.SchoolId = null;
				_teacherRepo.Update(teacher);
				_teacherRepo.Save();

				return "Success";
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

		public TransferTeacehrViewModel GetTransferTeacehrViewModel(int id)
		{
			var teacher = _teacherRepo.AllInclude(U => U.User, L => L.LocalGovernment, S => S.School).FirstOrDefault(t => t.Id == id);

			var model = _mapper.Map<TransferTeacehrViewModel>(teacher);
			model.LocalGovernmentsTo = _teacherRepo.RetLGA().ToList();
			model.SchoolsTo = _teacherRepo.RetSchools().ToList();

			return model;
		}
		public string TransferTeacher(TransferTeacehrViewModel transferTeacehrViewModel)
		{
			try
			{
				var teacher = _teacherRepo.AllInclude(U => U.User, L => L.LocalGovernment, S => S.School).FirstOrDefault(t => t.Id == transferTeacehrViewModel.Id);

				transferTeacehrViewModel.SchoolId = teacher.SchoolId.Value;
				transferTeacehrViewModel.LocalGovernmentId = teacher.LocalGovernmentId.Value;
				var log = _mapper.Map<TeacherLog>(transferTeacehrViewModel);
				log.LogType = LogType.Transfer;

				log.Details = teacher.User.Name + " was transferred from LGA: " + teacher.LocalGovernment.Name +
							  "and School: " + teacher.School.SchoolName + ", to LGA: "
							  + _teacherRepo.RetLGA().FirstOrDefault(l => l.Id == transferTeacehrViewModel.LocalGovernmentIdTo).Name +
							  "and School: " + _teacherRepo.RetSchools().FirstOrDefault(s => s.Id == transferTeacehrViewModel.LocalGovernmentId).SchoolName;
				log.CreatedAt = DateTime.Now;

				_LogAction.Insert(log);
				_LogAction.Save();

				teacher.SchoolId = transferTeacehrViewModel.SchoolIdTo;
				teacher.LocalGovernmentId = transferTeacehrViewModel.LocalGovernmentIdTo;


				_teacherRepo.Update(teacher);
				_teacherRepo.Save();
				return "Success";
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}
	}
}