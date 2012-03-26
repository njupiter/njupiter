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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace nJupiter.Web.Syndication {
	public class FeedReader {
		
		// Inspired by the RSS Parser in RSS Bandit http://rssbandit.org/

		private static readonly XmlDocument elementCreator = new XmlDocument();
		
		public static IFeed GetFeed(Uri url) {
			WebRequest webRequest = WebRequest.Create(url);
			HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
			if(httpWebRequest != null) {
				httpWebRequest.KeepAlive = true;
			}
			using(WebResponse webResponse = webRequest.GetResponse()) {
				return GetFeed(webResponse.GetResponseStream(), url);
			}
		}

		public static IFeed GetFeed(Stream stream, Uri baseUrl) {
			using(XmlReader xmlReader = XmlReader.Create(stream)){
				return GetFeed(baseUrl, xmlReader);
			}
		}

		public static IFeed GetFeed(Stream stream) {
			return GetFeed(stream, null);
		}

		public static IFeed GetFeed(Uri url, XmlReader reader) {
			reader.MoveToContent();
			string language = string.Empty;
			FeedType feedType;
			string namespaceUri = reader.NamespaceURI;
			if(	reader.LocalName.Equals("RDF") &&
				reader.NamespaceURI.Equals("http://www.w3.org/1999/02/22-rdf-syntax-ns#")) {
				feedType = FeedType.Rdf;
				reader.Read();
				reader.MoveToContent();
			} else if(reader.LocalName.Equals("rss")) {
				feedType = FeedType.Rss;

				do {
					reader.Read();
					reader.MoveToContent();
				} while(!reader.LocalName.Equals("channel") && !reader.LocalName.Equals("rss"));
			} else if(reader.NamespaceURI.Equals("http://purl.org/atom/ns#") && reader.LocalName.Equals("feed")) {
				if(reader.MoveToAttribute("version") && reader.Value.Equals("0.3")) {
					feedType = FeedType.Atom;
					language = reader.XmlLang;
					reader.MoveToElement();
				} else {
					throw new ApplicationException(string.Format("Unsupported Atom Version {0}", reader.Value));
				}
			} else if(reader.NamespaceURI.Equals("http://www.w3.org/2005/Atom") && reader.LocalName.Equals("feed")) {
				feedType = FeedType.Atom;
				language = reader.XmlLang;
			} else {
				throw new ApplicationException("Unknown Xml Dialect");
			}
			return PopulateFeed(url, reader, namespaceUri, feedType, language);
		}

		private static IFeed PopulateFeed(Uri url, XmlReader reader, string namespaceUri, FeedType feedType, string language) {
			List<IFeedItem> items = new List<IFeedItem>();
			string link = null;
			string selfLink = null;
			string title = null;
			string description = null;
			string id = null;
			IAuthor author = null;
			DateTime channelBuildDate = DateTime.MinValue;
			Dictionary<XmlQualifiedName, string> customElements = new Dictionary<XmlQualifiedName, string>();
			bool nodeRead = false;

			while(nodeRead || reader.Read()) {
				nodeRead = false;
				if(!reader.NodeType.Equals(XmlNodeType.Element)) {
					continue;
				}
				if(reader.NamespaceURI.Equals(namespaceUri) || reader.NamespaceURI.Equals(String.Empty)) {
					if(reader.LocalName.Equals("title")) {
						if(!reader.IsEmptyElement) {
							title = ReadElementString(reader);
						}
						continue;
					}
					if(feedType.Equals(FeedType.Rdf) || feedType.Equals(FeedType.Rss)) {
						if(reader.IsEmptyElement) {
							continue;
						}
						if(reader.LocalName.Equals("description")) {
							description = ReadElementString(reader);
							continue;
						}
						if(reader.LocalName.Equals("link")) {
							link = ReadElementString(reader);
							continue;
						}
						if(reader.LocalName.Equals("language")) {
							language = ReadElementString(reader);
							continue;
						}
						if(reader.LocalName.Equals("lastBuildDate")) {
							DateTime.TryParse(ReadElementString(reader), out channelBuildDate);
							continue;
						}
						if(reader.LocalName.Equals("pubDate") && channelBuildDate.Equals(DateTime.MinValue)) {
							DateTime.TryParse(ReadElementString(reader), out channelBuildDate);
							continue;
						}
						if(reader.LocalName.Equals("item")) {
							IFeedItem rssItem = PopulateFeedItem(reader, feedType, url);
							if(rssItem != null) {
								items.Add(rssItem);
							}
							continue;
						}

					} else if(feedType.Equals(FeedType.Atom)) {
						if(reader.LocalName.Equals("subtitle")) {
							if(!reader.IsEmptyElement) {
								description = ReadElementString(reader);
							}
							continue;
						}
						if(reader.LocalName.Equals("id")) {
							if(!reader.IsEmptyElement) {
								id = ReadElementString(reader);
							}
							continue;
						}
						if(reader.LocalName.Equals("link")) {
							string rel = reader.GetAttribute("rel");
							string href = reader.GetAttribute("href");
							if(string.IsNullOrEmpty(link)) {
								if(rel != null && href != null) {
									if(rel.Equals("alternate")) {
										link = href;
									}
									if(rel.Equals("self")) {
										selfLink = href;
									}
								}
							}
							continue;
						}
						if(reader.LocalName.Equals("author")) {
							author = PopulateAuthor(reader, feedType);
							continue;
						}
						if(reader.LocalName.Equals("updated") || reader.LocalName.Equals("modified")) {
							if(!reader.IsEmptyElement) {
								DateTime.TryParse(ReadElementString(reader), out channelBuildDate);
							}
							continue;
						}
						if(reader.LocalName.Equals("entry") && !reader.IsEmptyElement) {
							IFeedItem rssItem = PopulateFeedItem(reader, feedType, url);
							if(rssItem != null) {
								items.Add(rssItem);
							}
							continue;
						}
					}
				}

				XmlQualifiedName qname = new XmlQualifiedName(reader.LocalName, reader.NamespaceURI);
				string optionalNode = reader.ReadOuterXml();
				nodeRead = true;
				if(!customElements.ContainsKey(qname)) {
					customElements.Add(qname, optionalNode);
				}
			}
			Uri feedUrl = GetUrl(link, url);
			Uri selfFeedUrl = GetUrl(selfLink, url);
			return new Feed(title, id, feedUrl, selfFeedUrl, description, channelBuildDate, language, null, items.ToArray(), author, customElements);
		}

		private static Uri GetUrl(string link, Uri baseUri) {
			if(link != null) {
				link = HttpUtility.HtmlDecode(link.Replace(Environment.NewLine, String.Empty).Trim());
				if(link.Contains("://")) {
					return new Uri(link);
				}
				if(baseUri == null) {
					throw new ArgumentNullException("baseUri", "No baseUri defined");
				}
				return new Uri(baseUri, link);

			}
			return baseUri;
		}

		private static IFeedItem PopulateFeedItem(XmlReader reader, FeedType feedType, Uri url) {
			string description = null;
			string id = null;
			string link = null;
			string title = null;
			int depth = reader.Depth;
			DateTime date = DateTime.MinValue;
			IAuthor author = null;
			string itemNamespaceUri = reader.NamespaceURI;
			Dictionary<XmlQualifiedName, string> customElements = new Dictionary<XmlQualifiedName, string>();
			bool nodeRead = false;

			while(nodeRead || reader.Read()) {
				nodeRead = false;
				if(reader.NodeType.Equals(XmlNodeType.EndElement) && reader.Depth.Equals(depth)) {
					break;
				}
				if(!reader.NodeType.Equals(XmlNodeType.Element)) {
					continue;
				}
				
				if(link != null && link.Trim().Length == 0)
					link = null; // reset on empty elements

				bool nodeNamespaceUriEqual2Item = reader.NamespaceURI.Equals(itemNamespaceUri);
				if(feedType.Equals(FeedType.Rdf) || feedType.Equals(FeedType.Rss)) {
					
					if(reader.IsEmptyElement) {
						continue;
					}
				
					if(title == null && nodeNamespaceUriEqual2Item && reader.LocalName.Equals("title")) {
						title = ReadElementString(reader);
						continue;
					}

					if((link == null) || reader.LocalName.Equals("guid")) {
						//favor rss:guid over rss:link
						if(nodeNamespaceUriEqual2Item && reader.LocalName.Equals("guid")) {
							if((reader["isPermaLink"] == null) || string.Equals(reader["isPermaLink"], "true", StringComparison.OrdinalIgnoreCase)) {
								link = ReadElementString(reader);
							} else if(string.Equals(reader["isPermaLink"], "false", StringComparison.OrdinalIgnoreCase)) {
								id = ReadElementString(reader);
							}
							continue;
						}
						if(nodeNamespaceUriEqual2Item && reader.LocalName.Equals("link")) {
							link = ReadElementString(reader);
							continue;
						}
					}

					if(date.Equals(DateTime.MinValue)) {
						if(reader.LocalName.Equals("date") || (nodeNamespaceUriEqual2Item && reader.LocalName.Equals("pubDate"))) {
							DateTime.TryParse(ReadElementString(reader), out date);
							continue;
						}
					}

					if(description == null || reader.LocalName.Equals("body") || reader.LocalName.Equals("encoded")) {
						//prefer to replace rss:description/dc:description with content:encoded
						if(reader.NamespaceURI.Equals("http://www.w3.org/1999/xhtml") && reader.LocalName.Equals("body")) {
							XmlElement elem = (XmlElement)elementCreator.ReadNode(reader);
							description = elem != null ? elem.InnerXml : null;
							continue;
						}
						if(reader.NamespaceURI.Equals("http://purl.org/rss/1.0/modules/content/") && reader.LocalName.Equals("encoded")) {
							description = ReadElementString(reader);
							continue;
						}
						if(nodeNamespaceUriEqual2Item && reader.LocalName.Equals("description")) {
							description = ReadElementString(reader);
							continue;
						}
					}

					if(author == null || reader.LocalName.Equals("creator")) {
						//prefer dc:creator to <author>
						if(reader.LocalName.Equals("creator") || reader.LocalName.Equals("author")) {
							author = PopulateAuthor(reader, feedType);
							continue;
						}
					}

				}else if(feedType.Equals(FeedType.Atom)){
					if(title == null) {
						if(nodeNamespaceUriEqual2Item && reader.LocalName.Equals("title")) {
							if(!reader.IsEmptyElement) {
								title = GetContentFromAtomElement(reader);
							}
							continue;
						}
					}

					if(id == null) {
						if(nodeNamespaceUriEqual2Item && reader.LocalName.Equals("id")) {
							if(!reader.IsEmptyElement) {
								id = ReadElementString(reader);
							}
							continue;
						}
					}

					if(link == null) {
						if(nodeNamespaceUriEqual2Item && reader.LocalName.Equals("link") &&
							(reader["rel"] == null || reader["rel"].Equals("alternate"))) {
							if(reader["href"] != null) {
								link = reader.GetAttribute("href");
							}
							if(!reader.IsEmptyElement) {
								reader.Skip();
							}
							continue;
						}
					}

					if(date.Equals(DateTime.MinValue) || reader.LocalName.Equals("modified") || reader.LocalName.Equals("updated")) {
						//prefer modified date to publish date
						if(nodeNamespaceUriEqual2Item && (reader.LocalName.Equals("modified") || reader.LocalName.Equals("updated"))) {
							if(!reader.IsEmptyElement) {
								DateTime.TryParse(ReadElementString(reader), out date);
							}
							continue;
						}
						if(nodeNamespaceUriEqual2Item && (reader.LocalName.Equals("issued") || reader.LocalName.Equals("published") || reader.LocalName.Equals("created"))) {
							if(!reader.IsEmptyElement) {
								DateTime.TryParse(ReadElementString(reader), out date);
							}
							continue;
						}
					}

					if(description == null || (reader.LocalName.Equals("content") && reader["src"] == null)) {
						//prefer to replace atom:summary with atom:content
						if(!reader.IsEmptyElement) {
							if(nodeNamespaceUriEqual2Item && reader.LocalName.Equals("content")) {
								description = GetContentFromAtomElement(reader);
								continue;
							}
							if(nodeNamespaceUriEqual2Item && reader.LocalName.Equals("summary")) {
								description = GetContentFromAtomElement(reader);
								continue;
							}
						}
						
					}

					if(author == null && !reader.IsEmptyElement && nodeNamespaceUriEqual2Item && reader.LocalName.Equals("author")) {
						author = PopulateAuthor(reader, feedType);
						continue;
					}
				}

				XmlQualifiedName qname = new XmlQualifiedName(reader.LocalName, reader.NamespaceURI);
				string optionalNode = reader.ReadOuterXml();
				nodeRead = true;
				if(!customElements.ContainsKey(qname)) {
					customElements.Add(qname, optionalNode);
				}
			}
			Uri feedUrl = GetUrl(link, url);
			if(title == null)
				return null;
			return new FeedItem(title, id, description, feedUrl, date, author, customElements);
		}

		private static string GetContentFromAtomElement(XmlReader reader) {
			string typeAttr = reader.GetAttribute("type");
			string modeAttr = reader.GetAttribute("mode");
			string type = (typeAttr == null ? "text/plain" : typeAttr.ToLower());
			string mode = (modeAttr == null ? "xml" : modeAttr.ToLower());
			string result = string.Empty;
			if(reader.NamespaceURI.Equals("http://purl.org/atom/ns#")) {
				if(type.Contains("text") || type.Contains("html")) {
					if(mode.Equals("escaped") || type.Equals("html")) {
						result = ReadElementString(reader);
					}
					if(mode.Equals("xml")) {
						result = reader.ReadInnerXml();
					}
				}
			} else if(reader.NamespaceURI.Equals("http://www.w3.org/2005/Atom")) {
				if(type.Contains("xhtml")) {
					result = reader.ReadInnerXml();
				}
				if((type.Contains("text"))) {
					result = ReadElementString(reader).Replace("<", "&lt;").Replace(">", "&gt;");
				}
				if(type.Contains("html")) {
					result = ReadElementString(reader);
				}
			}
			return result;
		}

		private static IAuthor PopulateAuthor(XmlReader reader, FeedType feedType) {
			string author = null;
			string email = null;
			if(feedType.Equals(FeedType.Rdf) || feedType.Equals(FeedType.Rss)) {
				email = author = ReadElementString(reader);
			} 
			if(feedType.Equals(FeedType.Atom)) {
				int depth = reader.Depth;
				while(reader.Read()) {
					if(reader.NodeType.Equals(XmlNodeType.EndElement) && reader.Depth.Equals(depth)) {
						break;
					}
					if(!reader.NodeType.Equals(XmlNodeType.Element)) {
						continue;
					}
					if(!reader.IsEmptyElement) {
						if(reader.LocalName.Equals("name")) {
							author = ReadElementString(reader);
							continue;
						}
						if(reader.LocalName.Equals("email")) {
							email = ReadElementString(reader);
							continue;
						}
					}
				}
			}
			author = author ?? email;
			if(author != null) {
				return new Author(author, email);
			}
			return null;
		}

		private static string ReadElementString(XmlReader reader) {
			String result = reader.ReadString();
			StringBuilder sb = null;
			while(reader.NodeType != XmlNodeType.EndElement) {
				if(sb == null) {
					sb = new StringBuilder(result);
				}
				reader.Skip();
				sb.Append(reader.ReadString());
			}
			return sb == null ? result : sb.ToString();
		}

	}
}
