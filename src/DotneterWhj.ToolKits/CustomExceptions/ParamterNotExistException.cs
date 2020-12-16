using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.ToolKits
{
    public class ParamterNotExistException : Exception
    {
        public ParamterNotExistException(string message) : base(message)
        {
        }
    }
}
