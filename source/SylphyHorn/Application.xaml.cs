using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Livet;
using MetroRadiance.UI;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Linq;
using StatefulModel;
using SylphyHorn.Serialization;
using SylphyHorn.Services;

namespace SylphyHorn
{
	sealed partial class Application : IDisposableHolder
	{
		public static CommandLineArgs Args { get; private set; }

		static Application()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);
		}

		private readonly MultipleDisposable _compositeDisposable = new MultipleDisposable();

		internal HookService HookService { get; private set; }

		internal UwpInteropService InteropService { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			Args
#if APPX
				= new CommandLineArgs();
#else
				= new CommandLineArgs(e.Args);
#endif

			if (Args.Setup)
			{
				SetupShortcut();
			}

#if !DEBUG
			var appInstance = new MetroTrilithon.Desktop.ApplicationInstance().AddTo(this);
			if (appInstance.IsFirst || Args.Restarted.HasValue)
#endif
			{
				if (WindowsDesktop.VirtualDesktop.IsSupported)
				{
					this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
					this.DispatcherUnhandledException += (sender, args) =>
					{
						ReportException(sender, args.Exception);
						args.Handled = true;
					};

					DispatcherHelper.UIDispatcher = this.Dispatcher;

					LocalSettingsProvider.Instance.LoadAsync().Wait();
					LocalSettingsProvider.Instance.AddTo(this);

					ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);

					this.HookService = new HookService().AddTo(this);
					this.InteropService = new UwpInteropService(this.HookService, Settings.General).AddTo(this);

					var preparation = new ApplicationPreparation(this);
					preparation.ShowTaskTrayIcon();
					preparation.RegisterActions();

					NotificationService.Instance.AddTo(this);
					WallpaperService.Instance.AddTo(this);

#if !DEBUG
					appInstance.CommandLineArgsReceived += (sender, message) =>
					{
						var args = new CommandLineArgs(message.CommandLineArgs);
						if (args.Setup) SetupShortcut();
					};
#endif

					base.OnStartup(e);
				}
				else
				{
					MessageBox.Show("This applications is supported only Windows 10 (build 10240).", "Not supported", MessageBoxButton.OK, MessageBoxImage.Stop);
					this.Shutdown();
				}
			}
#if !DEBUG
			else
			{
				appInstance.SendCommandLineArgs(e.Args);
				this.Shutdown();
			}
#endif
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			((IDisposable)this).Dispose();
		}

		private static void SetupShortcut()
		{
			var startup = new Startup();
			if (!startup.IsExists)
			{
				startup.Create();
			}
		}

		private static void ReportException(object sender, Exception exception)
		{
			#region const

			const string messageFormat = @"
===========================================================
ERROR, date = {0}, sender = {1},
args = {2}
{3}
";
			const string path = "error.log";

			#endregion

			var message = "";

			try
			{
				message = string.Format(
					messageFormat,
					DateTimeOffset.Now,
					sender,
					Environment.GetCommandLineArgs().Skip(1).JoinString(" "),
					exception);

				Debug.WriteLine(message);
				File.AppendAllText(path, message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			try
			{
				if (Args != null)
				{
					var restartNum = Args.Restarted ?? 0;
					if ((DateTime.Now - Process.GetCurrentProcess().StartTime).TotalMinutes >= 3)
					{
						// 3 分以上生きてたら安定稼働と見做して、とりあえず再起動させる
						Process.Start(
							Environment.GetCommandLineArgs()[0],
							Args.Options
								.Where(x => x.Key != Args.GetKey(nameof(CommandLineArgs.Restarted)))
								.Concat(EnumerableEx.Return(Args.CreateOption(nameof(CommandLineArgs.Restarted), (restartNum + 1).ToString())))
								.Select(x => x.ToString())
								.JoinString(" "));
					}
					else
					{
						// ToDo: 例外ダイアログ
						MessageBox.Show(message, "Unexpected error occurred");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			Current.Shutdown();
		}

		#region IDisposable members

		ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this._compositeDisposable;

		void IDisposable.Dispose()
		{
			this._compositeDisposable.Dispose();
		}

		#endregion
	}
}
