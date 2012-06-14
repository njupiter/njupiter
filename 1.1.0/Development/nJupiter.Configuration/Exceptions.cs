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
using System.Runtime.Serialization;

namespace nJupiter.Configuration {
	/// <summary>
	/// Configuration Exception
	/// </summary>
	[Serializable]
	public class ConfigurationException : Exception {
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationException"/> class.
		/// </summary>
		public ConfigurationException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationException"/> class.
		/// </summary>
		/// <param name="message">The excetption message.</param>
		public ConfigurationException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationException"/> class.
		/// </summary>
		/// <param name="message">The exception message.</param>
		/// <param name="inner">The inner exeption.</param>
		public ConfigurationException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="info"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">
		/// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
		/// </exception>
		protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	/// <summary>
	/// Excetpion thrown when errors occur in <see href="Configurator" />
	/// </summary>
	[Serializable]
	public class ConfiguratorException : ConfigurationException {
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfiguratorException"/> class.
		/// </summary>
		public ConfiguratorException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfiguratorException"/> class.
		/// </summary>
		/// <param name="message">The excetption message.</param>
		public ConfiguratorException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfiguratorException"/> class.
		/// </summary>
		/// <param name="message">The excetption message.</param>
		/// <param name="inner">The inner excetption.</param>
		public ConfiguratorException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfiguratorException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="info"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">
		/// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
		/// </exception>
		protected ConfiguratorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	/// <summary>
	/// Excetpion thrown when errors occur in <see href="ConfigCollection" /> when trying to add a discarded config
	/// </summary>
	[Serializable]
	public class ConfigDiscardedException : ConfigurationException {
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfiguratorException"/> class.
		/// </summary>
		public ConfigDiscardedException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfiguratorException"/> class.
		/// </summary>
		/// <param name="message">The excetption message.</param>
		public ConfigDiscardedException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfiguratorException"/> class.
		/// </summary>
		/// <param name="message">The excetption message.</param>
		/// <param name="inner">The inner excetption.</param>
		public ConfigDiscardedException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfiguratorException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="info"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">
		/// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
		/// </exception>
		protected ConfigDiscardedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	/// <summary>
	/// Excetpion thrown when a configurated value is invalid
	/// </summary>
	[Serializable]
	public class InvalidConfigValueException : ConfigurationException {
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidConfigValueException"/> class.
		/// </summary>
		public InvalidConfigValueException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidConfigValueException"/> class.
		/// </summary>
		/// <param name="message">The excetption message.</param>
		public InvalidConfigValueException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidConfigValueException"/> class.
		/// </summary>
		/// <param name="message">The excetption message.</param>
		/// <param name="inner">The inner excetption.</param>
		public InvalidConfigValueException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidConfigValueException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="info"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">
		/// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
		/// </exception>
		protected InvalidConfigValueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	/// <summary>
	/// Excetpion thrown when a configurated type is invalid
	/// </summary>
	[Serializable]
	public class InvalidConfigTypeException : ConfigurationException {
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidConfigValueException"/> class.
		/// </summary>
		public InvalidConfigTypeException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidConfigValueException"/> class.
		/// </summary>
		/// <param name="message">The excetption message.</param>
		public InvalidConfigTypeException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidConfigValueException"/> class.
		/// </summary>
		/// <param name="message">The excetption message.</param>
		/// <param name="inner">The inner excetption.</param>
		public InvalidConfigTypeException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidConfigValueException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="info"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">
		/// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
		/// </exception>
		protected InvalidConfigTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	/// <summary>
	/// Exception is thrown when a given config value does not exist
	/// </summary>
	[Serializable]
	public class ConfigValueNotFoundException : InvalidConfigValueException {
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigValueNotFoundException"/> class.
		/// </summary>
		public ConfigValueNotFoundException() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigValueNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The excetption message.</param>
		public ConfigValueNotFoundException(string message) : base(message) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigValueNotFoundException"/> class.
		/// </summary>
		/// <param name="message">The excetption message.</param>
		/// <param name="inner">The inner excetption.</param>
		public ConfigValueNotFoundException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigValueNotFoundException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="info"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">
		/// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
		/// </exception>
		protected ConfigValueNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}


}
