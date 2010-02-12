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

namespace nJupiter.Services.Forum {

	[Serializable]
	public abstract class Attribute {
		#region Variables
		private readonly string	name;
		private object			value;
		#endregion

		#region Constructors
		protected Attribute(string attributeName) {
			if(attributeName == null) {
				throw new ArgumentNullException("attributeName");
			}
			if(attributeName.Length > 100) { 
				throw new ArgumentOutOfRangeException("attributeName");
			}
			this.name	= attributeName;
		}
		#endregion

		#region Properties
		public string Name { get { return this.name; } }
		public object Value {
			get { return this.value ?? this.DefaultValue; }
			set {
				if(value == null) {
					this.value = this.DefaultValue;
				} else if(this.AttributeValueType.IsInstanceOfType(value)) {
					this.value = value;
				} else {
					throw new ArgumentException("The value does not match the type of the attribute.", "value");
				}
			}
		}
		public abstract object DefaultValue { get; }
		public virtual bool IsEmpty { get { return this.Value == this.DefaultValue || this.Value.Equals(this.DefaultValue); } }
		public virtual bool SerializationPreservesOrder { get { return true; } }
		public abstract Type AttributeValueType { get; }
		#endregion

		#region Methods
		/// <summary>
		/// Serializes the attribute to a string. 
		/// This string should, if possible, maintain the sorting order of the underlying type.
		/// If not, the implemented attribute class must override the virtual property SerializationPreservesOrder and return false.
		/// </summary>
		public abstract string ToSerializedString();
		public abstract object DeserializeAttributeValue(string value);
		public sealed override bool Equals(object obj) {
			Attribute attribute = obj as Attribute;
			return attribute != null && 
				this.GetType().Equals(attribute.GetType()) &&
				this.name.Equals(attribute.name);
		}
		public sealed override int GetHashCode() {
			return this.name.GetHashCode();
		}
		#endregion

		#region Internal Methods
		internal Attribute Clone() {
			return (Attribute)base.MemberwiseClone();
		}
		internal static Attribute Create(string attributeName, Type attributeType) {
			if(attributeName == null) {
				throw new ArgumentNullException("attributeName");
			}
			if(attributeType == null) {
				throw new ArgumentNullException("attributeType");
			}
			return (Attribute)Activator.CreateInstance(attributeType, new object[] { attributeName });
		}
		#endregion
	}

	#region Implementations of Attribute

	#region StringAttribute
	[Serializable]
	public class StringAttribute : Attribute {
		#region Constructors
		public StringAttribute(string attributeName) : base(attributeName) {}
		#endregion

		#region Properties
		public override object DefaultValue { get { return string.Empty; } }
		new public string Value { get { return (string)base.Value; } set { base.Value = value; } }
		public override bool IsEmpty { get { return this.Value.TrimEnd().Length.Equals(0); } }
		public override Type AttributeValueType { get { return typeof(string); } }
		#endregion

		#region Methods
		public override string ToSerializedString() {
			return this.Value;
		}
		public override object DeserializeAttributeValue(string value) {
			return value ?? this.DefaultValue;
		}
		#endregion
	}
	#endregion

	#region DateTimeAttribute
	[Serializable]
	public class DateTimeAttribute : Attribute {
		#region Constants
		private const string Format = "D19";
		#endregion

		#region Constructors
		public DateTimeAttribute(string attributeName) : base(attributeName) {}
		#endregion

		#region Properties
		public override object DefaultValue { get { return DateTime.MinValue; } }
		new public DateTime Value { get { return (DateTime)base.Value; } set { base.Value = value; } }
		public override Type AttributeValueType { get { return typeof(DateTime); } }
		#endregion 

		#region Methods
		public override string ToSerializedString() {
			return this.Value.Ticks.ToString(Format, NumberFormatInfo.InvariantInfo);
		}
		public override object DeserializeAttributeValue(string value) {
			return value == null ? this.DefaultValue : new DateTime(long.Parse(value, NumberFormatInfo.InvariantInfo));
		}
		#endregion
	}
	#endregion

	#region BoolAttribute
	[Serializable]
	public class BoolAttribute : Attribute {
		#region Constructors
		public BoolAttribute(string attributeName) : base(attributeName) {}
		#endregion

		#region Properties
		public override object DefaultValue { get { return false; } }
		new public bool Value { get { return (bool)base.Value; } set { base.Value = value; } }
		public override Type AttributeValueType { get { return typeof(bool); } }
		#endregion

		#region Methods
		public override string ToSerializedString() {
			return this.Value.ToString();
		}
		public override object DeserializeAttributeValue(string value) {
			return value == null ? this.DefaultValue : bool.Parse(value);
		}
		#endregion
	}
	#endregion

	#region IntAttribute
	[Serializable]
	public class IntAttribute : Attribute {
		#region Constants
		private const string Format = "D10";
		#endregion

		#region Constructors
		public IntAttribute(string attributeName) : base(attributeName) {}
		#endregion

		#region Properties
		public override object DefaultValue { get { return 0; } }
		new public int Value { get { return (int)base.Value; } set { base.Value = value; } }
		public override Type AttributeValueType { get {	return typeof(int); } }
		#endregion

		#region Methods
		public override string ToSerializedString() {
			//convert to a non negative value for being able to sort lexicographically
			long longPositiveValue = (long)this.Value - int.MinValue;
			return longPositiveValue.ToString(Format, NumberFormatInfo.InvariantInfo);
		}
		public override object DeserializeAttributeValue(string value) {
			return value == null ? this.DefaultValue : (int)(long.Parse(value, NumberFormatInfo.InvariantInfo) + int.MinValue);
		}
		#endregion
	}
	#endregion

	#region BinaryAttribute
	[Serializable]
	public class BinaryAttribute : Attribute {
		#region Constructors
		public BinaryAttribute(string attributeName) : base(attributeName) {}
		#endregion

		#region Properties
		public override object DefaultValue { get { return null; } }
		public override Type AttributeValueType { get { return typeof(object); } }
		public override bool SerializationPreservesOrder { get { return false; } }
		#endregion
		
		#region Methods
		public override string ToSerializedString() {
			if(this.IsEmpty) {
				return null;
			}
			using(MemoryStream stream = new MemoryStream()) {
				new BinaryFormatter().Serialize(stream, this.Value);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		public override object DeserializeAttributeValue(string value) {
			if(value == null) {
				return this.DefaultValue;
			}
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				return new BinaryFormatter().Deserialize(stream);
			}
		}
		#endregion
	}
	#endregion
	
	#endregion

}