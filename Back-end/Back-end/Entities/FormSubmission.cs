using System;

namespace Core.Domain.Entities
{
    public class FormSubmission
    {
        public int Id { get; set; }
        public string LeadName { get; set; }
        public string ContactEmail { get; set; }
        public string StatusShipping { get; set; }
        public int TemplateId { get; set; }
        public string FormDataJson { get; set; }
        public DateTime DateOfSubmission { get; set; } = DateTime.UtcNow;
    }
}