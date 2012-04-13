using System.Collections.Specialized;
using System.Text;

namespace nJupiter.Web {
	public interface IMimeType {
		string Type { get; }
		string DiscreteType { get; }
		string CompositeType { get; }
		NameValueCollection Parameters { get; }
		string ContentType { get; }
		decimal Quality { get; }
		Encoding CharSet { get; }
		bool EqualsType(IMimeType mimeType);
		bool EqualsExactType(IMimeType mimeType);
	}
}