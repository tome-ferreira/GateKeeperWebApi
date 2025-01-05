using GateKeeperWebApiV1.Data;
using GateKeeperWebApiV1.Models;
using GateKeeperWebApiV1.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace GateKeeperWebApiV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : _BaseApiController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public HomePageController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        [HttpGet("GetDayTimeTable")]
        public async Task<IActionResult> GetDayTimeTable(string date, string companyId)
        {
            var email = GetUserIdFromToken();
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Unauthorized("Utilizador não autenticado");
            }

            var userWps = await dbContext.WorkerProfiles
                .Where(wp => wp.ApplicationUserId == user.Id).ToListAsync();
            var userWp = userWps.Where(wp => wp.CompanyId == Guid.Parse(companyId)).FirstOrDefault();

            var shifts = await dbContext.WorkerShifts
                .Where(w => w.WorkerId == userWp.Id)
                .Include(w => w.Shift)
                .ThenInclude(s => s.ShiftDays)
                .Include(w => w.Shift)
                .ThenInclude(s => s.ShiftDaysOfWeeks)
                .Select(w => w.Shift)
                .ToListAsync();

            var responseList = new List<ShiftHappensInstanceDto>();

            if (date.Contains(" "))
            {
                date = date.Substring(0, date.IndexOf(" "));
            }

            DateTime now = DateTime.Parse(date);
            var today = now.Date;

            foreach (var shift in shifts)
            {
                // To hold the next occurrence of this shift
                DateTime? nextStart = null;
                DateTime? nextEnd = null;

                // Handle one-time shifts
                if (shift.StartsDate.Date == today)
                {
                    nextStart = shift.StartsDate.Date + shift.Starts.ToTimeSpan();
                    nextEnd = shift.StartsDate.Date + shift.Ends.ToTimeSpan();
                    if (shift.IsOvernight) nextEnd = nextEnd.Value.AddDays(1);
                }
                else if (shift.ShiftDaysOfWeeks.Any()) // Handle recurring shifts
                {
                    // Get the current day of the week (Sunday = 1, Monday = 2, ..., Saturday = 7)
                    int currentDayOfWeek = (int)now.DayOfWeek + 1;

                    // Check if the shift is scheduled for today
                    if (shift.ShiftDaysOfWeeks.Any(d => d.DayOfWeek == currentDayOfWeek))
                    {
                        nextStart = today + shift.Starts.ToTimeSpan();
                        nextEnd = today + shift.Ends.ToTimeSpan();
                        if (shift.IsOvernight) nextEnd = nextEnd.Value.AddDays(1);
                    }
                }

                // Only add shifts that start today
                if (nextStart.HasValue && nextStart.Value.Date == today)
                {
                    responseList.Add(new ShiftHappensInstanceDto
                    {
                        id = shift.Id.ToString(),
                        name = shift.Name,
                        starts = nextStart.Value,
                        ends = nextEnd.Value
                    });
                }
            }

            return Ok(responseList);
        }


        [HttpGet("GetUserLink")]
        public async Task<IActionResult> GetUserLink(string companyId)
        {
            var email = GetUserIdFromToken();
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Unauthorized("Utilizador não autenticado");
            }

            var userWps = await dbContext.WorkerProfiles
                .Where(wp => wp.ApplicationUserId == user.Id).ToListAsync();
            var userWp = userWps.Where(wp => wp.CompanyId == Guid.Parse(companyId)).FirstOrDefault();

            string link = "https://gatekeeper.xiscard.eu/Q/Index?WId=" + userWp.Id.ToString();

            LinkDto response = new LinkDto()
            {
                link = link
            };

            return Ok(response);
        }

    }
}
