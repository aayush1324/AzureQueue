using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityLMS.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AssignmentId { get; set; }
        public int LanguageId{ get; set; }
        public DateTime CreatedAt { get; set; }
        public required string FilePath { get; set; }
        public bool IsProcessed { get; set; }
        public Guid SubmissionId { get; set; }
    }
}
