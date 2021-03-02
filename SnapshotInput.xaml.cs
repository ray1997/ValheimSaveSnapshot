using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ValheimSaveSnapshot
{
	/// <summary>
	/// Interaction logic for SnapshotInput.xaml
	/// </summary>
	public partial class SnapshotInput : Window
	{
		public SnapshotInput()
		{
			InitializeComponent();
		}

		private void CenterItself(object sender, RoutedEventArgs e)
		{
			Application curApp = Application.Current;
			Window mainWindow = curApp.MainWindow;
			this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
			this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
		}

		private void CloseAndConfirm(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
