using GateKeeperWebApiV1.Data;
using GateKeeperWebApiV1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Pkcs;

namespace GateKeeperWebApiV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovementController : _BaseApiController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<IdentityUser> userManager;

        public MovementController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        [HttpPost("PostMovement")]
        public async Task<IActionResult> PostMovement(string link, string shiftId, string isEntrance)
        {
            var email = GetUserIdFromToken();

            var user = await userManager.FindByEmailAsync(email);


            if (user == null)
            {
                return Unauthorized("Utilizador não autenticado");
            }

            string workerId = "";
            bool isEntranceBool = true;


            try
            {
                var uri = new Uri(link);

                
                if (uri.Host != "gatekeeper.xiscard.eu" || uri.AbsolutePath != "/Q/Index")
                {
                    return BadRequest("O link código QR não é válido");
                }

                
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                workerId = queryParams["WId"];

                if (string.IsNullOrEmpty(workerId))
                {
                    return BadRequest("O link código QR não é válido");
                }

            }
            catch (UriFormatException)
            {
                return BadRequest("O link código QR não é válido");
            }

            if (isEntrance == "true")
            {
                isEntranceBool = true;
            }
            else if (isEntrance == "false") 
            {
                isEntranceBool = false;
            }

            Movement movement = new Movement() 
            { 
                WorkerId = Guid.Parse(workerId),
                ShiftId = Guid.Parse(shiftId),
                DateTime = DateTime.Now,
                isEntrance = isEntranceBool
            };

            await dbContext.Movements.AddAsync(movement);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
