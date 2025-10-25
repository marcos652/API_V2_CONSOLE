using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Back_end.Entities
{
    public class Proposal
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public DateTime ProposalDate { get; set; }
        public string StatusValidation { get; set; } 
        public string TeamMessage { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Value { get; set; }
        public string Resposible { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}