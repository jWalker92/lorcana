using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace lorcanaApp
{	
	public partial class PasteTextPage : ContentPage
	{
		public TaskCompletionSource<string> PasteTextResultSource;

		public PasteTextPage ()
		{
			InitializeComponent ();
			PasteTextResultSource = new TaskCompletionSource<string>();
		}

		public static async Task<string> ShowDialog()
		{
			var page = new PasteTextPage();
			await App.NavigationPageInstance.PushAsync(page);
			var result = await page.PasteTextResultSource.Task;
            await App.NavigationPageInstance.PopAsync();
            return result;
		}

        void Cancel_Clicked(System.Object sender, System.EventArgs e)
        {
			PasteTextResultSource.SetResult(null);
        }

        void Save_Clicked(System.Object sender, System.EventArgs e)
        {
            PasteTextResultSource.SetResult(importEditor.Text);
        }
    }
}

