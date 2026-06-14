namespace NagrikSevaAPI.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string AppNo { get; set; } = string.Empty;
        public int CitizenId { get; set; }
        public User Citizen { get; set; } = null!;
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        public string Status { get; set; } = "Pending";
        // Pending, UnderReview, Approved, Rejected
        public string? Description { get; set; }
        public DateTime SubmittedOn { get; set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; set; }
        //  NEW: link to details and certificate
        public ApplicationDetails? Details { get; set; }
        public string? CertificatePath { get; set; }

        public ICollection<ApplicationTracking> Trackings { get; set; }
            = new List<ApplicationTracking>();
    }
}