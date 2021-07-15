using System;
using System.Collections.Generic;
using EIMS.Domain.Entities;
using EIMS.Domain.Entities.Enum;
using EIMS.Models;

namespace EIMS.Infrastructure.Interfaces
{
    public interface IExamService
    {
        ExamViewModel GetExam(int examId);
        int AddExam(AddExamViewModel exam);
        int UpdateExam(ExamViewModel exam);
        IEnumerable<ExamViewModel> GetAllExams(string status = null);
        Examination DeleteExam(int examId);
        int ActivateOrDeactivateExam(int examId, Status status);
    }
}
