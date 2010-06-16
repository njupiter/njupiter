#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace nJupiter.DataAccess.Users {
	// TODO: Implement clonable
	[Serializable]
	public abstract class AbstractProperty {
		#region Members
		private readonly string name;
		private readonly Context context;
		private object value;
		private bool isDirty;
		#endregion

		#region Constructors
		protected AbstractProperty(string propertyName, Context context) {
			this.name = propertyName;
			this.context = context;
		}
		#endregion

		#region Factory Method
		internal static AbstractProperty Create(string propertyName, string serializedPropertyValue, Type propertyType, Context context) {
			object[] constructorArgs = { propertyName, context };
			AbstractProperty property = (AbstractProperty)Activator.CreateInstance(propertyType, constructorArgs);
			property.value = property.DeserializePropertyValue(serializedPropertyValue);
			property.IsDirty = false;
			return property;
		}
		#endregion

		#region Methods
		public virtual bool IsEmpty() {
			object v = this.Value;
			return v == null || v.Equals(this.DefaultValue);
		}

		public abstract string ToSerializedString();

		public abstract object DeserializePropertyValue(string value);

		public abstract Type GetPropertyValueType();

		public abstract object DefaultValue { get; }

		public override int GetHashCode() {
			return this.Name.ToLowerInvariant().GetHashCode();
		}

		public override bool Equals(object obj) {
			AbstractProperty objProperty = obj as AbstractProperty;
			return objProperty != null && objProperty.Name.Equals(this.Name) && objProperty.Context.Equals(this.context);
		}
		#endregion

		#region Properties
		public string Name { get { return this.name; } }
		public Context Context { get { return this.context; } }


		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				isDirty = value;
			}
		}

		public object Value {
			get {
				if(!this.GetPropertyValueType().IsPrimitive && !(this.value is string) && !(this.value is DateTime)) {
					this.IsDirty = true;
				}
				return (this.value ?? this.DefaultValue);
			}
			set {
				if(!this.GetPropertyValueType().IsPrimitive && !(this.value is string) && !(this.value is DateTime)) {
					this.IsDirty = true;
				} else if(!object.Equals(value, this.value)) {
					this.IsDirty = true;
				}
				if(this.IsDirty) {
					if(value == null) {
						this.value = this.DefaultValue;
					} else if(this.GetPropertyValueType().IsInstanceOfType(value)) {
						this.value = value;
					} else {
						throw new TypeMismatchException("The value does not match the type of the current property.");
					}
				}
			}
		}
		public virtual bool SerializationPreservesOrder { get { return true; } }
		#endregion
	}

	#region Implementations of AbstractProperty

	#region StringProperty
	[Serializable]
	public class StringProperty : AbstractProperty {
		[NonSerialized]
		private readonly string defaultValue = string.Empty;

		public StringProperty(string propertyName, Context context) : base(propertyName, context) { }

		public override object DefaultValue { get { return this.defaultValue; } }

		public override Type GetPropertyValueType() { return typeof(string); }

		public override string ToSerializedString() {
			return this.Value;
		}

		public override object DeserializePropertyValue(string value) {
			return (value ?? this.DefaultValue);
		}

		new public string Value { get { return (string)base.Value; } set { base.Value = value; } }

		public override bool IsEmpty() {
			return this.Value.Trim().Equals(this.DefaultValue);
		}
	}
	#endregion

	#region DateTimeProperty
	[Serializable]
	public class DateTimeProperty : AbstractProperty {
		#region Constants
		private const string Format = "D19";
		#endregion

		[NonSerialized]
		private readonly DateTime defaultValue = DateTime.MinValue;

		public DateTimeProperty(string propertyName, Context context) : base(propertyName, context) { }

		public override object DefaultValue { get { return this.defaultValue; } }

		public override Type GetPropertyValueType() { return typeof(DateTime); }

		public override string ToSerializedString() {
			return this.Value.Ticks.ToString(Format, NumberFormatInfo.InvariantInfo);
		}

		public override object DeserializePropertyValue(string value) {
			return value == null ? this.DefaultValue : new DateTime(long.Parse(value, NumberFormatInfo.InvariantInfo));
		}

		new public DateTime Value { get { return (DateTime)base.Value; } set { base.Value = value; } }
	}
	#endregion

	#region BoolProperty
	[Serializable]
	public class BoolProperty : AbstractProperty {
		[NonSerialized]
		private const bool Default = false;

		public BoolProperty(string propertyName, Context context) : base(propertyName, context) { }

		public override object DefaultValue { get { return Default; } }

		public override Type GetPropertyValueType() { return typeof(bool); }

		public override string ToSerializedString() {
			return this.Value.ToString();
		}

		public override object DeserializePropertyValue(string value) {
			return value == null ? this.DefaultValue : bool.Parse(value);
		}

		new public bool Value { get { return (bool)base.Value; } set { base.Value = value; } }
	}
	#endregion

	#region IntProperty
	[Serializable]
	public class IntProperty : AbstractProperty {
		#region Constants
		private const string Format = "D10";
		#endregion

		[NonSerialized]
		private const int Default = 0;

		public IntProperty(string propertyName, Context context) : base(propertyName, context) { }

		public override object DefaultValue { get { return Default; } }

		public override Type GetPropertyValueType() { return typeof(int); }

		public override string ToSerializedString() {
			//convert to a non negative value for being able to sort lexicographically
			long longPositiveValue = (long)this.Value - int.MinValue;
			return longPositiveValue.ToString(Format, NumberFormatInfo.InvariantInfo);
		}

		public override object DeserializePropertyValue(string value) {
			return value == null ? this.DefaultValue : (int)(long.Parse(value, NumberFormatInfo.InvariantInfo) + int.MinValue);
		}

		new public int Value { get { return (int)base.Value; } set { base.Value = value; } }
	}
	#endregion

	#region BinaryProperty
	[Serializable]
	public class BinaryProperty : AbstractProperty {
		[NonSerialized]
		private const object Default = null;

		public BinaryProperty(string propertyName, Context context) : base(propertyName, context) { }

		public override object DefaultValue { get { return Default; } }
		public override bool SerializationPreservesOrder { get { return false; } }

		public override Type GetPropertyValueType() {
			return typeof(object);
		}

		public override string ToSerializedString() {
			if(this.Value == this.DefaultValue)
				return null;
			using(MemoryStream stream = new MemoryStream()) {
				new BinaryFormatter().Serialize(stream, this.Value);
				return Convert.ToBase64String(stream.ToArray());
			}
		}

		public override object DeserializePropertyValue(string value) {
			if(value == null)
				return null;
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				return new BinaryFormatter().Deserialize(stream);
			}
		}
	}
	#endregion

	#region XmlSerializedProperty
	[Serializable]
	public class XmlSerializedProperty : AbstractProperty {
		[NonSerialized]
		private const object Default = null;

		public XmlSerializedProperty(string propertyName, Context context) : base(propertyName, context) { }

		public override object DefaultValue { get { return Default; } }
		public override bool SerializationPreservesOrder { get { return false; } }

		public override Type GetPropertyValueType() {
			return typeof(object);
		}

		public override string ToSerializedString() {
			if(this.Value == this.DefaultValue)
				return null;
			Type type = this.Value.GetType();
			XmlSerializer serializer = new XmlSerializer(type);
			StringWriter stringwriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlwriter = new XmlTextWriter(stringwriter);
			serializer.Serialize(xmlwriter, this.Value);
			string xmlSerializedData = stringwriter.ToString();
			ValueWrapper valueWrapper = new ValueWrapper(type, xmlSerializedData);
			using(MemoryStream stream = new MemoryStream()) {
				new BinaryFormatter().Serialize(stream, valueWrapper);
				return Convert.ToBase64String(stream.ToArray());
			}
		}

		public override object DeserializePropertyValue(string value) {
			if(value == null)
				return null;
			ValueWrapper valueWrapper;
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				valueWrapper = (new BinaryFormatter()).Deserialize(stream) as ValueWrapper;
			}
			if(valueWrapper == null || valueWrapper.Type == null || valueWrapper.Value == null)
				return null;

			XmlSerializer serializer = new XmlSerializer(valueWrapper.Type);
			StringReader stringReader = new StringReader(valueWrapper.Value);
			XmlTextReader xmlReader = new XmlTextReader(stringReader);
			return serializer.Deserialize(xmlReader);
		}

		[Serializable]
		private sealed class ValueWrapper {
			private readonly Type type;
			private readonly string value;
			public ValueWrapper(Type type, string value) {
				this.type = type;
				this.value = value;
			}
			public Type Type { get { return this.type; } }
			public string Value { get { return this.value; } }
		}
	}
	#endregion

	#endregion
}
