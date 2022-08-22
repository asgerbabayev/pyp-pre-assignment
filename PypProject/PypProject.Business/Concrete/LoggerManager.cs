using NLog;
using PypProject.Business.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PypProject.Business.Concrete
{
    public class LoggerManager : ILoggerService
    {
        private readonly ILogger logger;

        public LoggerManager() => logger = LogManager.GetCurrentClassLogger();

        public void LogDebug(string message) => logger.Debug(message);

        public void LogError(string message) => logger.Error(message);

        public void LogInfo(string message) => logger.Info(message);

        public void LogWarn(string message) => logger.Warn(message);
    }
}
