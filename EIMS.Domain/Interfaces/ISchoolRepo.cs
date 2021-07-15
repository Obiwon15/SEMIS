﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EIMS.Domain.Entities;

namespace EIMS.Domain.Interfaces
{
	public interface ISchoolRepo
	{
		int CreateSchool(School school, string appUser);
		void DeleteSchool(int id);
		School FindSchool(int id);
		School FindSchoolEmail(string email);
		IEnumerable<School> GetAllSchoolByLga(int lgaId);
		Task<IEnumerable<School>> GetAllSchoolsAsync();
		int UpdateSchool(School updateSchool);
	}
}
