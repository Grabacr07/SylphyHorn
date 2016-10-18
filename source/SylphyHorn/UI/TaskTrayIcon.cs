using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SylphyHorn.Properties;

namespace SylphyHorn.UI
{
	public class TaskTrayIcon : IDisposable
	{
		private readonly Icon _icon;
		private readonly TaskTrayIconItem[] _items;
		private NotifyIcon _notifyIcon;

		public string Text { get; set; } = ProductInfo.Title;

		public TaskTrayIcon(Icon icon, TaskTrayIconItem[] items)
		{
			this._icon = icon;
			this._items = items;
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

		public void Dispose()
		{
			this._notifyIcon?.Dispose();
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
