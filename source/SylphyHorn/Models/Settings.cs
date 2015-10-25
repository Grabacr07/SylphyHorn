using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MetroRadiance;
using MetroTrilithon.Serialization;

namespace SylphyHorn.Models
{
	public static class Providers
	{
		public static string LocalFilePath { get; } = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			ProductInfo.Company,
			ProductInfo.Title,
			"Settings.xaml");

		public static ISerializationProvider Local { get; } = new FileSettingsProvider(LocalFilePath);
	}


	public static class GeneralSettings
	{
		public static SerializableProperty<bool> LoopDesktop { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Local);

		public static SerializableProperty<bool> NotificationWhenSwitchedDesktop { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Local, true);

		public static SerializableProperty<Theme?> Theme { get; }
			= new SerializableProperty<Theme?>(GetKey(), Providers.Local); 

		public static SerializableProperty<Accent?> AccentColor { get; }
			= new SerializableProperty<Accent?>(GetKey(), Providers.Local);

        public static SerializableProperty<bool> OverrideOSDefaultKeyCombination { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false);

        private static string GetKey([CallerMemberName] string caller = "")
		{
			return nameof(GeneralSettings) + "." + caller;
		}
	}


	public static class ShortcutSettings
	{
		public static SerializableProperty<ShortcutKey?> OpenDesktopSelector { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local);

		public static SerializableProperty<ShortcutKey?> MoveLeft { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local);

		public static SerializableProperty<ShortcutKey?> MoveLeftAndSwitch { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local, new ShortcutKey(Key.Left, Key.LeftCtrl, Key.LeftAlt, Key.LWin));

		public static SerializableProperty<ShortcutKey?> MoveRight { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local);

		public static SerializableProperty<ShortcutKey?> MoveRightAndSwitch { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local, new ShortcutKey(Key.Right, Key.LeftCtrl, Key.LeftAlt, Key.LWin));

		public static SerializableProperty<ShortcutKey?> MoveNew { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local);

		public static SerializableProperty<ShortcutKey?> MoveNewAndSwitch { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local, new ShortcutKey(Key.D, Key.LeftCtrl, Key.LeftAlt, Key.LWin));

        public static SerializableProperty<ShortcutKey?> SwitchToLeft { get; }
            = new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local, new ShortcutKey(Key.Left, Key.LeftCtrl, Key.LWin));

        public static SerializableProperty<ShortcutKey?> SwitchToRight { get; }
            = new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local, new ShortcutKey(Key.Right, Key.LeftCtrl, Key.LWin));

        public static SerializableProperty<ShortcutKey?> PinWindows { get; }
            = new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local, new ShortcutKey(Key.P, Key.LeftCtrl, Key.LeftAlt, Key.LWin));


        private static string GetKey([CallerMemberName] string caller = "")
		{
			return nameof(ShortcutSettings) + "." + caller;
		}
	}
}
