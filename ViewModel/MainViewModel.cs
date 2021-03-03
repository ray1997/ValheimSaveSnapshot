﻿using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using ValheimSaveSnapshot.Helper;
using ValheimSaveSnapshot.Messages;
using ValheimSaveSnapshot.Model;
using ValheimSaveSnapshot.Services;

namespace ValheimSaveSnapshot.ViewModel
{
	public class MainViewModel : Observable
	{
		DebounceDispatcher debouncer = new DebounceDispatcher(250);
		public MainViewModel()
		{
			AvailableProfiles = ProfileGatherService.Instance.GetProfiles();
			Messenger.Default.Register<ValueChanged>(this, UpdateSnapshotProfile);
			InitializeCommand();
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
				if (m.NewValue is null)
				{
					Snapshots = null;
					OnPropertyChanged(nameof(SelectedProfile));
					OnPropertyChanged(nameof(Snapshots));
					return;
				}
				else
				{
					if (Snapshots is null)
						Snapshots = new ObservableCollection<Snapshot>();
					else
						Snapshots.Clear();

					//Load available saved snapshot
					var newProfile = m.NewValue as Profile;
					FileInfo save = new FileInfo(newProfile.FilePath);
					string jsonPath = Path.Combine(save.DirectoryName, $"Snapshot_{newProfile.DisplayName}", $"{newProfile.DisplayName}.json");
					if (File.Exists(jsonPath))
					{
						var jsonFile = new FileInfo(jsonPath);
						using (StreamReader reader = jsonFile.OpenText())
						{
							string json = reader.ReadToEnd();
							Snapshots = JsonSerializer.Deserialize<ObservableCollection<Snapshot>>(json);
						}
					}
					else
					{
						Snapshots = null;
						OnPropertyChanged(nameof(Snapshots));
						OnPropertyChanged(nameof(SelectedProfile));
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

		public ICommand CreateNewSnapshot { get; private set; }

		private void InitializeCommand()
		{
			CreateNewSnapshot = new RelayCommand<RoutedEventArgs>(ExecuteCreateNewSnapshot);

			Messenger.Default.Register<SnapshotCreated>(this, SaveSnapshots);
			Messenger.Default.Register<RequestRestoreSnapshot>(this, RestoreSnapshot);
		}

		private void RestoreSnapshot(RequestRestoreSnapshot obj)
		{
			SnapshotService.Instance.RestoreSnapshot(SelectedProfile, obj.Name);
		}

		private void SaveSnapshots(SnapshotCreated obj)
		{
			var snap = Snapshots.FirstOrDefault(i => i.Name == obj.Name);
			int index = Snapshots.IndexOf(snap);
			if (index > -1)
				Snapshots[index].FullPath = obj.Path;
			FileInfo file = new FileInfo(SelectedProfile.FilePath);
			string savePath = Path.Combine(file.DirectoryName, $"Snapshot_{SelectedProfile.DisplayName}", $"{SelectedProfile.DisplayName}.json");
			using (StreamWriter writer = new StreamWriter(savePath))
			{
				string serialize = JsonSerializer.Serialize(Snapshots, new JsonSerializerOptions()
				{
					WriteIndented = true
				});
				writer.Write(serialize);
			}
		}

		private void ExecuteCreateNewSnapshot(RoutedEventArgs obj)
		{
			if (SelectedProfile != null)
			{
				if (Snapshots is null)
					Snapshots = new ObservableCollection<Snapshot>();
				if (Snapshots.Count < 1)
				{
					SnapshotService.Instance.CreateSnapshot(SelectedProfile, "First snapshot");
					Snapshots.Add(new Snapshot()
					{
						Description = $"A first snapshot created for {SelectedProfile.DisplayName}'s profile",
						Name = "First snapshot",
						SnapshotTime = DateTime.Now,
						IsLatestSnapshot = true
					});
					OnPropertyChanged(nameof(Snapshots));
				}
				else
				{
					SnapshotInput input = new SnapshotInput();
					input.ShowDialog();

					if (input.DialogResult.HasValue && input.DialogResult.Value)
					{
						if (Snapshots.Count > 1)
						{
							var item = Snapshots.FirstOrDefault(snap => snap.Name == input.snapshotName.Text);
							if (!(item is null))
							{
								MessageBox.Show($"A snapshot with a name {input.snapshotName.Text} already exist", "Error!");
								return;
							}
						}
						Snapshots.Add(new Snapshot()
						{
							Name = input.snapshotName.Text,
							Description = input.snapshotDesc.Text,
							SnapshotTime = DateTime.Now,
							IsLatestSnapshot = true
						});
						SnapshotService.Instance.CreateSnapshot(SelectedProfile, input.snapshotName.Text);
						OnPropertyChanged(nameof(Snapshots));
					}
				}
			}
		}
	}
}
