namespace NagrikSevaAPI.Models
{
    public class ApplicationTracking
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public Application Application { get; set; } = null!;
        public int OfficerId { get; set; }
        public User Officer { get; set; } = null!;
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.Now;
    }
}