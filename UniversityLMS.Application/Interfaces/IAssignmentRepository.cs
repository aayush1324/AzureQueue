using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.Application.Interfaces
{
    public interface IAssignmentRepository
    {
        Task AddAssignmentAsync(AssignmentDTO assignment);
        Task<IEnumerable<Assignment>> GetAllAssignmentsAsync();
        Task<Assignment> GetAssignmentByIdAsync(Guid id);
    }
}
