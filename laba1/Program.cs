//Лабораторная работа № 1 по дисциплине "Логические основы интеллектуальных систем"
//Выполнили студенты группы 121702 БГУИР Заяц Д.А. и Кимстач Д.Б.
//Файл содердит функцию Main, которая считывает строку и определеят является ли она КНФ
//Дата: 27.03.23

using System;

namespace CNFCheckerNamespace
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    string logicalExpression = Console.ReadLine();
                    Console.WriteLine(CNFChecker.GeeneralChecking(logicalExpression) ? "is CNF": "isn't CNF");
                }
            }
            catch(IncorrectExpressionException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}