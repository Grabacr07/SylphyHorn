using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace SylphyHorn.Serialization
{
	public class LocalStorageProperty<T> : SerializablePropertyBase<T>
	{
		public LocalStorageProperty(string key) : base(key, LocalStorageProvider.Instance) { }
		public LocalStorageProperty(string key, T defaultValue) : base(key, LocalStorageProvider.Instance, defaultValue) { }
	}
}
