public class Lexer
{
	private static readonly Dictionary<string, string> Keywords = new Dictionary<string, string>
	{
		{ "int", "Type" },
		{ "bool", "Type" },
		{ "void", "Type" },
		{ "main", "Main" },
		{ "return", "Return" },
		{ "for", "For" },
		{ "if", "If" }
	};

	public static List<Token> Tokenize(string input)
	{
		List<Token> tokens = new List<Token>();
		string currentToken = string.Empty;
		int line = 1;
		int column = 1;

		for (int i = 0; i < input.Length; i++)
		{
			char currentChar = input[i];

			// Обрабатываем переходы на новую строку
			if (currentChar == '\n')
			{
				line++;
				column = 1;
				continue;
			}

			// Пропускаем пробельные символы
			if (char.IsWhiteSpace(currentChar))
			{
				column++;
				continue;
			}

			// Если текущий символ - буква или _
			if (char.IsLetter(currentChar) || currentChar == '_')
			{
				// Начинаем собирать идентификатор
				currentToken += currentChar;

				// Собираем всю строку для идентификатора
				while (i + 1 < input.Length && (char.IsLetterOrDigit(input[i + 1]) || input[i + 1] == '_'))
				{
					i++;
					currentToken += input[i];
				}

				// Проверяем, является ли это ключевым словом
				if (Keywords.ContainsKey(currentToken))
				{
					tokens.Add(new Token(currentToken, Keywords[currentToken], line, column));
				}
				else
				{
					// Если это не ключевое слово, то это просто идентификатор
					tokens.Add(new Token(currentToken, "Identifier", line, column));
				}

				currentToken = string.Empty; // Очищаем текущий токен
			}
			else if (char.IsDigit(currentChar))
			{
				// Начинаем собирать число
				currentToken += currentChar;

				// Собираем всю строку для числа
				while (i + 1 < input.Length && char.IsDigit(input[i + 1]))
				{
					i++;
					currentToken += input[i];
				}

				tokens.Add(new Token(currentToken, "Number", line, column));
				currentToken = string.Empty; // Очищаем текущий токен
			}
			else if (currentChar == '=')
			{
				tokens.Add(new Token("=", "Assign", line, column));
			}
			else if (currentChar == '(' || currentChar == ')' || currentChar == '{' || currentChar == '}' || currentChar == ';')
			{
				tokens.Add(new Token(currentChar.ToString(), "Symbol", line, column));
			}
			else
			{
				throw new Exception($"Unexpected character at [{line},{column}]: {currentChar}");
			}

			column++;
		}

		return tokens;
	}
}

public class Token
{
	public string Value { get; }
	public string Type { get; }
	public int Line { get; }
	public int Column { get; }

	public Token(string value, string type, int line, int column)
	{
		Value = value;
		Type = type;
		Line = line;
		Column = column;
	}

	public override string ToString()
	{
		return $"[{Line},{Column}] {Type}: {Value}";
	}
}
