using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.ToolKits
{
    public class NLogger<T> : ICustomLogger<T>
    {
        private readonly ILogger _logger = LogManager.GetLogger(typeof(T).FullName, typeof(T));

        private readonly GlobalWholeLink _wholeLink;

        public NLogger(GlobalWholeLink wholeLink)
        {
            this._wholeLink = wholeLink;
        }

        public void Debug(string message)
        {
            _logger.Debug($"The Whole Link Id: {_wholeLink.WholeLinkId},{message}");
        }

        public void Error(string message)
        {
            _logger.Error($"The Whole Link Id: {_wholeLink.WholeLinkId},{message}");
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(exception, $"The Whole Link Id: {_wholeLink.WholeLinkId},{message}");
        }

        public void Fatal(string message)
        {
            _logger.Fatal($"The Whole Link Id: {_wholeLink.WholeLinkId},{message}");
        }

        public void Fatal(string message, Exception exception)
        {
            _logger.Fatal(exception, $"The Whole Link Id: {_wholeLink.WholeLinkId},{message}");
        }

        public void Info(string message)
        {
            _logger.Info($"The Whole Link Id: {_wholeLink.WholeLinkId},{message}");
        }

        public void Trace(string message)
        {
            _logger.Trace($"The Whole Link Id: {_wholeLink.WholeLinkId},{message}");
        }

        public void Warning(string message)
        {
            _logger.Warn($"The Whole Link Id: {_wholeLink.WholeLinkId},{message}");
        }
    }
}
