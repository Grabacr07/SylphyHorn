using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SylphyHorn.Serialization
{
	public static class Settings
	{
		public static GeneralSettings General { get; } = new GeneralSettings(LocalStorageProvider.Instance);

		public static ShortcutKeySettings ShortcutKey { get; } = new ShortcutKeySettings(LocalStorageProvider.Instance);
	}
}
