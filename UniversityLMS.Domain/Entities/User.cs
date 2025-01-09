using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UniversityLMS.Domain.Entities
{
    public class User
    {

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; } = string.Empty;

        public string UniversityID { get; set; }

        public string Email { get; set; }

        public string? Password { get; set; }

        public string Phone { get; set; }

        public string Role { get; set; }

         
        
        [JsonIgnore]
        public ICollection<Submission>? Submissions { get; set; } = new List<Submission>();
    }
}
