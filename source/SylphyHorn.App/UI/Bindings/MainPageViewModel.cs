using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SylphyHorn.Serialization;
using SylphyHorn.Services;
using Windows.System;

namespace SylphyHorn.UI.Bindings
{
	public class MainPageViewModel : BindableBase
	{
		private string _notificationDuration = Settings.General.NotificationDuration.Value.ToString();

		public GeneralSettings GeneralSettings { get; } = Settings.General;

		public ShortcutKeySettings ShortcutKeySettings { get; } = Settings.ShortcutKey;

		public int[] CreateNewShortcutKey { get; } = new ShortcutKey(VirtualKey.D, VirtualKey.LeftWindows, VirtualKey.LeftControl).ToSerializable();

		public string NotificationDuration
		{
			get { return this._notificationDuration; }
			set
			{
				if (this._notificationDuration != value)
				{
					// UI に何も通知しない手抜き実装
					int num;
					if (int.TryParse(value, out num))
					{
						Settings.General.NotificationDuration.Value = num;
					}

					this._notificationDuration = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public void GetEngine()
		{
			var downloader = new EngineDownloader();
			downloader.Launch();
		}
	}
}
