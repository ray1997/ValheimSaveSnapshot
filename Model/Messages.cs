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

	public class SnapshotCreated
	{
		public string Name;
		public string Path;
		public string ProfileName;
	}

	public class SnapshotDeleted
	{
		public string Name;
		public string Path;
	}

	public class SnapshotRestored
	{
		public string Name;
		public string Path;
	}

	public class RequestRestoreSnapshot 
	{
		public string Name;
	}
}
