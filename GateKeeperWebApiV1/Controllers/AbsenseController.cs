using GateKeeperWebApiV1.Data;
using GateKeeperWebApiV1.Models;
using GateKeeperWebApiV1.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;

namespace GateKeeperWebApiV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AbsenseController : _BaseApiController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;

        public AbsenseController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        [HttpGet("GetAbsencesJus")]
        public async Task<IActionResult> GetAbsencesJus(string companyId)
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

            var absensesJus = await dbContext.AbsenceJustifications
                .Where(a => a.CompanyId == Guid.Parse(companyId)).Where(a => a.WorkerId == userWp.Id).ToListAsync();

            List<AbsenceJustificationDto> model = new List<AbsenceJustificationDto>();

            foreach (var aj in absensesJus)
            {
                model.Add(new AbsenceJustificationDto()
                {
                    id = aj.Id.ToString(),
                    start = aj.AbsenceStarted,
                    end = aj.AbsenceFinished,
                    justification = aj.Justification,
                    status = aj.Status
                });
            }

            return Ok(model);
        }

        [HttpPost("PostAbsencesJus")]
        public async Task<IActionResult> PostAbsencesJus( string start, string end, string justification, string status, string companyId)
        {
            var email = GetUserIdFromToken();
            

            var user = await userManager.FindByEmailAsync(email);


            if (user == null)
            {
                return Unauthorized("Utilizador não autenticado");
            }

            DateTime dateTimeStart = DateTime.Parse(start);
            DateTime dateTimeEnd = DateTime.Parse(end);

            var userWps = await dbContext.WorkerProfiles
                .Where(wp => wp.ApplicationUserId == user.Id).ToListAsync();
            var userWp = userWps.Where(wp => wp.CompanyId == Guid.Parse(companyId)).FirstOrDefault();

            //string aaaa = "aaaaa";

            AbsenceJustification absJus = new AbsenceJustification()
            {
                WorkerId = userWp.Id,
                CompanyId = Guid.Parse(companyId),
                AbsenceStarted = dateTimeStart,
                AbsenceFinished = dateTimeEnd,
                Justification = justification,
                Status = status
            };

            await dbContext.AbsenceJustifications.AddAsync(absJus);
            await dbContext.SaveChangesAsync();

            return Ok();
        }



        [HttpPut("PutAbsencesJus")]
        public async Task<IActionResult> PutAbsencesJus(string start, string end, string justification, string status, string companyId, string id)
        {
            var email = GetUserIdFromToken();


            var user = await userManager.FindByEmailAsync(email);


            if (user == null)
            {
                return Unauthorized("Utilizador não autenticado");
            }

            DateTime dateTimeStart = DateTime.Parse(start);
            DateTime dateTimeEnd = DateTime.Parse(end);

            var userWps = await dbContext.WorkerProfiles
                .Where(wp => wp.ApplicationUserId == user.Id).ToListAsync();
            var userWp = userWps.Where(wp => wp.CompanyId == Guid.Parse(companyId)).FirstOrDefault();

            var absjus = await dbContext.AbsenceJustifications.FindAsync(Guid.Parse(id));

            absjus.AbsenceStarted = dateTimeStart;
            absjus.AbsenceFinished = dateTimeEnd;
            absjus.Justification = justification;
            absjus.Status = status;

            await dbContext.SaveChangesAsync();

            return Ok();
        }


        [HttpDelete("DeleteAbsencesJus")]
        public async Task<IActionResult> DeleteAbsencesJus(string id)
        {
            var email = GetUserIdFromToken();


            var user = await userManager.FindByEmailAsync(email);


            if (user == null)
            {
                return Unauthorized("Utilizador não autenticado");
            }

            var absjus = await dbContext.AbsenceJustifications.FindAsync(Guid.Parse(id));

            dbContext.Remove(absjus);

            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
