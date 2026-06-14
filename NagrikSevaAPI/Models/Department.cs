namespace NagrikSevaAPI.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public ICollection<Service> Services { get; set; }
            = new List<Service>();
    }
}