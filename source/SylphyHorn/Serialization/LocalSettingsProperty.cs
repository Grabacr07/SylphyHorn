using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace SylphyHorn.Serialization
{
	public class LocalSettingsProperty<T> : SerializablePropertyBase<T>
	{
		public LocalSettingsProperty(string key) : base(key, LocalSettingsProvider.Instance) { }
		public LocalSettingsProperty(string key, T defaultValue) : base(key, LocalSettingsProvider.Instance, defaultValue) { }
	}
}
