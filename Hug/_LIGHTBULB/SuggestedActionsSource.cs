using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;

namespace Hug
{
	internal class SuggestedActionsSource : ISuggestedActionsSource
	{
		// Events
		public event EventHandler<EventArgs> SuggestedActionsChanged;

		// Properties
		private ITextView TextView
		{
			get; set;
		}

		/// <summary>
		/// Constructor
		/// </summary>

		public SuggestedActionsSource( SuggestedActionsSourceProvider suggestedActionsSourceProvider,
									   ITextView textView,
									   ITextBuffer textBuffer )
		{
			TextView = textView;

			// This constructor is unnecessarily called every time a file is opened
			HugTags.I.Load();
		}




		/// <summary>
		/// Called by the editor to find out whether to display the light bulb.
		/// </summary>
		public Task<bool> HasSuggestedActionsAsync( ISuggestedActionCategorySet requestedActionCategories,
													SnapshotSpan range,
													CancellationToken cancellationToken )
		{
			return Task.Factory.StartNew( ( ) =>
			{
				return ( HugTags.I.Items.Count > 0
						 && TextView.Selection.IsEmpty == false );
			} );
		}




		/// <summary>
		/// Called by the editor to obtain an array of suggested actions.<para/>
		/// Must use the *obsolete* constructor of SuggestedActionSet or an exception is thrown when using ctrl+right arrow
		/// </summary>
		[ Obsolete ]
		public IEnumerable<SuggestedActionSet> GetSuggestedActions( ISuggestedActionCategorySet requestedActionCategories,
																	SnapshotSpan range,
																	CancellationToken cancellationToken )
		{
			var actions = new List<SuggestedActionSet>();

			// Must perform the same check here as in HasSuggestedActionsAsync as both methods are called (this one 1st, btw + wtf)
			if( HugTags.I.Items.Count == 0
				|| TextView.Selection.IsEmpty == true )
			{
				return actions;
			}

			var snapshotSpan	= TextView.Selection.StreamSelectionSpan.SnapshotSpan;
			var extent			= new TextExtent( snapshotSpan, true );
			var trackingSpan	= range.Snapshot.CreateTrackingSpan( extent.Span, SpanTrackingMode.EdgeInclusive );

			// Create as many entries as there are tags
			var ls		= new List<ISuggestedAction>();
			var nested	= new List<ISuggestedAction>();

			foreach( var item in HugTags.I.Items )
			{
				var hugAction = new HugSuggestedAction( trackingSpan, item );

				// If there are more than 5 tags, put them in a submenu to avoid crowding 'Quick Actions'
				if( ls.Count < 5 )
				{
					ls.Add( hugAction );
				}
				else
				{
					nested.Add( hugAction );
				}
			}

			// Add suggested actions
			actions.Add( new SuggestedActionSet( ls ) );

			if( nested.Count > 0 )
			{
				// There are too many to display, so add the remaining ones in a submenu
				actions.Add( new SuggestedActionSet( new List<ISuggestedAction>
				{
					new HugSuggestedActionNested( nested )
				} ) );
			}

			return actions;
		}




		/// <summary>
		/// This is a provider doesn't participate in LightBulb telemetry
		/// </summary>
		public bool TryGetTelemetryId( out Guid telemetryId )
		{
			telemetryId = Guid.Empty;
			return false;
		}




		/// <summary>
		/// Disposes of any resources
		/// </summary>
		public void Dispose()
		{
		}
	}
}