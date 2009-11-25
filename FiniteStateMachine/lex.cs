using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiniteStateMachine
{
    internal class lex
    {
        internal static ruleset Parse(System.IO.TextReader s)
        {
            //return (ruleset)new yy().yyparse(new input(s), new yydebug.yyDebugSimple(), null);
            return (ruleset)new yy().yyparse(new input(s), null);
        }

        internal class input : yyParser.yyInput
        {
            public input(System.IO.TextReader s)
            {
                cs = s;
            }

            public bool advance()
            {
                int i = cs.Read();
                if (i == -1) return false;
                switch ((char)i)
                {
                    case '\r':
                        {
                            int j = cs.Read();
                            if (j == '\n')
                            {
                                tok = Token.EOL;
                                current = null;
                            }
                            else
                            {
                                throw new Exception("Meet \r without \n.");
                            }
                        }
                        break;
                    case '\n':
                        tok = Token.EOL;
                        current = null;
                        break;
                    case ':':
                        tok = Token.COLON;
                        current = null;
                        break;
                    case '.':
                        tok = Token.DOT;
                        current = null;
                        break;
                    default:
                        tok = Token.CHAR;
                        current = (char)i;
                        break;
                }
                return true;
            }

            public int token()
            {
                return tok;
            }

            public Object value()
            {
                return current;
            }

            private int tok;
            private Object current;
            private System.IO.TextReader cs;
        }
    }
}
