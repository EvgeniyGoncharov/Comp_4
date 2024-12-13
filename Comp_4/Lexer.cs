using System;
using System.Collections.Generic;



public class Lexer
{
    private static readonly Dictionary<string, string> Keywords = new Dictionary<string, string>
{
    // Существующие ключевые слова...
    { "int", "Type" },
    { "bool", "Type" },
    { "void", "Type" },
    { "main", "Main" },
    { "return", "Return" },
    { "for", "For" },
    { "if", "If" },
    
    // Новые ключевые слова
    { "switch", "Switch" },   // Токен для ключевого слова "switch"
    { "case", "Case" }// Токен для ключевого слова "case"
};

    private static readonly HashSet<string> RelationalOperators = new HashSet<string>
	{
		"<", ">", "==", "!="
	};

	private static readonly HashSet<char> ArithmeticOperators = new HashSet<char>
	{
		'+', '-', '*', '/'
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
                currentToken += currentChar;

                while (i + 1 < input.Length && (char.IsLetterOrDigit(input[i + 1]) || input[i + 1] == '_'))
                {
                    i++;
                    currentToken += input[i];
                }

                if (Keywords.ContainsKey(currentToken))
                {
                    tokens.Add(new Token(currentToken, Keywords[currentToken], line, column));
                }
                else
                {
                    tokens.Add(new Token(currentToken, "Identifier", line, column));
                }

                currentToken = string.Empty;
            }
            else if (char.IsDigit(currentChar))
            {
                currentToken += currentChar;

                while (i + 1 < input.Length && char.IsDigit(input[i + 1]))
                {
                    i++;
                    currentToken += input[i];
                }

                tokens.Add(new Token(currentToken, "Number", line, column));
                currentToken = string.Empty;
            }
            else if (currentChar == '=')
            {
                if (i + 1 < input.Length && input[i + 1] == '=')
                {
                    tokens.Add(new Token("==", "RelationalOperator", line, column));
                    i++;
                }
                else
                {
                    tokens.Add(new Token("=", "Assign", line, column));
                }
            }
            else if (currentChar == '!' && i + 1 < input.Length && input[i + 1] == '=')
            {
                tokens.Add(new Token("!=", "RelationalOperator", line, column));
                i++;
            }
            else if (RelationalOperators.Contains(currentChar.ToString()))
            {
                tokens.Add(new Token(currentChar.ToString(), "RelationalOperator", line, column));
            }
            else if (ArithmeticOperators.Contains(currentChar))
            {
                tokens.Add(new Token(currentChar.ToString(), "ArithmeticOperator", line, column));
            }
            else if ("(){};:".Contains(currentChar))
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