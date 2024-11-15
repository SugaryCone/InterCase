using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1edsf.Models
{

	abstract class Stmt
	{
		protected TemplateModel Template; 

		public Stmt(TemplateModel template) { Template = template; }

		public abstract Object Interpret();
		public Object evaluate(Expr expr)
		{
			return expr.Interpret();
		}
		public String stringify(Object obj)
		{
			if (obj == null) return "nil";

			if (obj.GetType() == typeof(double))
			{
				String text = obj.ToString();
				if (text.EndsWith(".0"))
				{
					text = text.Substring(0, text.Length - 2);
				}
				return text;
			}
			if (obj.GetType() == typeof(System.Double[]))
			{
				String text = "[";
				double[] value = (System.Double[])obj;
				foreach(double d in value)
				{
					text += stringify(d) + " ";
				}
				text += "]";
				return text;
			}
			if (obj.GetType() == typeof(System.Int32[]))
			{
				String text = "[";
				System.Int32[] value = (System.Int32[])obj;
				foreach (double d in value)
				{
					text += stringify(d) + " ";
				}
				text += "]";
				return text;
			}
			return obj.ToString();
		}

		protected bool isTruthy(Object obj)
		{
			if (obj == null) return false;
			if (obj.GetType() == typeof(bool)) return (bool)obj;
			return true;
		}
		protected void ControllerSend(SlideView view)
		{
			Console.WriteLine(view.Type);
            Template.interpreter.OutputView = view;
			if(view.Type == "slide")
			{
				Console.WriteLine("-----------------------------------------------------");
				Template.interpreter.History.Add(view);
			}
		}
		protected Answer ControllerGet()
		{


            Template.interpreter.ModelEv.WaitOne();
            Answer res = Template.interpreter.Ans;
			return res;
		}
	}

	class Block : Stmt
	{
		List<Stmt> statements;

		public Block(TemplateModel template, List<Stmt> statements): base(template)
		{
			this.statements = statements;
		}

		public override Object Interpret()
		{
			executeBlock(statements, new Envire(Template.interpreter.environment));
			return null;
		}

		void executeBlock(List<Stmt> statements,
				  Envire environment)
		{
			Envire previous = Template.interpreter.environment;
			try
			{
				Template.interpreter.environment = environment;

				foreach (Stmt statement in statements)
				{
					statement.Interpret();
				}
			}
			finally
			{
				Template.interpreter.environment = previous;
			}
		}
	}
	
	class Expression:Stmt
	{
		public Expression(TemplateModel template, Expr expression) : base(template)
        {
			this.expression = expression;
		}


		public override Object Interpret()
		{
			return expression.Interpret();
		}

		Expr expression;
	}


	class If : Stmt
	{
		Expr condition;
		Stmt thenBranch;
		Stmt elseBranch;

		public If(TemplateModel template, Expr condition, Stmt thenBranch, Stmt elseBranch) : base(template)
        {
			this.condition = condition;
			this.thenBranch = thenBranch;
			this.elseBranch = elseBranch;
		}

		public override Object Interpret()
		{


			if (isTruthy(evaluate(condition)))
			{
				thenBranch.Interpret();
			}
			else if (elseBranch != null)
			{
				elseBranch.Interpret();
			}
			
			return null;
		}
	}

	class Print:Stmt
	{
		public Print(TemplateModel template, Expr expression) : base(template)
        {
			this.expression = expression;
		}

		public override Object Interpret()
		{
			 
			Object value = expression.Interpret();
			//Нодо прописать пул уведомлений
			//ControllerSend(new SlideView("alert","Вывод", stringify(value)));
			Template.interpreter.Alert = new SlideView("alert", "Вывод", stringify(value));

            return null;
		}

		Expr expression;
	}

	class Show : Stmt
	{
		public Show(TemplateModel template, Expr expression) : base(template)
        {
			this.expression = expression;
		}

		public override Object Interpret()
		{

           

			//TemplateModel.ModelEv.WaitOne();
			//Console.WriteLine("M1");
			Object value = expression.Interpret();
							Slide S = (Slide)value;
			ControllerSend(S.Show());

            //TemplateModel.ViewEv.Set();

            //TemplateModel.ModelEv.WaitOne();
            //Console.WriteLine("M2");
            Answer answer = ControllerGet();

			S.Update(answer);
			//Console.WriteLine(S.title);
			//Console.WriteLine(S.user_answer);
			//TemplateModel.ViewEv.Set();


			return answer;
		}

		Expr expression;
	}
	class Var : Stmt
	{
		public Var(TemplateModel template, Token name, Expr initializer) : base(template)
        {
			this.initializer = initializer;
			this.name = name;
		}

		public override Object Interpret()
		{
			Object value = null;
			if (initializer != null)
			{
				value = evaluate(initializer);
			}

			Template.interpreter.environment.define(name.lexeme, value);
			return null;
		}


		Token name;
		Expr initializer;
	}
	class Clear : Stmt
	{
		public Clear(TemplateModel template) : base(template) { }


        public override Object Interpret()
		{
			Console.Clear();
			return null;
		}
	}
	class Pause : Stmt
	{
		public Pause(TemplateModel template) : base(template) { }


        public override Object Interpret()
		{
			
			//ControllerGet();
			return null;
		}
	}
	class While : Stmt
	{
		Expr condition;
		Stmt body;

		public While(TemplateModel template, Expr condition, Stmt body) : base(template)
        {
			this.condition = condition;
			this.body = body;
		}

		public override Object Interpret()
		{
			while (isTruthy(evaluate(condition)))
			{
				body.Interpret();
			}
			return null;
		}
	}






}
