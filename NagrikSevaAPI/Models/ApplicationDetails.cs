namespace NagrikSevaAPI.Models
{
    public class ApplicationDetails
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public Application Application { get; set; } = null!;

        // Personal details
        public string FullName { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string AadhaarNumber { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        // Document
        // ✅ Replace single document fields with 3 sets
        public string? AadhaarDocPath { get; set; }
        public string? AadhaarDocName { get; set; }
        public string? AadhaarDocType { get; set; }

        public string? PhotoDocPath { get; set; }
        public string? PhotoDocName { get; set; }
        public string? PhotoDocType { get; set; }

        public string? AddressDocPath { get; set; }
        public string? AddressDocName { get; set; }
        public string? AddressDocType { get; set; }
    }

    
}