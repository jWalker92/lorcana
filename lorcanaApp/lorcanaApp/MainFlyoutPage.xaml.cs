using System;
using System.Collections.Generic;

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

