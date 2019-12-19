using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace Hug
{
	internal class HugSuggestedAction : ISuggestedAction
	{
		// Public properties
		public bool HasActionSets => false;
		public ImageMoniker IconMoniker => default;
		public string IconAutomationText => null;
		public string InputGestureText => null;
		public bool HasPreview => false;
		public string DisplayText
		{
			get; private set;
		}

		// Private properties
		private ITrackingSpan TrackingSpan
		{
			get; set;
		}
		private string Tagged
		{
			get; set;
		}
		private ITextSnapshot Snapshot
		{
			get; set;
		}
		private HugTags.Item Item
		{
			get; set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public HugSuggestedAction( ITrackingSpan trackingSpan, HugTags.Item item )
		{
			Item			= item;
			TrackingSpan	= trackingSpan;
			Snapshot		= TrackingSpan.TextBuffer.CurrentSnapshot;
			Tagged			= Item.Left + TrackingSpan.GetText( Snapshot ) + Item.Right;
			DisplayText		= string.Format( "Hug as {0}", Tagged );
		}




		public Task<object> GetPreviewAsync( CancellationToken cancellationToken )
		{
			return null;
		}




		public Task<IEnumerable<SuggestedActionSet> > GetActionSetsAsync( CancellationToken cancellationToken )
		{
			return Task.FromResult<IEnumerable<SuggestedActionSet> >( null );

			// Presumably, the above line is the same as the one below
			//return null;
		}




		public void Invoke( CancellationToken cancellationToken )
		{
			// Count the use of the tag
			HugTags.I.Count( Item );

			// Surround the selection with the tags
			TrackingSpan.TextBuffer.Replace( TrackingSpan.GetSpan( Snapshot ), Tagged );
		}




		public void Dispose()
		{
			// Nothing to dispose
		}




		public bool TryGetTelemetryId( out Guid telemetryId )
		{
			telemetryId = Guid.Empty;
			return false;
		}
	}
}