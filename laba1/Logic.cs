//Лабораторная работа № 1 по дисциплине "Логические основы интеллектуальных систем"
//Выполнили студенты группы 121702 БГУИР Заяц Д.А. и Кимстач Д.Б.
//Файл содержит класс, определяющий, является ли логическая формула КНФ
//Дата: 27.03.23

using System;
using System.Collections.Generic;
using System.Linq;

namespace CNFCheckerNamespace
{
    /// <summary>
    /// Class derived from the class Exception. Needed to report about any errors
    /// </summary>
    class IncorrectExpressionException : Exception 
    {
        public IncorrectExpressionException(string message)
        {
            Console.WriteLine(message);
        }
    }

    /// <summary>
    /// Main class that implements all functions to check if the expression is CNF
    /// </summary>
    class CNFChecker
    {
        /// <summary>
        /// Accepted logical operations
        /// </summary>
        private static readonly Dictionary<string, char> logicalOperations = new Dictionary<string, char>()
        {
            { "disjunction", '+' },
            { "conjunction", '*' },
            { "negation", '!' }
        };

        /// <summary>
        /// Other symbols that mean false
        /// </summary>
        private static readonly List<string> specialSymbols = new List<string> { "0", ".L"};

        /// <summary>
        /// Core function that calls all other function and return final result
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>True if expression is CNF, false in another way</returns>
        /// <exception cref="IncorrectExpressionException">report aboout any errors like incorrect placement of parenthesis, lack of terms or unreadable literal</exception>
        public static bool GeeneralChecking(string expression)
        {
            if (specialSymbols.Contains(expression))
                return true;
            //Creating list of all parenthesis in the expression
            List<char> expressionParenthesis = expression.Where(sym => sym == '(' || sym == ')').ToList();
            if (!CheckParenthesis(expressionParenthesis))
                throw new IncorrectExpressionException("Incorrect placement of parenthesis");
            //If expression is parenthesed removes outside parenthesis
            if (expression[0] == '(' && expression[expression.Length-1] == ')')
                expression = expression.Substring(1, expression.Length-2);  
            //Creating list of terms
            List<string> terms = GetTerms(expression);
            //If "GetTerms" function didn't find any terms throw IncorrectExpressionExceptionException
            if (!terms.Any())
                throw new IncorrectExpressionException("No terms founded. Looks like you have inserted empty parenthesis");
            //Checks each term if it is correct
            foreach (string term in terms)
                if (!IsPrimalDisjunction(term))
                    return false;
            return true;
        }

        /// <summary>
        /// Checks if all parenthesis are matched
        /// </summary>
        /// <param name="parenthesisList"></param>
        /// <returns>True if all parenthesis are match else returns false</returns>
        private static bool CheckParenthesis(List<char> parenthesisList)
        {
            List<char> parenthesisStack = new List<char>();
            Dictionary<char, char> mapping = new Dictionary<char, char>() { { ')', '(' } };
            foreach (char parenthesis in parenthesisList)
            {
                if (mapping.TryGetValue(parenthesis, out char openParenthesis))
                {
                    char topElement = parenthesisStack[parenthesisStack.Count-1];
                    parenthesisStack.RemoveAt(parenthesisStack.Count - 1);
                    if (openParenthesis != topElement)
                        return false;
                }
                else
                    parenthesisStack.Add(parenthesis);
            }
            return !parenthesisStack.Any();
        }

        /// <summary>
        /// Divides expression into terms
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>List of terms of logical expression</returns>
        private static List<string> GetTerms(string expression)
        {
            List<string> terms = new List<string>();
            string rawTerm = "";
            int parenthesisRank = 0;
            //Adds the next symbol until it finds the Null reference or conjunction sign
            foreach (char sym in expression)
            {
                if (sym == '(')
                    parenthesisRank++;
                else if (sym == ')')
                    parenthesisRank--;
                //If conjunction symbol is detected and inside parenthesis are matched then add substring to the terms list
                else if (sym == logicalOperations["conjunction"] && parenthesisRank == 0)
                {
                    terms.Add(rawTerm);
                    rawTerm = "";
                    continue;
                }
                rawTerm += sym;
            }
            //If finds Null reference then adds last substring to the terms list
            if (!string.IsNullOrEmpty(rawTerm))
                terms.Add(rawTerm);
            return terms;
        }

        /// <summary>
        /// Checks if the incoming string variable is primal disjunction
        /// </summary>
        /// <param name="term"></param>
        /// <returns>True if input string is primal disjunction, false in the another way</returns>
        /// <exception cref="IncorrectExpressionException">Report about empty terms</exception>
        private static bool IsPrimalDisjunction(string term)
        {
            //If term is Empty throws IncorrectExpressionException
            if (string.IsNullOrEmpty(term))
                throw new IncorrectExpressionException("Empty term.");
            //If terms is parenthesed removes outside parenthesis
            if (term[0] == '(' && term[term.Length-1] == ')')
                term = term.Substring(1, term.Length-2);
            //If term contains negation of binary formula returns false
            if (term.Contains("!("))
                return false;
            //Divides term into literals
            List<string> literals = term.Split(logicalOperations["disjunction"]).ToList();
            //Cheks each literal if it is correct
            foreach (string literal in literals)
            {
                if (!IsLiteral(literal))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the incoming string variable is correct literal
        /// </summary>
        /// <param name="literal"></param>
        /// <returns>True if input string is correct literal, false in the another way</returns>
        /// <exception cref="IncorrectExpressionException">Report about unreadable literal</exception>
        private static bool IsLiteral(string literal)
        {
            //If literal is Empty throws IncorrectExpressionException
            if (string.IsNullOrEmpty(literal))
            {
                throw new IncorrectExpressionException("Unreadable literal.");
            }
            //Checks if literal is logical constant
            if (literal == ".L" || literal == "T")
                return true;
            //Removes every parethesis and negation symbol
            literal = literal.Replace("(", "").Replace(")", "");
            literal = literal.Replace(CNFChecker.logicalOperations["negation"].ToString(), "");
            //If symbol is not uppercase returns false 
            if (!char.IsUpper(literal[0]))
                return false;
            //Checks index of literal, returns false if symbol isn't digit or first number is zero
            if (literal.Length > 1)
            {
                if (!char.IsDigit(literal[1]) || literal[1] == '0')
                    return false;
                for(int i = 2; i < literal.Length; i++)
                {
                    if (!(char.IsDigit(literal[i])))
                        return false;
                }
            }
            return true;
        }
    }
}
