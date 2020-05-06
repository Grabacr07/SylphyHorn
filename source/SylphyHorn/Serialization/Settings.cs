using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SylphyHorn.Serialization
{
	public static class Settings
	{
		public static GeneralSettings General { get; set; } = new GeneralSettings(LocalSettingsProvider.Instance);

		public static ShortcutKeySettings ShortcutKey { get; set;  } = new ShortcutKeySettings(LocalSettingsProvider.Instance);
	}
}
