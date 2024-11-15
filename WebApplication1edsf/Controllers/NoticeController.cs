using Escript;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1edsf.Models;

namespace WebApplication1edsf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        [HttpGet]
        public SlideView Get()
        {

            SlideView  res = Program.A.interpreter.Alert;
            Program.A.interpreter.Alert = null;
            return res;
        }
    }
}
