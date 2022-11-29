using System.Linq.Expressions;

namespace ALinqyCalculator
{
    public abstract class Visitor
    {
        private readonly Expression node;
        protected readonly List<string> Output;

        protected Visitor(Expression node, List<string> output)
        {
            this.node = node;
            Output = output;
        }

        public abstract void Visit(string prefix);

        public void AddLine(string line)
        {
            Output.Add(line);
        }

        public override string ToString()
        {
            return string.Join('\n', Output);
        }

        public ExpressionType NodeType => this.node.NodeType;

        public static Visitor CreateFromExpression(Expression node, List<string> output)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Constant:
                    return new ConstantVisitor((ConstantExpression)node, output);
                case ExpressionType.Lambda:
                    return new LambdaVisitor((LambdaExpression)node, output);
                case ExpressionType.Parameter:
                    return new ParameterVisitor((ParameterExpression)node, output);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.SubtractChecked:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.Power:
                    return new BinaryVisitor((BinaryExpression)node, output);
                case ExpressionType.Call:
                    return new CallVisitor((MethodCallExpression)node, output);
                case ExpressionType.MemberAccess:
                    return new MemberAccessVisitor((MemberExpression)node, output);
                default:
                    Console.Error.WriteLine($"Node not processed yet: {node.NodeType}");
                    return default(Visitor);
            }
        }
    }

    // Lambda Visitor
    public class LambdaVisitor : Visitor
    {
        private readonly LambdaExpression node;
        public LambdaVisitor(LambdaExpression node, List<string> output) : base(node, output)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            AddLine($"{prefix}This expression is a {NodeType} expression type");
            AddLine($"{prefix}The name of the lambda is {((node.Name == null) ? "<<anonymous>>" : node.Name)}");
            AddLine($"{prefix}The return type is {node.ReturnType.ToString()}");
            AddLine($"{prefix}The expression has {node.Parameters.Count} argument(s).");
            // Visit each parameter:
            foreach (var argumentExpression in node.Parameters)
            {
                var argumentVisitor = Visitor.CreateFromExpression(argumentExpression, Output);
                argumentVisitor.Visit(prefix + "  ");
            }
            AddLine($"{prefix}The expression body is:");
            // Visit the body:
            var bodyVisitor = Visitor.CreateFromExpression(node.Body, Output);
            bodyVisitor.Visit(prefix + "  ");
        }
    }

    // Binary Expression Visitor:
    public class BinaryVisitor : Visitor
    {
        private readonly BinaryExpression node;
        public BinaryVisitor(BinaryExpression node, List<string> output) : base(node, output)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            AddLine($"{prefix}This binary expression is a {NodeType} expression");
            AddLine($"{prefix}'{node}'");

            var left = Visitor.CreateFromExpression(node.Left, Output);
            AddLine($"{prefix}The Left argument is:");
            left.Visit(prefix + "\t");
            var right = Visitor.CreateFromExpression(node.Right, Output);
            AddLine($"{prefix}The Right argument is:");
            right.Visit(prefix + "\t");
        }
    }

    // Parameter visitor:
    public class ParameterVisitor : Visitor
    {
        private readonly ParameterExpression node;
        public ParameterVisitor(ParameterExpression node, List<string> output) : base(node, output)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            AddLine($"{prefix}This is an {NodeType} expression type");
            AddLine($"{prefix}Type: {node.Type.ToString()}, Name: {node.Name}, ByRef: {node.IsByRef}");
        }
    }

    // Constant visitor:
    public class ConstantVisitor : Visitor
    {
        private readonly ConstantExpression node;
        public ConstantVisitor(ConstantExpression node, List<string> output) : base(node, output)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            AddLine($"{prefix}{NodeType} : {node.Type} = {node.Value}");
        }
    }

    public class MemberAccessVisitor : Visitor
    {
        private readonly MemberExpression node;
        public MemberAccessVisitor(MemberExpression node, List<string> output) : base(node, output)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            AddLine($"{prefix}{NodeType} : {node.Type} = {node.Member.Name}");
        }
    }

    // Constant visitor:
    public class CallVisitor : Visitor
    {
        private readonly MethodCallExpression node;
        public CallVisitor(MethodCallExpression node, List<string> output) : base(node, output)
        {
            this.node = node;
        }

        public override void Visit(string prefix)
        {
            AddLine($"{prefix}This is an {NodeType} expression type");
            AddLine($"{prefix}The method name is {node.Method.Name}");

            var paramTypes = node.Method.GetParameters().Select(p => p.ToString()).ToArray();

            AddLine($"{prefix}The parameters are:"); // {string.Join(',', node.Method.GetParameters().Select(p => $"{p.ToString()}"))}");

            int argNo = 0;
            foreach (var p in node.Arguments)
            {
                AddLine($"{prefix}  {p.ToString()}: {paramTypes[argNo]}");
                argNo++;
            }
        }
    }
}
