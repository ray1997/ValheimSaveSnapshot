using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using ValheimSaveSnapshot.Helper;
using ValheimSaveSnapshot.Messages;
using ValheimSaveSnapshot.Model;
using ValheimSaveSnapshot.Services;

namespace ValheimSaveSnapshot.ViewModel
{
	public class MainViewModel : Observable
	{
		public MainViewModel()
		{
			AvailableProfiles = ProfileGatherService.Instance.GetProfiles();
			Messenger.Default.Register<ValueChanged>(this, UpdateSnapshotProfile);
		}

		ObservableCollection<Profile> _profiles = new ObservableCollection<Profile>();
		public ObservableCollection<Profile> AvailableProfiles
		{
			get => _profiles;
			set => Set(ref _profiles, value);
		}

		Profile _selected;
		public Profile SelectedProfile
		{
			get => _selected;
			set => Set(ref _selected, value, true);
		}

		private void UpdateSnapshotProfile(ValueChanged m)
		{
			if (m.PropertyName == nameof(SelectedProfile))
			{
				if (m.NewValue is null) //TODO: Show select profile 
				{
					Snapshots = null;
					return;
				}
				else
				{
					if (Snapshots is null)
						Snapshots = new ObservableCollection<Snapshot>();
					else
						Snapshots.Clear();

					//Load available saved snapshot
					FileInfo file = new FileInfo(SnapshotService.Instance.GetSavedSnapshotCollection(m.NewValue as Profile));
					if (File.Exists(file.FullName))
					{
						using (StreamReader reader = file.OpenText())
						{
							string json = reader.ReadToEnd();
							Snapshots = JsonSerializer.Deserialize<ObservableCollection<Snapshot>>(json);
						}
					}
				}
			}
		}

		ObservableCollection<Snapshot> _snaps;
		public ObservableCollection<Snapshot> Snapshots
		{
			get => _snaps;
			set => Set(ref _snaps, value);
		}
	}
}
