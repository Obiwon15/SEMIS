using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EIMS.Domain.Entities.Enum;
using EIMS.Models;

namespace EIMS.Infrastructure.Interfaces
{
	public interface ISubjectServices
	{
		string CreateSubject(string subjectName, SchoolCategory subjectLevel);
		IEnumerable<SubjectViewModel> GetAllSubjects();
		string UpdateStudent(SubjectViewModel viewModel);
		string RemoveClass(int id);
	}
}
