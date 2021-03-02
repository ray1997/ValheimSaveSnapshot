using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ValheimSaveSnapshot.Helper;
using ValheimSaveSnapshot.Model;
using ValheimSaveSnapshot.Services;

namespace ValheimSaveSnapshot.ViewModel
{
	public class MainViewModel : Observable
	{
		public MainViewModel()
		{
			AvailableProfiles = ProfileGatherService.Instance.GetProfiles();
		}

		ObservableCollection<Profile> _profiles = new ObservableCollection<Profile>();
		public ObservableCollection<Profile> AvailableProfiles
		{
			get => _profiles;
			set => Set(ref _profiles, value);
		}
	}
}
