using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using WindowsDesktop;
using Livet;
using MetroTrilithon.Lifetime;
using StatefulModel;
using SylphyHorn.Models;

namespace SylphyHorn
{
	sealed partial class Application : IDisposableHolder
	{
		private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();
		private HookService hookService;

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
				this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
				this.DispatcherUnhandledException += (sender, args) =>
				{
					ReportException(sender, args.Exception);
					args.Handled = true;
				};

				DispatcherHelper.UIDispatcher = this.Dispatcher;
				this.MainWindow = new MainWindow();
				this.MainWindow.Show();

				if (VirtualDesktop.IsSupported)
				{
					this.hookService = new HookService();
				}
				else { }
			}
#if !DEBUG
			else
			{
				appInstance.SendCommandLineArgs(e.Args);
				this.Shutdown();
			}
#endif

			base.OnStartup(e);
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
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			// とりあえずもう終了させるしかないもじゃ
			// 救えるパターンがあるなら救いたいけど方法わからんもじゃ
			Current.Shutdown();
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
