using GateKeeperWebApiV1.Data;
using GateKeeperWebApiV1.Models;
using GateKeeperWebApiV1.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace GateKeeperWebApiV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyController : _BaseApiController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public CompanyController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) 
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        [HttpGet("GetCompanies")]
        public async Task<IActionResult> GetCompanies()
        {
            var email = GetUserIdFromToken();

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Unauthorized("Utilizador não autenticado");
            }

            List<Company> allCompanies = new List<Company>();

            // Get the list of Company IDs for the user
            var companiesIds = await dbContext.WorkerProfiles
                .Where(wp => wp.ApplicationUserId == user.Id)
                .Select(wp => wp.CompanyId)
                .ToListAsync();

            // Retrieve the Company objects that match the IDs
            allCompanies = await dbContext.Companies
                .Where(c => companiesIds.Contains(c.Id))
                .ToListAsync();

            List<CompanyDto> companiesDtos = new List<CompanyDto>();

            foreach(var c in allCompanies)
            {
                companiesDtos.Add(new CompanyDto() { Id = c.Id.ToString(), Name = c.Name });
            }

            return Ok(companiesDtos);
        }
    }
}
