using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
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
		public static bool IsWindowsBridge { get; }
#if APPX
			= true;
#else
			= false;
#endif

		public static CommandLineArgs Args { get; private set; }

		static Application()
		{
			AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
		}

		private readonly MultipleDisposable _compositeDisposable = new MultipleDisposable();

		internal HookService HookService { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			Args = new CommandLineArgs(e.Args);

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
					this.DispatcherUnhandledException += this.HandleDispatcherUnhandledException;

					DispatcherHelper.UIDispatcher = this.Dispatcher;

					LocalSettingsProvider.Instance.LoadAsync().Wait();

					Settings.General.Culture.Subscribe(x => ResourceService.Current.ChangeCulture(x)).AddTo(this);
					ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);
					UI.Themes.ThemeService2.Current.Register(this);

					this.HookService = new HookService().AddTo(this);

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

		private void HandleDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
		{
			LoggingService.Instance.Register(args.Exception);
			args.Handled = true;
		}

		private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			if ((DateTime.Now - Process.GetCurrentProcess().StartTime).TotalMinutes >= 3)
			{
				// 3 分以上生きてたら安定稼働と見做して再起動させる
				Restart();
			}
			else
			{
				// ToDo: Exception dialog
			}
		}

		private static void Restart()
		{
			if (Args != null)
			{
				var restartCount = Args.Restarted ?? 0;

				Process.Start(
					Environment.GetCommandLineArgs()[0],
					Args.Options
						.Where(x => x.Key != Args.GetKey(nameof(CommandLineArgs.Restarted)))
						.Concat(EnumerableEx.Return(Args.CreateOption(nameof(CommandLineArgs.Restarted), (restartCount + 1).ToString())))
						.Select(x => x.ToString())
						.JoinString(" "));
			}
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
