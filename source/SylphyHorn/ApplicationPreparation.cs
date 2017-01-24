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
				.Register(()=>settings.MoveLeft.ToShortcutKey(), (hWnd, keyDetector, key) => hWnd.MoveToLeft())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveLeftAndSwitch.ToShortcutKey(), (hWnd, keyDetector, key) =>
                    hWnd.MoveToLeft()?.Switch(hWnd, keyDetector, Settings.General.SmoothSwitch, settings.SwitchToLeft.ToShortcutKey(), settings.SwitchToRight.ToShortcutKey(), null))
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveRight.ToShortcutKey(), (hWnd, keyDetector, key) => hWnd.MoveToRight())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveRightAndSwitch.ToShortcutKey(), (hWnd, keyDetector, key) =>
                    hWnd.MoveToRight()?.Switch(hWnd, keyDetector, Settings.General.SmoothSwitch, settings.SwitchToLeft.ToShortcutKey(), settings.SwitchToRight.ToShortcutKey(), null))
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveNew.ToShortcutKey(), (hWnd, keyDetector, key) => hWnd.MoveToNew())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveNewAndSwitch.ToShortcutKey(), (hWnd, keyDetector, key) =>
                    hWnd.MoveToNew()?.Switch(hWnd, keyDetector, Settings.General.SmoothSwitch, settings.SwitchToLeft.ToShortcutKey(), settings.SwitchToRight.ToShortcutKey(), null))
				.AddTo(this._application);

			this._application.HookService
				.Register(
					() => settings.SwitchToLeft.ToShortcutKey(),
                    (hWnd, keyDetector, key) =>
                        VirtualDesktopService.GetLeft()?.Switch(IntPtr.Zero, keyDetector, Settings.General.SmoothSwitch, settings.SwitchToLeft.ToShortcutKey(), settings.SwitchToRight.ToShortcutKey(), key),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop,
                    () => false)
				.AddTo(this._application);

			this._application.HookService
				.Register(
					() => settings.SwitchToRight.ToShortcutKey(),
                    (hWnd, keyDetector, key) =>
                        VirtualDesktopService.GetRight()?.Switch(IntPtr.Zero, keyDetector, Settings.General.SmoothSwitch, settings.SwitchToLeft.ToShortcutKey(), settings.SwitchToRight.ToShortcutKey(), key),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop,
                    () => false)
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.CloseAndSwitchLeft.ToShortcutKey(), (hWnd, keyDetector, key) =>
                    VirtualDesktopService.CloseAndSwitchLeft(keyDetector, Settings.General.SmoothSwitch, settings.SwitchToLeft.ToShortcutKey(), settings.SwitchToRight.ToShortcutKey()))
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.CloseAndSwitchRight.ToShortcutKey(), (hWnd, keyDetector, key) =>
                    VirtualDesktopService.CloseAndSwitchRight(keyDetector, Settings.General.SmoothSwitch, settings.SwitchToLeft.ToShortcutKey(), settings.SwitchToRight.ToShortcutKey()))
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.Pin.ToShortcutKey(), (hWnd, keyDetector, key) => hWnd.Pin())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.Unpin.ToShortcutKey(), (hWnd, keyDetector, key) => hWnd.Unpin())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.TogglePin.ToShortcutKey(), (hWnd, keyDetector, key) => hWnd.TogglePin())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.PinApp.ToShortcutKey(), (hWnd, keyDetector, key) => hWnd.PinApp())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.UnpinApp.ToShortcutKey(), (hWnd, keyDetector, key) => hWnd.UnpinApp())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.TogglePinApp.ToShortcutKey(), (hWnd, keyDetector, key) => hWnd.TogglePinApp())
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
