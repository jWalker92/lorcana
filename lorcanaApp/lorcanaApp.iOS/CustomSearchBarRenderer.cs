using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SearchBar), typeof(lorcanaApp.iOS.CustomSearchBarRenderer))]
namespace lorcanaApp.iOS
{
    public class CustomSearchBarRenderer : SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var searchbar = (UISearchBar)Control;
                searchbar.SearchTextField.LeftView.TintColor = UIColor.White;

            }
        }
    }
}