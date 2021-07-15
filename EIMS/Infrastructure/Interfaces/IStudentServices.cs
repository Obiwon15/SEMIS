using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EIMS.Domain.Entities;
using EIMS.Models;
using Microsoft.AspNetCore.Http;

namespace EIMS.Infrastructure.Interfaces
{
	public interface IStudentServices
	{
		string CreateStudent(CreateStudentViewModel teacherViewModel);
		Task<string> UploadStudents(IFormFile file);
		IEnumerable<StudentViewModel> GetAllStudents(string query);
		string AssignStudent(AssignStudentViewModel assignStudentViewModel);
		string UnassignStudent(int id);
		string RemoveStudent(int id);
		IEnumerable<School> GetSchoolByLGA(int id);
		CreateStudentViewModel GetStudent(int id);
		string UpdateStudent(CreateStudentViewModel createStudentViewModel);
		string TransferStudent(StudentTransferViewModel studentViewModel);
	}
}
