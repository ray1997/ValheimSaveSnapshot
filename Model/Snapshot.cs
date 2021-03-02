using System;
using System.Collections.Generic;
using System.Text;
using ValheimSaveSnapshot.Helper;

namespace ValheimSaveSnapshot.Model
{
	public class Snapshot : Observable
	{
		string _name;
		public string Name
		{
			get => _name;
			set => Set(ref _name, value);
		}

		DateTime _snapTime;
		public DateTime SnapshotTime
		{
			get => _snapTime;
			set => Set(ref _snapTime, value);
		}

		string _desc;
		public string Description
		{
			get => _desc;
			set => Set(ref _desc, value);
		}

		string _path;
		public string FullPath
		{
			get => _path;
			set => Set(ref _path, value);
		}
	}
}
