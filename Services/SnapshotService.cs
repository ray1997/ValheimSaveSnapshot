using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using ValheimSaveSnapshot.Helper;
using ValheimSaveSnapshot.Messages;
using ValheimSaveSnapshot.Model;

namespace ValheimSaveSnapshot.Services
{
	public class SnapshotService : Observable
	{
		public static SnapshotService Instance => Singleton<SnapshotService>.Instance;

		public SnapshotService()
		{
			
		}

		public string GetSnapshotPath(Profile selected)
		{
			if (selected is null)
				return "";
			return Path.Combine(new FileInfo(selected.FilePath).DirectoryName,
				$@"\Snapshot_{selected.DisplayName.ToLower()}");
		}

		public string GetSavedSnapshotCollection(Profile selected)
		{
			if (selected is null)
				return "";
			return Path.Combine(new FileInfo(selected.FilePath).DirectoryName,
				$@"\Snapshot_{selected.DisplayName.ToLower()}\",
				$@"{selected.DisplayName}.json");
		}

		public bool IsSnapshotProfileExist(Profile selected) => File.Exists(GetSnapshotPath(selected));

		public void CreateSnapshot(Profile selected, string name)
		{
			FileInfo saveFile = new FileInfo(selected.FilePath);
			string snapshotFolder = Path.Combine(saveFile.DirectoryName, $"Snapshot_{selected.DisplayName}");
			if (!Directory.Exists(snapshotFolder))
				Directory.CreateDirectory(snapshotFolder);
			string newPath = Path.Combine(snapshotFolder, name);
			Task.Run(() =>
			{
				saveFile.CopyTo(newPath);
			}).Await(() =>
			{
				Messenger.Default.Send(new SnapshotCreated()
				{
					Name = name,
					Path = newPath,
					ProfileName = selected.DisplayName
				});
			});
		}

		public void DeleteSnapshot(Profile selected, string name)
		{
			FileInfo saveFile = new FileInfo(selected.FilePath);
			string deletedPath = Path.Combine(saveFile.DirectoryName, $"Snapshot_{selected.DisplayName}", name);
			if (File.Exists(deletedPath))
			{
				Task.Run(() =>
				{
					FileInfo deleted = new FileInfo(deletedPath);
					deleted.Delete();
				}).Await(() =>
				{
					Messenger.Default.Send(new SnapshotDeleted()
					{
						Name = name
					});
				});
			}
		}

		public void RestoreSnapshot(Profile selected, string name)
		{
			FileInfo saveFile = new FileInfo(selected.FilePath);
			string restoredPath = Path.Combine(saveFile.DirectoryName, $"Snapshot_{selected.DisplayName}", name);
			if (File.Exists(restoredPath))
			{
				Task.Run(() =>
				{
					FileInfo newSave = new FileInfo(restoredPath);
					newSave.CopyTo(saveFile.FullName, true);
				}).Await(() =>
				{
					Messenger.Default.Send(new SnapshotRestored()
					{
						Name = name,
						Path = restoredPath
					});
				});
			}
		}

		public void DuplicateSnapshot(Profile selected, string originalName, string NewName)
		{
			FileInfo saveFile = new FileInfo(selected.FilePath);
			string dupedPath = Path.Combine(saveFile.DirectoryName, $"Snapshot_{selected.DisplayName}", originalName);
			if (File.Exists(dupedPath))
			{
				string newPath = Path.Combine(saveFile.DirectoryName, $"Snapshot_{selected.DisplayName}", NewName);
				Task.Run(() =>
				{					
					FileInfo newSave = new FileInfo(dupedPath);
					newSave.CopyTo(newPath);
				}).Await(() =>
				{
					Messenger.Default.Send(new SnapshotCreated()
					{
						Name = NewName,
						ProfileName = selected.Name,
						Path = newPath
					});
				});
			}
		}

		public void SaveDatabase(Profile selectedProfile, IList<Snapshot> snapshots)
		{
			FileInfo file = new FileInfo(selectedProfile.FilePath);
			string savePath = Path.Combine(file.DirectoryName, $"Snapshot_{selectedProfile.DisplayName}", $"{selectedProfile.DisplayName}.json");
			using (StreamWriter writer = new StreamWriter(savePath))
			{
				string serialize = JsonSerializer.Serialize(snapshots, new JsonSerializerOptions()
				{
					WriteIndented = true
				});
				writer.Write(serialize);
			}
		}
	}
}
