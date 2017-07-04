using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SylphyHorn.Properties;
using WindowsDesktop;
using SylphyHorn.Interop;
using SylphyHorn.Serialization;

namespace SylphyHorn.UI
{
	public class TaskTrayIcon : IDisposable
	{
		private Icon _icon;
        private readonly Icon _defaultIcon;
		private readonly TaskTrayIconItem[] _items;
		private NotifyIcon _notifyIcon;
        private DynamicInfoTrayIcon _infoIcon;

		public string Text { get; set; } = ProductInfo.Title;

		public TaskTrayIcon(Icon icon, TaskTrayIconItem[] items)
		{
            _defaultIcon = icon;

			this._icon = icon;
			this._items = items;

            VirtualDesktop.CurrentChanged += OnCurrentDesktopChanged;
		}

        public void Show()
		{
			var menus = this._items
				.Where(x => x.CanDisplay())
				.Select(x => new MenuItem(x.Text, (sender, args) => x.ClickAction()))
				.ToArray();

			this._notifyIcon = new NotifyIcon()
			{
				Text = this.Text,
				Icon = this._icon,
				Visible = true,
				ContextMenu = new ContextMenu(menus),
			};
		}

		public TaskTrayBaloon CreateBaloon() => new TaskTrayBaloon(this);

		internal void ShowBaloon(TaskTrayBaloon baloon)
		{
			this._notifyIcon.ShowBalloonTip(
				(int)baloon.Timespan.TotalMilliseconds,
				baloon.Title,
				baloon.Text,
				ToolTipIcon.None);

		}

        public void UpdateWithDesktopInfo(VirtualDesktop currentDesktop)
        {
            var desktops = VirtualDesktop.GetDesktops();
            var currentDesktopIndex = Array.IndexOf(desktops, currentDesktop) + 1;
            var totalDesktopCount = desktops.Length;

            if (_infoIcon == null)
            {
                _infoIcon = new DynamicInfoTrayIcon(totalDesktopCount);
            }

            ChangeIcon(_infoIcon.GetDesktopInfoIcon(currentDesktopIndex, totalDesktopCount));
        }

        private void OnCurrentDesktopChanged(object sender, VirtualDesktopChangedEventArgs e)
        {
            if (Settings.General.TrayShowDesktop)
            {
                UpdateWithDesktopInfo(e.NewDesktop);
            }
            else if (_icon != _defaultIcon)
            {
                _infoIcon?.Dispose();
                _infoIcon = null;

                ChangeIcon(_defaultIcon);
            }
        }

        private void ChangeIcon(Icon newIcon)
        {
            if (_icon != _defaultIcon)
            {
                _icon?.Dispose();
            }

            _icon = newIcon;
            _notifyIcon.Icon = newIcon;
        }

        public void Dispose()
		{
			this._notifyIcon?.Dispose();
			this._icon?.Dispose();

            VirtualDesktop.CurrentChanged -= OnCurrentDesktopChanged;
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
