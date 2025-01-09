using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.Application.Interfaces
{
    public interface IOutboxMessageRepository
    {
        Task<Guid> AddSubmission(OutboxMessage outboxMessage);
        Task OutboxProcessor();
    }
}
