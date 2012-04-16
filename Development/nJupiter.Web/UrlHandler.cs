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
using System.Web;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;

namespace nJupiter.Web {

	public static class UrlHandler {
		public static string AddQueryKeyValue(string url, string key, string value, bool encodeValue) {
			ValidateArguments(url, key, value);
			if(encodeValue) {
				value = HttpUtility.UrlEncode(value);
			}
			return AddQueryKeyValue(url, key, value);
		}

		public static string AddQueryKeyValue(string url, string key, string value) {
			ValidateArguments(url, key, value);
			return AddQueryParams(url, string.Format("{0}={1}", key, value));
		}

		public static string ReplaceQueryKeyValue(string url, string key, string value, bool encodeValue) {
			ValidateArguments(url, key, value);
			if(encodeValue) {
				value = HttpUtility.UrlEncode(value);
			}
			url = RemoveQueryKey(url, key);
			return AddQueryKeyValue(url, key, value);
		}

		public static string ReplaceQueryKeyValue(string url, string key, string value) {
			ValidateArguments(url, key, value);
			url = RemoveQueryKey(url, key);
			return AddQueryParams(url, string.Format("{0}={1}", key, value));
		}

		public static string AddQueryParams(string url, params string[] parameters) {
			if(url == null) {
				throw new ArgumentNullException("url");
			}
			if(parameters == null) {
				throw new ArgumentNullException("parameters");
			}

			if(parameters.Length > 0) {
				var queryString = new StringBuilder(url);
				var hashPos = url.IndexOf("#", StringComparison.Ordinal);
				var hashPosFromEnd = hashPos >= 0 ? url.Length - hashPos : hashPos;
				if(hashPosFromEnd >= 0) {
					queryString.Insert(queryString.Length - hashPosFromEnd, url.IndexOf("?", StringComparison.Ordinal) > 0 ? "&" : "?");
				} else {
					queryString.Append(url.IndexOf("?", StringComparison.Ordinal) > 0 ? "&" : "?");
				}
				for(var i = 0; i < parameters.Length; i++) {
					if(parameters[i].Length > 0) {
						if(hashPosFromEnd >= 0) {
							queryString.Insert(queryString.Length - hashPosFromEnd, parameters[i]);
							if(i != parameters.Length - 1)
								queryString.Insert(queryString.Length - hashPosFromEnd, "&");
						} else {
							queryString.Append(parameters[i]);
							if(i != parameters.Length - 1)
								queryString.Append("&");
						}
					}
				}
				if(queryString.Length > url.Length + 1) // has any query parameter been added?
					return queryString.ToString();
			}
			return url;
		}
		public static string AddQueryParams(string url, NameValueCollection queryStringCollection, bool encodeValues) {
			if(url == null) {
				throw new ArgumentNullException("url");
			}
			if(queryStringCollection == null) {
				throw new ArgumentNullException("queryStringCollection");
			}
			var parameters = new string[queryStringCollection.Count];
			var i = 0;
			foreach(string name in queryStringCollection) {
				parameters[i++] = string.Format(CultureInfo.InvariantCulture, "{0}={1}", name, (encodeValues ? HttpUtility.UrlEncode(queryStringCollection[name]) : queryStringCollection[name]));
			}
			return AddQueryParams(url, parameters);
		}
		public static string AddQueryParams(string url, NameValueCollection queryStringCollection) {
			return AddQueryParams(url, queryStringCollection, false);
		}

		public static NameValueCollection GetQueryString(string url) {
			if(url == null) {
				throw new ArgumentNullException("url");
			}
			var nvc = new NameValueCollection();
			if(url.IndexOf('?') > 0)
				url = url.Substring(url.IndexOf('?') + 1);
			var queryParams = url.Split('&');
			foreach(var queryParam in queryParams) {
				if(queryParam.IndexOf('=') > 0) {
					var q = queryParam.Split('=');
					if(q.Length == 2)
						nvc.Add(q[0], q[1]);
					else if(q.Length == 1)
						nvc.Add(q[0], string.Empty);
				}
			}
			return nvc;
		}

		public static string GetQueryString(string url, bool encodeValues, params string[] parametersToRemove) {
			return GetQueryString(GetQueryString(url), encodeValues, parametersToRemove);
		}

		public static string GetQueryString(string url, params string[] parametersToRemove) {
			return GetQueryString(GetQueryString(url), parametersToRemove);
		}

		public static string GetQueryString(NameValueCollection queryStringCollection, bool encodeValues, params string[] parametersToRemove) {
			return GetQueryString(RemoveQueryParams(queryStringCollection, parametersToRemove), encodeValues);
		}
		public static string GetQueryString(NameValueCollection queryStringCollection, params string[] parametersToRemove) {
			return GetQueryString(RemoveQueryParams(queryStringCollection, parametersToRemove));
		}

		public static string GetQueryString(NameValueCollection queryStringCollection) {
			return GetQueryString(queryStringCollection, false);
		}

