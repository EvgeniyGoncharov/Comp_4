using System;
using System.Collections.Generic;

public class Token
{
	public string Value { get; set; }
	public string Type { get; set; }
	public int Line { get; set; }
	public int Column { get; set; }

	public Token(string value, string type, int line, int column)
	{
		Value = value;
		Type = type;
		Line = line;
		Column = column;
	}

	public override string ToString()
	{
		return $"{Line},{Column} {Type}: {Value}";
	}
}

public class Parser
{
    private Queue<Token> tokens;
    private List<string> errors;

    public Parser(List<Token> tokenList)
    {
        tokens = new Queue<Token>(tokenList);
        errors = new List<string>();
    }

    public void ParseProgram()
    {
        ParseType();
        Match("main");
        Match("(");
        Match(")");
        Match("{");
        ParseStatements();
        Match("}");
    }

    public void ParseType()
    {
        if (Match("int") || Match("bool") || Match("void"))
            return;
        RecordError("Expected type (int, bool, void).");
    }

    public void ParseStatements()
    {
        while (tokens.Count > 0)
        {
            if (Match("{"))
            {
                ParseStatements();
                Match("}");
            }
            else if (Match("for"))
            {
                ParseFor();
            }
            else if (Match("switch")) // Обработчик для switch
            {
                ParseSwitch(); // Новый метод для обработки switch
            }
            else if (Match("if"))
            {
                ParseIf();
            }
            else if (Match("return"))
            {
                ParseReturn();
            }
            else if (Match("int") || Match("bool") || Match("void"))
            {
                ParseDeclaration();
                Match(";");
            }
            else if (Match("}"))
            {
                break;
            }
            else
            {
                RecordError("Expected statement.");
            }
        }
    }

    // Метод для обработки конструкции switch
    public void ParseSwitch()
    {
        Match("("); // Открывающая скобка выражения switch
        ParseIdentifier(); // Выражение для switch
        Match(")"); // Закрывающая скобка выражения switch
        Match("{"); // Открывающая фигурная скобка блока switch

        while (true)
        {
            if (Match("case")) // Проверяем на наличие case
            {
                ParseCase(); // Вызов метода для обработки case
            }
            else if (Match("default")) // Проверяем на наличие default
            {
                ParseDefault(); // Вызов метода для обработки default
            }
            else if (Match("}")) // Конец блока switch
            {
                break;
            }
            else
            {
                RecordError("Expected 'case' or 'default'.");
            }
        }
    }

    // Метод для обработки конструкции case
    public void ParseCase()
    {

        var constant = ParseConstant(); // Получаем константу для сравнения
        Match(":"); // Двоеточие после case
        ParseDeclaration(); // Блок операторов для данного case
    }

    // Метод для обработки конструкции default
    public void ParseDefault()
    {
        Match(":"); // Двоеточие после default
        ParseStatements(); // Блок операторов для default
    }

    // Метод для получения константы (для case)
    public int ParseConstant()
    {
        if (MatchNumber()) // Если это число
        {
            var token = tokens.Peek();
            int result = int.Parse(token.Value);
			tokens.Dequeue();
            return result;
        }
        else
        {
            RecordError("Expected constant value for case.");
            return 0; // В случае ошибки возвращаем нулевое значение
        }
    }

    public void ParseFor()
	{
		Match("(");
		ParseDeclaration();
		Match(";");
		ParseBoolExpression();
		Match(";");
		ParseArithmeticExpression();
		Match(")");
		Match("{");
		ParseStatements();
		Match("}");
	}

	public void ParseIf()
	{
		Match("(");
		ParseBoolExpression();
		Match(")");
		Match("{");
		ParseStatements();
		Match("}");
	}

	public void ParseBoolExpression()
	{
		ParseArithmeticExpression();
		ParseRelop();
		ParseArithmeticExpression();
	}

	public void ParseRelop()
	{
		if (Match("<") || Match(">") || Match("==") || Match("!="))
			return;
		RecordError("Expected relational operator.");
	}

	public void ParseDeclaration()
	{
		ParseIdentifier();
		ParseAssign();
		ParseArithmeticExpression();
	}

	public void ParseReturn()
	{
		Match("return");
		ParseArithmeticExpression();
		Match(";");
	}

	public void ParseArithmeticExpression()
	{
		ParseTerm();

		while (Match("+") || Match("-"))
		{
			ParseTerm();
		}
	}

	public void ParseTerm()
	{
		ParseFactor();

		while (Match("*") || Match("/"))
		{
			ParseFactor();
		}
	}

	public void ParseFactor()
	{
		if (Match("("))
		{
			ParseArithmeticExpression();
			Match(")");
		}
		else if (MatchNumber() || MatchIdentifier())
		{
			ParseNumber()
		}
		else
		{
			RecordError("Expected a number, identifier, or parenthesized expression.");
		}
	}

	public void ParseIdentifier()
	{
		if (!MatchIdentifier())
		{
			RecordError("Expected identifier.");
		}
	}

	public void ParseAssign()
	{
		if (!Match("="))
		{
			RecordError("Expected assignment operator '='.");
		}
	}

	public bool Match(string value)
	{
		if (tokens.Count == 0) return false;

		var token = tokens.Peek();
		if (token.Value == value)
		{
			Console.WriteLine($"Matched: {token}");
			tokens.Dequeue();
			return true;
		}
		return false;
	}

	public bool MatchIdentifier()
	{
		if (tokens.Count == 0) return false;

		var token = tokens.Peek();
		if (char.IsLetter(token.Value[0]))
		{
			Console.WriteLine($"Matched: {token}");
			tokens.Dequeue();
			return true;
		}
		return false;
	}

	public bool MatchNumber()
	{
		if (tokens.Count == 0) return false;

		var token = tokens.Peek();
		if (int.TryParse(token.Value, out _))
		{
			Console.WriteLine($"Matched: {token}");
			return true;
		}
		return false;
	}

	private void RecordError(string message)
	{
		if (tokens.Count > 0)
		{
			var token = tokens.Peek();
			errors.Add($"Error at [{token.Line},{token.Column}]: {message}. Token: {token}");
			tokens.Dequeue();
		}
		else
		{
			errors.Add("Error at [EOF]: Unexpected end of input.");
		}
	}

	public void PrintErrors()
	{
		if (errors.Count > 0)
		{
			Console.WriteLine("\n--- Errors ---");
			foreach (var error in errors)
			{
				Console.WriteLine(error);
			}
		}
		else
		{
			Console.WriteLine("\nNo errors found.");
		}
	}
}

class Program
{
	static void Main(string[] args)
	{
		string input = "int main() { int a = 0; return 10; }";

		var tokens = Lexer.Tokenize(input);

		Console.WriteLine("Tokens:");
		foreach (var token in tokens)
		{
			Console.WriteLine(token);
		}

		Parser parser = new Parser(tokens);
		parser.ParseProgram();
		parser.PrintErrors();
	}
}
