using AnonymousMessageApplication.Data;
using AnonymousMessageApplication.DTOs;
using AnonymousMessageApplication.Models.Entities;
using AnonymousMessageApplication.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnonymousMessageApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComplaintsController(AppDbContext context)
        {
            _context = context;
        }

        // Students submit anonymous complaint
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateComplaint([FromBody] ComplaintRequest request)
        {
            var complaint = new Complaint
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Complaint submitted successfully", ComplaintId = complaint.Id });
        }

        // Admins & SuperAdmin get all complaints
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetComplaints()
        {
            var complaints = await _context.Complaints
                .Select(c => new ComplaintResponse
                {
                    Id = c.Id,
                    Description = c.Description,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(complaints);
        }

        // Admins & SuperAdmin update complaint status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] ComplaintStatus status)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            complaint.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Complaint status updated successfully" });
        }

        // Get single complaint
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetComplaint(Guid id)
        {
            var complaint = await _context.Complaints
                .Where(c => c.Id == id)
                .Select(c => new ComplaintResponse
                {
                    Id = c.Id,
                    Description = c.Description,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (complaint == null) return NotFound();

            return Ok(complaint);
        }
    }
}
