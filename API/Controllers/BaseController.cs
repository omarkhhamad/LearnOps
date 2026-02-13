using Microsoft.AspNetCore.Mvc;
using Application.Bases;

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

        protected IActionResult ToActionResult(Application.Bases.Result result)
        {
            return StatusCode(result.StatusCode ?? 200, result);
        }
    }
}
