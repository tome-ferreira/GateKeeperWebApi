using GateKeeperWebApiV1.Data;
using GateKeeperWebApiV1.Models;
using GateKeeperWebApiV1.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace GateKeeperWebApiV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ShiftController : _BaseApiController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;

        public ShiftController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        [HttpGet("GetShiftsForRegister")]
        public async Task<IActionResult> GetShiftsForRegister(string companyId)
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
                .Where(w => w.Shift.ShiftLeaderId == userWp.Id)
                .Select(w => w.Shift)
                .ToListAsync();

            var responseList = new List<ShiftHappensInstanceDto>();
            var now = DateTime.Now;
            var today = now.Date;

            foreach (var shift in shifts)
            {
                // To hold the next occurrence of this shift
                DateTime? nextStart = null;
                DateTime? nextEnd = null;

                // Handle one-time shifts
                if (shift.StartsDate >= today)
                {
                    nextStart = shift.StartsDate.Date + shift.Starts.ToTimeSpan();
                    nextEnd = shift.StartsDate.Date + shift.Ends.ToTimeSpan();
                    if (shift.IsOvernight) nextEnd = nextEnd.Value.AddDays(1);
                }
                else if (shift.ShiftDaysOfWeeks.Any()) // Handle recurring shifts
                {
                    // Get the current day of the week (Sunday = 1, Monday = 2, ..., Saturday = 7)
                    int currentDayOfWeek = (int)now.DayOfWeek + 1;

                    // Find the next day this shift occurs
                    var nextDay = shift.ShiftDaysOfWeeks
                        .Select(d => d.DayOfWeek)
                        .Where(d => d >= currentDayOfWeek) // Later this week
                        .OrderBy(d => d)
                        .FirstOrDefault();

                    // If no days remain this week, wrap to the first day next week
                    if (nextDay == 0)
                    {
                        nextDay = shift.ShiftDaysOfWeeks
                            .Select(d => d.DayOfWeek)
                            .OrderBy(d => d)
                            .First();
                    }

                    // Calculate the date of the next occurrence
                    var daysUntilNext = (nextDay - currentDayOfWeek + 7) % 7;
                    var nextDate = today.AddDays(daysUntilNext);

                    // Calculate start and end times
                    nextStart = nextDate.Date + shift.Starts.ToTimeSpan();
                    nextEnd = nextDate.Date + shift.Ends.ToTimeSpan();
                    if (shift.IsOvernight) nextEnd = nextEnd.Value.AddDays(1);
                }

                // If a next occurrence was calculated, add it to the response
                if (nextStart.HasValue && nextEnd.HasValue)
                {
                    responseList.Add(new ShiftHappensInstanceDto
                    {
                        id = shift.Id.ToString(),
                        name = shift.Name,
                        starts = nextStart.Value.AddMinutes(-10),
                        ends = nextEnd.Value.AddMinutes(10)
                    });
                }
            }

            return Ok(responseList); // Return all calculated shift occurrences
        }


        [HttpGet("GetWorkersOfShift")]
        public async Task<IActionResult> GetWorkersOfShift(string shiftId, string starts, string ends)
        {
            var email = GetUserIdFromToken();

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Unauthorized("Utilizador não autenticado");
            }

            if (starts.Contains(" "))
            {
                starts = starts.Substring(0, starts.IndexOf(" "));
            }

            if (ends.Contains(" "))
            {
                ends = ends.Substring(0, ends.IndexOf(" "));
            }

            DateTime dateTimeStart = DateTime.Parse(starts);
            DateTime dateTimeEnd = DateTime.Parse(ends);

            var shift = await dbContext.Shifts
                .Where(s => s.Id.ToString() == shiftId)
                .Include(s => s.WorkerShifts)
                .ThenInclude(ws => ws.Worker)
                .ThenInclude(w => w.ApplicationUser)
                .Include(s => s.Movements).FirstOrDefaultAsync();

            List<WorkerOnShiftDto> responde = new List<WorkerOnShiftDto>();

            foreach(var worker in shift.WorkerShifts)
            {
                WorkerOnShiftDto dto = new WorkerOnShiftDto() 
                {
                    name = worker.Worker.ApplicationUser.Name + " " + worker.Worker.ApplicationUser.Surname
                };

                if(!shift.Movements
                    .Where(w => w.WorkerId == worker.WorkerId)
                    .Where(w => w.DateTime >= dateTimeStart)
                    .Where(w => w.DateTime <= dateTimeEnd)
                    .Any())
                {
                    dto.whereIs = "Falta";
                }
                else
                {
                    var newestMovement = shift.Movements
                        .Where(w => w.WorkerId == worker.WorkerId)
                        .Where(w => w.DateTime >= dateTimeStart)
                        .Where(w => w.DateTime <= dateTimeEnd)
                        .OrderByDescending(w => w.DateTime)
                        .FirstOrDefault();

                    if (newestMovement != null)
                    {
                        if (newestMovement.isEntrance)
                        {
                            dto.whereIs = "A trabalhar";
                        }
                        else
                        {
                            dto.whereIs = "Ausente";
                        }
                    }
                    else
                    {
                        dto.whereIs = "Falta"; 
                    }
                }

                responde.Add(dto);
            }

            return Ok(responde);
        }

    }
}
