using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlLinqProvider
{
    class JiraLog
    {
        public DateTime LogDate { get; set; }
        public LogLevel LogLevel { get; set; }
    }
    enum LogLevel { WARN, ERROR, INFO, FATAL }
}
