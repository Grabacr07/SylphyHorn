using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SylphyHorn.Models
{
	public class ShortcutKeyPressedEventArgs
	{
		public ShortcutKey ShortcutKey { get; }

		public bool Handled { get; set; }

		public ShortcutKeyPressedEventArgs(ShortcutKey shortcutKey)
		{
			this.ShortcutKey = shortcutKey;
		}

		internal ShortcutKeyPressedEventArgs(Key key, ICollection<Key> modifiers)
		{
			this.ShortcutKey = new ShortcutKey(key, modifiers);
		}
	}
}
