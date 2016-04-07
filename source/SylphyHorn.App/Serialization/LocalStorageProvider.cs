using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace SylphyHorn.Serialization
{
	public class LocalStorageProvider : DictionaryProvider
	{
		public static LocalStorageProvider Instance { get; } = new LocalStorageProvider();

		private LocalStorageProvider() { }

		protected override async Task SaveAsyncCore(IDictionary<string, object> dic)
		{
			var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(this.Filename, CreationCollisionOption.ReplaceExisting);
			var serializer = new DataContractSerializer(typeof(Dictionary<string, object>));
			var settings = new XmlWriterSettings
			{
				Indent = true, // more readable!!!
			};

			using (var stream = await file.OpenStreamForWriteAsync())
			using (var writer = XmlWriter.Create(stream, settings))
			{
				serializer.WriteObject(writer, new Dictionary<string, object>(dic));
			}
		}

		protected override async Task<IDictionary<string, object>> LoadAsyncCore()
		{
			var file = await ApplicationData.Current.LocalFolder.GetFileAsync(this.Filename);
			if (!file.IsAvailable) return null;

			var serializer = new DataContractSerializer(typeof(Dictionary<string, object>));

			using (var stream = await file.OpenStreamForReadAsync())
			{
				var data = serializer.ReadObject(stream) as Dictionary<string, object>;
				if (data != null)
				{
					return data;
				}

				throw new FormatException();
			}
		}
	}
}
