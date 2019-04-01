using LoggingSample_Logs_DAL.Entities;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LoggingSample_BLL.LogTargets
{
    [Target("XMLTarget")]
    public class XMLTarget : AsyncTaskTarget
    {
        private readonly object threadLock;

        public XMLTarget()
        {
            Host = Environment.MachineName;
            threadLock = new object();
        }

        [RequiredParameter]
        public string Host { get; set; }

        protected override async Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {
            var filePath = LogManager.Configuration.Variables["xmlLogFilePath"].Render(logEvent);

            var log = GetXElement(logEvent);

            await Task.Run(() =>
            {
                lock (threadLock)
                {
                    if (!File.Exists(filePath))
                    {
                        using (var xmlWriter = XmlWriter.Create(filePath))
                        {
                            xmlWriter.WriteStartDocument();
                            xmlWriter.WriteStartElement("Logs");
                        }
                    }

                    var xmlDocument = XDocument.Load(filePath);
                    xmlDocument.Root.Add(log);
                    xmlDocument.Save(filePath);
                }
            });          
        }

        private XElement GetXElement(LogEventInfo logEvent)
        {
            return new XElement("Log",
                        new XElement("MachineName", this.Host),
                        new XElement("Exception", logEvent.Exception?.ToString()),
                        new XElement("LoggerName", logEvent.LoggerName),
                        new XElement("Level", logEvent.Level.ToString()),
                        new XElement("Message", logEvent.Message),
                        new XElement("MessageSource", logEvent.CallerFilePath),
                        new XElement("TimeStamp", logEvent.TimeStamp));
        }
    }
}
