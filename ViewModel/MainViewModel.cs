using GalaSoft.MvvmLight.Messaging;
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
		public ICommand ClearAllSnapshot { get; private set; }

		private void InitializeCommand()
		{
			CreateNewSnapshot = new RelayCommand<RoutedEventArgs>(ExecuteCreateNewSnapshot);
			ClearAllSnapshot = new RelayCommand<RoutedEventArgs>(ExecuteClearAllSnapshots);

			Messenger.Default.Register<SnapshotCreated>(this, SaveSnapshots);
			Messenger.Default.Register<RequestRestoreSnapshot>(this, RestoreSnapshot);
			Messenger.Default.Register<RequestDuplicateSnapshot>(this, DuplicateSnapshot);
			Messenger.Default.Register<RequestDeleteSnapshot>(this, DeleteSnapshot);
		}

		private void ExecuteClearAllSnapshots(RoutedEventArgs obj)
		{
			var result = MessageBox.Show("This will permanantly delete all snapshots", "Are you sure?", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				FileInfo file = new FileInfo(SelectedProfile.FilePath);
				string snapFolder = Path.Combine(file.DirectoryName, $"Snapshot_{SelectedProfile.DisplayName}");
				if (Directory.Exists(snapFolder))
				{
					Directory.Delete(snapFolder, true);
					Snapshots.Clear();
					Snapshots = null;
					OnPropertyChanged(nameof(Snapshots));
				}
			}
		}

		private void DeleteSnapshot(RequestDeleteSnapshot obj)
		{
			var result = MessageBox.Show("This will permanantly delete this snapshot", "Are you sure?", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				var deleted = Snapshots.FirstOrDefault(snap => snap.Name == obj.Name);
				Snapshots.Remove(deleted);
				SnapshotService.Instance.DeleteSnapshot(SelectedProfile, obj.Name);
				SnapshotService.Instance.SaveDatabase(SelectedProfile, Snapshots);
			}
		}

		private void DuplicateSnapshot(RequestDuplicateSnapshot obj)
		{
			SnapshotInput input = new SnapshotInput
			{
				Title = "Duplicate snapshot"
			};
			input.closeConfirmBTN.Content = "Duplicate";
			input.ShowDialog();

			if (input.DialogResult.HasValue && input.DialogResult.Value)
			{
				Snapshots.Add(new Snapshot()
				{
					Name = input.snapshotName.Text,
					Description = input.snapshotDesc.Text,
					SnapshotTime = DateTime.Now,
					IsLatestSnapshot = true
				});
				SnapshotService.Instance.DuplicateSnapshot(SelectedProfile, obj.Name, input.snapshotName.Text);
				SnapshotService.Instance.SaveDatabase(SelectedProfile, Snapshots);
				OnPropertyChanged(nameof(Snapshots));
			}
		}

		private void RestoreSnapshot(RequestRestoreSnapshot obj)
		{
			SnapshotService.Instance.RestoreSnapshot(SelectedProfile, obj.Name);
			SnapshotService.Instance.SaveDatabase(SelectedProfile, Snapshots);
		}

		private void SaveSnapshots(SnapshotCreated obj)
		{
			var snap = Snapshots.FirstOrDefault(i => i.Name == obj.Name);
			int index = Snapshots.IndexOf(snap);
			if (index > -1)
				Snapshots[index].FullPath = obj.Path;
			SnapshotService.Instance.SaveDatabase(SelectedProfile, Snapshots);
		}

		private void ExecuteCreateNewSnapshot(RoutedEventArgs obj)
		{
			if (SelectedProfile != null)
			{
				if (Snapshots is null)
					Snapshots = new ObservableCollection<Snapshot>();
				if (Snapshots.Count < 1)
				{
					Snapshots.Add(new Snapshot()
					{
						Description = $"A first snapshot created for {SelectedProfile.DisplayName}'s profile",
						Name = "First snapshot",
						SnapshotTime = DateTime.Now,
						IsLatestSnapshot = true
					});
					OnPropertyChanged(nameof(Snapshots));
					SnapshotService.Instance.CreateSnapshot(SelectedProfile, "First snapshot");
					SnapshotService.Instance.SaveDatabase(SelectedProfile, Snapshots);
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
						SnapshotService.Instance.SaveDatabase(SelectedProfile, Snapshots);
						OnPropertyChanged(nameof(Snapshots));
					}
				}
			}
		}
	}
}
