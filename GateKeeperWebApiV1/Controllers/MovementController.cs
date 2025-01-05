using GateKeeperWebApiV1.Data;
using GateKeeperWebApiV1.Models;
using GateKeeperWebApiV1.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Pkcs;

namespace GateKeeperWebApiV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovementController : _BaseApiController
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public MovementController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        [HttpPost("PostMovement")]
        public async Task<IActionResult> PostMovement(string link, string shiftId, string isEntrance)
        {
            var email = GetUserIdFromToken();
            //var email = "tomepereiraferreira@gmail.com";

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
                    return Ok("O link código QR não é válido");
                }

                
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                workerId = queryParams["WId"];

                if (string.IsNullOrEmpty(workerId))
                {
                    return Ok("O link código QR não é válido");
                }

            }
            catch (UriFormatException)
            {
                return Ok("O link código QR não é válido");
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

            return Ok("Movimento registado");
        }


        [HttpGet("GetWorker")]
        public async Task<IActionResult> GetWorker(string link)
        {
            var email = GetUserIdFromToken();
            //var email = "tomepereiraferreira@gmail.com";

            var user = await userManager.FindByEmailAsync(email);


            if (user == null)
            {
                return Unauthorized("Utilizador não autenticado");
            }

            string workerId = "";

            try
            {
                var uri = new Uri(link);

                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                workerId = queryParams["WId"];
            }
            catch (UriFormatException)
            {
                
            }

            var worker = await dbContext.WorkerProfiles.Include(w => w.ApplicationUser).Where(w => w.Id.ToString() == workerId).FirstOrDefaultAsync();

            WorkerInfoDto response = new WorkerInfoDto()
            {
                number = worker.InternalNumber.ToString(),
                name = worker.ApplicationUser.Name + " " + worker.ApplicationUser.Surname,
                email = worker.ApplicationUser.Email
            };

            return Ok(response);
        }
    }
}
