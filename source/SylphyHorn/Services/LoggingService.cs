using System;
using System.Collections.Generic;
using System.Linq;
using Livet;
using MetroTrilithon.Threading.Tasks;

namespace SylphyHorn.Services
{
	public interface ILog
	{
		DateTimeOffset DateTime { get; }
		string Header { get; }
		string Content { get; }
	}

	public class LoggingService
	{
		public static LoggingService Instance { get; } = new LoggingService();

		public ObservableSynchronizedCollection<ILog> Logs { get; } = new ObservableSynchronizedCollection<ILog>();

		private LoggingService() { }

		public void Register(ILog log)
		{
			this.Logs.Add(log);
		}

		public void Register(Exception ex)
		{
			this.Register(new Log(ex.GetType().Name, ex.ToString()));
		}

		public void Register(TaskLog log)
		{
			var content = $@"Unhandled exception was thrown by Task<T>.
{log.CallerMemberName} ({System.IO.Path.GetFileName(log.CallerFilePath)}#{log.CallerLineNumber})
-----
{log.Exception}";
			this.Register(new Log(log.Exception.GetType().Name, content));
		}

		private class Log : ILog
		{
			public DateTimeOffset DateTime { get; } = DateTimeOffset.Now;

			public string Header { get; }

			public string Content { get; }

			public Log(string header, string content)
			{
				this.Header = header;
				this.Content = content;
			}
		}
	}
}
