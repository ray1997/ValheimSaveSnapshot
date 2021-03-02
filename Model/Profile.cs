using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ValheimSaveSnapshot.Helper;

namespace ValheimSaveSnapshot.Model
{
	public class Profile : Observable
	{
		string _name;
		public string Name
		{
			get => _name;
			set => Set(ref _name, value);
		}

		string _path;
		public string FilePath
		{
			get => _path;
			set => Set(ref _path, value);
		}

		string _snapshotFolder;
		public string SnapshotFolderName
		{
			get => _snapshotFolder;
			set => Set(ref _snapshotFolder, value);
		}

		ObservableCollection<string> _snaps;
		public ObservableCollection<string> Snapshots
		{
			get => _snaps;
			set => Set(ref _snaps, value);
		}
	}
}
