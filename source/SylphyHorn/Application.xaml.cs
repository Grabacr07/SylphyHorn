using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using WindowsDesktop;
using Livet;
using MetroRadiance;
using MetroTrilithon.Lifetime;
using StatefulModel;
using SylphyHorn.Models;
using SylphyHorn.ViewModels;
using SylphyHorn.Views;
using MessageBox = System.Windows.MessageBox;

namespace SylphyHorn
{
	sealed partial class Application : IDisposableHolder
	{
		private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private TransparentWindow transparentWindow;
		private HookService hookService;
		private NotificationService notificationService;

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

					DispatcherHelper.UIDispatcher = this.Dispatcher;

					ThemeService.Current.Initialize(
						this,
						GeneralSettings.Theme.Value ?? (VisualHelper.IsDarkTheme() ? Theme.Dark : Theme.Light), 
						GeneralSettings.AccentColor.Value ?? Accent.Blue);

					this.transparentWindow = new TransparentWindow();
					this.transparentWindow.Show();

					if (GeneralSettings.AccentColor.Value == null)
					{
						VisualHelper.ForceChangeAccent(VisualHelper.GetWindowsAccentColor());
					}

					this.ShowNotifyIcon();
					this.hookService = new HookService().AddTo(this);
					this.notificationService = new NotificationService().AddTo(this);

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
		

		private void ShowNotifyIcon()
		{
			const string iconUri = "pack://application:,,,/SylphyHorn;Component/Assets/tasktray.ico";

			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri)) return;

			var streamResourceInfo = GetResourceStream(uri);
			if (streamResourceInfo == null) return;

			using (var stream = streamResourceInfo.Stream)
			{
				this.notifyIcon = new System.Windows.Forms.NotifyIcon
				{
					Text = ProductInfo.Title,
					Icon = new System.Drawing.Icon(stream, new System.Drawing.Size(16, 16)),
					Visible = true,
					ContextMenu = new System.Windows.Forms.ContextMenu(new[]
					{
						new System.Windows.Forms.MenuItem("&Settings (S)", (sender, args) => this.ShowSettings()),
						new System.Windows.Forms.MenuItem("E&xit (X)", (sender, args) => this.Shutdown()),
					}),
				};
				this.notifyIcon.AddTo(this);
			}
		}

		private void ShowSettings()
		{
			using (this.hookService.Suspend())
			{
				var window = new SettingsWindow { DataContext = new SettingsWindowViewModel(), };
				window.ShowDialog();
			}
		}


		#region IDisposable members

		ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

		void IDisposable.Dispose()
		{
			this.compositeDisposable.Dispose();
		}

		#endregion
	}
}
