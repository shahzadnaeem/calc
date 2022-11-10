using System;
using Sprache;
using Categories;

namespace ALinqyCalculator
{
    class Program
    {
        static void Main()
        {
            // Categories.Main.Run();

            Console.Clear();

            var line = "";
            while (Prompt(out line))
            {
                try
                {
                    var parsed = ExpressionParser.ParseExpression(line);
                    Console.WriteLine("Parsed as {0}", parsed);
                    var visitor = new LambdaVisitor(parsed, new List<string>());
                    Console.WriteLine("Details: ");
                    visitor.Visit("> ");
                    Console.WriteLine(visitor.ToString());

                    Console.WriteLine("Value is {0}", parsed.Compile()());

                }
                catch (ParseException ex)
                {
                    Console.WriteLine("There was a problem with your input: {0}", ex.Message);
                }

                Console.WriteLine();
            }
        }

        static bool Prompt(out string value)
        {
            Console.Write("Enter a numeric expression, or 'q' to quit: ");
            var line = Console.ReadLine() ?? "q";

            if (line.ToLowerInvariant().Trim() == "q")
            {
                value = line;
                return false;
            }
            else
            {
                value = line;
                return true;
            }
        }
    }
}
