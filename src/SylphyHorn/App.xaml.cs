using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SylphyHorn;

public partial class App
{
	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		WPFUI.Theme.Watcher.Start();
		new UI.Preferences.Window().Show();
	}
}
