using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace SylphyHorn.Models
{
	public struct ShortcutKey
	{
		public Key Key { get; set; }
		public Key[] Modifiers { get; set; }

		[NonSerialized]
		internal ICollection<Key> ModifiersInternal;
		
		public bool Equals(ShortcutKey other)
		{
			return Equals(this.ModifiersInternal, other.ModifiersInternal) && this.Key == other.Key && Equals(this.Modifiers, other.Modifiers);
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
				var hashCode = this.ModifiersInternal?.GetHashCode() ?? 0;
				hashCode = (hashCode * 397) ^ (int)this.Key;
				hashCode = (hashCode * 397) ^ (this.Modifiers?.GetHashCode() ?? 0);
				return hashCode;
				// ReSharper restore NonReadonlyMemberInGetHashCode
			}
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

		public static ShortcutKey Create(Key key, params Key[] modifiers)
		{
			return new ShortcutKey
			{
				Key = key,
				Modifiers = modifiers,
				ModifiersInternal = modifiers,
			};
		}

		internal static ShortcutKey Create(Key key, ICollection<Key> modifiers)
		{
			return new ShortcutKey
			{
				Key = key,
				ModifiersInternal = modifiers,
			};
		}

		private static bool Equals(ICollection<Key> keys1, ICollection<Key> keys2)
		{
			return keys1.Count == keys2.Count && !keys1.Except(keys2).Any();
		}
	}
}