﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WindowsDesktop;
using MetroTrilithon.Lifetime;
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
		private readonly HookService _hookService;
		private readonly Action _shutdownAction;
		private readonly IDisposableHolder _disposable;
		private TaskTrayIcon _taskTrayIcon;

		public event Action VirtualDesktopInitialized;

		public event Action VirtualDesktopInitializationCanceled;

		public event Action<Exception> VirtualDesktopInitializationFailed;

		public ApplicationPreparation(HookService hookService, Action shutdownAction, IDisposableHolder disposable)
		{
			this._hookService = hookService;
			this._shutdownAction = shutdownAction;
			this._disposable = disposable;
		}

		public void RegisterActions()
		{
			var settings = Settings.ShortcutKey;

			this._hookService
				.Register(()=>settings.MoveLeft.ToShortcutKey(), hWnd => hWnd.MoveToLeft())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveLeftAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToLeft()?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveRight.ToShortcutKey(), hWnd => hWnd.MoveToRight())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveRightAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToRight()?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveNew.ToShortcutKey(), hWnd => hWnd.MoveToNew())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveNewAndSwitch.ToShortcutKey(), hWnd => hWnd.MoveToNew()?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(
					() => settings.SwitchToLeft.ToShortcutKey(),
					_ => VirtualDesktopService.GetLeft()?.Switch(),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop)
				.AddTo(this._disposable);

			this._hookService
				.Register(
					() => settings.SwitchToRight.ToShortcutKey(),
					_ => VirtualDesktopService.GetRight()?.Switch(),
					() => Settings.General.OverrideWindowsDefaultKeyCombination || Settings.General.ChangeBackgroundEachDesktop)
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.CloseAndSwitchLeft.ToShortcutKey(), _ => VirtualDesktopService.CloseAndSwitchLeft())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.CloseAndSwitchRight.ToShortcutKey(), _ => VirtualDesktopService.CloseAndSwitchRight())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.Pin.ToShortcutKey(), hWnd => hWnd.Pin())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.Unpin.ToShortcutKey(), hWnd => hWnd.Unpin())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.TogglePin.ToShortcutKey(), hWnd => hWnd.TogglePin())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.PinApp.ToShortcutKey(), hWnd => hWnd.PinApp())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.UnpinApp.ToShortcutKey(), hWnd => hWnd.UnpinApp())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.TogglePinApp.ToShortcutKey(), hWnd => hWnd.TogglePinApp())
				.AddTo(this._disposable);
		}

		public TaskTrayIcon CreateTaskTrayIcon()
		{
			if (this._taskTrayIcon == null)
			{
				const string iconUri = "pack://application:,,,/SylphyHorn;Component/.assets/tasktray.ico";

				if (!Uri.TryCreate(iconUri, UriKind.Absolute, out var uri)) return null;

				var icon = IconHelper.GetIconFromResource(uri);
				var menus = new[]
				{
					new TaskTrayIconItem(Resources.TaskTray_Menu_Settings, ShowSettings, () => Application.Args.CanSettings),
					new TaskTrayIconItem(Resources.TaskTray_Menu_Exit, this._shutdownAction),
				};

				this._taskTrayIcon = new TaskTrayIcon(icon, menus);
			}

			return this._taskTrayIcon;

			void ShowSettings()
			{
				using (this._hookService.Suspend())
				{
					if (SettingsWindow.Instance != null)
					{
						SettingsWindow.Instance.Activate();
					}
					else
					{
						SettingsWindow.Instance = new SettingsWindow
						{
							DataContext = new SettingsWindowViewModel(this._hookService),
						};

						SettingsWindow.Instance.ShowDialog();
						SettingsWindow.Instance = null;
					}
				}
			}
		}

		public TaskTrayBaloon CreateFirstTimeBaloon()
		{
			var baloon = this.CreateTaskTrayIcon().CreateBaloon();
			baloon.Title = ProductInfo.Title;
			baloon.Text = Resources.TaskTray_FirstTimeMessage;
			baloon.Timespan = TimeSpan.FromMilliseconds(5000);

			return baloon;
		}

		public void PrepareVirtualDesktop()
		{
			var provider = new VirtualDesktopProvider()
			{
				ComInterfaceAssemblyPath = Path.Combine(Directories.LocalAppData.FullName, "assemblies"),
			};

			VirtualDesktop.Provider = provider;
			VirtualDesktop.Provider.Initialize().ContinueWith(Continue, TaskScheduler.FromCurrentSynchronizationContext());

			void Continue(Task t)
			{
				switch (t.Status)
				{
					case TaskStatus.RanToCompletion:
						this.VirtualDesktopInitialized?.Invoke();
						break;

					case TaskStatus.Canceled:
						this.VirtualDesktopInitializationCanceled?.Invoke();
						break;

					case TaskStatus.Faulted:
						this.VirtualDesktopInitializationFailed?.Invoke(t.Exception);
						break;
				}
			}
		}
	}
}
