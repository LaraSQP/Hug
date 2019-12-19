using System;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using LaraSPQ.Tools;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System.Windows.Input;

namespace Hug
{
	/// <summary>
	/// Interaction logic for TagManager.xaml
	/// </summary>
	public partial class TagManager : Window
	{
		// Settings' constants
		private const string	SS_Collection	= "Hug";
		private const string	SS_WindowTop	= "WindowTop";
		private const string	SS_WindowLeft	= "WindowLeft";
		private const string	SS_WindowHeight = "WindowHeight";
		private const string	SS_WindowWidth	= "WindowWidth";
		private const string	SS_WindowState	= "WindowState";
		private const string	SS_AutoSort		= "AutoSort";

		// Properties
		private AsyncPackage Package
		{
			get; set;
		}
		private WritableSettingsStore WritableSettingsStore
		{
			get; set;
		}



		/// <summary>
		/// Constructor
		/// </summary>
		public TagManager( AsyncPackage package )
		{
			InitializeComponent();

			Package = package;
		}




		/// <summary>
		/// Called when the window has loaded
		/// </summary>
		private void OnLoad( object sender, RoutedEventArgs e )
		{
			// Window icon
			var icon = WindowExtensions.ImageSourceFromIcon( Properties.Resources.hug );

			if( icon != null )
			{
				Icon = icon;
			}

			// Hide/disable minimize button
			WindowExtensions.HideMinimizeAndMaximizeButtons( this, hideMaximize: false );

			// Load available tags
			HugTags.I.Load();

			// Restore window size and position, if any
			try
			{
				var settingsManager = new ShellSettingsManager( Package );

				WritableSettingsStore = settingsManager.GetWritableSettingsStore( SettingsScope.UserSettings );

				if( WritableSettingsStore.CollectionExists( SS_Collection ) == true
					&& WritableSettingsStore.PropertyExists( SS_Collection, SS_WindowTop ) == true )
				{
					// These two are required from now on
					WindowStartupLocation	= WindowStartupLocation.Manual;
					SizeToContent			= SizeToContent.Manual;

					// Window size and position
					Top			= WritableSettingsStore.GetInt32( SS_Collection, SS_WindowTop );
					Left		= WritableSettingsStore.GetInt32( SS_Collection, SS_WindowLeft );
					Height		= WritableSettingsStore.GetInt32( SS_Collection, SS_WindowHeight );
					Width		= WritableSettingsStore.GetInt32( SS_Collection, SS_WindowWidth );
					WindowState = (WindowState)WritableSettingsStore.GetInt32( SS_Collection, SS_WindowState );

					// Other settings
					chAutoSort.IsChecked = WritableSettingsStore.GetBoolean( SS_Collection, SS_AutoSort );
				}
				else
				{
					// Create collection now to be able to check for other settings the 1st time around
					WritableSettingsStore.CreateCollection( SS_Collection );

					// Default to sorted by count
					chAutoSort.IsChecked = true;
				}

				// Disable when automatic sorting by count is enabled
				btUp.IsEnabled = btDown.IsEnabled = ( chAutoSort.IsChecked == false );
			}
			catch( Exception ex )
			{
				// Report and move on
				Box.Error( "Unable to load settings.",
						   "Exception:",
						   ex.Message );
			}

			// Display tags
			HugTags.I.SortByCount( chAutoSort.IsChecked ?? false );

			lvTags.ItemsSource = HugTags.I.Items;
		}




		/// <summary>
		/// Called when the window is about to close
		/// </summary>
		private void OnClosing( object sender, CancelEventArgs e )
		{
			try
			{
				// This check should be unnecessary unless there was an Exception in OnLoad
				if( WritableSettingsStore.CollectionExists( SS_Collection ) == false )
				{
					WritableSettingsStore.CreateCollection( SS_Collection );
				}

				// Window size and position
				WritableSettingsStore.SetInt32( SS_Collection, SS_WindowTop, ( int )Top );
				WritableSettingsStore.SetInt32( SS_Collection, SS_WindowLeft, ( int )Left );
				WritableSettingsStore.SetInt32( SS_Collection, SS_WindowWidth, ( int )Width );
				WritableSettingsStore.SetInt32( SS_Collection, SS_WindowHeight, ( int )Height );
				WritableSettingsStore.SetInt32( SS_Collection, SS_WindowState, ( int )WindowState );

				// Other settings
				WritableSettingsStore.SetBoolean( SS_Collection, SS_AutoSort, chAutoSort.IsChecked ?? false );
			}
			catch( Exception ex )
			{
				// Report and move on
				Box.Error( "Unable to save settings.",
						   "Exception:",
						   ex.Message );
			}
		}




		private void BtAdd_Click( object sender, RoutedEventArgs e )
		{
			AddTags();
		}




		private void TbRightTag_KeyDown( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if( e.Key == Key.Enter )
			{
				AddTags();

				e.Handled = true;
			}
		}




		private void AddTags()
		{
			var left	= tbLeftTag.Text.Trim( '\r', '\n' );
			var right	= tbRightTag.Text.Trim( '\r', '\n' );

			if( left == ""
				|| right == "" )
			{
				Box.Info( "Cannot create an entry unless both left and right tags are provided." );
				return;
			}

			if( HugTags.I.AddItem( left, right ) == true )
			{
				// Added to the collection
				tbLeftTag.Text = tbRightTag.Text = "";
			}
			else
			{
				// Already exists in the collection
				Box.Info( "The pair of tags is already in the collection." );
			}
		}




		private void lvTags_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.Key == Key.Delete
				|| e.Key == Key.Back )
			{
				DeleteSelectedTags();

				e.Handled = true;
			}
		}




		private void btDelete_Click( object sender, RoutedEventArgs e )
		{
			DeleteSelectedTags();
		}




		private void DeleteSelectedTags()
		{
			if( lvTags.SelectedIndex < 0 )
			{
				Box.Info( "Nothing to delete.",
						  "Select one or several tags first." );
				return;
			}

			try
			{
				HugTags.I.DeleteItems( lvTags.SelectedItems );
			}
			catch( InvalidOperationException ex )
			{
				Box.Error( "Unable to properly delete the selected tag(s).",
						   "Exception:",
						   ex.Message );
			}
		}




		private void ChAutoSort_Click( object sender, RoutedEventArgs e )
		{
			btUp.IsEnabled = btDown.IsEnabled = ( chAutoSort.IsChecked == false );

			HugTags.I.SortByCount( chAutoSort.IsChecked ?? false );
		}




		private void BtUp_Click( object sender, RoutedEventArgs e )
		{
			HugTags.I.MoveSelectedItems( lvTags.SelectedItems, Key.Up );
		}




		private void BtDown_Click( object sender, RoutedEventArgs e )
		{
			HugTags.I.MoveSelectedItems( lvTags.SelectedItems, Key.Down );
		}




		private void LvTags_MouseDoubleClick( object sender, MouseButtonEventArgs e )
		{
			var item = lvTags.SelectedItem as HugTags.Item;

			if( item == null )
			{
				return;
			}

			tbLeftTag.Text	= item.Left;
			tbRightTag.Text = item.Right;
		}
	}
}
