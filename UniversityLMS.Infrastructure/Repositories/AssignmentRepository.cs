using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Domain.Entities;
using UniversityLMS.Infrastructure.DbContexts;

namespace UniversityLMS.Infrastructure.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly AppDbContext _context;

        public AssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAssignmentAsync(AssignmentDTO assignment)
        {
            var assignmentnew = new Assignment
            {
                Title = assignment.Title,
                Description = assignment.Description,
                DueDate = assignment.DueDate,
                
            };
            await _context.Assignments.AddAsync(assignmentnew);
            await _context.SaveChangesAsync();
            foreach (var testcase in assignment.TestCases)
            {
                var test = new TestCase
                {
                    AssignmentId= assignmentnew.Id,
                    Input = testcase.Input,
                    ExpectedOutput = testcase.ExpectedOutput,
                };
                await _context.TestCases.AddAsync(test);
            }
            await _context.SaveChangesAsync();
        }







        public async Task<IEnumerable<Assignment>> GetAllAssignmentsAsync()
        {
            return await _context.Assignments.Include(a => a.TestCases).ToListAsync();
        }


        public async Task<Assignment> GetAssignmentByIdAsync(Guid id)
        {
            return await _context.Assignments
                                .Include(a => a.TestCases)
                                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
