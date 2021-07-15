using System;
using System.Collections.Generic;
using System.Text;
using EIMS.Domain.Data;
using EIMS.Domain.Entities;
using EIMS.Domain.Interfaces;

namespace EIMS.Domain.Repository
{
	public class SubjectRepo : GenericRepo<Subject>, ISubjectRepo
	{
		public SubjectRepo(ApplicationDbContext context)
			: base(context)
		{
			_context = context;
		}
    }
}
