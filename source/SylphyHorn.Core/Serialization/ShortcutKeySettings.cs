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

		public ShortcutkeyProperty SwitchToLeft => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchToLeftDefaultValue));

		public ShortcutkeyProperty SwitchToRight => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchToRightDefaultValue));

		public ShortcutkeyProperty SwitchToPrevious => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchToPreviousDefaultValue));

		public ShortcutkeyProperty MoveLeft => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveLeftDefaultValue));

		public ShortcutkeyProperty MoveLeftAndSwitch => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveLeftAndSwitchDefaultValue));

		public ShortcutkeyProperty MoveRight => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveRightDefaultValue));

		public ShortcutkeyProperty MoveRightAndSwitch => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveRightAndSwitchDefaultValue));

		public ShortcutkeyProperty MoveNew => this.Cache(key => new ShortcutkeyProperty(key, this._provider));

		public ShortcutkeyProperty MoveNewAndSwitch => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveNewAndSwitchDefaultValue));

		public ShortcutkeyProperty CloseAndSwitchLeft => this.Cache(key => new ShortcutkeyProperty(key, this._provider, CloseAndSwitchLeftDefaultValue));

		public ShortcutkeyProperty CloseAndSwitchRight => this.Cache(key => new ShortcutkeyProperty(key, this._provider));

		public ShortcutkeyProperty Pin => this.Cache(key => new ShortcutkeyProperty(key, this._provider));

		public ShortcutkeyProperty Unpin => this.Cache(key => new ShortcutkeyProperty(key, this._provider));

		public ShortcutkeyProperty TogglePin => this.Cache(key => new ShortcutkeyProperty(key, this._provider, TogglePinDefaultValue));

		public ShortcutkeyProperty PinApp => this.Cache(key => new ShortcutkeyProperty(key, this._provider));

		public ShortcutkeyProperty UnpinApp => this.Cache(key => new ShortcutkeyProperty(key, this._provider));

		public ShortcutkeyProperty TogglePinApp => this.Cache(key => new ShortcutkeyProperty(key, this._provider));

		public ShortcutkeyProperty RenameCurrent => this.Cache(key => new ShortcutkeyProperty(key, this._provider, RenameCurrentDefaultValue));

		public ShortcutkeyProperty SwapDesktopLeft => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwapDesktopLeftDefaultValue));

		public ShortcutkeyProperty SwapDesktopRight => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwapDesktopRightDefaultValue));

		public ShortcutkeyProperty SwitchTo1 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchTo1DefaultValue));

		public ShortcutkeyProperty SwitchTo2 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchTo2DefaultValue));

		public ShortcutkeyProperty SwitchTo3 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchTo3DefaultValue));

		public ShortcutkeyProperty SwitchTo4 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchTo4DefaultValue));

		public ShortcutkeyProperty SwitchTo5 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchTo5DefaultValue));

		public ShortcutkeyProperty SwitchTo6 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchTo6DefaultValue));

		public ShortcutkeyProperty SwitchTo7 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchTo7DefaultValue));

		public ShortcutkeyProperty SwitchTo8 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchTo8DefaultValue));

		public ShortcutkeyProperty SwitchTo9 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchTo9DefaultValue));

		public ShortcutkeyProperty SwitchTo10 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, SwitchTo10DefaultValue));

		public ShortcutkeyProperty MoveTo1 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveTo1DefaultValue));

		public ShortcutkeyProperty MoveTo2 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveTo2DefaultValue));

		public ShortcutkeyProperty MoveTo3 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveTo3DefaultValue));

		public ShortcutkeyProperty MoveTo4 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveTo4DefaultValue));

		public ShortcutkeyProperty MoveTo5 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveTo5DefaultValue));

		public ShortcutkeyProperty MoveTo6 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveTo6DefaultValue));

		public ShortcutkeyProperty MoveTo7 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveTo7DefaultValue));

		public ShortcutkeyProperty MoveTo8 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveTo8DefaultValue));

		public ShortcutkeyProperty MoveTo9 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveTo9DefaultValue));

		public ShortcutkeyProperty MoveTo10 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveTo10DefaultValue));

		public ShortcutkeyProperty MoveAndSwitchTo1 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveAndSwitchTo1DefaultValue));

		public ShortcutkeyProperty MoveAndSwitchTo2 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveAndSwitchTo2DefaultValue));

		public ShortcutkeyProperty MoveAndSwitchTo3 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveAndSwitchTo3DefaultValue));

		public ShortcutkeyProperty MoveAndSwitchTo4 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveAndSwitchTo4DefaultValue));

		public ShortcutkeyProperty MoveAndSwitchTo5 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveAndSwitchTo5DefaultValue));

		public ShortcutkeyProperty MoveAndSwitchTo6 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveAndSwitchTo6DefaultValue));

		public ShortcutkeyProperty MoveAndSwitchTo7 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveAndSwitchTo7DefaultValue));

		public ShortcutkeyProperty MoveAndSwitchTo8 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveAndSwitchTo8DefaultValue));

		public ShortcutkeyProperty MoveAndSwitchTo9 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveAndSwitchTo9DefaultValue));

		public ShortcutkeyProperty MoveAndSwitchTo10 => this.Cache(key => new ShortcutkeyProperty(key, this._provider, MoveAndSwitchTo10DefaultValue));

		#region default values

		private static int[] MoveLeftDefaultValue { get; } =
			{
				037, // <=
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveRightDefaultValue { get; } =
			{
				039, // =>
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] SwapDesktopLeftDefaultValue { get; } =
			{
				037, // <=
				160, // Left Shift
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] SwapDesktopRightDefaultValue { get; } =
			{
				039, // =>
				160, // Left Shift
				164, // Left Alt
				091, // Left Windows
			};
		
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

		private static int[] SwitchToPreviousDefaultValue { get; } =
			{
				088, // X
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

		private static int[] CloseAndSwitchLeftDefaultValue { get; } =
			{
				115, // F4
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] TogglePinDefaultValue { get; } =
			{
				080, // P
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] RenameCurrentDefaultValue { get; } =
			{
				082, // R
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] SwitchTo1DefaultValue { get; } =
			{
				049, // 1
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] SwitchTo2DefaultValue { get; } =
			{
				050, // 2
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] SwitchTo3DefaultValue { get; } =
			{
				051, // 3
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] SwitchTo4DefaultValue { get; } =
			{
				052, // 4
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] SwitchTo5DefaultValue { get; } =
			{
				053, // 5
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] SwitchTo6DefaultValue { get; } =
			{
				054, // 6
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] SwitchTo7DefaultValue { get; } =
			{
				055, // 7
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] SwitchTo8DefaultValue { get; } =
			{
				056, // 8
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] SwitchTo9DefaultValue { get; } =
			{
				057, // 9
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] SwitchTo10DefaultValue { get; } =
			{
				048, // 0
				162, // Left Ctrl
				091, // Left Windows
			};

		private static int[] MoveTo1DefaultValue { get; } =
			{
				049, // 1
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveTo2DefaultValue { get; } =
			{
				050, // 2
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveTo3DefaultValue { get; } =
			{
				051, // 3
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveTo4DefaultValue { get; } =
			{
				052, // 4
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveTo5DefaultValue { get; } =
			{
				053, // 5
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveTo6DefaultValue { get; } =
			{
				054, // 6
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveTo7DefaultValue { get; } =
			{
				055, // 7
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveTo8DefaultValue { get; } =
			{
				056, // 8
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveTo9DefaultValue { get; } =
			{
				057, // 9
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveTo10DefaultValue { get; } =
			{
				048, // 0
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveAndSwitchTo1DefaultValue { get; } =
			{
				049, // 1
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveAndSwitchTo2DefaultValue { get; } =
			{
				050, // 2
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveAndSwitchTo3DefaultValue { get; } =
			{
				051, // 3
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveAndSwitchTo4DefaultValue { get; } =
			{
				052, // 4
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveAndSwitchTo5DefaultValue { get; } =
			{
				053, // 5
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveAndSwitchTo6DefaultValue { get; } =
			{
				054, // 6
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveAndSwitchTo7DefaultValue { get; } =
			{
				055, // 7
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveAndSwitchTo8DefaultValue { get; } =
			{
				056, // 8
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveAndSwitchTo9DefaultValue { get; } =
			{
				057, // 9
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		private static int[] MoveAndSwitchTo10DefaultValue { get; } =
			{
				048, // 0
				162, // Left Ctrl
				164, // Left Alt
				091, // Left Windows
			};

		#endregion
	}
}
