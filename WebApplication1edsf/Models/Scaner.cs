using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApplication1edsf.Models
{








	internal class Scaner
	{
		private String source;
		private List<Token> tokens = new List<Token>();
		private int start = 0;
		private int current = 0;
		private int line = 1;



		private static Dictionary<String, TokenType> keywords = new Dictionary<String, TokenType>
		{
			{ "and", TokenType.AND},
			{ "class",  TokenType.CLASS },//
			{ "else",   TokenType.ELSE},
			{ "false",  TokenType.FALSE },
			{ "for",    TokenType.FOR},
			{ "fun",    TokenType.FUN },//
			{ "if",     TokenType.IF},
			{ "nil",    TokenType.NIL },
			{ "or",     TokenType.OR},
			{ "print",  TokenType.PRINT },
			{ "return", TokenType.RETURN},
			{ "super",  TokenType.SUPER },//
			{ "this",   TokenType.THIS},//
			{ "true",   TokenType.TRUE },
			{ "var",    TokenType.VAR},
			{ "while",  TokenType.WHILE},
			{ "show",  TokenType.SHOW},
			{ "clear",  TokenType.CLEAR},
			{ "pause",  TokenType.PAUSE},




		};


		TemplateModel Template;

		public Scaner(TemplateModel template, String source)
		{
			this.source = source;
			Template = template;
		}

		public List<Token> scanTokens()
		{
			while (!isAtEnd())
			{
				// We are at the beginning of the next lexeme.
				start = current;
				scanToken();
			}
			tokens.Add(new Token(TokenType.EOF, "", ' ', line));
			
			return tokens;
		}

		bool isAtEnd()
		{
			return current >= source.Length;
		}

		private void scanToken()
		{
			char c = advance();
			switch (c)
			{
				case '(': addToken(TokenType.LEFT_PAREN); break;
				case ')': addToken(TokenType.RIGHT_PAREN); break;
				case '[': get_num_array(); break;
				case '{': addToken(TokenType.LEFT_BRACE); break;
				case '}': addToken(TokenType.RIGHT_BRACE); break;
				case ',': addToken(TokenType.COMMA); break;
				case '.': addToken(TokenType.DOT); break;
				case '-': addToken(TokenType.MINUS); break;
				case '+': addToken(TokenType.PLUS); break;
				case ';': addToken(TokenType.SEMICOLON); break;
				case '*': addToken(TokenType.STAR); break;
				case '%': addToken(TokenType.PROC); break;
				case '!':
					addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
					break;
				case '=':
					addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
					break;
				case '<':
					addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
					break;
				case '>':
					addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
					break;
				case '/':
					addToken(match('/') ? TokenType.DSLASH : TokenType.SLASH);
					break;
				/*				case '/':
									if (match('/'))
									{
										addToken(TokenType.DSLASH);
										// A comment goes until the end of the line.
										//while (peek() != '\n' && !isAtEnd()) advance();
									}
									else
									{
										addToken(TokenType.SLASH);
									}
									break;*/
				case ' ':
				case '\r':
				case '\t':
					// Ignore whitespace.
					break;

				case '\n':
					line++;
					break;
				case '"': get_string(); break;

				default:
					if (isDigit(c)) number();
					else if (isAlpha(c)) identifier();
					//else TemplateModel.error(line, "Unexpected character.");

					break;
			}
		}

		private char advance()
		{
			return source[current++];
		}
		private bool match(char expected)
		{
			if (isAtEnd()) return false;
			if (source[current] != expected) return false;

			current++;
			return true;
		}
		private char peek()
		{
			if (isAtEnd()) return '\0';
			return source[current];
		}

		private bool isDigit(char c)
		{
			return c >= '0' && c <= '9';
		}
		private void number()
		{
			while (isDigit(peek())) advance();

			// Look for a fractional part.
			if (peek() == '.' && isDigit(peekNext()))
			{
				// Consume the "."
				advance();

				while (isDigit(peek())) advance();
			}
			string a = source.Substring(start, current - start);
			
			addToken(TokenType.NUMBER,
				double.Parse(a, CultureInfo.CreateSpecificCulture("en-US")));
		}

		private char peekNext()
		{
			if (current + 1 >= source.Length) return '\0';
			return source[current + 1];
		}

		private void get_num_array()
		{
			while (peek() != ']' && !isAtEnd())
			{
				if (peek() == '\n') line++;
				advance();
			}

			if (isAtEnd())
			{
                Template.error(line, "Unterminated string.");
				return;
			}
			// The closing ".
			advance();

			// Trim the surrounding quotes.
			String string_value_ = source.Substring(start + 1, current - start - 2);
			
			string string_value = "";

			foreach(char s in string_value_)
			{
				if (s != ' ') string_value += s;
			}
			//Console.WriteLine(string_value);
			Regex regex = new Regex(@"\d+\.*\d*");

			MatchCollection matches = regex.Matches(string_value);

			if (matches.Count > 0)
			{
				int i = 0;
				double[] value = new double[matches.Count];
				foreach (Match match in matches)
				{
					//Console.WriteLine(match);
					value[i] = double.Parse(match.Value, CultureInfo.CreateSpecificCulture("en-US"));
					i++;
				}

				addToken(TokenType.ARRAY, value);
			}
			else addToken(TokenType.ARRAY, null);
		}

		private void get_string () {
			while (peek() != '"' && !isAtEnd()) {
			  if (peek() == '\n') line++;
			  advance();
			}

			if (isAtEnd()) {
              Template.error(line, "Unterminated string.");
			  return;
			}
			// The closing ".
			advance();

			// Trim the surrounding quotes.
			String value = source.Substring(start + 1, current - start-2);
			addToken(TokenType.STRING, value);
		}
		private void identifier()
		{
			while (isAlphaNumeric(peek())) advance();

			String text = source.Substring(start, current - start);

			if (peek() == '[' && tokens.Last().type != TokenType.DOT)
			{
				addToken(TokenType.IDENTIFIER);
				advance();
				//Console.WriteLine("yse");


				//String num = source.Substring(start + 2, current - start -2);
				//Console.WriteLine(num);



				addToken(TokenType.LEFT_ARRAY_INDEX, "[", " ");
				while (source[current] != ']')
				{
					start = current;
					scanToken();
					//Console.WriteLine(peek());
					//Console.WriteLine(current);
				}

				addToken(TokenType.RIGHT_ARRAY_INDEX, "]", " ");
			}
			else
			{
				TokenType type;
				if (!keywords.ContainsKey(text)) { addToken(TokenType.IDENTIFIER); }
/*				else if (keywords.ContainsKey(text) && keywords[text] == TokenType.ANSWER)
				{
					
					addToken(TokenType.ANSWER);
					if (peek() == '[')
					{
						advance();
						//Console.WriteLine("yse");


						//String num = source.Substring(start + 2, current - start -2);
						//Console.WriteLine(num);



						addToken(TokenType.LEFT_ARRAY_INDEX, "[", " ");
						while (source[current] != ']')
						{
							start = current;
							scanToken();

						}

						addToken(TokenType.RIGHT_ARRAY_INDEX, "]", " ");
					}
				}*/
				else {
					type = keywords[text];
					addToken(type);
				}
				
			}

		}
		private bool isAlpha(char c)
		{
			return (c >= 'a' && c <= 'z') ||
				   (c >= 'A' && c <= 'Z') ||
					c == '_';
		}

		private bool isAlphaNumeric(char c)
		{
			return isAlpha(c) || isDigit(c);
		}
		private void addToken(TokenType type)
		{
			addToken(type, " ");
		}

		private void addToken(TokenType type, Object literal)
		{
			
			String text = source.Substring(start, current - start);
			//Console.WriteLine(text);
			tokens.Add(new Token(type, text, literal, line));
		}
		private void addToken(TokenType type, string lex, Object literal)
		{
			tokens.Add(new Token(type, lex, literal, line));
		}
	}




}
