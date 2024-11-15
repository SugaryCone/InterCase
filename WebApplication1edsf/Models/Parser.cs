using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;



namespace WebApplication1edsf.Models
{
	internal class Parser
	{
		private List<Token> tokens;
		private int current = 0;
		private TemplateModel Template;
		private class ParseError: Exception { }
		public Parser(TemplateModel template, List<Token> tokens)
		{
			this.tokens = tokens;
			Template = template;
		}

		public List<Stmt> parse()
		{
			List<Stmt> statements = new List<Stmt>();
			while (!isAtEnd())
			{
				statements.Add(declaration());
			}

			return statements;
		}

		private Stmt declaration()
		{
			try
			{
				if (match(TokenType.VAR)) return varDeclaration();
//				if (match(TokenType.LEFT_SBRACE)) return arrayDeclaration();
				return statement();
			}
			catch (ParseError error)
			{
				synchronize();
				return null;
			}
		}

/*		private Stmt arrayDeclaration()
		{
			Token name = consume(TokenType.IDENTIFIER, "Expect variable name.");

			Expr initializer = null;
			if (match(TokenType.EQUAL))
			{
				initializer = expression();
			}

			consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
			return new Var(name, initializer);
		}*/

		private Stmt varDeclaration()
		{
			Token name = consume(TokenType.IDENTIFIER, "Expect variable name.");

			Expr initializer = null;
			if (match(TokenType.EQUAL))
			{
				initializer = expression();
			}

			consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
			return new Var(Template, name, initializer);
		}

		private Stmt statement()
		{
			if (match(TokenType.PRINT)) return printStatement();
			if (match(TokenType.SHOW)) return showStatement();
			if (match(TokenType.CLEAR)) return clearStatement();
			if (match(TokenType.PAUSE)) return pauseStatement();
			if (match(TokenType.WHILE)) return whileStatement();
			if (match(TokenType.LEFT_BRACE)) return new Block(Template, block());

			if (match(TokenType.IF)) return ifStatement();
			return expressionStatement();
		}
		private Stmt whileStatement()
		{
			consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
			Expr condition = expression();
			consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
			Stmt body = statement();

			return new While(Template, condition, body);
		}
		private Stmt ifStatement()
		{
			consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
			Expr condition = expression();
			consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

			Stmt thenBranch = statement();
			Stmt elseBranch = null;
			if (match(TokenType.ELSE))
			{
				elseBranch = statement();
			}

			return new If(Template, condition, thenBranch, elseBranch);
		}
		private List<Stmt> block()
		{
			List<Stmt> statements = new List<Stmt>();

			while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
			{
				statements.Add(declaration());
			}

			consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
			return statements;
		}


		private Stmt printStatement()
		{
			Expr value = expression();
			consume(TokenType.SEMICOLON, "Expect ';' after value.");
			return new Print(Template, value);
		}

		private Stmt showStatement()
		{
			Expr value = expression();
			consume(TokenType.SEMICOLON, "Expect ';' after value.");
			return new Show(Template, value);
		}

		private Stmt expressionStatement()
		{
			Expr expr = expression();
			consume(TokenType.SEMICOLON, "Expect ';' after expression.");
			return new Expression(Template, expr);
		}

		private Stmt clearStatement()
		{
			consume(TokenType.SEMICOLON, "Expect ';' after expression.");
			return new Clear(Template);
		}
		private Stmt pauseStatement()
		{
			consume(TokenType.SEMICOLON, "Expect ';' after expression.");
			return new Pause(Template);
		}

		private Expr expression()
		{
			return  assignment(); 
		}

		private Expr assignment()
		{
			Expr expr = or();

			if (match(TokenType.EQUAL))
			{

				Token equals = previous();
				Expr value = assignment();
				
				if (expr.GetType() == typeof(Variable)) {
					Variable varb = (Variable)expr;
					Token name = varb.name;				
					return new Assign(Template, name, value, varb.index);
				}

				error(equals, "Invalid assignment target.");
			}

			return expr;
		}
		private Expr or()
		{
			Expr expr = and();

			while (match(TokenType.OR))
			{
				Token Operator = previous();
				Expr right = and();
				expr = new Logical(Template, expr, Operator, right);
			}

			return expr;
		}

