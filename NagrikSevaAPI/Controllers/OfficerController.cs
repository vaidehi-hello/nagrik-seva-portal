using Microsoft.AspNetCore.Mvc;
using NagrikSevaAPI.Repositories.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NagrikSevaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfficerController : ControllerBase
    {
        private readonly IApplicationRepository _appRepo;
        private readonly IWebHostEnvironment _env;

        public OfficerController(
            IApplicationRepository appRepo,
            IWebHostEnvironment env)
        {
            _appRepo = appRepo;
            _env = env;
        }

        [HttpGet("applications")]
        public async Task<IActionResult> GetApplications()
        {
            var applications = await _appRepo.GetAllAsync();
            return Ok(applications);
        }

        // ✅ NEW: GET api/officer/application-details/{id}
        [HttpGet("application-details/{id}")]
        public async Task<IActionResult> GetApplicationDetails(
            int id)
        {
            var app = await _appRepo.GetByIdAsync(id);
            if (app == null)
                return NotFound(new {
                    message = "Application not found!"
                });
            return Ok(app);
        }

        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(
            int id, [FromBody] StatusRequest request)
        {
            var updated = await _appRepo.UpdateStatusAsync(
                id, request.Status,
                request.Remarks, request.OfficerId);

            if (updated == null)
                return NotFound(new {
                    message = "Application not found!"
                });

            return Ok(new {
                message = "Status updated successfully!",
                status = updated.Status
            });
        }

        // ✅ NEW: POST api/officer/approve/{id}
        // Approves AND generates certificate
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveAndGenerate(
            int id, [FromBody] StatusRequest request)
        {
            try
            {
                var app = await _appRepo.GetByIdAsync(id);
                if (app == null)
                    return NotFound(new {
                        message = "Application not found!"
                    });

                // Update status to Approved
                await _appRepo.UpdateStatusAsync(
                    id, "Approved",
                    request.Remarks, request.OfficerId);

                // Generate certificate PDF
                QuestPDF.Settings.License =
                    LicenseType.Community;

                var certFolder = Path.Combine(
                    _env.WebRootPath ?? "wwwroot",
                    "uploads",
                    "certificates");
                Directory.CreateDirectory(certFolder);

                var certFileName =
                    $"CERT_{app.AppNo}.pdf";
                var certPath = Path.Combine(
                    certFolder, certFileName);

                // Create PDF certificate
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);
                        page.DefaultTextStyle(
                            x => x.FontSize(12));

                        page.Header().Column(col =>
                        {
                            col.Item().AlignCenter()
                                .Text("GOVERNMENT OF INDIA")
                                .Bold().FontSize(16)
                                .FontColor("#1a3a6b");

                            col.Item().AlignCenter()
                                .Text("NAGRIK SEVA PORTAL")
                                .Bold().FontSize(20)
                                .FontColor("#1a3a6b");

                            col.Item().AlignCenter()
                                .Text("OFFICIAL CERTIFICATE")
                                .FontSize(14)
                                .FontColor("#FF6B00");

                            col.Item().PaddingTop(8)
                                .LineHorizontal(2)
                                .LineColor("#1a3a6b");
                        });

                        page.Content().PaddingTop(20)
                            .Column(col =>
                        {
                            col.Item().PaddingBottom(16)
                                .Text(
                                    $"Certificate No: {app.AppNo}")
                                .Bold().FontSize(13);

                            col.Item().PaddingBottom(8)
                                .Text(
                                    $"Service: {app.Service?.ServiceName ?? "N/A"}")
                                .FontSize(13);

                            col.Item().PaddingBottom(8)
                                .Text(
                                    $"Applicant: {app.Details?.FullName ?? app.Citizen?.Name ?? "N/A"}")
                                .FontSize(13);

                            if (app.Details != null)
                            {
                                col.Item().PaddingBottom(8)
                                    .Text(
                                        $"Date of Birth: {app.Details.DateOfBirth}")
                                    .FontSize(13);

                                col.Item().PaddingBottom(8)
                                    .Text(
                                        $"Aadhaar No: {app.Details.AadhaarNumber}")
                                    .FontSize(13);

                                col.Item().PaddingBottom(8)
                                    .Text(
                                        $"Address: {app.Details.Address}")
                                    .FontSize(13);
                            }

                            col.Item().PaddingVertical(12)
                                .LineHorizontal(1)
                                .LineColor("#e2e8f0");

                            col.Item().PaddingBottom(8)
                                .Text(
                                    $"Status: APPROVED")
                                .Bold().FontColor("#22c55e");

                            col.Item().PaddingBottom(8)
                                .Text(
                                    $"Approved Date: {DateTime.Now:dd MMMM yyyy}")
                                .FontSize(13);

                            col.Item().PaddingTop(40)
                                .Text(
                                    "Authorized Signatory")
                                .Bold();

                            col.Item()
                                .Text(
                                    "Nagrik Seva Portal, Government of India")
                                .FontSize(11)
                                .FontColor("#666666");
                        });

                        page.Footer().AlignCenter()
                            .Text(
                                $"This is a digitally generated certificate. Verify at nagrikseva.gov.in | {app.AppNo}")
                            .FontSize(9)
                            .FontColor("#888888");
                    });
                }).GeneratePdf(certPath);

                // Save certificate path to DB
                await _appRepo.SaveCertificatePathAsync(
                    id, certFileName);

                return Ok(new {
                    message =
                        "Application approved and certificate generated!",
                    certificateReady = true,
                    appNo = app.AppNo
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    message = "Approval failed: " + ex.Message
                });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _appRepo.GetStatusCountsAsync();
            return Ok(stats);
        }
    }

    public class StatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public int OfficerId { get; set; }
    }
}