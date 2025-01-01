using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GateKeeperWebApiV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _BaseApiController : ControllerBase
    {
        protected string GetUserIdFromToken()
        {
            string userId = string.Empty;

            string tryGetUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (tryGetUserId != null) 
            {
                userId = tryGetUserId;
            }

            return userId;
        }
    }
}
