using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace DJIDemo.Tool
{
    class ReadFromLocal
    {
        public static async void Loadpicture(MainPageViewModel viewModel)
        {
            FileOpenPicker openFile = new FileOpenPicker();
            openFile.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openFile.ViewMode = PickerViewMode.List;
            openFile.FileTypeFilter.Add(".png");
            openFile.FileTypeFilter.Add(".jpg");
            openFile.FileTypeFilter.Add(".bmp");

            // 选取单个文件
            StorageFile file = await openFile.PickSingleFileAsync();
            if (file != null)
            {
                RunMLAndShow(file,viewModel);
            }

        }

        private static async void  RunMLAndShow(StorageFile file,MainPageViewModel viewModel)
        {
            using (IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read))
            {
                var debmp = await BitmapDecoder.CreateAsync(readStream);

                var pix = await debmp.GetPixelDataAsync();
                viewModel.DjiClient_FrameArived(pix.DetachPixelData().AsBuffer(), debmp.PixelWidth, debmp.PixelHeight, 0);

            }
        }
    }
}
