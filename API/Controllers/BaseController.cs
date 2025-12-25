using Microsoft.AspNetCore.Mvc;
using Application.Result;

namespace API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult ToActionResult<T>(Result<T> result)
        {
            return StatusCode(result.StatusCode, result);
        }
    }
}
