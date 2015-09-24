using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MetroRadiance.UI;
using SylphyHorn.Bootstrapper.UI;
using SylphyHorn.Bootstrapper.UI.Bindings;
using WixToolset.Bootstrapper;
using StartupEventArgs = System.Windows.StartupEventArgs;

namespace SylphyHorn.Bootstrapper
{
	sealed partial class Application
	{
		private readonly Installer _installer;

		public Application(Installer installer)
		{
			this._installer = installer;
			this.DispatcherUnhandledException += (sender, e) =>
			{
				e.Handled = true;
				installer.Log(LogLevel.Verbose, e.Exception.Message);
				installer.Quit(e.Exception.HResult);
			};
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);

			var window = new MainWindow
			{
				DataContext = new MainWindowViewModel(this._installer),
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
			};

			window.Show();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
		}
	}
}
