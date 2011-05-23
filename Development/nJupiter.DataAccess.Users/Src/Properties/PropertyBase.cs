#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

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
using System.Data;

namespace nJupiter.DataAccess.Users {
	
	[Serializable]
	public abstract class PropertyBase<T> : IProperty {
		private readonly string name;
		private readonly Context context;
		private T value;
		private bool isDirty;
		private bool isReadOnly;

		protected PropertyBase(string propertyName, Context context) {
			this.name = propertyName;
			this.context = context;
		}

		public virtual bool IsEmpty() {
			object v = this.Value;
			return v == null || v.Equals(default(T));
		}

		public abstract string ToSerializedString();

		public abstract T DeserializePropertyValue(string value);

		public Type GetPropertyValueType() {
			return typeof(T);
		}

		public override int GetHashCode() {
			return this.Name.ToLowerInvariant().GetHashCode();
		}

		public override bool Equals(object obj) {
			PropertyBase<T> objProperty = obj as PropertyBase<T>;
			return objProperty != null && objProperty.Name.Equals(this.Name) && objProperty.Context.Equals(this.context);
		}

		public string Name { get { return this.name; } }
		public Context Context { get { return this.context; } }
		public T DefaultValue { get { return default(T); } }

		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				isDirty = value;
			}
		}

		public T Value {
			get {
				if(CheckIfDirty()) {
					this.IsDirty = true;
				}
				return this.value;
			}
			set {
				if(isReadOnly) {
					throw new ReadOnlyException();
				}
				if(CheckIfDirty(value)) {
					this.IsDirty = true;
				}
				if(this.IsDirty) {
					this.value = value;
				}
			}
		}

		private bool CheckIfDirty(T v) {
			if(CheckIfDirty() || !object.Equals(v, this.value)) {
				return true;
			}
			return false;
		}

		private bool CheckIfDirty() {
			return !this.GetPropertyValueType().IsPrimitive && !(this.value is string) && !(this.value is DateTime);
		}

		public virtual bool SerializationPreservesOrder { get { return true; } }
		
		object IProperty.DefaultValue { get { return default(T); } }		
		object IProperty.Value { get { return value; }  set { this.value = (T)value; } }
		object IProperty.DeserializePropertyValue(string v) {
			return this.DeserializePropertyValue(v);
		}

		public object Clone() {
			var newProperty = (PropertyBase<T>)this.MemberwiseClone();
			if(!this.GetPropertyValueType().IsPrimitive){
				newProperty.Value = DeserializePropertyValue(this.ToSerializedString());
			}
			newProperty.isReadOnly = false;
			newProperty.isDirty = false;
			return newProperty;
		}

		public void MakeReadOnly() {
			isReadOnly = true;
		}

		public bool IsReadOnly { get { return isReadOnly; } }
	}
}
