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
using System.Collections.Specialized;

namespace nJupiter.Web {
	public static class UrlHandler {

		#region Legacy
		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string AddQueryKeyValue(string url, string key, string value, bool encodeValue) {
			return Instance.AddQueryKeyValue(url, key, value, encodeValue);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string AddQueryKeyValue(string url, string key, string value) {
			return Instance.AddQueryKeyValue(url, key, value);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string ReplaceQueryKeyValue(string url, string key, string value, bool encodeValue) {
			return Instance.ReplaceQueryKeyValue(url, key, value, encodeValue);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string ReplaceQueryKeyValue(string url, string key, string value) {
			return Instance.ReplaceQueryKeyValue(url, key, value);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string AddQueryParams(string url, params string[] parameters) {
			return Instance.AddQueryParams(url, parameters);
		}
		
		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string AddQueryParams(string url, NameValueCollection queryStringCollection, bool encodeValues) {
			return Instance.AddQueryParams(url, queryStringCollection, encodeValues);
		}
		
		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string AddQueryParams(string url, NameValueCollection queryStringCollection) {
			return Instance.AddQueryParams(url, queryStringCollection);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static NameValueCollection GetQueryString(string url) {
			return Instance.GetQueryString(url);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string GetQueryString(string url, bool encodeValues, params string[] parametersToRemove) {
			return Instance.GetQueryString(url, encodeValues, parametersToRemove);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string GetQueryString(string url, params string[] parametersToRemove) {
			return Instance.GetQueryString(url, parametersToRemove);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string GetQueryString(NameValueCollection queryStringCollection, bool encodeValues, params string[] parametersToRemove) {
			return Instance.GetQueryString(queryStringCollection, encodeValues, parametersToRemove);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string GetQueryString(NameValueCollection queryStringCollection, params string[] parametersToRemove) {
			return Instance.GetQueryString(queryStringCollection, parametersToRemove);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string GetQueryString(NameValueCollection queryStringCollection) {
			return Instance.GetQueryString(queryStringCollection);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string GetQueryString(NameValueCollection queryStringCollection, bool encodeValues) {
			return Instance.GetQueryString(queryStringCollection, encodeValues);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static NameValueCollection RemoveQueryParams(NameValueCollection queryStringCollection, params string[] parametersToRemove) {
			return Instance.RemoveQueryParams(queryStringCollection, parametersToRemove);
		}

		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string RemoveQueryKeys(string url, params string[] keys) {
			return Instance.RemoveQueryKeys(url, keys);
		}
		
		[Obsolete("Use the same method on the UrlHandler.Instance instead")]
		public static string RemoveQueryKey(string url, string key) {
			return Instance.RemoveQueryKey(url, key);
		}

		[Obsolete("Originaly used to get around an Friendly Url Rewriter in an old CMS. Use HttpContextHandler.Instance.Current.Request.Path instead")]
		public static string CurrentPath {
			get {
				return HttpContextHandler.Instance.Current.Request.Path;
			}
		}

		[Obsolete("Originaly used to get around an Friendly Url Rewriter in an old CMS. HttpContextHandler.Instance.Current.Request.QueryString instead")]
		public static NameValueCollection CurrentQueryString {
			get {
				return HttpContextHandler.Instance.Current.Request.QueryString;
			}
		}

		[Obsolete("Originaly used to get around an Friendly Url Rewriter in an old CMS. HttpContextHandler.Instance.Current.Request.Url.PathAndQuery instead")]
		public static string CurrentPathAndQuery {
			get {
				return HttpContextHandler.Instance.Current.Request.Url.PathAndQuery;
			}
		}

		[Obsolete("Originaly used to get around an Friendly Url Rewriter in an old CMS. HttpContextHandler.Instance.Current.Request.Url.Form instead")]
		public static NameValueCollection CurrentForm {
			get {
				return HttpContextHandler.Instance.Current.Request.Form;
			}
		}
		#endregion

		public static IUrlHandler Instance { get { return NestedSingleton.instance; } }

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly IUrlHandler instance = new UrlHandlerImpl();
		}

	}
}