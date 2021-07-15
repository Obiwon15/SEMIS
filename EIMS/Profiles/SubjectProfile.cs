using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EIMS.Domain.Entities;
using EIMS.Models;

namespace EIMS.Profiles
{
	public class SubjectProfile : Profile
	{
		public SubjectProfile()
		{
			CreateMap<SubjectViewModel, Subject>().ReverseMap();
		}
    }
}
