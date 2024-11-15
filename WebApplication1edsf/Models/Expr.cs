using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebApplication1edsf.Models
{


	abstract class Expr
	{

		protected TemplateModel Template;
		public Expr(TemplateModel template)
		{
			Template = template;
		}

		public abstract void Visit();
		public abstract Object Interpret();



		public override string ToString()
		{
			return base.ToString();
		}
		protected bool isTruthy(Object obj)
		{
			if (obj == null) return false;
			if (obj.GetType() == typeof(bool)) return (bool)obj;
			return true;
		}
	}


	class Assign : Expr
	{
		Token name;
		Expr value;
		public Expr? index
		{
			get; private set;
		} = null;
		public Assign(TemplateModel template, Token name, Expr value):base(template)
		{
			this.name = name;
			this.value = value;

		}
		public Assign(TemplateModel template, Token name, Expr value, Expr? index) : base(template)
        {
			this.name = name;
			this.value = value;
			this.index = index;
		}
		public override void Visit()
		{

			Console.Write(")");

		}

		public override Object Interpret()
		{
			Object value_object = value.Interpret();

			//double[] res = (System.Double[])TemplateModel.interpreter.environment.get(name);
			if (index != null)
			{
				double[] res = (System.Double[])Template.interpreter.environment.get(name);
				int i = (int)(Double)index.Interpret();
				res[i] = (Double)value_object;
				Template.interpreter.environment.assign(name, res);
				return res[i];
			}
			else
			{
				Template.interpreter.environment.assign(name, value_object);
				return value_object;
			}

		}
	}
	class Binary : Expr
	{
		Expr left;
		Token Operator;
		Expr right;
		public Binary(TemplateModel template, Expr left, Token Operator, Expr right) : base(template)
        {
			this.left = left;
			this.Operator = Operator;
			this.right = right;
		}

		public override void Visit()
		{
			left.Visit(); Console.Write(Operator.lexeme); right.Visit();
		}
		public override Object Interpret()
		{
			Object left_object = left.Interpret();
			Object right_object = right.Interpret();

			switch (Operator.type) {

				case TokenType.MINUS:
					checkNumberOperands(Operator, left_object, right_object);
					return (double)left_object - (double)right_object;
				case TokenType.SLASH:
					checkNumberOperands(Operator, left_object, right_object);
					return (double)left_object / (double)right_object;
				case TokenType.PROC:
					checkNumberOperands(Operator, left_object, right_object);
					return (double)left_object % (double)right_object;
				case TokenType.DSLASH:
					checkNumberOperands(Operator, left_object, right_object);
					double a = (int)(double)left_object / (int)(double)right_object;
					return a;
				case TokenType.PLUS:
					if (left_object.GetType() == typeof(double) && right_object.GetType() == typeof(double)) {
						return (double)left_object + (double)right_object;
					}

					if (left_object.GetType() == typeof(string) && right_object.GetType() == typeof(string)) {
						return (String)left_object + (String)right_object;
					}

					break;
				case TokenType.STAR:
					return (double)left_object * (double)right_object;

				case TokenType.GREATER:
					checkNumberOperands(Operator, left_object, right_object);
					return (double)left_object > (double)right_object;
				case TokenType.GREATER_EQUAL:
					checkNumberOperands(Operator, left_object, right_object);
					return (double)left_object >= (double)right_object;
				case TokenType.LESS:
					checkNumberOperands(Operator, left_object, right_object);
					return (double)left_object < (double)right_object;
				case TokenType.LESS_EQUAL:
					checkNumberOperands(Operator, left_object, right_object);
					return (double)left_object <= (double)right_object;
				case TokenType.BANG_EQUAL: return !isEqual(left_object, right_object);
				case TokenType.EQUAL_EQUAL: return isEqual(left_object, right_object);
			}

			// Unreachable.
			return null;
		}

		private void checkNumberOperand(Token Operator, Object operand)
		{
			if (operand.GetType() == typeof(double)) return;
			throw new RuntimeError(Operator, $"{operand.GetType()} Operand must be a number.");
		}

		private void checkNumberOperands(Token Operator,
								 Object left, Object right)
		{
			if (left.GetType() == typeof(double) && right.GetType() == typeof(double)) return;

			throw new RuntimeError(Operator, "Operands must be numbers.");
		}

		private bool isEqual(Object a, Object b)
		{
			if (a == null && b == null) return true;
			if (a == null) return false;

			return Object.Equals(a, b);

		}


		public override string ToString()
		{
			return $"{left} {Operator} {right}";
		}
	}
	class Call : Expr
	{
		Expr callee;
		Token paren;
		List<Expr> arguments;

		public Call(TemplateModel template, Expr callee, Token paren, List<Expr> arguments) : base(template)
        {
			this.callee = callee;
			this.paren = paren;
			this.arguments = arguments;
		}

		public override void Visit()
		{

			Console.Write("CALL");
		}

		public override Object Interpret()
		{
			
			Object callee_object = callee.Interpret(); 
			
			List<Object> arguments_obj = new List<Object>();
			foreach (Expr argument in arguments)
			{
				
				
				arguments_obj.Add(argument.Interpret());
			}
			Callable s = (Callable)callee_object;
			if (arguments_obj.Count != s.arity() && s.arity() != 0)
			{
				throw new RuntimeError(paren, "Expected " +
					s.arity() + " arguments but got " +
					arguments_obj.Count() + ".");
			}
				

			return s.call(arguments_obj);

		}

	}

	abstract class Callable
	{
		protected TemplateModel Template;
		public Callable(TemplateModel template)
		{
			Template = template;
		}
		public abstract int arity();
		public abstract Object call(List<Object> arguments);

        //ЭТО КАКАЯ_ТО ХЕРЬ НАДО ПЕРЕДЕЛАТЬ
        protected void ControllerSend(SlideView view)
        {

            Template.interpreter.OutputView = view;
            if (view.Type == "slide")
            {
                Console.WriteLine("-----------------------------------------------------");
                Template.interpreter.History.Add(view);
            }
        }
        protected Answer ControllerGet()
        {


            Template.interpreter.ModelEv.WaitOne();
            Answer res = Template.interpreter.Ans;
            //TemplateModel.Ans = "";
            return res;
        }
    }

	class Answered : Callable
	{

		public Answered(TemplateModel template) : base(template) { }

        public override int arity()
		{
			return 1;
		}


		public override Object call(List<Object> arguments)
		{
			Object res = false;
			for (int i = 0; i < arguments.Count; ++i)
			{
				Slide s  = (Slide)arguments[i];
				res = s.answered;
			}
			return res;
		}


	}
	class sAnswer : Callable
	{
        public sAnswer(TemplateModel template) : base(template) { }
        public override int arity()
		{
			return 1;
		}


		public override Object call(List<Object> arguments)
		{
			Object res = " ";
			for (int i = 0; i < arguments.Count; ++i)
			{
				Slide s = (Slide)arguments[i];
				res = s.user_answer;
			}

			return res;
		}
	}

	class correctAnswer : Callable
	{
        public correctAnswer(TemplateModel template) : base(template) { }
        public override int arity()
		{
			return 1;
		}
		public override object call(List<object> arguments)
		{
			Object res = false;
			if (arguments[0].GetType() == typeof(SelectSlide))
			{
				Slide s = (Slide)arguments[0];
				res = s.correct_answer;
			}
			else if (arguments[0].GetType() == typeof(String))
			{
				object arg = Template.interpreter.environment.get((string)arguments[0]);

				if (arg.GetType() == typeof(SelectSlide))
				{
					Slide s = (Slide)arg;
					res = s.correct_answer;
				}
			}
			

			return res;
		}
	}
	class arrayAnswer : Callable
	{
        public arrayAnswer(TemplateModel template) : base(template) { }
        public override int arity()
		{
			return 1;
		}
		public override object call(List<object> arguments)
		{
			Object res = " ";

			if (arguments[0].GetType() == typeof(SelectSlide))
			{
			
					SelectSlide s = (SelectSlide)arguments[0];
					res = s.answer;
				
			}
			else if (arguments[0].GetType() == typeof(String))
			{
				object arg = Template.interpreter.environment.get((string)arguments[0]);

				if (arg.GetType() == typeof(SelectSlide))
				{
					SelectSlide s = (SelectSlide)arg;
					res = s.answer;
				}
			}



			return res;
		}
	}

	class showSlide: Callable
	{
        public showSlide(TemplateModel template) : base(template) { }
        public override int arity()
		{
			return 1;
		}
		public override object call(List<object> arguments)
		{
			Object res = " ";
			string name = (string)arguments[0];
			if(name != "empty")
			{
				Slide slide = (Slide)Template.interpreter.environment.get(name);
				ControllerSend(slide.Show());

                Answer answer = ControllerGet();

				slide.Update(answer);
			}

			return res;
		}
	}

	class optionsCount : Callable
	{
        public optionsCount(TemplateModel template) : base(template) { }
        public override int arity()
		{
			return 1;
		}
		public override object call(List<object> arguments)
		{
			

			if(arguments[0].GetType() == typeof(String))

			{
				object arg = Template.interpreter.environment.get((string)arguments[0]);

				if (arg.GetType() == typeof(SelectSlide))
				{

					SelectSlide slide = (SelectSlide)arg;

					return (Double)(slide.options.Length);
				}
			}
			if (arguments[0].GetType() == typeof(SelectSlide))
			{

				SelectSlide slide = (SelectSlide)arguments[0];

				return (Double)(slide.options.Length);
			}
			return 0.0;
		}
	}
	class valuesSlide: Callable
	{
        public valuesSlide(TemplateModel template) : base(template) { }
        public override int arity()
		{
			return 2;
		}
		public override object call(List<object> arguments)
		{
			Object res = " ";
			if (arguments[0].GetType() == typeof(SelectSlide))
			{
				SelectSlide s = (SelectSlide)arguments[0];
				int index = (int)(double)arguments[1];
				res = s.values[index];
			}
			else if (arguments[0].GetType() == typeof(String))
			{
				object arg = Template.interpreter.environment.get((string)arguments[0]);

				if (arg.GetType() == typeof(SelectSlide))
				{
					SelectSlide s = (SelectSlide)arg;
					int index = (int)(double)arguments[1];
					res = s.values[index];
				}
			}
			return res;
		}
	}

	class Array:Callable
	{

        public Array(TemplateModel template) : base(template) { }
        public override int arity()
		{
			return 0;
		}


		public override Object call(List<Object> arguments)
		{
			System.Double[] res = new Double[arguments.Count];
			for(int i = 0; i < arguments.Count; ++i)
			{
				res[i] = (double)arguments[i];
			}
			return res;
		}

		
	}
	class Grouping : Expr
	{
		Expr expression;
		public Grouping(TemplateModel template, Expr expression) : base(template)
        {
			this.expression = expression;
		}

		public override void Visit()
		{

			Console.Write("(");

			Console.Write(" ");
			expression.Visit();

			Console.Write(")");

		}

		public override Object Interpret()
		{
			return expression.Interpret();
		}

	}

	class Logical : Expr
	{
		Expr left;
		Token Operator;
		Expr right;
		public Logical(TemplateModel template, Expr left, Token Operator, Expr right) : base(template)
        {
			this.left = left;
			this.Operator = Operator;
			this.right = right;
		}

		public override void Visit()
		{
			left.Visit(); Console.Write(Operator.lexeme); right.Visit();
		}
		public override Object Interpret()
		{
			Object left_object = left.Interpret();

			if (Operator.type == TokenType.OR) {
				if (isTruthy(left_object)) return left_object;
			} else
			{
				if (!isTruthy(left_object)) return left_object;
			}

			return right.Interpret();
        }
	}


	class Literal : Expr
	{
		Object value;
		public Literal(TemplateModel template, Object value) : base(template)
        {
			this.value = value;
		}
		public override void Visit()
		{
			Console.Write(value); Console.Write("Literal");
		}
		public override Object Interpret()
		{
			return value;
		}
		public override string ToString()
		{
			return $"{value}";
		}

		public override bool Equals(object? obj)
		{
			return obj.Equals(value);
		}
	}

	class Unary : Expr
	{
		Token Operator;
		Expr right;

		public Unary(TemplateModel template, Token Operator, Expr right) : base(template)
        {
			this.Operator = Operator;
			this.right = right;
		}

		public override void Visit()
		{
			Console.Write(Operator.lexeme);
		}

		public override Object Interpret()
		{
			Object right_object = right.Interpret();

			switch (Operator.type) {
				case TokenType.BANG:
					return !isTruthy(right_object);
				case TokenType.MINUS:
					return -(double)right_object;
			}

			// Unreachable.
			return null;
		}

		private bool isTruthy(Object obj)
		{
			if (obj == null) return false;
			if (obj.GetType() == typeof(bool)) return (bool)obj;
			return true;
		}

		public override string ToString()
		{
			return $"{Operator} {right}";
		}
	}

	class Variable : Expr
	{
		public Token name
		{
			get; private set;
		}
		public Expr? index
		{
			get; private set;
		} = null;
		public Variable(TemplateModel template, Token name) : base(template)
        {
			this.name = name;
		}
		public Variable(TemplateModel template, Token name, Expr index) : base(template)
        {
			this.name = name;
			this.index = index;
		}
		public override void Visit()
		{
			Console.Write(name.lexeme);
		}


		public override Object Interpret()
		{
			if (index != null)
			{
				double[] res = (System.Double[])Template.interpreter.environment.get(name);
				int i = (int)(Double)index.Interpret();
				//Console.WriteLine(i);
				return res[i];
			}
			return Template.interpreter.environment.get(name);
		}

		private bool isTruthy(Object obj)
		{
			if (obj == null) return false;
			if (obj.GetType() == typeof(bool)) return (bool)obj;
			return true;
		}

		public override string ToString()
		{
			return $"{name}";
		}
	}




}
