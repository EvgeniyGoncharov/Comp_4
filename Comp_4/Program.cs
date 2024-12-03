using System;
using System.Collections.Generic;



public class Parser
{
	private Queue<Token> tokens;
	private int currentPosition = 0;

	public Parser(List<Token> tokenList)
	{
		tokens = new Queue<Token>(tokenList);
	}

	public void ParseProgram()
	{
		ParseType();
		Match("main");
		Match("(");
		Match(")");
		Match("{");
		ParseStatement();
		Match("}");
	}

	public void ParseType()
	{
		if (Match("int") || Match("bool") || Match("void"))
			return;
		ThrowError("Expected type (int, bool, void).");
	}

	public void ParseStatement()
	{
		if (Match("{"))
		{
			ParseStatement();
			Match("}");
		}
		else if (Match("for"))
		{
			ParseFor();
			ParseStatement();
		}
		else if (Match("if"))
		{
			ParseIf();
			ParseStatement();
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
		else
		{
			ThrowError("Expected statement.");
		}
	}

	public void ParseFor()
	{
		Match("(");
		ParseDeclaration();
		Match(";");
		ParseBoolExpression();
		Match(";");
		Match(")");
	}

	public void ParseIf()
	{
		Match("(");
		ParseBoolExpression();
		Match(")");
	}

	public void ParseBoolExpression()
	{
		ParseIdentifier();
		ParseRelop();
		ParseIdentifier();
	}

	public void ParseRelop()
	{
		if (Match("<") || Match(">") || Match("==") || Match("!="))
			return;
		ThrowError("Expected relational operator.");
	}

	public void ParseDeclaration()
	{
		ParseIdentifier();  // Здесь проверяем на идентификатор (например, переменная)
		Match("=");  // Присваивание
		ParseAssignEnd();  // Значение после присваивания
	}

	public void ParseIdentifier()
	{
		// Проверяем, что это корректный идентификатор
		if (!MatchIdentifier())
			ThrowError("Expected identifier.");
	}

	public void ParseAssignEnd()
	{
		if (!MatchNumber() && !MatchIdentifier())  // Можно присвоить число или другой идентификатор
			ThrowError("Expected assignment value.");
	}

	public void ParseReturn()
	{
		Match("return");
		ParseNumber();
		Match(";");
	}

	public void ParseNumber()
	{
		if (!MatchNumber())
			ThrowError("Expected a number.");
	}

	public bool Match(string value)
	{
		if (tokens.Count == 0) return false;

		var token = tokens.Peek();
		if (token.Value == value)
		{
			tokens.Dequeue();
			Console.WriteLine($"Matched: {token}");
			return true;
		}
		return false;
	}

	public bool MatchCharacter()
	{
		var token = tokens.Peek();
		if (char.IsLetter(token.Value[0]) || token.Value[0] == '_')
		{
			tokens.Dequeue();
			Console.WriteLine($"Matched: {token}");
			return true;
		}
		return false;
	}

	public bool MatchNumber()
	{
		var token = tokens.Peek();
		if (int.TryParse(token.Value, out _))
		{
			tokens.Dequeue();
			Console.WriteLine($"Matched: {token}");
			return true;
		}
		return false;
	}

	public bool MatchIdentifier()
	{
		var token = tokens.Peek();
		if (char.IsLetter(token.Value[0]) || token.Value[0] == '_')
		{
			// Считываем идентификатор
			tokens.Dequeue();
			Console.WriteLine($"Matched: {token}");
			return true;
		}
		return false;
	}

	private void ThrowError(string message)
	{
		var token = tokens.Peek();
		throw new Exception($"Error at [{token.Line},{token.Column}]: {message}. Token: {token}");
	}
}

class Program
{
	static void Main(string[] args)
	{
		string input = "int main() { int x = 5; return 10; }";

		// 1. Лексический анализ
		var tokens = Lexer.Tokenize(input);

		// Выводим все токены
		Console.WriteLine("Токены:");
		foreach (var token in tokens)
		{
			Console.WriteLine(token);
		}

		// 2. Синтаксический анализ
		Parser parser = new Parser(tokens);
		try
		{
			parser.ParseProgram();
			Console.WriteLine("Программа синтаксически корректна.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка синтаксического анализа: {ex.Message}");
		}
	}
}
