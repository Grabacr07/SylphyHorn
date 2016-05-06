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
using SylphyHorn.Properties;
using SylphyHorn.Serialization;
using SylphyHorn.Services;
using SylphyHorn.ViewModels;
using SylphyHorn.Views;
using VDMHelperCLR.Common;
using WindowsDesktop;
using MessageBox = System.Windows.MessageBox;

namespace SylphyHorn
{
	sealed partial class Application : IDisposableHolder
	{
		private const string CanOpenSettingsArg = "-s";
		private const string RestartedArg = "-restarted";

		private readonly MultipleDisposable _compositeDisposable = new MultipleDisposable();
		private System.Windows.Forms.NotifyIcon _notifyIcon;

		internal IVdmHelper VdmHelper { get; private set; }

		internal HookService HookService { get; private set; }

		internal PinService PinService { get; private set; }

		internal UwpInteropService InteropService { get; private set; }

		internal static Dictionary<string, string> CommandLineArgs { get; private set; }

		static Application()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			// -Key (Value = null) or -Key=Value
			CommandLineArgs = e.Args
				.Select(x => x.Split(new[] { '=', }, 2))
				.GroupBy(xs => xs[0], (k, ys) => ys.Last()) // 重複の場合は後ろの引数を優先
				.ToDictionary(xs => xs[0], xs => xs.Length == 1 ? null : xs[1], StringComparer.OrdinalIgnoreCase);

#if !DEBUG
			var appInstance = new MetroTrilithon.Desktop.ApplicationInstance().AddTo(this);
			if (appInstance.IsFirst || CommandLineArgs.ContainsKey(RestartedArg))
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

					//var pinnedapps = WindowsDesktop.Interop.VirtualDesktopInteropHelper.GetVirtualDesktopPinnedApps();
					//Debug.WriteLine($"IsPinnedWindow: {pinnedapps.IsPinnedWindow(MetroRadiance.Interop.Win32.User32.GetForegroundWindow())}");
					//Debug.WriteLine($"IsPinnedApp   : {pinnedapps.IsPinnedApp(MetroRadiance.Interop.Win32.User32.GetForegroundWindow())}");

					DispatcherHelper.UIDispatcher = this.Dispatcher;

					LocalSettingsProvider.Instance.LoadAsync().Wait();
					LocalSettingsProvider.Instance.AddTo(this);

					ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);

					this.ShowNotifyIcon(CommandLineArgs.ContainsKey(CanOpenSettingsArg));

					this.VdmHelper = VdmHelperFactory.CreateInstance().AddTo(this);
					this.VdmHelper.Init();
					this.PinService = new PinService(this.VdmHelper).AddTo(this);
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
					() => Settings.General.OverrideWindowsDefaultKeyCombination)
				.AddTo(this);

			this.HookService
				.Register(
					settings.SwitchToRight.ToShortcutKey(),
					_ => VirtualDesktopService.GetRight()?.Switch(),
					() => Settings.General.OverrideWindowsDefaultKeyCombination)
				.AddTo(this);

			this.HookService
				.Register(settings.Pin.ToShortcutKey(), hWnd => this.PinService.Register(hWnd))
				.AddTo(this);

			this.HookService
				.Register(settings.Unpin.ToShortcutKey(), hWnd => this.PinService.Unregister(hWnd))
				.AddTo(this);

			this.HookService
				.Register(settings.TogglePin.ToShortcutKey(), hWnd => this.PinService.ToggleRegister(hWnd))
				.AddTo(this);
		}

		private static void ReportException(object sender, Exception exception)
		{
			#region const

			const string messageFormat = @"
===========================================================
ERROR, date = {0}, sender = {1},
{2}
";
			const string path = "error.log";

			#endregion

			// ToDo: 例外ダイアログ

			try
			{
				var message = string.Format(messageFormat, DateTimeOffset.Now, sender, exception);

				Debug.WriteLine(message);
				File.AppendAllText(path, message);
				MessageBox.Show(message, "Unexpected error occurred");
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			// 仕方ないので 3 回だけ再起動のチャンスを与えてやって、殺す
			if (CommandLineArgs != null)
			{
				int restartNum;
				if (!int.TryParse(CommandLineArgs.ContainsKey(RestartedArg) ? CommandLineArgs[RestartedArg] : null, out restartNum)) restartNum = 0;
				if (restartNum < 3)
				{
					Process.Start(
						Environment.GetCommandLineArgs()[0],
						CommandLineArgs
							.Where(x => x.Key != RestartedArg)
							.Select(x => x.Value == null ? x.Key : $"{x.Key}={x.Value}")
							.JoinString(" ") + $" {RestartedArg}={++restartNum}");
				}
			}
			
			Current.Shutdown();
		}


		private void ShowNotifyIcon(bool canOpenSettings)
		{
			const string iconUri = "pack://application:,,,/SylphyHorn;Component/Assets/tasktray.ico";

			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri)) return;

			var streamResourceInfo = GetResourceStream(uri);
			if (streamResourceInfo == null) return;

			using (var stream = streamResourceInfo.Stream)
			{
				var menus = new List<System.Windows.Forms.MenuItem>();
				if (canOpenSettings) menus.Add(new System.Windows.Forms.MenuItem("&Settings (S)", (sender, args) => this.ShowSettings()));
				menus.Add(new System.Windows.Forms.MenuItem("E&xit (X)", (sender, args) => this.Shutdown()));

				this._notifyIcon = new System.Windows.Forms.NotifyIcon
				{
					Text = ProductInfo.Title,
					Icon = new System.Drawing.Icon(stream, new System.Drawing.Size(16, 16)),
					Visible = true,
					ContextMenu = new System.Windows.Forms.ContextMenu(menus.ToArray()),
				};
				this._notifyIcon.AddTo(this);
			}
		}

		private void ShowSettings()
		{
			using (this.HookService.Suspend())
			{
				var window = new SettingsWindow { DataContext = new SettingsWindowViewModel(this.HookService), };
				window.ShowDialog();
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
