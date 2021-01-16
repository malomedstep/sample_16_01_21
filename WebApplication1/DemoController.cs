using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1 {
    [Route("[controller]")]
    [ApiController]
    public class DemoController : ControllerBase {
        [HttpPost]
        public async Task<ActionResult> Read() {
            return Ok(HttpContext.Items["data"]);
        }
    }
}