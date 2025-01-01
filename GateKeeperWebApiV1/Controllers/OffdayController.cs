using GateKeeperWebApiV1.Data;
using GateKeeperWebApiV1.Models;
using GateKeeperWebApiV1.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GateKeeperWebApiV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OffdayController : _BaseApiController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;

        public OffdayController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }


        [HttpGet("GetOffdayRes")]
        public async Task<IActionResult> GetOffdayRes(string companyId)
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

            var getOffdayRes = await dbContext.OffdayVacationRequests
                .Where(a => a.CompanyId == Guid.Parse(companyId)).Where(a => a.WorkerId == userWp.Id).ToListAsync();

            List<OffdayRequestDto> model = new List<OffdayRequestDto>();

            foreach (var aj in getOffdayRes)
            {
                model.Add(new OffdayRequestDto()
                {
                    id = aj.Id.ToString(),
                    start = aj.Starts,
                    end = aj.Ends,
                    notes = aj.Notes,
                    status = aj.Status
                });
            }

            return Ok(model);
        }




        [HttpPost("PostOffdayRes")]
        public async Task<IActionResult> PostOffdayRes(string start, string end, string notes, string status, string companyId)
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

            

            OffdayVacationRequest offdayres = new OffdayVacationRequest()
            {
                WorkerId = userWp.Id,
                CompanyId = Guid.Parse(companyId),
                Starts = dateTimeStart,
                Ends = dateTimeEnd,
                Notes = notes,
                Status = status
            };

            await dbContext.OffdayVacationRequests.AddAsync(offdayres);
            await dbContext.SaveChangesAsync();

            return Ok();
        }





        [HttpPut("PutOffdayRes")]
        public async Task<IActionResult> PutOffdayRes(string start, string end, string notes, string status, string companyId, string id)
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

            var offdayres = await dbContext.OffdayVacationRequests.FindAsync(Guid.Parse(id));

            offdayres.Starts = dateTimeStart;
            offdayres.Ends = dateTimeEnd;
            offdayres.Notes = notes;
            offdayres.Status = status;

            await dbContext.SaveChangesAsync();

            return Ok();
        }




        [HttpDelete("DeleteOffdayRes")]
        public async Task<IActionResult> DeleteOffdayRes(string id)
        {
            var email = GetUserIdFromToken();


            var user = await userManager.FindByEmailAsync(email);


            if (user == null)
            {
                return Unauthorized("Utilizador não autenticado");
            }

            var offdayRes = await dbContext.OffdayVacationRequests.FindAsync(Guid.Parse(id));

            dbContext.Remove(offdayRes);

            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
