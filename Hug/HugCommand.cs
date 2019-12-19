using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Windows.Interop;
using EnvDTE80;
using LaraSPQ.Tools;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Hug
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class HugCommand
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0100;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid( "2720e8a1-6147-400a-8289-dd86b59e4887" );

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private AsyncPackage Package
		{
			get; set;
		}
		private static DTE2 Dte
		{
			get; set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HugCommand"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private HugCommand( AsyncPackage package, OleMenuCommandService commandService )
		{
			Package			= package ?? throw new ArgumentNullException( nameof( package ) );
			commandService	= commandService ?? throw new ArgumentNullException( nameof( commandService ) );

			var menuCommandID	= new CommandID( CommandSet, CommandId );
			var menuItem		= new MenuCommand( this.Execute, menuCommandID );

			commandService.AddCommand( menuItem );
		}




		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static HugCommand Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
		{
			get
			{
				return Package;
			}
		}

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <exception cref="OperationCanceledException"></exception>
		public static async Task InitializeAsync( AsyncPackage package, DTE2 dte )
		{
			// Switch to the main thread - the call to AddCommand in Hug's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync( package.DisposalToken );

			Dte = dte;

			OleMenuCommandService commandService = await package.GetServiceAsync( typeof( IMenuCommandService ) ) as OleMenuCommandService;

			Instance = new HugCommand( package, commandService );
		}




		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked.
		/// See the constructor to see how the menu item is associated with this function using
		/// OleMenuCommandService service and MenuCommand class.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		private void Execute( object sender, EventArgs e )
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var dlg		= new TagManager( Package );
			var hwnd	= new IntPtr( Dte.MainWindow.HWnd );
			var window	= ( System.Windows.Window )HwndSource.FromHwnd( hwnd ).RootVisual;

			dlg.Owner = window;

			try
			{
				dlg.ShowDialog();
			}
			catch( InvalidOperationException ex )
			{
				Box.Error( "Unable to display Hug's Tag Manager window, exception:", ex.Message );
			}
		}
	}
}
