using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MetroTrilithon.Linq;
using MetroTrilithon.Serialization;

namespace SylphyHorn.Serialization
{
	public class SerializableProperty<T> : SerializablePropertyBase<T>
	{
		public SerializableProperty(string key, ISerializationProvider provider) : base(key, provider) { }
		public SerializableProperty(string key, ISerializationProvider provider, T defaultValue) : base(key, provider, defaultValue) { }
	}


	public class ShortcutkeyProperty : SerializablePropertyBase<int[]>
	{
		private const string _empryString = "(none)";

		public ShortcutkeyProperty(string key, ISerializationProvider provider) : base(key, provider) { }
		public ShortcutkeyProperty(string key, ISerializationProvider provider, params int[] defaultValue) : base(key, provider, defaultValue) { }

		protected override object SerializeCore(int[] value)
		{
			if (value == null || value.Length == 0) return _empryString;

			return value
				.Select(x => x.ToString(CultureInfo.InvariantCulture))
				.JoinString(",");
		}

		protected override int[] DeserializeCore(object value)
		{
			var data = value as string;
			if (data == null) return base.DeserializeCore(value);

			if (string.IsNullOrEmpty(data)) return null;
			if (string.Equals(data, _empryString, StringComparison.OrdinalIgnoreCase)) return Array.Empty<int>();

			return data.Split(',')
				.Select(x => int.Parse(x))
				.ToArray();
		}
	}
}