		public static string GetQueryString(NameValueCollection queryStringCollection, bool encodeValues) {
			if(queryStringCollection == null) {
				throw new ArgumentNullException("queryStringCollection");
			}
			var sb = new StringBuilder();
			foreach(string name in queryStringCollection) {
				var value = (encodeValues ? HttpUtility.UrlEncode(queryStringCollection[name]) : queryStringCollection[name]);
				sb.AppendFormat("{0}={1}&", name, value);
			}
			if(sb.Length == 0)
				return string.Empty;
			return sb.ToString(0, sb.Length - 1);
		}

		public static NameValueCollection RemoveQueryParams(NameValueCollection queryStringCollection, params string[] parametersToRemove) {
			if(queryStringCollection == null) {
				throw new ArgumentNullException("queryStringCollection");
			}
			if(parametersToRemove == null) {
				throw new ArgumentNullException("parametersToRemove");
			}
			queryStringCollection = new NameValueCollection(queryStringCollection);
			foreach(var t in parametersToRemove) {
				queryStringCollection.Remove(t);
			}
			return queryStringCollection;
		}

		public static string RemoveQueryKeys(string url, params string[] keys) {
			var path = url;
			foreach(var key in keys) {
				path = RemoveQueryKey(path, key);
			}
			return path;
		}
		public static string RemoveQueryKey(string url, string key) {
			ValidateArguments(url, key);
			var queryStringSeparatorPos = url.IndexOf("?", StringComparison.Ordinal);
			var hashSeparatorPos = url.IndexOf("#", StringComparison.Ordinal);
			var path = url;
			if(queryStringSeparatorPos > -1) {
				if(hashSeparatorPos > -1)
					path = path.Substring(0, queryStringSeparatorPos) + path.Substring(hashSeparatorPos);
				else
					path = path.Substring(0, queryStringSeparatorPos);
			}
			return AddQueryParams(path, GetQueryString(url, key));
		}

		private static void ValidateArguments(string url, string key) {
			if(url == null) {
				throw new ArgumentNullException("url");
			}
			if(key == null) {
				throw new ArgumentNullException("key");
			}
		}

		private static void ValidateArguments(string url, string key, string value) {
			ValidateArguments(url, key);
			if(value == null) {
				throw new ArgumentNullException("value");
			}
		}

		[Obsolete("Originaly used to get around an Friendly Url Rewriter in an old CMS. Use System.Web.HttpContext.Current.Request.Path instead")]
		public static string CurrentPath {
			get {
				if(HttpContext.Current.Items["FriendlyUrlModule.CurrentFriendlyUrl"] != null)
					return HttpContext.Current.Items["FriendlyUrlModule.CurrentFriendlyUrl"].ToString();
				return HttpContext.Current.Request.Path;
			}
		}

		[Obsolete("Originaly used to get around an Friendly Url Rewriter in an old CMS. System.Web.HttpContext.Current.Request.QueryString instead")]
		public static NameValueCollection CurrentQueryString {
			get {
				if(HttpContext.Current.Items["nJupiter.Web.UrlHandler.CurrentQueryString"] == null) {
					if(HttpContext.Current.Items["FriendlyUrlModule.CurrentFriendlyUrl"] != null) {
						HttpContext.Current.Items["nJupiter.Web.UrlHandler.CurrentQueryString"] = RemoveQueryParams(HttpContext.Current.Request.QueryString, "id", "epslanguage");
					} else {
						return HttpContext.Current.Request.QueryString;
					}
				}
				return HttpContext.Current.Items["nJupiter.Web.UrlHandler.CurrentQueryString"] as NameValueCollection;
			}
		}

		[Obsolete("Originaly used to get around an Friendly Url Rewriter in an old CMS. System.Web.HttpContext.Current.Request.Url.PathAndQuery instead")]
		public static string CurrentPathAndQuery {
			get {
				var queryString = CurrentQueryString;
				if(queryString != null && queryString.Count > 0)
					return AddQueryParams(CurrentPath, GetQueryString(CurrentQueryString));
				return CurrentPath;
			}
		}

		[Obsolete("Originaly used to get around an Friendly Url Rewriter in an old CMS. System.Web.HttpContext.Current.Request.Url.Form instead")]
		public static NameValueCollection CurrentForm {
			get {
				if(HttpContext.Current.Items["nJupiter.Web.UrlHandler.CurrentForm"] == null) {
					if(HttpContext.Current.Items["FriendlyUrlModule.CurrentFriendlyUrl"] != null) {
						HttpContext.Current.Items["nJupiter.Web.UrlHandler.CurrentForm"] = RemoveQueryParams(HttpContext.Current.Request.Form, "id", "epslanguage");
					} else {
						return HttpContext.Current.Request.Form;
					}
				}
				return HttpContext.Current.Items["nJupiter.Web.UrlHandler.CurrentForm"] as NameValueCollection;
			}
		}

	}
}