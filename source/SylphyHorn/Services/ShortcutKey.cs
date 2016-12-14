using System;
using System.Collections.Generic;
using System.Linq;
using MetroTrilithon.Linq;

#if WINDOWS_UWP
using Windows.System;
#else
using VirtualKey = System.Windows.Forms.Keys;
#endif

namespace SylphyHorn.Services
{
	/// <summary>
	/// Represents a shortcut key ([modifer key(s)] + [key] style).
	/// </summary>
	public struct ShortcutKey
	{
		public VirtualKey Key { get; }
		public VirtualKey[] Modifiers { get; }

		internal ICollection<VirtualKey> ModifiersInternal { get; }

		public ShortcutKey(VirtualKey key, params VirtualKey[] modifiers)
		{
			this.Key = key;
			this.Modifiers = modifiers;
			this.ModifiersInternal = modifiers;
		}

		internal ShortcutKey(VirtualKey key, ICollection<VirtualKey> modifiers) : this()
		{
			this.Key = key;
			this.ModifiersInternal = modifiers;
		}

		public bool Equals(ShortcutKey other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is ShortcutKey && this.Equals((ShortcutKey)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (this.ModifiersInternal ?? this.Modifiers)?.GetHashCode() ?? 0;
				hashCode = (hashCode * 397) ^ (int)this.Key;
				return hashCode;
			}
		}

		public override string ToString()
		{
			return (this.ModifiersInternal ?? this.Modifiers ?? Enumerable.Empty<VirtualKey>())
				.OrderBy(x => x)
				.Select(x => x + " + ")
				.Concat(EnumerableEx.Return(this.Key == VirtualKey.None ? "" : this.Key.ToString()))
				.JoinString("");
		}

		public static bool operator ==(ShortcutKey key1, ShortcutKey key2)
		{
			return key1.Key == key2.Key
				&& Equals(
					key1.ModifiersInternal ?? key1.Modifiers ?? Array.Empty<VirtualKey>(),
					key2.ModifiersInternal ?? key2.Modifiers ?? Array.Empty<VirtualKey>());
		}

		public static bool operator !=(ShortcutKey key1, ShortcutKey key2)
		{
			return !(key1 == key2);
		}

		private static bool Equals(ICollection<VirtualKey> key1, ICollection<VirtualKey> key2)
		{
			return key1.Count == key2.Count && !key1.Except(key2).Any();
		}


		public static readonly ShortcutKey None = new ShortcutKey(VirtualKey.None);
	}
}
