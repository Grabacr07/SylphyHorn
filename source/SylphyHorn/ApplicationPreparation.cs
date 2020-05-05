using System;
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
				.Register(()=>settings.MoveDesktopLeft.ToShortcutKey(), hWnd => VirtualDesktopMoveService.MoveLeft())
				.AddTo(this._disposable);
			
			this._hookService
				.Register(()=>settings.MoveDesktopRight.ToShortcutKey(), hWnd => VirtualDesktopMoveService.MoveRight())
				.AddTo(this._disposable);
			
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

			this._hookService
				.Register(() => settings.RenameCurrent.ToShortcutKey(), _ => VirtualDesktopService.RaiseRenameEvent())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.SwitchTo1.ToShortcutKey(), _ => VirtualDesktopService.Get(1)?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.SwitchTo2.ToShortcutKey(), _ => VirtualDesktopService.Get(2)?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.SwitchTo3.ToShortcutKey(), _ => VirtualDesktopService.Get(3)?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.SwitchTo4.ToShortcutKey(), _ => VirtualDesktopService.Get(4)?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.SwitchTo5.ToShortcutKey(), _ => VirtualDesktopService.Get(5)?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.SwitchTo6.ToShortcutKey(), _ => VirtualDesktopService.Get(6)?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.SwitchTo7.ToShortcutKey(), _ => VirtualDesktopService.Get(7)?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.SwitchTo8.ToShortcutKey(), _ => VirtualDesktopService.Get(8)?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.SwitchTo9.ToShortcutKey(), _ => VirtualDesktopService.Get(9)?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.SwitchTo10.ToShortcutKey(), _ => VirtualDesktopService.Get(10)?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveTo1.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(1)))
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveTo2.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(2)))
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveTo3.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(3)))
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveTo4.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(4)))
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveTo5.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(5)))
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveTo6.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(6)))
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveTo7.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(7)))
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveTo8.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(8)))
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveTo9.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(9)))
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveTo10.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(10)))
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveAndSwitchTo1.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(1))?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveAndSwitchTo2.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(2))?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveAndSwitchTo3.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(3))?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveAndSwitchTo4.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(4))?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveAndSwitchTo5.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(5))?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveAndSwitchTo6.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(6))?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveAndSwitchTo7.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(7))?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveAndSwitchTo8.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(8))?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveAndSwitchTo9.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(9))?.Switch())
				.AddTo(this._disposable);

			this._hookService
				.Register(() => settings.MoveAndSwitchTo10.ToShortcutKey(), hWnd => hWnd.MoveTo(VirtualDesktopService.Get(10))?.Switch())
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
