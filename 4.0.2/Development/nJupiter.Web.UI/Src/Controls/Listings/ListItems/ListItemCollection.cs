using System;
using System.Collections;

namespace nJupiter.Web.UI.Controls.Listings {

	[Obsolete("Try using System.Web.UI.WebControls.ListView instead if possible")]
	public sealed class ListItemCollection : ICollection {

		private readonly ArrayList items;

		public ListItemCollection() {
			this.items = new ArrayList();
		}

		public int Count { get { return this.items.Count; } }
		public bool IsReadOnly { get { return this.items.IsReadOnly; } }
		public bool IsSynchronized { get { return this.items.IsSynchronized; } }
		public ListItemBase this[int index] { get { return (ListItemBase)this.items[index]; } }
		public object SyncRoot { get { return this; } }

		internal void Add(ListItemBase item) {
			this.items.Add(item);
		}

		public void CopyTo(ListItemBase[] listItems, int index) {
			this.items.CopyTo(listItems, index);
		}

		void ICollection.CopyTo(Array array, int index) {
			this.items.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator() {
			return this.items.GetEnumerator();
		}
	}
}