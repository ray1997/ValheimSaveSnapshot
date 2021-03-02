using System;

namespace ValheimSaveSnapshot.Messages
{
	public class ValueChanged
	{
		public object NewValue;
		public object OldValue;
		public Type ValueType;
		public string PropertyName;
	}
}
