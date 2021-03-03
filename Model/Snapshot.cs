using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
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

		public ICommand RequestRestore { get; private set; }
		public ICommand RequestDuplicate { get; private set; }
		public Snapshot()
		{
			RequestRestore = new RelayCommand<string>((r) =>
			{
				Messenger.Default.Send(new RequestRestoreSnapshot() { Name = r });
			});
			RequestDuplicate = new RelayCommand<string>((r) =>
			{
				Messenger.Default.Send(new RequestDuplicateSnapshot() { Name = r });
			});

			Messenger.Default.Register<SnapshotCreated>(this, NewSnapshotReact);
			Messenger.Default.Register<SnapshotRestored>(this, RestoredSnapshotReact);
		}

		private void RestoredSnapshotReact(SnapshotRestored obj)
		{
			if (obj.Name == Name)
				IsLatestSnapshot = true;
			else
				IsLatestSnapshot = false;
		}

		private void NewSnapshotReact(SnapshotCreated obj)
		{
			if (obj.Name != Name)
				IsLatestSnapshot = false;
		}
	}
}
