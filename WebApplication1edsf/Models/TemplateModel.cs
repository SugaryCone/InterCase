using Microsoft.JSInterop.Implementation;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace WebApplication1edsf.Models
{

    partial class  TemplateModel
    {
        

        



        

        public static string name = "";
        public Thread myThread;
        public Interpreter interpreter;

        public ErrorHandler Error = new ErrorHandler();


        public TemplateModel() {
            interpreter = new Interpreter(this);
        
        
        }



        public void Start(string slides, string filename)
        {
            string jtext;
            using (StreamReader reader = new StreamReader(slides, System.Text.Encoding.UTF8))
            {
                jtext = reader.ReadToEnd();
                JsonNode jsob = JsonNode.Parse(jtext);
                foreach (JsonNode jn in jsob.AsArray())
                {
                    Console.WriteLine(jn!["type"]!.ToString());
                    if (jn!["type"]!.ToString() == "slide")
                    {
                        interpreter.environment.define(jn!["var"]!.ToString(), new Slide(jn!["title"]!.ToString(), jn!["content"]!.ToString()));

                    }
                    else if (jn!["type"]!.ToString() == "dislide")
                    {
                        string[] values = new string[jn!["values"]!.AsArray().Count];
                        JsonNode varray = jn!["values"]!;
                        for (int i = 0; i < jn!["values"]!.AsArray().Count; ++i)
                        {
                            values[i] = varray[i]!.ToString();
                        }


                        interpreter.environment.define(jn!["var"]!.ToString(), new DilemmaSlide(jn!["title"]!.ToString(), jn!["content"]!.ToString(), (int)jn!["answer"]!));

                    }
                    else if (jn!["type"]!.ToString() == "selslide")
                    {
                        string[] values = new string[jn!["values"]!.AsArray().Count];
                        JsonNode varray = jn!["values"]!;
                        for (int i = 0; i < jn!["values"]!.AsArray().Count; ++i)
                        {
                            values[i] = varray[i]!.ToString();
                        }
                        string[] options = new string[jn!["options"]!.AsArray().Count];
                        JsonNode oarray = jn!["options"]!;
                        for (int i = 0; i < jn!["options"]!.AsArray().Count; ++i)
                        {
                            options[i] = oarray[i]!.ToString();
                        }
                        double[] correct_choice = new double[jn!["correct_choice"]!.AsArray().Count];
                        JsonNode carray = jn!["correct_choice"]!;
                        for (int i = 0; i < jn!["correct_choice"]!.AsArray().Count; ++i)
                        {
                            correct_choice[i] = ((double)carray[i]!);
                        }
                        interpreter.environment.define(jn!["var"]!.ToString(), new SelectSlide(jn!["title"]!.ToString(), jn!["content"]!.ToString(), options, correct_choice, values));

                    }


                }

                // JsonArray slideArr = jsob as JsonArray;
            }

            myThread = new(() => runFile(filename));
            myThread.Name = $"Slide";
            myThread.Start();
        }

        void runFile(string file)
        {
            using (StreamReader reader = new StreamReader(file, System.Text.Encoding.UTF8))
            {
                string text = reader.ReadToEnd();
                run(text);
            }
            if (Error.hadError) { Environment.Exit(65); }
            if (Error.hadRuntimeError) Environment.Exit(70);
        }

        private void runPrompt()
        {

            for (; ; )
            {
                Console.Write(">_ ");
                var line = Console.ReadLine();
                if (line == null) break;
                run(line);
                Error.hadError = false;
            }
        }

        void run(string text)
        {
            

   
            Scaner scanner = new Scaner(this, text);

            List<Token> tokens = scanner.scanTokens();


            interpreter.environment.define("array", new Array(this));
            interpreter.environment.define("answered", new Answered(this));
            interpreter.environment.define("string_answer", new sAnswer(this));
            interpreter.environment.define("correct_answer", new correctAnswer(this));

            interpreter.environment.define("array_answer", new arrayAnswer(this));
            interpreter.environment.define("values_slide", new valuesSlide(this));
            interpreter.environment.define("show_slide", new showSlide(this));
            interpreter.environment.define("options_count", new optionsCount(this));




            Parser parser = new Parser(this, tokens);

            List<Stmt> statements = parser.parse();


            if (Error.hadError) return;

            interpreter.interpret(statements);

        }
        public void error(int line, String message)
        {
            Error.report(line, "", message);
        }



    

}
}
