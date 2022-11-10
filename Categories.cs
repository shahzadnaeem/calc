
namespace Categories
{
    public interface ICategory<TObject, TMorphism>
    {
        IEnumerable<TObject> Objects { get; }

        TMorphism Compose(TMorphism m2, TMorphism m1);

        TMorphism Id(TObject obj);
    }

    public static partial class Functions
    {
        public static Func<TSource, TResult> o<TSource, TMiddle, TResult>(
            this Func<TMiddle, TResult> function2, Func<TSource, TMiddle> function1) =>
                value => function2(function1(value));

        public static TSource Id<TSource>(TSource value) => value;
    }

    public class Main
    {

        public static string NumToString(int num)
        {
            return $"{num}";
        }

        public static int StringToNum(string s)
        {
            return s.Length;
        }

        public static void Run()
        {
            Console.WriteLine(Functions.o<int, string, int>(StringToNum, NumToString)(123));
        }
    }
}
