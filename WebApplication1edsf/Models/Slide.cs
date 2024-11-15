using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;


namespace WebApplication1edsf.Models
{
	internal class Slide
	{
		public SlideView view { get; private set; }
		public string user_answer
		{
			get;
			protected set;
		} = "empty";
		public bool answered
		{
			get;
			protected set;
		} = false;
		public bool correct_answer
		{
			get;
			protected set;
		} = false;
		
		public Slide(string title, string content)
		{
			view = new SlideView(title, content);
			
		}
		public virtual SlideView Show()
		{
			
			return view;
		}
		public virtual void Update(string answer)
		{
			answer = answer.Trim(' ');
			answered = true;
			correct_answer = true;
			user_answer = answer;
		}
        public virtual void Update(Answer answer)
        {
            
            answered = true;
            correct_answer = true;
            user_answer = "answer"; //нужно переделать
        }
        public override string ToString()
		{
			return view.Title;
		}

	}
	internal class DilemmaSlide : Slide
	{
		int correct_answer_number = 0;
		public DilemmaSlide(string title, string content, int correct_answer_number) : base(title, content)
		{
			this.correct_answer_number = correct_answer_number;
		}

		public DilemmaSlide(string title, string content, int correct_answer_number, string[] values) : base(title, content)
		{
			this.correct_answer_number = correct_answer_number;
			this.values = values;
		}

		public string[] values
		{
			get;
			protected set;
		} = { "empty", "empty" };
		public override SlideView Show()
		{
			string form = "<fieldset><legend>Выбирете один ответ</legend>";
			form += "<p><input type = 'radio' name = 'fslide'  value = '1' /><label >Да</label>";
			form += "<input type = 'radio' name = 'fslide' value = '2' /><label>Нет</label></p>";

            form += "<button onclick = 'send_form()'>yes</button></fieldset>";
            view.Forms = form;
            view.Type = "dilemma";
            return base.Show();
        }


		public override void Update(string answer)
		{
			answer = answer.Trim(' ');
			answered = true;
			
			if (correct_answer_number == 0 && answer == "1" || correct_answer_number == 1 && answer == "2") correct_answer = true;
			user_answer = answer;
		}
	}

	internal class SelectSlide : Slide
	{
		public string[] options
		{
			get;
			protected set;
		}
		public double[] correct_choice
		{
			get;

			protected set;
		}
		public double[] answer
		{
			get;
			protected set;
		}
		public string[] values
		{
			get;
			protected set;
		}
		public SelectSlide(string title, string content, string[] options, double[] correct_choice, string[] values) : base(title, content)
		{
			this.options = options;
			this.correct_choice = correct_choice;
			this.values = values;
		}

		public override SlideView Show()
		{

            string form =   "<p>Выбирете ответы</p>";
            

            for (int i = 0; i < options.Length; ++i)
			{
				//res += $"<li>{i+1}." + options[i] + "</li>";
				form += $"<div class=\"form-check\"> <input  name = 'fslide' class=\"form-check-input\" type=\"checkbox\" value=\"{i + 1}\" id=\"flexCheckDefault{i + 1}\"><label class=\"form-check-label\" for=\"flexCheckDefault{i + 1}\">{options[i]}</label></div>";


				
            }
            view.Forms = form;
			view.Type = "select";
            return base.Show();
		}

		public override void Update(string answer)
		{
			answer = answer.Trim(' ');
			answered = true;
			user_answer = answer;
			string[] s_answer = answer.Split(" ");
			int n = options.Length;

			this.answer = new double[n];
			for (int i = 0; i <n; ++i)
				{
					this.answer[i] = 0;
				}
			correct_answer = true;
			for (int i = 0; i < s_answer.Length; ++i)
				{
					this.answer[Convert.ToInt32(s_answer[i]) - 1] = 1;
				}
			for (int i = 0; i < n; ++i)
			{
				//Console.Write(this.answer[i]); Console.WriteLine(correct_choice[i]);
				
				if (this.answer[i] != correct_choice[i])
				{
					
					correct_answer = false;
				}
			}

		}
        public override void Update(Answer answer)
		{
            int n = options.Length;
            this.answer = new double[n];
            for (int i = 0; i < n; ++i)
            {
                this.answer[i] = 0;
            }
            correct_answer = true;
			Console.WriteLine(answer.answer.Length); Console.WriteLine(this.answer.Length); 
            for (int i = 0; i < answer.answer.Length; ++i)
            {
                if(answer.answer[i] != 0)
					this.answer[answer.answer[i] - 1] = 1;
            }

            for (int i = 0; i < answer.answer.Length; ++i)
            {
                //Console.Write(this.answer[i]); Console.WriteLine(correct_choice[i]);

                if (this.answer[i] != correct_choice[i])
                {

                    correct_answer = false;
                }
            }
        }


    }
}

