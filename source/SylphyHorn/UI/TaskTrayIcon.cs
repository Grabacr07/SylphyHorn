﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MetroRadiance.Platform;
using SylphyHorn.Properties;
using SylphyHorn.Serialization;
using SylphyHorn.Services;
using WindowsDesktop;

namespace SylphyHorn.UI
{
	public class TaskTrayIcon : IDisposable
	{
		private Icon _icon;
		private readonly Icon _defaultIcon;
		private readonly Icon _lightIcon;
		private readonly TaskTrayIconItem[] _items;
		private NotifyIcon _notifyIcon;
		private DynamicInfoTrayIcon _infoIcon;

		public TaskTrayIcon(Icon icon, Icon lightIcon, TaskTrayIconItem[] items)
		{
			this._defaultIcon = icon;
			this._lightIcon = lightIcon;

			this._icon = WindowsTheme.SystemTheme.Current == Theme.Light ? this._lightIcon : this._defaultIcon;
			this._items = items;

			WindowsTheme.SystemTheme.Changed += this.OnSystemThemeChanged;
			WindowsTheme.Accent.Changed += this.OnAccentChanged;
			WindowsTheme.ColorPrevalence.Changed += this.OnColorPrevalenceChanged;
			VirtualDesktop.CurrentChanged += this.OnCurrentDesktopChanged;
		}

		public void Show()
		{
			if (this._notifyIcon != null) return;

			var menus = this._items
				.Where(x => x.CanDisplay())
				.Select(x => new MenuItem(x.Text, (sender, args) => x.ClickAction()))
				.ToArray();

			this._notifyIcon = new NotifyIcon()
			{
				Text = ProductInfo.Title,
				Icon = this._icon,
				Visible = true,
				ContextMenu = new ContextMenu(menus),
			};
		}

		public TaskTrayBaloon CreateBaloon() => new TaskTrayBaloon(this);

		internal void ShowBaloon(TaskTrayBaloon baloon)
		{
			if (this._notifyIcon == null) this.Show();

			this._notifyIcon.ShowBalloonTip(
				(int)baloon.Timespan.TotalMilliseconds,
				baloon.Title,
				baloon.Text,
				ToolTipIcon.None);
		}

		public void Reload(VirtualDesktop desktop = null)
		{
			if (Settings.General.TrayShowDesktop)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.UpdateWithDesktopInfo(desktop ?? VirtualDesktop.Current));
			}
			else if (this._icon != this._defaultIcon && this._icon != this._lightIcon)
			{
				this._infoIcon = null;

				this.ChangeIcon(WindowsTheme.SystemTheme.Current == Theme.Light
					? this._lightIcon
					: this._defaultIcon);
			}
		}

		private void UpdateWithDesktopInfo(VirtualDesktop currentDesktop)
		{
			var desktops = VirtualDesktop.GetDesktops();
			var currentDesktopIndex = Array.IndexOf(desktops, currentDesktop) + 1;
			var totalDesktopCount = desktops.Length;

			var text = string.Format(
				Resources.TaskTray_TooltipText_DesktopCount + "\n" + ProductInfo.Title,
				currentDesktopIndex,
				totalDesktopCount);
			this.ChangeText(text);

			if (this._infoIcon == null)
			{
				this._infoIcon = new DynamicInfoTrayIcon(
					WindowsTheme.SystemTheme.Current,
					WindowsTheme.ColorPrevalence.Current);
			}

			this.ChangeIcon(this._infoIcon.GetDesktopInfoIcon(currentDesktopIndex, totalDesktopCount));
		}

		private void OnCurrentDesktopChanged(object sender, VirtualDesktopChangedEventArgs e)
		{
			this.Reload(e.NewDesktop);
		}

		private void OnAccentChanged(object sender, System.Windows.Media.Color e)
		{
			var colorPrevalence = WindowsTheme.ColorPrevalence.Current;
			if (Settings.General.TrayShowDesktop && colorPrevalence)
			{
				this._infoIcon.UpdateBrush(WindowsTheme.SystemTheme.Current, colorPrevalence);
				VisualHelper.InvokeOnUIDispatcher(() => this.UpdateWithDesktopInfo(VirtualDesktop.Current));
			}
		}

		private void OnColorPrevalenceChanged(object sender, bool e)
		{
			if (Settings.General.TrayShowDesktop)
			{
				this._infoIcon.UpdateBrush(WindowsTheme.SystemTheme.Current, e);
				VisualHelper.InvokeOnUIDispatcher(() => this.UpdateWithDesktopInfo(VirtualDesktop.Current));
			}
		}

		private void OnSystemThemeChanged(object sender, Theme e)
		{
			if (Settings.General.TrayShowDesktop)
			{
				this._infoIcon.UpdateBrush(e, WindowsTheme.ColorPrevalence.Current);
				VisualHelper.InvokeOnUIDispatcher(() => this.UpdateWithDesktopInfo(VirtualDesktop.Current));
			}
			else
			{
				this.ChangeIcon(e == Theme.Light
					? this._lightIcon
					: this._defaultIcon);
			}
		}

		private void ChangeText(string newText)
		{
			this._notifyIcon.Text = newText;
		}

		private void ChangeIcon(Icon newIcon)
		{
			if (this._icon != this._defaultIcon && this._icon != this._lightIcon)
			{
				this._icon?.Dispose();
			}

			this._icon = newIcon;
			this._notifyIcon.Icon = newIcon;
		}

		public void Dispose()
		{
			WindowsTheme.SystemTheme.Changed -= this.OnSystemThemeChanged;
			WindowsTheme.Accent.Changed -= this.OnAccentChanged;
			WindowsTheme.ColorPrevalence.Changed -= this.OnColorPrevalenceChanged;
			VirtualDesktop.CurrentChanged -= this.OnCurrentDesktopChanged;

			this._notifyIcon?.Dispose();
			this._lightIcon?.Dispose();
			this._icon?.Dispose();
		}
	}

	public class TaskTrayIconItem
	{
		public string Text { get; }

		public Action ClickAction { get; }

		public Func<bool> CanDisplay { get; }

		public TaskTrayIconItem(string text, Action clickAction) : this(text, clickAction, () => true) { }

		public TaskTrayIconItem(string text, Action clickAction, Func<bool> canDisplay)
		{
			this.Text = text;
			this.ClickAction = clickAction;
			this.CanDisplay = canDisplay;
		}
	}

	public class TaskTrayBaloon
	{
		private readonly TaskTrayIcon _icon;

		public string Title { get; set; }

		public string Text { get; set; }

		public TimeSpan Timespan { get; set; }

		internal TaskTrayBaloon(TaskTrayIcon icon)
		{
			this._icon = icon;
		}

		public void Show()
		{
			this._icon.ShowBaloon(this);
		}
	}
}
