using MetroRadiance.Platform;
using MetroRadiance.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace SylphyHorn.UI.Themes
{
	// Source from https://github.com/Grabacr07/MetroRadiance/blob/master/source/MetroRadiance.Core/Platform/WindowsTheme.cs
	public abstract class WindowsThemeValue<T>
	{
		private event EventHandler<T> _changedEvent;
		private readonly HashSet<EventHandler<T>> _handlers = new HashSet<EventHandler<T>>();
		private ListenerWindow _listenerWindow;
		private T _current;
		private bool _hasValidValue;

		public T Current
		{
			get
			{
				if (!this._hasValidValue)
				{
					this._current = this.GetValue();
					this._hasValidValue = true;
				}

				return this._current;
			}
			set
			{
				this._current = value;
				this._hasValidValue = true;
			}
		}

		public event EventHandler<T> Changed
		{
			add { this.Add(value); }
			remove { this.Remove(value); }
		}


		[EditorBrowsable(EditorBrowsableState.Never)]
		public IDisposable RegisterListener(Action<T> callback)
		{
			EventHandler<T> handler = (sender, e) => callback?.Invoke(e);
			this.Changed += handler;

			return Disposable.Create(() => this.Changed -= handler);
		}

		private void Add(EventHandler<T> listener)
		{
			if (this._handlers.Add(listener))
			{
				this._changedEvent += listener;

				if (this._listenerWindow == null)
				{
					this._listenerWindow = new ListenerWindow(this.GetType().Name, this.WndProc);
					this._listenerWindow.Show();
				}
			}
		}

		private void Remove(EventHandler<T> listener)
		{
			if (this._handlers.Remove(listener))
			{
				this._changedEvent -= listener;

				if (this._handlers.Count == 0)
				{
					this._listenerWindow?.Close();
					this._listenerWindow = null;
				}
			}
		}

		internal void Update(T data)
		{
			this.Current = data;
			this._changedEvent?.Invoke(this, data);
		}

		internal abstract T GetValue();

		internal abstract IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);

		private class ListenerWindow : TransparentWindow
		{
			private readonly HwndSourceHook _hook;

			public ListenerWindow(string name, HwndSourceHook hook)
			{
				this.Name = $"{name} listener window";
				this._hook = hook;
			}

			protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
			{
				var result = this._hook(hwnd, msg, wParam, lParam, ref handled);
				return handled ? result : base.WndProc(hwnd, msg, wParam, lParam, ref handled);
			}
		}
	}

	public sealed class HighContrastValue : WindowsThemeValue<bool>
	{
		internal override bool GetValue() => System.Windows.SystemParameters.HighContrast;

		internal override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)MetroRadiance.Interop.Win32.WindowsMessages.WM_THEMECHANGED)
			{
				this.Update(this.GetValue());
				handled = true;
			}

			return IntPtr.Zero;
		}
	}

	public sealed class ColorPrevalenceValue : WindowsThemeValue<bool>
	{
		internal override bool GetValue()
		{
			const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
			const string valueName = "ColorPrevalence";

			return Registry.GetValue(keyName, valueName, null) as int? != 0;
		}

		internal override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)MetroRadiance.Interop.Win32.WindowsMessages.WM_SETTINGCHANGE)
			{
				var systemParmeter = Marshal.PtrToStringAuto(lParam);
				if (systemParmeter == "ImmersiveColorSet")
				{
					this.Update(this.GetValue());
					handled = true;
				}
			}

			return IntPtr.Zero;
		}
	}

	public sealed class TransparencyValue : WindowsThemeValue<bool>
	{
		internal override bool GetValue()
		{
			const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
			const string valueName = "EnableTransparency";

			return Registry.GetValue(keyName, valueName, null) as int? != 0;
		}

		internal override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)MetroRadiance.Interop.Win32.WindowsMessages.WM_SETTINGCHANGE)
			{
				var systemParmeter = Marshal.PtrToStringAuto(lParam);
				if (systemParmeter == "ImmersiveColorSet")
				{
					this.Update(this.GetValue());
					handled = true;
				}
			}

			return IntPtr.Zero;
		}
	}

	public static class WindowsTheme2
	{
		public static HighContrastValue HighContrast { get; } = new HighContrastValue();

		public static ColorPrevalenceValue ColorPrevalence { get; } = new ColorPrevalenceValue();

		public static TransparencyValue Transparency { get; } = new TransparencyValue();
	}
}