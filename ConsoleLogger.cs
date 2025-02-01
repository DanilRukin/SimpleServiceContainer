using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceContainer
{
    public class ConsoleLogger<T> : ILogger<T>
    {
        public void LogInformation(string message)
        {
            Console.WriteLine($"Info: {typeof(T)} - {message}");
        }
    }
}
