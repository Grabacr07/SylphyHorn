using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MetroTrilithon.Linq;
using MetroTrilithon.Serialization;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SylphyHorn.Serialization
{
	public class SerializableProperty<T> : SerializablePropertyBase<T>
	{
		public SerializableProperty(string key, ISerializationProvider provider) : base(key, provider) { }
		public SerializableProperty(string key, ISerializationProvider provider, T defaultValue) : base(key, provider, defaultValue) { }
	}

	public class StringHolder : INotifyPropertyChanged
	{
		private string _value;

		public string Value
		{
			get => _value; set
			{
				if (_value == value) return;
				_value = value;
				NotifyOfPropertyChange();
			}
		}

		public static implicit operator string(StringHolder stringHolder) => stringHolder.Value;

		public static implicit operator StringHolder(string s) => new StringHolder { Value = s };

		public void NotifyOfPropertyChange([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public event PropertyChangedEventHandler PropertyChanged;
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
