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
using System.Runtime.Serialization;

namespace nJupiter.Services.Forum {

	[Serializable]
	public class ForumException : Exception {
		#region Constructors
		public ForumException() { }
		public ForumException(string message) : base(message) { }
		public ForumException(string message, Exception innerException) : base(message, innerException) { }
		protected ForumException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endregion
	}

	[Serializable]
	public sealed class CategoryNotFoundException : ForumException {
		#region Constructors
		public CategoryNotFoundException() { }
		public CategoryNotFoundException(string message) : base(message) { }
		public CategoryNotFoundException(string message, Exception innerException) : base(message, innerException) { }
		private CategoryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endregion
	}

	[Serializable]
	public sealed class PostNotFoundException : ForumException {
		#region Constructors
		public PostNotFoundException() { }
		public PostNotFoundException(string message) : base(message) { }
		public PostNotFoundException(string message, Exception innerException) : base(message, innerException) { }
		private PostNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endregion
	}

	[Serializable]
	public sealed class CategoryNameAlreadyExistsException : ForumException {
		#region Constructors
		public CategoryNameAlreadyExistsException() { }
		public CategoryNameAlreadyExistsException(string message) : base(message) { }
		public CategoryNameAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
		private CategoryNameAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endregion
	}

	[Serializable]
	public sealed class InvalidPostMoveException : ForumException {
		#region Constructors
		public InvalidPostMoveException() { }
		public InvalidPostMoveException(string message) : base(message) { }
		public InvalidPostMoveException(string message, Exception innerException) : base(message, innerException) { }
		private InvalidPostMoveException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endregion
	}

	[Serializable]
	public sealed class ConcurrentUpdateException : ForumException {
		#region Constructors
		public ConcurrentUpdateException() { }
		public ConcurrentUpdateException(string message) : base(message) { }
		public ConcurrentUpdateException(string message, Exception innerException) : base(message, innerException) { }
		private ConcurrentUpdateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endregion
	}

	[Serializable]
	public sealed class OrphanInsertException : ForumException {
		#region Constructors
		public OrphanInsertException() { }
		public OrphanInsertException(string message) : base(message) { }
		public OrphanInsertException(string message, Exception innerException) : base(message, innerException) { }
		private OrphanInsertException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endregion
	}
}
