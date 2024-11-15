

namespace WebApplication1edsf.Models
{
    enum TokenType
    {
        // Single-character tokens.
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR, PROC, DSLASH,
        RIGHT_SBRACE, LEFT_SBRACE,
        // One or two character tokens.
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,

        // Literals.
        IDENTIFIER, STRING, NUMBER, ARRAY, LEFT_ARRAY_INDEX, RIGHT_ARRAY_INDEX,

        // Keywords.
        AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR,
        PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

        CLEAR, PAUSE,

        SHOW, SANSWER, CANSWER, ANSWER,

        EOF
    }

    class Token
    {
        public TokenType type
        {
            get; private set;
        }
        public String lexeme
        {
            get; private set;
        }
        public Object literal
        {
            get; private set;
        }
        public int line
        {
            get; private set;
        }

        public Token(TokenType type, String lexeme, Object literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override String ToString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }
}
