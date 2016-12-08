using System;
using System.Collections.Generic;
using System.Linq;
using Livet;

namespace SylphyHorn.Services
{
	public class LoggingService
	{
		public static LoggingService Instance { get; } = new LoggingService();

		public ObservableSynchronizedCollection<Exception> Exceptions { get; } = new ObservableSynchronizedCollection<Exception>();

		private LoggingService() { }

		public void Register(Exception exception)
		{
			this.Exceptions.Add(exception);
		}
	}
}
