using Escript;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1edsf.Models;

namespace WebApplication1edsf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        [HttpGet]
        public List<SlideView> Get()
        {


            Console.WriteLine(Program.A.interpreter.History.Count);






            return Program.A.interpreter.History;
        }
    }
}
