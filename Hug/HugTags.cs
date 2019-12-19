using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LaraSPQ.Tools;

namespace Hug
{
	internal sealed class HugTags
	{
		/// <summary>
		/// Instance by which to access the singleton's methods
		/// </summary>
		internal static HugTags			I => lazy.Value;
		static readonly Lazy<HugTags>	lazy = new Lazy<HugTags>( ( ) => new HugTags() );
		private HugTags() => Math.Abs( 0 );

		// Settings' constants
		private const string DEFAULT_TAGS = "\"\"''()[]{}<>##**";

		// Public properties
		internal ObservableCollection<Item> Items
		{
			get; private set;
		} = null;
		internal bool SortedByCount
		{
			get; private set;
		} = true;
		// Private properties
		private string TagsFilePath
		{
			get; set;
		}

		// Data
		internal class Item
		{
			public string Left
			{
				get; set;
			}
			public string Right
			{
				get; set;
			}
			public int Count
			{
				get; set;
			}
		}



		/// <summary>
		/// Loads tags from the settings or creates default ones if none have been set
		/// </summary>
		internal void Load()
		{
			if( Items != null )
			{
				// No need to reload once the collection exists, even if it is empty (for whatever reason)
				return;
			}

			Items = new ObservableCollection<Item>();

			// Create directory
			var appData		= Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
			var hugFolder	= Path.Combine( appData, "Hug" );

			try
			{
				Directory.CreateDirectory( hugFolder );
			}
			catch( Exception ex )
			{
				Box.Error( "Unable to create Hug's setting's folder in LocalApplicationData.",
						   "Exception:",
						   ex.Message );

				return;
			}

			TagsFilePath = Path.Combine( hugFolder, "HugTags.txt" );

			if( File.Exists( TagsFilePath ) == false )
			{
				// Use defaults
				for( int i = 0; i < DEFAULT_TAGS.Length; i += 2 )
				{
					AddItem( DEFAULT_TAGS[ i ].ToString(),
							 DEFAULT_TAGS[ i + 1 ].ToString(),
							 save: false );
				}

				Save();
			}
			else
			{
				try
				{
					var lines = File.ReadAllLines( TagsFilePath, Encoding.UTF8 );

					for( int i = 0; i < lines.Length; i += 3 )
					{
						Items.Add( new Item
						{
							Left = lines[ i ],
							Right = lines[ i + 1 ],
							Count = int.Parse( lines[ i + 2 ] ),
						} );
					}
				}
				catch( Exception ex )
				{
					Box.Error( "Unable to read tags file in Hug's setting's folder in LocalApplicationData.",
							   "Exception:",
							   ex.Message );

					return;
				}
			}
		}




		/// <summary>
		/// Save
		/// </summary>
		private void Save()
		{
			var ls = new List<string>();

			foreach( var item in Items )
			{
				ls.Add( item.Left );
				ls.Add( item.Right );
				ls.Add( item.Count.ToString() );
			}

			try
			{
				File.WriteAllLines( TagsFilePath, ls, Encoding.UTF8 );
			}
			catch( Exception ex )
			{
				Box.Error( "Unable to save tags file in Hug's setting's folder in LocalApplicationData.",
						   "Exception:",
						   ex.Message );

				return;
			}
		}




		/// <summary>
		/// Increases the count of usage of an item
		/// </summary>
		internal void Count( Item item )
		{
			item.Count++;

			SortByCount( SortedByCount );

			Save();
		}




		/// <summary>
		/// Adds an item to the collection
		/// </summary>
		internal bool AddItem( string left,
							   string right,
							   int count = 0,
							   bool save = true )
		{
			var isNew = ( null == Items.Where( Item => ( Item.Left == left && Item.Right == right ) ).FirstOrDefault() );

			if( isNew == true )
			{
				Items.Add( new Item
				{
					Left = left,
					Right = right,
					Count = count,
				} );

				if( save == true )
				{
					Save();
				}
			}

			return isNew;
		}




		/// <summary>
		/// Deletes one or several items from the collection
		/// </summary>
		/// <param name="selectedItems"></param>
		internal void DeleteItems( IList selectedItems )
		{
			// Copy references to the selected items to avoid enumeration collisions
			var ls = new List<Item>();

			foreach( Item item in selectedItems )
			{
				if( Items.Contains( item ) == true )
				{
					ls.Add( item );
				}
			}

			foreach( Item item in ls )
			{
				Items.Remove( item );
			}

			Save();
		}




		/// <summary>
		/// Sorts the items in the collection by counts of usage, from most to least used
		/// </summary>
		internal void SortByCount( bool sortedByCount )
		{
			SortedByCount = sortedByCount;

			if( SortedByCount == true )
			{
				var temp = new ObservableCollection<Item>( Items.OrderByDescending( item => item.Count ) );

				Items.Clear();

				foreach( var item in temp )
				{
					Items.Add( item );
				}

				Save();
			}
		}




		/// <summary>
		/// Repositions selected items up/down in the collection by one position
		/// </summary>
		internal void MoveSelectedItems( IList selectedItems, Key key )
		{
			var hasChanged = Items.MoveItems( selectedItems, key );

			if( hasChanged == true )
			{
				Save();
			}
		}
	}
}
