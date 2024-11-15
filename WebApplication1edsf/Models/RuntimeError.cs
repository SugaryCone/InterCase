using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1edsf.Models
{
	class RuntimeError : Exception
	{
		public Token token
		{
			get; private set;
		}

		public RuntimeError(Token token, String message): base(message)
		{
			
			this.token = token;
		}
		public RuntimeError(String name, String message) : base(message)
		{

			this.token = new Token(TokenType.IDENTIFIER, name, "", 0);
		}
	}
}