using Escript;

namespace WebApplication1edsf.Models
{
    class ErrorHandler
    {
        public bool hadError { get; set; } = false;
        public bool hadRuntimeError { get; set; } = false;





        public void report(int line, String where,
                            String message)
        {
            Console.WriteLine(
                "[line " + line + "] Error" + where + ": " + message);
            //todo
            hadError = true;
        }


        public void error(Token token, String message)
        {
            if (token.type == TokenType.EOF)
            {
                report(token.line, " at end", message);
            }
            else
            {
                report(token.line, " at '" + token.lexeme + "'", message);
            }
        }

        public void runtimeError(RuntimeError error)
        {
            Console.WriteLine(error.Message +
                "\n[line " + error.token.line + "]");
            hadRuntimeError = true;
        }
        
    }

}
