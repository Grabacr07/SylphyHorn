using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using MetroTrilithon.Threading.Tasks;
using SylphyHorn.Properties;
using WixToolset.Bootstrapper;

namespace SylphyHorn.Bootstrapper
{
	partial class Installer
	{
		public sealed class SylphyHornBootstrapperApplication : BootstrapperApplication
		{
			private Installer _installer;

			protected override void Run()
			{
				this.Engine.Log(LogLevel.Verbose, $"Bundle.Name: {this.BAManifest.Bundle.Name}");
				this.Engine.Log(LogLevel.Verbose, $"CurrentUICulture: {CultureInfo.CurrentUICulture}");

				Resources.Culture = CultureInfo.CurrentUICulture;
				AppDomain.CurrentDomain.UnhandledException += (sender, e) => this.Engine.Log(LogLevel.Verbose, e.ExceptionObject?.ToString());
				TaskLog.Occured += (sender, log) => this.Engine.Log(LogLevel.Error, $"Unhandled Exception: {log.Exception.Message}");

				this._installer = new Installer(this);
				var application = new Application(this._installer);
				var exitCode = application.Run();

				this.Engine.Quit(exitCode);
			}

			protected override void OnDetectPackageComplete(DetectPackageCompleteEventArgs e)
			{
				base.OnDetectPackageComplete(e);

				if (e.PackageId == this._installer.PackageId)
				{
					if (this._installer.Operation != Operation.None)
					{
						this.Engine.Log(LogLevel.Error, $"Install target '{this._installer.PackageId}' is already detected.");
						return;
					}

					switch (e.State)
					{
						case PackageState.Absent:
							this._installer.Operation = Operation.Install;
							break;

						case PackageState.Present:
							this._installer.Operation = Operation.Uninstall;
							break;

						case PackageState.Obsolete:
							this._installer.Operation = Operation.Upgrade;
							break;
					}
				}
			}

			protected override void OnDetectMsiFeature(DetectMsiFeatureEventArgs e)
			{
				base.OnDetectMsiFeature(e);
			}

			protected override void OnDetectComplete(DetectCompleteEventArgs e)
			{
				base.OnDetectComplete(e);

				this._installer.Status = Status.Detected;

				if (e.Status >= 0 && this.Command.Display != Display.Full)
				{
					this.Engine.Log(LogLevel.Verbose, "Invoking automatic plan for non-interactive mode.");
					this.Engine.Plan(this.Command.Action);
				}
			}

			protected override void OnExecuteProgress(ExecuteProgressEventArgs e)
			{
				if (this._installer._canceled)
				{
					e.Result = WixToolset.Bootstrapper.Result.Cancel;
				}

				base.OnExecuteProgress(e);
			}

			protected override void OnPlanBegin(PlanBeginEventArgs e)
			{
				base.OnPlanBegin(e);
				this._installer.Status = Status.Planning;
			}

			protected override void OnPlanComplete(PlanCompleteEventArgs e)
			{
				base.OnPlanComplete(e);

				if (e.Status >= 0)
				{
					this.Engine.Apply(IntPtr.Zero);
				}
			}

			protected override void OnApplyBegin(ApplyBeginEventArgs e)
			{
				base.OnApplyBegin(e);
				this._installer.Status = Status.Applying;
			}

			protected override void OnApplyComplete(ApplyCompleteEventArgs e)
			{
				base.OnApplyComplete(e);
				this._installer.Result = new InstallResult(e.Status == 0, e.Restart == ApplyRestart.RestartRequired);
				this._installer.Status = Status.Completed;
			}
		}
	}
}
