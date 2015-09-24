using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace SylphyHorn.Serialization
{
	public class ShortcutKeySettings : SettingsHost
	{
		private readonly ISerializationProvider _provider;

		public ShortcutKeySettings(ISerializationProvider provider)
		{
			this._provider = provider;
		}

		public ShortcutkeyProperty MoveLeft => this.Cache(key => new ShortcutkeyProperty(key, this._provider));

		public ShortcutkeyProperty MoveLeftAndSwitch => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveLeftAndSwitchDefaultValue));

		public ShortcutkeyProperty MoveRight => this.Cache(key => new ShortcutkeyProperty(key, this._provider));

		public ShortcutkeyProperty MoveRightAndSwitch => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveRightAndSwitchDefaultValue));

		public ShortcutkeyProperty MoveNew => this.Cache(key => new ShortcutkeyProperty(key, this._provider));

		public ShortcutkeyProperty MoveNewAndSwitch => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveNewAndSwitchDefaultValue));

		public ShortcutkeyProperty SwitchToLeft => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchToLeftDefaultValue));

		public ShortcutkeyProperty SwitchToRight => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchToRightDefaultValue));

		public ShortcutkeyProperty Pin => this.Cache(key => new ShortcutkeyProperty(key, this._provider, PinDefaultValue));

		public ShortcutkeyProperty Unpin => this.Cache(key => new ShortcutkeyProperty(key, this._provider, UnpinDefaultValue));

		#region default values

		private static int[] SwitchToLeftDefaultValue { get; } =
			{
				037, // <=
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] SwitchToRightDefaultValue { get; } =
			{
				039, // =>
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] MoveLeftAndSwitchDefaultValue { get; } =
			{
				037, // <=
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveRightAndSwitchDefaultValue { get; } =
			{
				039, // =>
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveNewAndSwitchDefaultValue { get; } =
			{
				068, // D
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] PinDefaultValue { get; } =
			{
				080, // P
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] UnpinDefaultValue { get; } =
			{
				080, // P
				162, // Left Ctrl
				164, // Left Alt
				160, // Left Shift
				091, // Left Windows
			};

		#endregion
	}
}
