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
using VDMHelperCLR.Common;
using WindowsDesktop;
using SylphyHorn.Interop;
using SylphyHorn.UI;
using SylphyHorn.UI.Bindings;
using MessageBox = System.Windows.MessageBox;

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

		internal IVdmHelper VdmHelper { get; private set; }
		internal HookService HookService { get; private set; }
		internal UwpInteropService InteropService { get; private set; }

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
				if (VirtualDesktop.IsSupported)
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

					this.ShowTaskTrayIcon();

					this.VdmHelper = VdmHelperFactory.CreateInstance().AddTo(this);
					this.VdmHelper.Init();
					this.HookService = new HookService(this.VdmHelper).AddTo(this);
					this.InteropService = new UwpInteropService(this.HookService, Settings.General).AddTo(this);
					this.RegisterActions();

					NotificationService.Instance.AddTo(this);
					WallpaperService.Instance.AddTo(this);

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


		private void RegisterActions()
		{
			var settings = Settings.ShortcutKey;

			this.HookService
				.Register(settings.MoveLeft.ToShortcutKey(), hWnd => hWnd.MoveToLeft(this.VdmHelper))
				.AddTo(this);

			this.HookService
				.Register(settings.MoveLeftAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToLeft(this.VdmHelper)?.Switch())
				.AddTo(this);

			this.HookService
				.Register(settings.MoveRight.ToShortcutKey(), hWnd => hWnd.MoveToRight(this.VdmHelper))
				.AddTo(this);

			this.HookService
				.Register(settings.MoveRightAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToRight(this.VdmHelper)?.Switch())
				.AddTo(this);

			this.HookService
				.Register(settings.MoveNew.ToShortcutKey(), hWnd => hWnd.MoveToNew(this.VdmHelper))
				.AddTo(this);

			this.HookService
				.Register(settings.MoveNewAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToNew(this.VdmHelper)?.Switch())
				.AddTo(this);

			this.HookService
				.Register(
					settings.SwitchToLeft.ToShortcutKey(),
					_ => VirtualDesktopService.GetLeft()?.Switch(),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop)
				.AddTo(this);

			this.HookService
				.Register(
					settings.SwitchToRight.ToShortcutKey(),
					_ => VirtualDesktopService.GetRight()?.Switch(),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop)
				.AddTo(this);

			this.HookService
				.Register(settings.Pin.ToShortcutKey(), hWnd => hWnd.Pin())
				.AddTo(this);

			this.HookService
				.Register(settings.Unpin.ToShortcutKey(), hWnd => hWnd.Unpin())
				.AddTo(this);

			this.HookService
				.Register(settings.TogglePin.ToShortcutKey(), hWnd => hWnd.TogglePin())
				.AddTo(this);
		}

		private void ShowTaskTrayIcon()
		{
			const string iconUri = "pack://application:,,,/SylphyHorn;Component/Assets/tasktray.ico";

			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri)) return;

			var icon = IconHelper.GetIconFromResource(uri);
			var menus = new[]
			{
				new TaskTrayIconItem("&Settings (S)", () => this.ShowSettings(), () => Args.CanSettings),
				new TaskTrayIconItem("E&xit (X)", () => this.Shutdown()),
			};

			var taskTrayIcon = new TaskTrayIcon(icon, menus);
			taskTrayIcon.Show();
			taskTrayIcon.AddTo(this);
		}

		private void ShowSettings()
		{
			using (this.HookService.Suspend())
			{
				var window = new SettingsWindow { DataContext = new SettingsWindowViewModel(this.HookService), };
				window.ShowDialog();
			}
		}

		private static void SetupShortcut()
		{

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

			try
			{
				var message = string.Format(messageFormat,
					DateTimeOffset.Now,
					sender,
					Environment.GetCommandLineArgs().Skip(1).JoinString(" "),
					exception);

				Debug.WriteLine(message);
				File.AppendAllText(path, message);
				MessageBox.Show(message, "Unexpected error occurred");
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			if (Args != null)
			{

				var restartNum = Args.Restarted ?? 0;
				if ((DateTime.Now - Process.GetCurrentProcess().StartTime).TotalMinutes > 3)
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
				}
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
