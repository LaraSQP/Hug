using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Windows;
using System.Windows.Input;

namespace LaraSPQ.Tools
{
	internal static class DataExtensions
	{
		/// <summary>
		/// Moves the selected items in the direction of the key pressed (i.e., up or down) one position,<para/>
		/// crunching them at the top/bottom when they reach it.<para/>
		/// </summary>
		public static bool MoveItems<T>( this ObservableCollection<T> oc, IList selectedItems, Key key )
		{
			var changes = false;

			if( selectedItems.Count == 0 )
			{
				// Nothing to move
				return changes;
			}

			// Assume up direction
			if( key != Key.Up
				&& key != Key.Down )
			{
				// Not an up/down arrow key
				return changes;
			}

			// There is something to move so the collection will change
			changes = true;

			// Copy references to the selected items to avoid enumeration collisions
			var ls = new List<T>();

			foreach( T item in selectedItems )
			{
				if( oc.Contains( item ) == true )
				{
					ls.Add( item );
				}
			}

			// Sort items to face the correct direction
			if( key == Key.Up )
			{
				ls = ls.OrderBy( item => oc.IndexOf( item ) ).ToList();
			}
			else
			{
				ls = ls.OrderByDescending( item => oc.IndexOf( item ) ).ToList();
			}

			// Move items
			foreach( T item in ls )
			{
				if( key == Key.Up )
				{
					oc.MoveItemUp( item );
				}
				else
				{
					oc.MoveItemDown( item );
				}
			}

			return changes;
		}




		/// <summary>
		/// The next 4 methods come from https://stackoverflow.com/a/21329822/1908746
		/// </summary>
		public static void MoveItemUp<T>( this ObservableCollection<T> oc, int selectedIndex )
		{
			// Check if move is possible
			if( selectedIndex <= 0 )
			{
				return;
			}

			// Move item
			oc.Move( selectedIndex - 1, selectedIndex );
		}




		public static void MoveItemDown<T>( this ObservableCollection<T> oc, int selectedIndex )
		{
			// Check if move is possible
			if( selectedIndex < 0
				|| selectedIndex + 1 >= oc.Count )
			{
				return;
			}

			// Move item
			oc.Move( selectedIndex + 1, selectedIndex );
		}




		public static void MoveItemDown<T>( this ObservableCollection<T> oc, T selectedItem )
		{
			// MoveDown based on Item
			oc.MoveItemDown( oc.IndexOf( selectedItem ) );
		}




		public static void MoveItemUp<T>( this ObservableCollection<T> oc, T selectedItem )
		{
			// MoveUp based on Item
			oc.MoveItemUp( oc.IndexOf( selectedItem ) );
		}




		/// <summary>
		/// Copies text to the clipboard in text and Unicode formats
		/// </summary>
		public static bool CopyToClipboard( this string text )
		{
			var success = true;

			// DataObject.SetText() will thrown an exception on null or ""
			if( text.IsNullOrWhitespace() == false )
			{
				try
				{
					var dataObject = new DataObject();

					dataObject.SetText( text, TextDataFormat.Text );
					dataObject.SetText( text, TextDataFormat.UnicodeText );

					Clipboard.Clear();

					Clipboard.SetDataObject( dataObject );
				}
				catch( Exception ex )
				{
					Box.Error( "Unable to copy data to the clipboard, exception:", ex.Message,
							   "Carry on, this is a non-critical error." );

					success = false;
				}
			}

			return success;
		}




		/// <summary>
		/// Encodes problematic XML characters
		/// </summary>
		internal static string Escape( this string value )
		{
			return SecurityElement.Escape( value );
		}




		/// <summary>
		/// Decodes everything, including problematic XML characters
		/// </summary>
		internal static string Unescape( this string value )
		{
			return WebUtility.HtmlDecode( value );
		}




		/// <summary>
		/// Checks whether a filename contains invalid characters
		/// </summary>
		internal static bool IsFilenameValid( this string filename )
		{
			return ( filename.IndexOfAny( Path.GetInvalidPathChars() ) == -1 );
		}




		/// <summary>
		/// Gets the paths to all files in a folder, with optional search pattern and filename only (no extension).<para/>
		/// </summary>
		/// <exception cref="System.Security.SecurityException"></exception>
		/// <exception cref="PathTooLongException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		internal static List<string> GetFilesInFolder( this string sourcePath,
													   string searchPattern = "*.*",
													   bool filenamesOnly = false )
		{
			if( sourcePath == ""
				|| Directory.Exists( sourcePath ) == false )
			{
				return null;
			}

			var di		= new DirectoryInfo( sourcePath );
			var fsi		= di.GetFileSystemInfos( searchPattern );
			var paths	= fsi.Where( f => ( f.Attributes & FileAttributes.Archive ) == FileAttributes.Archive )
						  .OrderBy( f => f.Name );
			var ls = new List<string>( paths.Count() );

			foreach( var path in paths )
			{
				if( filenamesOnly == true )
				{
					ls.Add( Path.GetFileNameWithoutExtension( path.Name ) );
				}
				else
				{
					ls.Add( path.FullName );
				}
			}

			return ls;
		}




		/// <summary>
		/// Simplifies string.IsNullOrWhiteSpace(...) by turning it into an extension, e.g., text.IsNullOrWhitespace()<para/>
		/// </summary>
		internal static bool IsNullOrWhitespace( this string text )
		{
			return string.IsNullOrWhiteSpace( text );
		}




		/// <summary>
		/// Simplifies string.Format(...) by turning it into an extension, e.g., "{0}-{1}".FormatWith(x,y)<para/>
		/// </summary>
		internal static string FormatWith( this string format, params object[] args )
		{
			if( format.IsNullOrWhitespace() == true )
			{
				return "";
			}

			return string.Format( format, args );
		}




		/// <summary>
		/// Simplifies string.Join(...) by turning it into an extension, e.g., collection.Join( EOL )<para/>
		/// </summary>
		internal static string Join( this IEnumerable<string> ie, string delimiter )
		{
			return string.Join( delimiter, ie );
		}
	}
}
