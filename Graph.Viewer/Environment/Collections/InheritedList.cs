using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace KG.SE2.Utils.Collections
{
	public class InheritedList<TParent, TChild> : TransformList<TChild, TParent>
		where TChild : TParent
	{
		public InheritedList()
			: base(child => (TParent)child, parent => (TChild)parent)
		{
		}

		public InheritedList(IList<TChild> collection)
			: base(collection, child => (TParent)child, parent => (TChild)parent)
		{
		}
	}


	


}