using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MetroTrilithon.Serialization;

namespace SylphyHorn.Models
{
	public static class Providers
	{
		public static string LocalFilePath { get; } = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			ProductInfo.Company,
			ProductInfo.Title);

		public static ISerializationProvider Local { get; } = new FileSettingsProvider(LocalFilePath);
	}


	public static class GeneralSettings
	{
		public static SerializableProperty<bool> LoopDesktop { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Local);

		private static string GetKey([CallerMemberName] string caller = "")
		{
			return nameof(ShortcutSettings) + "." + caller;
		}
	}


	public static class ShortcutSettings
	{
		public static SerializableProperty<ShortcutKey?> OpenDesktopSelector { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local);

		public static SerializableProperty<ShortcutKey?> MoveLeft { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local);

		public static SerializableProperty<ShortcutKey?> MoveLeftAndSwitch { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local, ShortcutKey.Create(Key.Left, Key.LeftCtrl, Key.LeftAlt, Key.LWin));

		public static SerializableProperty<ShortcutKey?> MoveRight { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local);

		public static SerializableProperty<ShortcutKey?> MoveRightAndSwitch { get; }
			= new SerializableProperty<ShortcutKey?>(GetKey(), Providers.Local, ShortcutKey.Create(Key.Right, Key.LeftCtrl, Key.LeftAlt, Key.LWin));


		private static string GetKey([CallerMemberName] string caller = "")
		{
			return nameof(ShortcutSettings) + "." + caller;
		}
	}
}
