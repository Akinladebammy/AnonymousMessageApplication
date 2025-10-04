using AnonymousMessageApplication.Models.Enums;

namespace AnonymousMessageApplication.DTOs
{
    // Student submits complaint
    public class ComplaintRequest
    {
        public string Description { get; set; } = string.Empty;
    }

    // Response DTO for complaint list
    public class ComplaintResponse
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public ComplaintStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
