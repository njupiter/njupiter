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

namespace nJupiter.DataAccess.Users {
	[Serializable]
	public class UsersException : Exception {
		public UsersException() { }
		public UsersException(string message) : base(message) { }
		public UsersException(string message, Exception inner) : base(message, inner) { }
		protected UsersException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable]
	public class DuplicateUserException : UsersException {
		public DuplicateUserException() { }
		public DuplicateUserException(string message) : base(message) { }
		public DuplicateUserException(string message, Exception inner) : base(message, inner) { }
		protected DuplicateUserException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable]
	public class UserNameAlreadyExistsException : DuplicateUserException {
		public UserNameAlreadyExistsException() { }
		public UserNameAlreadyExistsException(string message) : base(message) { }
		public UserNameAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
		protected UserNameAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable]
	public class UserNameEmptyException : UsersException {
		public UserNameEmptyException() { }
		public UserNameEmptyException(string message) : base(message) { }
		public UserNameEmptyException(string message, Exception inner) : base(message, inner) { }
		protected UserNameEmptyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable]
	public class UserDoesNotExistException : UsersException {
		public UserDoesNotExistException() { }
		public UserDoesNotExistException(string message) : base(message) { }
		public UserDoesNotExistException(string message, Exception inner) : base(message, inner) { }
		protected UserDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable]
	public class ContextAlreadyExistsException : UsersException {
		public ContextAlreadyExistsException() { }
		public ContextAlreadyExistsException(string message) : base(message) { }
		public ContextAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
		protected ContextAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable]
	public class TypeMismatchException : UsersException {
		public TypeMismatchException() { }
		public TypeMismatchException(string message) : base(message) { }
		public TypeMismatchException(string message, Exception inner) : base(message, inner) { }
		protected TypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable]
	public class UnsupportedTypeException : UsersException {
		public UnsupportedTypeException() { }
		public UnsupportedTypeException(string message) : base(message) { }
		public UnsupportedTypeException(string message, Exception inner) : base(message, inner) { }
		protected UnsupportedTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable]
	public class PropertyCollectionMismatchException : UsersException {
		public PropertyCollectionMismatchException() { }
		public PropertyCollectionMismatchException(string message) : base(message) { }
		public PropertyCollectionMismatchException(string message, Exception inner) : base(message, inner) { }
		protected PropertyCollectionMismatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

}