		private Expr and()
		{
			Expr expr = equality();

			while (match(TokenType.AND))
			{
				Token Operator = previous();
				Expr right = equality();
				expr = new Logical(Template, expr, Operator, right);
			}

			return expr;
		}
		private Expr equality()
		{
			Expr expr = comparison();

			while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
			{
				Token Operator = previous();
				Expr right = comparison();
				expr = new Binary(Template, expr, Operator, right);
			}

			return expr;
		}

		private bool match(params TokenType[] types)
		{
			foreach (TokenType type in types)
			{
				if (check(type))
				{
					advance();
					return true;
				}
			}

			return false;
		}




		private bool check(TokenType type)
		{
			if (isAtEnd()) return false;
			return peek().type == type;
		}

		private Token advance()
		{
			if (!isAtEnd()) current++;
			return previous();
		}

		private bool isAtEnd()
		{
			return peek().type == TokenType.EOF;
		}

		private Token peek()
		{
			return tokens[current];
		}

		private Token previous()
		{
			return tokens[current - 1];
		}


		private Expr comparison()
		{
			Expr expr = term();

			while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
			{
				Token Operator = previous();
				Expr right = term();
				expr = new Binary(Template, expr, Operator, right);
			}

			return expr;
		}

		private Expr term()
		{
			Expr expr = factor();

			while (match(TokenType.MINUS, TokenType.PLUS))
			{
				Token Operator = previous();
				Expr right = factor();
				expr = new Binary(Template, expr, Operator, right);
			}

			return expr;
		}


		private Expr factor()
		{
			Expr expr = unary();

			while (match(TokenType.SLASH, TokenType.STAR, TokenType.PROC, TokenType.DSLASH))
			{
				Token Operator = previous();
				Expr right = unary();
				expr = new Binary(Template, expr, Operator, right);
			}

			return expr;
		}


		private Expr unary()
		{
			if (match(TokenType.BANG, TokenType.MINUS))
			{
				Token Operator = previous();
				Expr right = unary();
				return new Unary(Template, Operator, right);
			}

			return call(); 
		}

		private Expr call()
		{
			Expr expr = primary();

			while (true)
			{
				if (match(TokenType.LEFT_PAREN))
				{
					expr = finishCall(expr);
				}
				else
				{
					break;
				}
			}

			return expr;
		}

		private Expr finishCall(Expr callee)
		{
			List<Expr> arguments = new List<Expr>();
		
			if (!check(TokenType.RIGHT_PAREN))
			{
				do
				{
					arguments.Add(expression());
				} while (match(TokenType.COMMA));
			}

			Token paren = consume(TokenType.RIGHT_PAREN,
								  "Expect ')' after arguments.");

			return new Call(Template, callee, paren, arguments);
		}
		private Expr primary()
		{
			
			if (match(TokenType.FALSE)) return new Literal(Template, false);
			if (match(TokenType.TRUE)) return new Literal(Template, true);
			if (match(TokenType.NIL)) return new Literal(Template, null);

			if (match(TokenType.NUMBER, TokenType.STRING, TokenType.ARRAY))
			{
				return new Literal(Template, previous().literal);
			}

			if (match(TokenType.LEFT_PAREN))
			{
				Expr expr = expression();
				consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
				return new Grouping(Template, expr);
			}
			if (match(TokenType.IDENTIFIER))
			{

				Token id = previous();

				if (match(TokenType.LEFT_ARRAY_INDEX))
				{

					Expr expr = expression();

					consume(TokenType.RIGHT_ARRAY_INDEX, "Expect ']' after expression.");

					return new Variable(Template, id, expr);
				}
				return new Variable(Template, previous());
			}

			throw error(peek(), "Expect expression.");
		}


		private Token consume(TokenType type, String message)
		{
			if (check(type)) return advance();

			throw error(peek(), message);
		}

		private ParseError error(Token token, String message)
		{

            Template.Error.error(token, message);
			return new ParseError();
		}
		private void synchronize()
		{
			advance();

			while (!isAtEnd())
			{
				if (previous().type == TokenType.SEMICOLON) return;

				switch (peek().type)
				{
					case TokenType.CLASS:
					case TokenType.FUN:
					case TokenType.VAR:
					case TokenType.FOR:
					case TokenType.IF:
					case TokenType.WHILE:
					case TokenType.PRINT:
					case TokenType.RETURN:
						return;
				}

				advance();
			}
		}

	}
}
