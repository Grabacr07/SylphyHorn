using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MetroTrilithon.Linq;

namespace SylphyHorn.Models
{
	/// <summary>
	/// Represents a shortcut key ([modifer key(s)] + [key] style).
	/// </summary>
	public struct ShortcutKey
	{
		public Key Key { get; set; }
		public Key[] Modifiers { get; set; }

		internal ICollection<Key> ModifiersInternal { get; }
		
		public ShortcutKey(Key key, params Key[] modifiers) : this()
		{
			this.Key = key;
			this.Modifiers = modifiers;
			this.ModifiersInternal = modifiers;
		}

		internal ShortcutKey(Key key, ICollection<Key> modifiers) : this()
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
				// ReSharper disable NonReadonlyMemberInGetHashCode
				var hashCode = (this.ModifiersInternal ?? this.Modifiers)?.GetHashCode() ?? 0;
				hashCode = (hashCode * 397) ^ (int)this.Key;
				return hashCode;
				// ReSharper restore NonReadonlyMemberInGetHashCode
			}
		}

		public override string ToString()
		{
			return (this.ModifiersInternal ?? this.Modifiers)
				.OrderBy(x => x)
				.Select(x => x + " + ")
				.Concat(EnumerableEx.Return(this.Key == Key.None ? "" : this.Key.ToString()))
				.JoinString("");
		}

		public static bool operator ==(ShortcutKey key1, ShortcutKey key2)
		{
			return key1.Key == key2.Key
				   && (Equals(key1.ModifiersInternal ?? key1.Modifiers, key2.ModifiersInternal ?? key2.Modifiers));
		}

		public static bool operator !=(ShortcutKey key1, ShortcutKey key2)
		{
			return !(key1 == key2);
		}

		private static bool Equals(ICollection<Key> keys1, ICollection<Key> keys2)
		{
			return keys1.Count == keys2.Count && !keys1.Except(keys2).Any();
		}
	}
}
