using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServiceContainer
{
    public interface ILogger<T>
    {
        void LogInformation(string message);
    }
}
