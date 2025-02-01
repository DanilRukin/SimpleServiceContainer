namespace SimpleServiceContainer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IServiceContainer services = new ServiceContainer();

            services.Register<ICalculator, Calculator>();
            services.Register<ILogger<Calculator>, ConsoleLogger<Calculator>>();
            //services.Register<ICalculator>(() =>
            //{
            //    return new Calculator(services.Resolve<ILogger<Calculator>>());
            //});

            double a, b;
            Console.Write("Input a = ");
            a = Convert.ToDouble(Console.ReadLine());
            Console.Write("Input b = ");
            b = Convert.ToDouble(Console.ReadLine());

            

            var calculator = services.Resolve<ICalculator>();

            calculator.Add(a, b);
            calculator.Subtract(a, b);
            calculator.Multiply(a, b);
            calculator.Div(a, b);

            Console.Write("Press any key");
            Console.Read();
        }
    }
}
