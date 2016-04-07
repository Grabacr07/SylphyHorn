using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SylphyHorn.Services
{
	public class ShortcutKeyPressedEventArgs
	{
		public ShortcutKey ShortcutKey { get; }

		public bool Handled { get; set; }

		public ShortcutKeyPressedEventArgs(ShortcutKey shortcutKey)
		{
			this.ShortcutKey = shortcutKey;
		}

		internal ShortcutKeyPressedEventArgs(Keys key, ICollection<Keys> modifiers)
		{
			this.ShortcutKey = new ShortcutKey(key, modifiers);
		}
	}
}
