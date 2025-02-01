using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceContainer
{
    public interface ICalculator
    {
        double Add(double first, double second);
        double Div(double first, double second);
        double Multiply(double first, double second);
        double Subtract(double first, double second);
    }
}
