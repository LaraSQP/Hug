using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace Hug
{
	[ Export( typeof( ISuggestedActionsSourceProvider ) ) ]
	[ Name( "Test Suggested Actions" ) ]
	[ ContentType( "text" ) ]
	internal class SuggestedActionsSourceProvider : ISuggestedActionsSourceProvider
	{
		[ Import( typeof( ITextStructureNavigatorSelectorService ) ) ]
		internal ITextStructureNavigatorSelectorService NavigatorService
		{
			get; set;
		}



		/// <summary>
		/// Creates the provider if there is an active document
		/// </summary>
		public ISuggestedActionsSource CreateSuggestedActionsSource( ITextView textView, ITextBuffer textBuffer )
		{
			if( textBuffer == null
				|| textView == null )
			{
				return null;
			}

			return new SuggestedActionsSource( this, textView, textBuffer );
		}
	}
}
