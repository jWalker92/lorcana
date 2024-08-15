using System.IO;
using lorcana.Cards;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace lorcanaApp
{
    public static class ImportManager
	{
        internal static void ExportCards(Page p)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                App.FlyoutInstance.IsPresented = false;

                var choice = await p.DisplayActionSheet("Export", "Cancel", null, "Share File", "To Clipboard", "To Dreamborn CSV (Deltas)");
                if (choice == "Share File")
                {
                    var csvContent = CardCollection.ListToCsv(await Database.Instance.GetCardsAsync());
                    var fn = "app_export.csv";
                    var file = Path.Combine(FileSystem.CacheDirectory, fn);
                    File.WriteAllText(file, csvContent);

                    await Share.RequestAsync(new ShareFileRequest
                    {
                        Title = "Collection Export",
                        File = new ShareFile(file)
                    });
                }
                else if (choice == "To Clipboard")
                {
                    var csvContent = CardCollection.ListToCsv(await Database.Instance.GetCardsAsync());
                    await Clipboard.SetTextAsync(csvContent);
                }
                else if (choice == "To Dreamborn CSV (Deltas)")
                {
                    var csvContent = CardCollection.ListToCsvDreamborn(await Database.Instance.GetCardsAsync());
                    var fn = "deltas_export.csv";
                    var file = Path.Combine(FileSystem.CacheDirectory, fn);
                    File.WriteAllText(file, csvContent);

                    await Share.RequestAsync(new ShareFileRequest
                    {
                        Title = "Collection Export",
                        File = new ShareFile(file)
                    });
                }
            });
        }

        internal static void ImportCards(Page p)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                App.FlyoutInstance.IsPresented = false;
                var importDataStr = string.Empty;
                var choice = await p.DisplayActionSheet("Import", "Cancel", null, "From File", "From Text");
                if (choice == "From File")
                {
                    var pickedFile = await FilePicker.PickAsync();
                    if (pickedFile != null)
                    {
                        importDataStr = File.ReadAllText(pickedFile.FullPath);
                    }
                }
                else if (choice == "From Text")
                {
                    importDataStr = await PasteTextPage.ShowDialog();
                }
                if (string.IsNullOrEmpty(importDataStr))
                {
                    return;
                }
                var importedCollection = new CardCollection();
                importedCollection.InitializeWithCsv(CardListPage.CardLibrary.List, importDataStr, false);
                foreach (var card in importedCollection.List)
                {
                    card.NormalsOnImport = card.Normals;
                    card.FoilsOnImport = card.Foils;
                    await Database.Instance.AddOrReplaceCardAsync(card);
                }
                Database.Instance.FireCollectionChanged();
            });
        }
    }
}

