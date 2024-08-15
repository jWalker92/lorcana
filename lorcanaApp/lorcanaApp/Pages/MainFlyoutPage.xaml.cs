using Xamarin.Forms;

namespace lorcanaApp
{	
	public partial class MainFlyoutPage : FlyoutPage
	{	
		public MainFlyoutPage ()
		{
			InitializeComponent();
			App.NavigationPageInstance = navPage;
            navPage.Pushed += NavPage_PushedOrPopped;
            navPage.Popped += NavPage_PushedOrPopped;
		}

        private void NavPage_PushedOrPopped(object sender, NavigationEventArgs e)
        {
            App.FlyoutInstance.IsGestureEnabled = App.NavigationPageInstance.Navigation.NavigationStack.Count == 1;
        }
    }
}

