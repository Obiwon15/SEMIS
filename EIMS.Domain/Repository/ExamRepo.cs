using System;
using System.Collections.Generic;
using System.Text;
using EIMS.Domain.Data;
using EIMS.Domain.Entities;
using EIMS.Domain.Entities.Enum;
using EIMS.Domain.Interfaces;

namespace EIMS.Domain.Repository
{
    public class ExamRepo : GenericRepo<Examination>, IExamRepo
    {
        public ExamRepo(ApplicationDbContext context)
        : base(context)
        {
            _context = context;
        }

        public void ActivateOrDeactivateExam(int id, Status action)
        {
            var exam = GetById(id);
            if (exam != null)
            {
                switch (action)
                {
                    case Status.Active:
                        exam.ExamStatus = Status.Active;
                        break;
                    case Status.Inactive:
                        exam.ExamStatus = Status.Inactive;
                        break;
                }
                Update(exam);
            }
            else
            {
                throw new Exception("Exam cannot be found");
            }
        }
    }
}
