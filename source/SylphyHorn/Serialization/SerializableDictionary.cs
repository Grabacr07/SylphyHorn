using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SylphyHorn.Serialization
{
	public class SerializableDictionary<TKey, TValue> : SortedDictionary<TKey, TValue>, IXmlSerializable
	{
		public XmlSchema GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			var serializer = new XmlSerializer(typeof(KeyValueItem));
			reader.ReadStartElement();
			try
			{
				while (reader.NodeType != XmlNodeType.EndElement)
				{
					var item = (KeyValueItem)serializer.Deserialize(reader);
					this.Add(item.Key, item.Value);
				}
			}
			finally
			{
				reader.ReadEndElement();
			}
		}
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			var ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			var serializer = new XmlSerializer(typeof(KeyValueItem));
			foreach (var item in this.Keys.Select(key => new KeyValueItem { Key = key, Value = this[key], }))
			{
				serializer.Serialize(writer, item, ns);
			}
		}

		[XmlRoot(nameof(KeyValueItem))]
		public class KeyValueItem
		{
			[XmlElement(nameof(Key))]
			public TKey Key;
			[XmlElement(nameof(Value))]
			public TValue Value;
		}
	}
}
