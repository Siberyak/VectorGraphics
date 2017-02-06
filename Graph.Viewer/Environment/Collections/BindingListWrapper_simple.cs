using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KG.SE2.Utils.Collections
{
	public class BindingListWrapper<TItem> : BindingListWrapper<TItem, TItem>
	{
		public BindingListWrapper(IList<TItem> baseList)
			: base(baseList, x => x, x => x)
		{
		}

		protected override void OnInnerListChanged(object sender, ListChangedEventArgs e)
		{
			if(e.ListChangedType == ListChangedType.ItemChanged)
				OnListChanged(e);
			else
				base.OnInnerListChanged(sender, e);
		}
	}
}