using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using ValheimSaveSnapshot.Helper;
using ValheimSaveSnapshot.Model;

namespace ValheimSaveSnapshot.Services
{
	public class ProfileGatherService
	{
		public static ProfileGatherService Instance => Singleton<ProfileGatherService>.Instance;

		public ObservableCollection<Profile> GetProfiles()
		{
			//Find save game folder
			string path = "";
			path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
				"AppData",
				"LocalLow",
				"IronGate",
				"Valheim",
				"characters");
			DirectoryInfo charFolder = new DirectoryInfo(path);
			//Get all profile info
			ObservableCollection<Profile> profiles = new ObservableCollection<Profile>();
			if (!Directory.Exists(path))
				return profiles;
			var files = charFolder.GetFiles();
			foreach (var file in files)
			{
				switch (file.Extension.ToLower())
				{
					case ".vdf":
					case ".old":
					default:
						break;
					case ".fch":
						profiles.Add(new Profile()
						{
							Name = file.Name,
							FilePath = file.FullName
						});
						break;
				}
			}
			return profiles;
		}
	}
}
