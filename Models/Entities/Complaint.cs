using AnonymousMessageApplication.Models.Enums;

namespace AnonymousMessageApplication.Models.Entities
{
    public class Complaint
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        //public string Title { get; set; }
        public string Description { get; set; }
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
