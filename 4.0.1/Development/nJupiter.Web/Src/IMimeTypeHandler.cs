using System.Collections.Generic;
using System.IO;

namespace nJupiter.Web {
	public interface IMimeTypeHandler {
		/// <summary>
		/// Get accepted types for a specific http-contect implemented after RFC2616
		/// </summary>
		/// <returns>A collection of accepted types</returns>
		/// <seealso cref="http://www.faqs.org/rfcs/rfc2616.html"/>
		IEnumerable<IMimeType> GetAcceptedTypes();

		IMimeType GetHighestQuality(IMimeType mimeType);
		IMimeType GetHighestQuality(IMimeType mimeType, bool exactType);

		///	<summary>
		///	</summary>
		/// <param name="stream">Stream containing the file</param>
		/// <returns>Returns a MimeType object</returns>
		IMimeType GetMimeType(Stream stream);

		/// <summary>
		/// Ensures that file exists and retrieves the content type
		/// </summary>
		/// <param name="file">FileInfo object containing the file</param>
		/// <returns>Returns a MimeType object</returns>
		IMimeType GetMimeType(FileInfo file);

		///	<summary>
		///	</summary>
		/// <param name="bytes">Byte array containing the file</param>
		/// <returns>Returns a MimeType object</returns>
		IMimeType GetMimeType(byte[] bytes);
	}
}