using System;
using System.Collections.Generic;
using System.Linq;
using MetroTrilithon.Lifetime;
using SylphyHorn.Interop;
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
				.Register(settings.MoveLeft.ToShortcutKey(), hWnd => hWnd.MoveToLeft())
				.AddTo(this._application);

			this._application.HookService
				.Register(settings.MoveLeftAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToLeft()?.Switch())
				.AddTo(this._application);

			this._application.HookService
				.Register(settings.MoveRight.ToShortcutKey(), hWnd => hWnd.MoveToRight())
				.AddTo(this._application);

			this._application.HookService
				.Register(settings.MoveRightAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToRight()?.Switch())
				.AddTo(this._application);

			this._application.HookService
				.Register(settings.MoveNew.ToShortcutKey(), hWnd => hWnd.MoveToNew())
				.AddTo(this._application);

			this._application.HookService
				.Register(settings.MoveNewAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToNew()?.Switch())
				.AddTo(this._application);

			this._application.HookService
				.Register(
					settings.SwitchToLeft.ToShortcutKey(),
					_ => VirtualDesktopService.GetLeft()?.Switch(),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop)
				.AddTo(this._application);

			this._application.HookService
				.Register(
					settings.SwitchToRight.ToShortcutKey(),
					_ => VirtualDesktopService.GetRight()?.Switch(),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop)
				.AddTo(this._application);

			this._application.HookService
				.Register(settings.Pin.ToShortcutKey(), hWnd => hWnd.Pin())
				.AddTo(this._application);

			this._application.HookService
				.Register(settings.Unpin.ToShortcutKey(), hWnd => hWnd.Unpin())
				.AddTo(this._application);

			this._application.HookService
				.Register(settings.TogglePin.ToShortcutKey(), hWnd => hWnd.TogglePin())
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
				new TaskTrayIconItem("&Settings (S)", () => this.ShowSettings(), () => Application.Args.CanSettings),
				new TaskTrayIconItem("E&xit (X)", () => this._application.Shutdown()),
			};

			var taskTrayIcon = new TaskTrayIcon(icon, menus);
			taskTrayIcon.Show();
			taskTrayIcon.AddTo(this._application);
		}

		private void ShowSettings()
		{
			using (this._application.HookService.Suspend())
			{
				var window = new SettingsWindow
				{
					DataContext = new SettingsWindowViewModel(this._application.HookService),
				};
				window.ShowDialog();
			}
		}
	}
}
