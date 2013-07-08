#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

using nJupiter.DataAccess.Users.Sql.Serialization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class XmlSerializedProperty : PropertyBase<object>, ISqlProperty {
		public XmlSerializedProperty(string propertyName, IContext context) : base(propertyName, context) {}

		protected override bool SetDirtyOnTouch { get { return true; } }

		public override string ToSerializedString() {
			if(IsEmpty()) {
				return null;
			}
			var type = Value.GetType();
			var serializer = new XmlSerializer(type);
			var stringwriter = new StringWriter(CultureInfo.InvariantCulture);
			var xmlwriter = new XmlTextWriter(stringwriter);
			serializer.Serialize(xmlwriter, ValueUntouched);
			var xmlSerializedData = stringwriter.ToString();
			var valueWrapper = new ValueWrapper(type, xmlSerializedData);
			using(var stream = new MemoryStream()) {
				new BinaryFormatter().Serialize(stream, valueWrapper);
				return Convert.ToBase64String(stream.ToArray());
			}
		}

		public override object DeserializePropertyValue(string value) {
			if(string.IsNullOrEmpty(value)) {
				return DefaultValue;
			}
			ValueWrapper valueWrapper;
			using(var stream = new MemoryStream(Convert.FromBase64String(value))) {
				var formatter = new BinaryFormatter();
				var surrogateSelector = new SurrogateSelector();
				formatter.SurrogateSelector = surrogateSelector;
				var deserializationBinder = new DeserializationBinder(surrogateSelector);
				formatter.Binder = deserializationBinder;
				valueWrapper = formatter.Deserialize(stream) as ValueWrapper;
			}
			if(valueWrapper == null || valueWrapper.Type == null || valueWrapper.Value == null) {
				return DefaultValue;
			}

			var serializer = new XmlSerializer(valueWrapper.Type);
			var stringReader = new StringReader(valueWrapper.Value);
			var xmlReader = new XmlTextReader(stringReader);
			return serializer.Deserialize(xmlReader);
		}

		[Serializable]
		internal sealed class ValueWrapper {
			private readonly Type type;
			private readonly string value;

			public ValueWrapper(Type type, string value) {
				this.type = type;
				this.value = value;
			}

			public Type Type { get { return type; } }
			public string Value { get { return value; } }
		}

		public bool SerializationPreservesOrder { get { return false; } }
	}
}