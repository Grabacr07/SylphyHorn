using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using WindowsDesktop;
using WindowsDesktop.Interop;
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

		    Func<IntPtr, IShortcutKey, SmoothSwitchData> buildSwitchData =
		        (hWnd, key) =>
		            new SmoothSwitchData(
		                hWnd,
		                Settings.General.SmoothSwitch ? SwitchType.Smooth : SwitchType.Quick,
		                this._application.HookService.KeyDetector,
		                settings.SwitchToLeft.ToShortcutKey(),
		                settings.SwitchToRight.ToShortcutKey(),
		                key);


			this._application.HookService
				.Register(()=>settings.MoveLeft.ToShortcutKey(), (hWnd, key) => hWnd.MoveToLeft().Execute(this._application.HookService.KeyDetector))
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveLeftAndSwitch.ToShortcutKey(), (hWnd, key) =>
                    hWnd.MoveToLeft()?.Switch(buildSwitchData(hWnd, key), AdjacentDesktop.LeftDirection, Settings.General.LoopDesktop).Execute(this._application.HookService.KeyDetector))
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveRight.ToShortcutKey(), (hWnd, key) => hWnd.MoveToRight().Execute(this._application.HookService.KeyDetector))
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveRightAndSwitch.ToShortcutKey(), (hWnd, key) =>
                    hWnd.MoveToRight()?.Switch(buildSwitchData(hWnd, key), AdjacentDesktop.RightDirection, Settings.General.LoopDesktop).Execute(this._application.HookService.KeyDetector))
                .AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveNew.ToShortcutKey(), (hWnd, key) => hWnd.MoveToNew().Execute(this._application.HookService.KeyDetector))
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.MoveNewAndSwitch.ToShortcutKey(), (hWnd, key) =>
                    hWnd.MoveToNew()?.Switch(buildSwitchData(hWnd, key), AdjacentDesktop.Jump, false).Execute(this._application.HookService.KeyDetector))
                .AddTo(this._application);

			this._application.HookService
				.Register(
					() => settings.SwitchToLeft.ToShortcutKey(),
                    (hWnd, key) => VirtualDesktopService.GetLeft()?
                        .Switch(buildSwitchData(IntPtr.Zero, key), AdjacentDesktop.LeftDirection, Settings.General.LoopDesktop)
                        .Execute(this._application.HookService.KeyDetector),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop,
                    () => !this._application.HookService.KeyDetector.IsSuspendedUntilKeyPress)
				.AddTo(this._application);

			this._application.HookService
				.Register(
					() => settings.SwitchToRight.ToShortcutKey(),
                    (hWnd, key) => VirtualDesktopService.GetRight()?
                        .Switch(buildSwitchData(IntPtr.Zero, key), AdjacentDesktop.RightDirection, Settings.General.LoopDesktop)
                        .Execute(this._application.HookService.KeyDetector),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop,
                    () => !this._application.HookService.KeyDetector.IsSuspendedUntilKeyPress)
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.CloseAndSwitchLeft.ToShortcutKey(), (hWnd, key) =>
                    VirtualDesktopService.CloseAndSwitchLeft(buildSwitchData(hWnd, key)))
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.CloseAndSwitchRight.ToShortcutKey(), (hWnd, key) =>
                    VirtualDesktopService.CloseAndSwitchRight(buildSwitchData(hWnd, key)))
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.Pin.ToShortcutKey(), (hWnd, key) => hWnd.Pin())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.Unpin.ToShortcutKey(), (hWnd, key) => hWnd.Unpin())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.TogglePin.ToShortcutKey(), (hWnd, key) => hWnd.TogglePin())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.PinApp.ToShortcutKey(), (hWnd, key) => hWnd.PinApp())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.UnpinApp.ToShortcutKey(), (hWnd, key) => hWnd.UnpinApp())
				.AddTo(this._application);

			this._application.HookService
				.Register(() => settings.TogglePinApp.ToShortcutKey(), (hWnd, key) => hWnd.TogglePinApp())
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
