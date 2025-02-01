using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceContainer
{
    public class Calculator : ICalculator
    {
        private readonly ILogger<Calculator> _logger;

        public Calculator(ILogger<Calculator> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public double Add(double first, double second)
        {
            _logger.LogInformation("start add operation...");
            double result = first + second;
            _logger.LogInformation("stop add operation");
            return result;
        }

        public double Div(double first, double second)
        {
            _logger.LogInformation("start div operation...");
            if (second < 0.000000001)
            {
                _logger.LogInformation("second argument is very small, operation aborted");
                return first;
            }
            double result = first / second;
            _logger.LogInformation("stop div operation");
            return result;
        }

        public double Multiply(double first, double second)
        {
            _logger.LogInformation("start multiply operation...");
            double result = first * second;
            _logger.LogInformation("stop multiply operation");
            return result;
        }

        public double Subtract(double first, double second)
        {
            _logger.LogInformation("start subtract operation...");
            double result = first - second;
            _logger.LogInformation("stop subtract operation");
            return result;
        }
    }
}
