using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
			return selected is null ? "" : 
				Path.Combine(selected.FilePath, 
				$"\\Snapshot_{selected.DisplayName.ToLower()}\\");
		}

		public string GetSavedSnapshotCollection(Profile selected)
		{
			return selected is null ? "" :
				Path.Combine(selected.FilePath,
				$"\\Snapshot_{selected.DisplayName.ToLower()}\\",
				$"{selected.DisplayName}.json");
		}

		public bool IsSnapshotProfileExist(Profile selected) => File.Exists(GetSnapshotPath(selected));

		public void CreateSnapshot(Profile selected, string name)
		{
			FileInfo saveFile = new FileInfo(selected.FilePath);
			string newPath = Path.Combine(GetSnapshotPath(selected), name);
			Task.Run(() =>
			{
				saveFile.CopyTo(newPath);
			}).Await(() =>
			{
				Messenger.Default.Send(new Messages.SnapshotCreated()
				{
					Name = name,
					Path = newPath
				});
			});
		}

		public void DeleteSnapshot(Profile selected, string name)
		{
			FileInfo saveFile = new FileInfo(selected.FilePath);
			string newPath = Path.Combine(GetSnapshotPath(selected), name);
			Task.Run(() =>
			{
				saveFile.CopyTo(newPath);
			}).Await(() =>
			{
				Messenger.Default.Send(new Messages.SnapshotCreated()
				{
					Name = name,
					Path = newPath
				});
			});
		}
	}
}
