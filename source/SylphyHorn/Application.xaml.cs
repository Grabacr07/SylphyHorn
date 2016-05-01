using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Livet;
using MetroRadiance.UI;
using MetroTrilithon.Lifetime;
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
		private readonly MultipleDisposable _compositeDisposable = new MultipleDisposable();
		private System.Windows.Forms.NotifyIcon _notifyIcon;

		internal IVdmHelper VdmHelper { get; private set; }

		internal HookService HookService { get; private set; }

		internal PinService PinService { get; private set; }

		internal UwpInteropService InteropService { get; private set; }

		static Application()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
#if !DEBUG
			var appInstance = new MetroTrilithon.Desktop.ApplicationInstance().AddTo(this);
			if (appInstance.IsFirst)
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

					var s = e.Args.Select(x => x.ToLower()).Any(x => x == "-s");
					this.ShowNotifyIcon(s);

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

			// とりあえずもう終了させるしかないもじゃ
			// 救えるパターンがあるなら救いたいけど方法わからんもじゃ
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
