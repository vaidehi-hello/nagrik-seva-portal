using Microsoft.AspNetCore.Mvc;
using NagrikSevaAPI.Models;
using NagrikSevaAPI.Repositories.Interfaces;

namespace NagrikSevaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitizenController : ControllerBase
    {
        private readonly IApplicationRepository _appRepo;
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public CitizenController(
            IApplicationRepository appRepo,
            IAuthRepository authRepo,
            IConfiguration config,
            IWebHostEnvironment env)
        {
            _appRepo = appRepo;
            _authRepo = authRepo;
            _config = config;
            _env = env;
        }

        // POST: api/citizen/apply
        [HttpPost("apply")]
        public async Task<IActionResult> Apply(
            [FromBody] ApplyRequest request)
        {
            var application = new Application
            {
                CitizenId = request.CitizenId,
                ServiceId = request.ServiceId,
                Description = request.Description,
                Status = "Pending"
            };

            var created = await _appRepo.CreateAsync(application);
            return Ok(new {
                message = "Application submitted successfully!",
                appNo = created.AppNo,
                id = created.Id
            });
        }

        // ✅ UPDATED: POST api/citizen/apply-with-details (3 documents)
        [HttpPost("apply-with-details")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ApplyWithDetails(
            [FromForm] ApplyWithDetailsRequest request)
        {
            try
            {
                // Step 1: Create application
                var application = new Application
                {
                    CitizenId = request.CitizenId,
                    ServiceId = request.ServiceId,
                    Description = request.Description,
                    Status = "Pending"
                };

                var created = await _appRepo
                    .CreateAsync(application);

                // Step 2: Prepare uploads folder
                var uploadsFolder = Path.Combine(
                    _env.WebRootPath ?? "wwwroot",
                    "uploads",
                    "documents");

                Directory.CreateDirectory(uploadsFolder);

                // Step 3: Build details object
                var details = new ApplicationDetails
                {
                    ApplicationId = created.Id,
                    FullName = request.FullName,
                    DateOfBirth = request.DateOfBirth,
                    AadhaarNumber = request.AadhaarNumber,
                    FatherName = request.FatherName,
                    Address = request.Address,
                    MobileNumber = request.MobileNumber
                };

                // Helper to save each uploaded file
                async Task<(string? path, string? name, string? type)> SaveFile(
                    IFormFile? file, string prefix)
                {
                    if (file == null || file.Length == 0)
                        return (null, null, null);

                    var ext = Path.GetExtension(file.FileName);
                    var fileName =
                        $"{created.AppNo}_{prefix}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(
                        filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    return (fileName, file.FileName, file.ContentType);
                }

                // Save Aadhaar document
                var (aPath, aName, aType) =
                    await SaveFile(request.AadhaarDocument, "aadhaar");
                details.AadhaarDocPath = aPath;
                details.AadhaarDocName = aName;
                details.AadhaarDocType = aType;

                // Save Photo document
                var (pPath, pName, pType) =
                    await SaveFile(request.PhotoDocument, "photo");
                details.PhotoDocPath = pPath;
                details.PhotoDocName = pName;
                details.PhotoDocType = pType;

                // Save Address proof document
                var (adPath, adName, adType) =
                    await SaveFile(request.AddressDocument, "address");
                details.AddressDocPath = adPath;
                details.AddressDocName = adName;
                details.AddressDocType = adType;

                await _appRepo.SaveDetailsAsync(details);

                return Ok(new {
                    message =
                        "Application submitted successfully!",
                    appNo = created.AppNo,
                    id = created.Id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = "Failed: " + ex.Message
                });
            }
        }

        // ✅ UPDATED: GET api/citizen/document/{appId}/{docType}
        // docType = aadhaar | photo | address
        [HttpGet("document/{appId}/{docType}")]
        public async Task<IActionResult> GetDocument(
            int appId, string docType)
        {
            var app = await _appRepo.GetByIdAsync(appId);

            if (app?.Details == null)
                return NotFound(new {
                    message = "Application details not found!"
                });

            string? path = null, name = null, contentType = null;

            switch (docType.ToLower())
            {
                case "aadhaar":
                    path = app.Details.AadhaarDocPath;
                    name = app.Details.AadhaarDocName;
                    contentType = app.Details.AadhaarDocType;
                    break;
                case "photo":
                    path = app.Details.PhotoDocPath;
                    name = app.Details.PhotoDocName;
                    contentType = app.Details.PhotoDocType;
                    break;
                case "address":
                    path = app.Details.AddressDocPath;
                    name = app.Details.AddressDocName;
                    contentType = app.Details.AddressDocType;
                    break;
                default:
                    return BadRequest(new {
                        message = "Invalid document type! Use aadhaar, photo, or address."
                    });
            }

            if (path == null)
                return NotFound(new {
                    message = "Document not uploaded!"
                });

            var filePath = Path.Combine(
                _env.WebRootPath ?? "wwwroot",
                "uploads",
                "documents",
                path);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new {
                    message = "File not found on server!"
                });

            var fileBytes =
                await System.IO.File.ReadAllBytesAsync(filePath);

            return File(fileBytes,
                contentType ?? "application/octet-stream",
                name ?? "document");
        }

        // GET api/citizen/certificate/{appId}
        [HttpGet("certificate/{appId}")]
        public async Task<IActionResult> GetCertificate(
            int appId)
        {
            var app = await _appRepo.GetByIdAsync(appId);

            if (app == null)
                return NotFound(new {
                    message = "Application not found!"
                });

            if (app.Status != "Approved")
                return BadRequest(new {
                    message =
                        "Certificate only available for approved applications!"
                });

            if (app.CertificatePath == null)
                return NotFound(new {
                    message = "Certificate not yet generated!"
                });

            var certPath = Path.Combine(
                _env.WebRootPath ?? "wwwroot",
                "uploads",
                "certificates",
                app.CertificatePath);

            if (!System.IO.File.Exists(certPath))
                return NotFound(new {
                    message = "Certificate file not found!"
                });

            var fileBytes =
                await System.IO.File.ReadAllBytesAsync(
                    certPath);

            return File(fileBytes, "application/pdf",
                $"Certificate_{app.AppNo}.pdf");
        }

        // Existing endpoints below
        [HttpGet("applications/{citizenId}")]
        public async Task<IActionResult> GetMyApplications(
            int citizenId)
        {
            var applications = await _appRepo
                .GetByCitizenAsync(citizenId);
            return Ok(applications);
        }

        [HttpGet("track/{appNo}")]
        public async Task<IActionResult> TrackApplication(
            string appNo)
        {
            var application = await _appRepo
                .GetByAppNoAsync(appNo);
            if (application == null)
                return NotFound(new {
                    message = "Application not found!"
                });
            return Ok(application);
        }

        [HttpGet("stats/{citizenId}")]
        public async Task<IActionResult> GetStats(
            int citizenId)
        {
            var stats = await _appRepo
                .GetCitizenStatusCountsAsync(citizenId);
            return Ok(stats);
        }
    }

    public class ApplyRequest
    {
        public int CitizenId { get; set; }
        public int ServiceId { get; set; }
        public string? Description { get; set; }
    }

    // ✅ UPDATED: now takes 3 separate documents
    public class ApplyWithDetailsRequest
    {
        public int CitizenId { get; set; }
        public int ServiceId { get; set; }
        public string? Description { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string AadhaarNumber { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public IFormFile? AadhaarDocument { get; set; }
        public IFormFile? PhotoDocument { get; set; }
        public IFormFile? AddressDocument { get; set; }
    }
}