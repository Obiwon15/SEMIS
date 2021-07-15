using System;
using System.Collections.Generic;
using System.Text;
using EIMS.Domain.Entities;
using EIMS.Domain.Entities.Enum;

namespace EIMS.Domain.Interfaces
{
    public interface IExamRepo: IGenericRepo<Examination>
    {
        void ActivateOrDeactivateExam(int id, Status action);
    }
}
