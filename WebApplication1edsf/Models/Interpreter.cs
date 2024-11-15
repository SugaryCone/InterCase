using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1edsf.Models
{
	internal class Interpreter
	{
		//public Interpreter() { globals.define("clock", new Pause()}
		
		static Envire globals = new Envire();

		public Envire environment = globals;
        public Stmt CurrentStmt;

		public TemplateModel Template;

        public SlideView OutputView = new SlideView("Hello", "Time to choose");
        public SlideView? Alert = null;

        public Answer Ans;
        public List<SlideView> History = new List<SlideView>();
        public AutoResetEvent ModelEv = new AutoResetEvent(false);

		public Interpreter(TemplateModel template)
		{
			Template = template;
		}

        public void interpret(Expr expression)
		{
			try
			{
				Object value = expression.Interpret();
				Console.WriteLine(stringify(value));
			}
			catch (RuntimeError error)
			{
				Template.Error.runtimeError(error);
			}
		}
		public void interpret(List<Stmt> statements)
		{
			try
			{
				foreach (Stmt statement in statements)
				{
					//HEAR 
					//Здесь нужно сделать какой-то возврат из interprt для контроля за slids и их выводом
					//
					//

                    CurrentStmt = statement;
					Object value = statement.Interpret();
					//Console.WriteLine(stringify(value));
				}
			}
			catch (RuntimeError error)
			{
                Template.Error.runtimeError(error);
			}
		}
		public String stringify(Object obj)
		{
			if (obj == null) return "nil";

			if (obj.GetType() == typeof(double)) {
				String text = obj.ToString();
				if (text.EndsWith(".0"))
				{
					text = text.Substring(0, text.Length - 2);
				}
				return text;
			}

			return obj.ToString();
		}
	}
}
