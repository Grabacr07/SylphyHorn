﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace SylphyHorn.Serialization
{
	public class GeneralSettings : SettingsHost
	{
		private readonly ISerializationProvider _provider;

		public GeneralSettings(ISerializationProvider provider)
		{
			this._provider = provider;
		}

		public SerializableProperty<bool> LoopDesktop => this.Cache(key => new SerializableProperty<bool>(key, this._provider));

		public SerializableProperty<bool> NotificationWhenSwitchedDesktop => this.Cache(key => new SerializableProperty<bool>(key, this._provider, true));

		public SerializableProperty<int> NotificationDuration => this.Cache(key => new SerializableProperty<int>(key, this._provider, 2500));

		public SerializableProperty<bool> ChangeBackgroundEachDesktop => this.Cache(key => new SerializableProperty<bool>(key, this._provider));

		public SerializableProperty<string> DesktopBackgroundFolderPath => this.Cache(key => new SerializableProperty<string>(key, this._provider));

		public SerializableProperty<bool> OverrideWindowsDefaultKeyCombination => this.Cache(key => new SerializableProperty<bool>(key, this._provider, false));

		public SerializableProperty<bool> SuspendKeyDetection => this.Cache(key => new SerializableProperty<bool>(key, this._provider));

		public SerializableProperty<bool> FirstTime => this.Cache(key => new SerializableProperty<bool>(key, this._provider, true));

		public SerializableProperty<string> Culture => this.Cache(key => new SerializableProperty<string>(key, this._provider));

		public SerializableProperty<uint> Placement => this.Cache(key => new SerializableProperty<uint>(key, this._provider, 5 /* Center */));

		public SerializableProperty<uint> Display => this.Cache(key => new SerializableProperty<uint>(key, this._provider, 0));

		public SerializableProperty<bool> TrayShowDesktop => this.Cache(key => new SerializableProperty<bool>(key, this._provider, false));

		public SerializableProperty<string> DesktopName1 => this.Cache(key => new SerializableProperty<string>(key, this._provider, null));

		public SerializableProperty<string> DesktopName2 => this.Cache(key => new SerializableProperty<string>(key, this._provider, null));

		public SerializableProperty<string> DesktopName3 => this.Cache(key => new SerializableProperty<string>(key, this._provider, null));

		public SerializableProperty<string> DesktopName4 => this.Cache(key => new SerializableProperty<string>(key, this._provider, null));

		public SerializableProperty<string> DesktopName5 => this.Cache(key => new SerializableProperty<string>(key, this._provider, null));

		public SerializableProperty<string> DesktopName6 => this.Cache(key => new SerializableProperty<string>(key, this._provider, null));

		public SerializableProperty<string> DesktopName7 => this.Cache(key => new SerializableProperty<string>(key, this._provider, null));

		public SerializableProperty<string> DesktopName8 => this.Cache(key => new SerializableProperty<string>(key, this._provider, null));

		public SerializableProperty<string> DesktopName9 => this.Cache(key => new SerializableProperty<string>(key, this._provider, null));

		public SerializableProperty<string> DesktopName10 => this.Cache(key => new SerializableProperty<string>(key, this._provider, null));
	}
}
