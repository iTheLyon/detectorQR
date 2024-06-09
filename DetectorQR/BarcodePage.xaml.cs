using System.Threading.Tasks;
using ZXing.Net.Maui.Controls;
using ZXing.Net.Maui;

namespace DetectorQR;

public partial class BarcodePage : ContentPage
{
    public event Action<byte[]> ImageStreamGenerated;

    public BarcodePage()
	{
		InitializeComponent();

        barcodeView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            Multiple = true
        };
    }

    protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        foreach (var barcode in e.Results)
            Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");

        var first = e.Results?.FirstOrDefault();
        if (first is not null)
        {
            Dispatcher.Dispatch(async () =>
            {
                // Update BarcodeGeneratorView
                barcodeGenerator.ClearValue(BarcodeGeneratorView.ValueProperty);
                barcodeGenerator.Format = first.Format;
                barcodeGenerator.Value = first.Value;

                // Update Label
                ResultLabel.Text = $"Barcodes: {first.Format} -> {first.Value}";
                
                await GenerateAndSaveBarcodeImageAsync(barcodeGenerator, "barcode.png");
            });
        }
    }

    void SwitchCameraButton_Clicked(object sender, EventArgs e)
    {
        barcodeView.CameraLocation = barcodeView.CameraLocation == CameraLocation.Rear ? CameraLocation.Front : CameraLocation.Rear;
    }

    void TorchButton_Clicked(object sender, EventArgs e)
    {
        barcodeView.IsTorchOn = !barcodeView.IsTorchOn;
    }

    public async Task GenerateAndSaveBarcodeImageAsync(BarcodeGeneratorView barcodeGeneratorView, string fileName)
    {
        // Renderizar el contenido visual en una imagen
        var imageBytes = await RenderBarcodeToImageAsync(barcodeGeneratorView);
        //var imageStream = ByteArrayToStream(imageBytes);

        //// Guardar la imagen en el almacenamiento local
        //await SaveBarcodeImageAsync(imageStream, fileName);
        // Esperar 3 segundos
        await Task.Delay(3000);

        ImageStreamGenerated?.Invoke(imageBytes);        

        await Navigation.PopAsync();
    }

    //public async Task SaveBarcodeImageAsync(byte[] imageBytes, string fileName)
    //{
    //    // Obtener el directorio de almacenamiento local
    //    var localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
    //    //var downloadsPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, fileName);
    //    // Guardar la imagen en el archivo
    //    //using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write);
    //    await File.WriteAllBytesAsync(localPath, imageBytes);

    //    FotoProducto.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
    //}

    public async Task<byte[]> RenderBarcodeToImageAsync(BarcodeGeneratorView barcodeGeneratorView)
    {
        // Renderizar el contenido visual del BarcodeGeneratorView en una imagen
        var screenshotResult = await barcodeGeneratorView.CaptureAsync();

        // Convertir la captura en un stream
        using var stream = await screenshotResult.OpenReadAsync();

        // Leer el stream en un array de bytes
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

  
}