using System;

namespace KG.SE2.Utils.Collections
{
	public interface IBindingListItem
	{
		event EventHandler Deleted;
		void OnDeleted();
	}
}