using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

		public string BackupProfile => $"{FilePath}.old";

		public string DisplayName => $"{Name[0].ToString().ToUpper()}{Name[1..Name.IndexOf(".")]}";
	}
}
