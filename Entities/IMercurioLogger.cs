using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface IMercurioLogger
    {
        void SetLevel(LogMessageLevelEnum level);
        void Log(LogMessageLevelEnum level, string message);
    }
}
