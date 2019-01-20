using CustomVision;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DJIDemo.AIModel
{
    class ProcessWithONNX
    {
        SolidColorBrush _fillBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
        SolidColorBrush _lineBrushRed = new SolidColorBrush(Windows.UI.Colors.Red);
        SolidColorBrush _lineBrushGreen = new SolidColorBrush(Windows.UI.Colors.Green);
        double _lineThickness = 2.0;
        StorageFile file = null;


        ObjectDetection objectDetection ;
        public ProcessWithONNX()
        {
            List<String> labels = new List<String> {  "aeroplane", "bicycle", "bird", "boat", "bottle",
                "bus", "car", "cat", "chair", "cow",
                "diningtable", "dog", "horse", "motorbike", "person",
                "pottedplant", "sheep", "sofa", "train", "tvmonitor" };
            objectDetection = new ObjectDetection(labels,20,0.4F,0.45F);
 
        }
    
        public  async Task<string> ProcessSoftwareBitmap(SoftwareBitmap bitmap, ulong timeStamp, MainPageViewModel viewmodel)
        {

            string ret = "";
            if (!Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.Media.VideoFrame", "CreateWithSoftwareBitmap"))
            {
                return "";
            }
            //Convert SoftwareBitmap  into VideoFrame
            using (VideoFrame frame = VideoFrame.CreateWithSoftwareBitmap(bitmap))
            {

                try
                {
                    if (file == null)
                    {
                        file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///AIModel/TinyYOLO.onnx"));
                        await objectDetection.Init(file);
                    }



                    var output = await objectDetection.PredictImageAsync(frame);

                   
                    if (output != null)
                    {

                        UpdateResult(output, viewmodel, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight);

                    }



                }
                catch (Exception e)
                {
                    string s = e.Message;
                    viewmodel.ShowMessagePopup(e.Message);
                }

            }

            return ret;
        }

        private  async void UpdateResult(IList<PredictionModel> outputlist, MainPageViewModel viewmodel, uint pixelWidth, uint pixelHeight)
        {
            try
            {




                await viewmodel.GetDispatcher().RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var overlayCanvas = viewmodel.GetMainPage().GetCanvas();

                    var VideoActualWidth = (uint)viewmodel.GetMainPage().GetWidth();
                    var VideoActualHeight = (uint)viewmodel.GetMainPage().GetHeight();


                    overlayCanvas.Children.Clear();



                    foreach (var output in outputlist)
                    {

                        var box = output.BoundingBox;

                       
                        double x = (double)Math.Max(box.Left, 0);
                        double y = (double)Math.Max(box.Top, 0);
                        double w = (double)Math.Min(1 - x, box.Width);
                        double h = (double)Math.Min(1 - y, box.Height);



                        string boxTest = output.TagName;



                        x = VideoActualWidth * x;
                        y = VideoActualHeight * y;
                        w = VideoActualWidth * w;
                        h = VideoActualHeight * h;
                        
                        var rectStroke = boxTest == "person"? _lineBrushGreen: _lineBrushRed;

                        var r = new Windows.UI.Xaml.Shapes.Rectangle
                        {
                            Tag = box,
                            Width = w,
                            Height = h,
                            Fill = _fillBrush,
                            Stroke = rectStroke,
                            StrokeThickness = _lineThickness,
                            Margin = new Thickness(x, y, 0, 0)
                        };



                        var tb = new TextBlock
                        {
                            Margin = new Thickness(x + 4, y + 4, 0, 0),
                            Text = $"{boxTest} ({Math.Round(output.Probability, 4)})",
                            FontWeight = FontWeights.Bold,
                            Width = 126,
                            Height = 21,
                            HorizontalTextAlignment = TextAlignment.Center
                        };

                        var textBack = new Windows.UI.Xaml.Shapes.Rectangle
                        {
                            Width = 134,
                            Height = 29,
                            Fill = rectStroke,
                            Margin = new Thickness(x, y, 0, 0)
                        };

                        overlayCanvas.Children.Add(textBack);
                        overlayCanvas.Children.Add(tb);
                        overlayCanvas.Children.Add(r);



                    }
                    // viewmodel.GetMainPage().GetDebugLog().Text = strdebuglog;
                });
            }
            catch (Exception ex)
            {
                viewmodel.ShowMessagePopup(ex.Message);

            }

        }
    }
}
