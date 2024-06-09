using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace DetectorQR
{
    public partial class MainPage : ContentPage
    {      
        public MainPage()
        {
            InitializeComponent();
           
        }


        private async void NavigateToBarcodePage(object sender, EventArgs e)
        {
            var barcodePage = new BarcodePage();
            barcodePage.ImageStreamGenerated += OnImageStreamGenerated;

            await Navigation.PushAsync(barcodePage);
        }

        private async void OnImageStreamGenerated(byte[] imageBytes)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // Convertir el Stream a una imagen y mostrarla en el Image
                var imageSource = ImageSource.FromStream(() => ByteArrayToStream(imageBytes));
                barcodeImage.Source = imageSource;
                await SaveBarcodeImageAsync(imageBytes, "barcode.png");
            });
        }

        public async Task SaveBarcodeImageAsync(byte[] imageBytes, string fileName)
        {
            // Obtener el directorio de almacenamiento local
            var localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            //var downloadsPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, fileName);            
            // Guardar la imagen en el archivo
            //using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write);
            await File.WriteAllBytesAsync(localPath, imageBytes);
            //using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write);
            //await imageStream.CopyToAsync(fileStream);
        }

        private Stream ByteArrayToStream(byte[] imageBytes)
        {
            return new MemoryStream(imageBytes);
        }

    }

}

