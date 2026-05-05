using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Project.Core.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public DateTime Timestamp { get; set; }
        [ForeignKey("Actor")]
        public string ActorId { get; set; }
        public ApplicationUser Actor { get; set; }


    }
}
