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
		private readonly IContext context;
		private T value;
		private bool isDirty;
		private bool isReadOnly;

		protected PropertyBase(string name, IContext context) {
			this.name = name;
			this.context = context;
		}

		public virtual bool IsEmpty() {
			return Equals(value, this.DefaultValue) || (value is string && value == null);
		}

		public abstract string ToSerializedString();

		public abstract T DeserializePropertyValue(string value);

		public string Name { get { return this.name; } }
		public Type Type { get { return typeof(T); } }
		public IContext Context { get { return this.context; } }
		protected virtual bool SetDirtyOnTouch { get { return false; } }
		protected T ValueUntouched { get { return value; } }

		public T DefaultValue {
			get {
				if(typeof(T) == typeof(string)) {
					object emptyString = string.Empty;
					return (T)emptyString;
				}
				return default(T);
			}
		}

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
				if(this.SetDirtyOnTouch) {
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
			if(this.SetDirtyOnTouch || !Equals(v, value)) {
				return true;
			}
			return false;
		}

		

		object IProperty.DefaultValue { get { return this.DefaultValue; } }		
		object IProperty.Value { get { return Value; }  set { this.Value = (T)value; } }
		object IProperty.DeserializePropertyValue(string v) {
			return this.DeserializePropertyValue(v);
		}

		public IProperty CreateWritable() {
			var newProperty = (PropertyBase<T>)this.MemberwiseClone();
			if(!this.Type.IsPrimitive){
				newProperty.value = DeserializePropertyValue(this.ToSerializedString());
			}
			newProperty.isReadOnly = false;
			newProperty.isDirty = false;
			return newProperty;
		}

		public object Clone() {
			return CreateWritable();
		}

		public void MakeReadOnly() {
			isReadOnly = true;
		}

		public bool IsReadOnly { get { return isReadOnly; } }
	}
}
