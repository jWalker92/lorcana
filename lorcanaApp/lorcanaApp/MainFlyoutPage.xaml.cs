using Xamarin.Forms;

namespace lorcanaApp
{	
	public partial class MainFlyoutPage : FlyoutPage
	{	
		public MainFlyoutPage ()
		{
			InitializeComponent();
			App.NavigationPageInstance = navPage;
		}
	}
}

