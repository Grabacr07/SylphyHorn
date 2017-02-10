using System;
using System.Collections.Generic;
using System.Linq;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Threading.Tasks;
using SylphyHorn.Interop;
using SylphyHorn.Properties;
using SylphyHorn.Serialization;
using SylphyHorn.Services;
using SylphyHorn.UI;
using SylphyHorn.UI.Bindings;

namespace SylphyHorn
{
	public class ApplicationPreparation
	{
		private readonly Application _application;

		public ApplicationPreparation(Application application)
		{
			this._application = application;
		}

		public void RegisterActions()
		{
			var settings = Settings.ShortcutKey;

			this._application.HookService
				.Register(()=>settings.MoveLeft.ToShortcutKey(), hWnd => hWnd.MoveToLeft())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveLeftAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToLeft()?.Switch())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveRight.ToShortcutKey(), hWnd => hWnd.MoveToRight())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveRightAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToRight()?.Switch())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveNew.ToShortcutKey(), hWnd => hWnd.MoveToNew())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveNewAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToNew()?.Switch())
				.AddTo(this._application);

			this._application.HookService
				.Register(
					() => settings.SwitchToLeft.ToShortcutKey(),
					_ => VirtualDesktopService.GetLeft()?.Switch(),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop)
				.AddTo(this._application);

			this._application.HookService
				.Register(
					() => settings.SwitchToRight.ToShortcutKey(),
					_ => VirtualDesktopService.GetRight()?.Switch(),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop)
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.CloseAndSwitchLeft.ToShortcutKey(), _ => VirtualDesktopService.CloseAndSwitchLeft())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.CloseAndSwitchRight.ToShortcutKey(), _ => VirtualDesktopService.CloseAndSwitchRight())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.Pin.ToShortcutKey(), hWnd => hWnd.Pin())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.Unpin.ToShortcutKey(), hWnd => hWnd.Unpin())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.TogglePin.ToShortcutKey(), hWnd => hWnd.TogglePin())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.PinApp.ToShortcutKey(), hWnd => hWnd.PinApp())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.UnpinApp.ToShortcutKey(), hWnd => hWnd.UnpinApp())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.TogglePinApp.ToShortcutKey(), hWnd => hWnd.TogglePinApp())
				.AddTo(this._application);
		}


		public void ShowTaskTrayIcon()
		{
			const string iconUri = "pack://application:,,,/SylphyHorn;Component/Assets/tasktray.ico";

			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri)) return;

			var icon = IconHelper.GetIconFromResource(uri);
			var menus = new[]
			{
				new TaskTrayIconItem(Resources.TaskTray_Menu_Settings, () => this.ShowSettings(), () => Application.Args.CanSettings),
				new TaskTrayIconItem(Resources.TaskTray_Menu_Exit, () => this._application.Shutdown()),
			};

			var taskTrayIcon = new TaskTrayIcon(icon, menus);
			taskTrayIcon.Show();
			taskTrayIcon.AddTo(this._application);

			if (Settings.General.FirstTime)
			{
				var baloon = taskTrayIcon.CreateBaloon();
				baloon.Title = ProductInfo.Title;
				baloon.Text = Resources.TaskTray_FirstTimeMessage;
				baloon.Timespan = TimeSpan.FromMilliseconds(5000);
				baloon.Show();

				Settings.General.FirstTime.Value = false;
				LocalSettingsProvider.Instance.SaveAsync().Forget();
			}
		}

		private void ShowSettings()
		{
			using (this._application.HookService.Suspend())
			{
				if (SettingsWindow.Instance != null)
				{
					SettingsWindow.Instance.Activate();
				}
				else
				{
					SettingsWindow.Instance = new SettingsWindow
					{
						DataContext = new SettingsWindowViewModel(this._application.HookService),
					};

					SettingsWindow.Instance.ShowDialog();
					SettingsWindow.Instance = null;
				}
			}
		}
	}
}
