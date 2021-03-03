using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using ValheimSaveSnapshot.Helper;
using ValheimSaveSnapshot.Messages;

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

		bool _latest;
		public bool IsLatestSnapshot
		{
			get => _latest;
			set => Set(ref _latest, value);
		}

		public Snapshot()
		{
			Messenger.Default.Register<SnapshotCreated>(this, NewSnapshotReact);
		}

		private void NewSnapshotReact(SnapshotCreated obj)
		{
			if (obj.Name != Name)
				IsLatestSnapshot = false;
		}
	}
}
