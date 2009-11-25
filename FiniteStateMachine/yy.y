%{
using System.Collections.Generic;
namespace FiniteStateMachine {
%}

%token CHAR EOL COLON DOT

%start  rules

%%

rules:
	rules rule
		{
			$$ = $1;
			((ruleset)$$).Add((rule)$2);
		}
	| rule
		{
			$$ = new ruleset();
			((ruleset)$$).Add((rule)$1);
		}
	;

rule:
	input COLON string EOL
		{
			$$ = new rule((match)$1, (string)$3);
		}

input:
	string DOT string
		{
			$$ = new match();
			((match)$$).replay = (string)$1;
			((match)$$).input = (string)$3;
		}
	| string
		{
			$$ = new match();
			((match)$$).input = (string)$1;
		}
		
string: CHAR
		{
			$$ = $1.ToString();
		}
	| string CHAR
		{
			$$ = (string)$1 + (char)$2;
		}
	;

%%
