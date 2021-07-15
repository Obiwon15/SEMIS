using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EIMS.Domain.Entities;
using EIMS.Domain.Entities.Enum;
using EIMS.Domain.Interfaces;
using EIMS.Infrastructure.Interfaces;
using EIMS.Models;

namespace EIMS.Infrastructure.Services
{
    public class ExamService: IExamService
    {
        private readonly IExamRepo _examRepo;
        private readonly IMapper _mapper;

        public ExamService(IExamRepo examRepo, IMapper mapper)
        {
            _examRepo = examRepo;
            _mapper = mapper;
        }

        public int AddExam(AddExamViewModel exam)
        {
            var examination = _mapper.Map<Examination>(exam);
            _examRepo.Insert(examination);
            return _examRepo.Save();
        }

        public int UpdateExam(ExamViewModel exam)
        {
            var examination = _mapper.Map<Examination>(exam);
            _examRepo.Update(examination);
            return _examRepo.Save();
        }

        public Examination DeleteExam(int examId)
        {
            var exam = _examRepo.GetById(examId);
            if (exam == null)
            {
                throw new Exception("Exam does not exist in the database");
            }
            _examRepo.Delete(examId);
            var result = _examRepo.Save();
            if (result < 1)
            {
                throw new Exception($"Failed to delete exam");
            }

            return exam;
        }

        public IEnumerable<ExamViewModel> GetAllExams(string status = null)
        {
            switch (status)
            {
                case "Active":
                    return _mapper.Map<IEnumerable<ExamViewModel>>(_examRepo.Find(ex=>ex.ExamStatus==Status.Active));
                case "Inactive":
                    return _mapper.Map<IEnumerable<ExamViewModel>>(_examRepo.Find(ex => ex.ExamStatus == Status.Inactive));
            }
            return _mapper.Map<IEnumerable<ExamViewModel>>(_examRepo.GetAll());
        }

        public ExamViewModel GetExam(int examId)
        { 
            var exam = _examRepo.GetById(examId);
            if (exam == null)
            {
                throw new Exception("Exam was not found");
            }

            return _mapper.Map<ExamViewModel>(exam);
            
        }

        public int ActivateOrDeactivateExam(int examId, Status status)
        {
            var exam = _examRepo.GetById(examId);
            if (exam == null)
            {
                throw new Exception("Exam was not found");
            }

            switch (status)
            {
                case Status.Active:
                    exam.ExamStatus = Status.Active;
                    break;
                case Status.Inactive:
                    exam.ExamStatus = Status.Inactive;
                    break;
            }
            _examRepo.Update(exam);
            var result = _examRepo.Save();
            var action = status == Status.Active ? "activate" : "deactivate";
            if (result < 1)
            {
                throw new Exception($"Failed to {action} exam");
            }

            return result;
        }
    }
}
