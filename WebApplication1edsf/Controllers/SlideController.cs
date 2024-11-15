using Escript;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1edsf.Models;



namespace WebApplication1edsf.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SlideController : ControllerBase
	{
		[HttpGet]
		public SlideView Get()
		{
			

            Console.WriteLine(Program.A.interpreter.CurrentStmt.GetType());
           
			Console.WriteLine(Program.A.myThread.ThreadState);
			Console.Write("GET");  Console.WriteLine(Program.A.myThread.ThreadState);
            Console.WriteLine(Program.A.interpreter.OutputView.Title);
            
				

			
			return Program.A.interpreter.OutputView;
		}
		[HttpPost]
		public string Post(Answer answer)
		{
			string res = "";
			Console.Write("POST"); Console.WriteLine(Program.A.myThread.ThreadState);
            //Переписать все методы update на Answer объект и охватом всего контента - string, int bool
            Program.A.interpreter.Ans = answer;
            Program.A.interpreter.ModelEv.Set();
			
			return res;
		}
        public void Options()
		{ }
        }
}
