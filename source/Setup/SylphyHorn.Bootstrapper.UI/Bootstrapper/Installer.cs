using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;
using WixToolset.Bootstrapper;

namespace SylphyHorn.Bootstrapper
{
	public partial class Installer : NotificationObject
	{
		private readonly SylphyHornBootstrapperApplication _ba;
		private Status _status = Status.Preparing;
		private Operation _operation;
		private InstallResult? _result;
		private bool _canceled;

		public string PackageId { get; } = "SylphyHornMsiPackage";

		public bool HasDisplay => this._ba.Command.Display == Display.Full || this._ba.Command.Display == Display.Passive;

		public Operation Operation
		{
			get { return this._operation; }
			private set
			{
				if (this._operation != value)
				{
					this._operation = value;
					this._ba.Engine.Log(LogLevel.Verbose, $"{nameof(Installer)}.{nameof(this.Operation)} = {value}");

					this.RaisePropertyChanged();
				}
			}
		}

		public Status Status
		{
			get { return this._status; }
			private set
			{
				if (this._status != value)
				{
					this._status = value;
					this._ba.Engine.Log(LogLevel.Verbose, $"{nameof(Installer)}.{nameof(this.Status)} = {value}");

					this.RaisePropertyChanged();
				}
			}
		}

		public InstallResult? Result
		{
			get { return this._result; }
			private set
			{
				if (this._result != value)
				{
					this._result = value;
					this._ba.Engine.Log(LogLevel.Verbose, $"{nameof(Installer)}.{nameof(this.Result)} = {value}");

					this.RaisePropertyChanged();
				}
			}
		}

		private Installer(SylphyHornBootstrapperApplication ba)
		{
			this._ba = ba;
		}

		public void Detect()
		{
			this._ba.Engine.Detect();
		}

		public void Start()
		{
			switch (this.Operation)
			{
				case Operation.None:
					throw new InvalidOperationException();

				case Operation.Install:
				case Operation.Upgrade:
					this._ba.Engine.Plan(LaunchAction.Install);
					break;

				case Operation.Uninstall:
					this._ba.Engine.Plan(LaunchAction.Uninstall);
					break;

				default:
					throw new NotImplementedException();
			}
		}

		public void Cancel()
		{
			this._canceled = true;
		}

		public void Log(LogLevel level, string message)
		{
			this._ba.Engine.Log(level, message);
		}

		public void Quit(int exitCode)
		{
			this._ba.Engine.Quit(exitCode);
		}
	}
}
