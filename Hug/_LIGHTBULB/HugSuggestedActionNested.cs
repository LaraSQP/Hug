using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
	internal class HugSuggestedActionNested : ISuggestedAction
	{
		// Public properties
		public bool			HasActionSets => true;
		public ImageMoniker IconMoniker => default;
		public string		IconAutomationText => null;
		public string		InputGestureText => null;
		public bool			HasPreview => false;
		public string DisplayText
		{
			get; private set;
		}

		// Private properties
		public ImmutableArray<SuggestedActionSet> Nested
		{
			get; private set;
		}

		/// <summary>
		/// Constructor
		/// Must use the *obsolete* constructor of SuggestedActionSet or an exception is thrown when using ctrl+right arrow
		/// </summary>
		[ Obsolete ]
		public HugSuggestedActionNested( List<ISuggestedAction> nested )
		{
			// Must use an immutable array, created indirectly (as shown below), or the call to
			// the method GetActionSetsAsync will throw an exception
			var ia = ImmutableArray.CreateBuilder<SuggestedActionSet>( nested.Count );

			ia.Add( new SuggestedActionSet( nested ) );

			Nested		= ia.ToImmutable();
			DisplayText = "More Hugs...";
		}




		public Task<IEnumerable<SuggestedActionSet> > GetActionSetsAsync( CancellationToken cancellationToken )
		{
			return Task.FromResult<IEnumerable<SuggestedActionSet> >( Nested );
		}




		public Task<object> GetPreviewAsync( CancellationToken cancellationToken )
		{
			return null;
		}




		public void Invoke( CancellationToken cancellationToken )
		{
			// Nothing to invoke
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