
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Graphics.Imaging;

using Windows.Storage.Streams;

using Windows.UI.Core;
using Windows.UI.Xaml;

using Windows.UI.Xaml.Media.Imaging;
using DJIDemo.AIModel;
using DJIDemo.Controls;
using DJIDemo.Tool;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace DJIDemo
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private CoreDispatcher Dispatcher;
        private MainPage parent;
        private Task runProcessTask = null;
        private ProcessWithONNX processWithONNX = null;

        public MainPageViewModel(CoreDispatcher dispatcher, MainPage page)
        {
            this.Dispatcher = dispatcher;
            parent = page;
            page.windowsSDKManager.InitializeWindowsSDK(this);
            processWithONNX = new ProcessWithONNX();
            InitializeAIComponents();

            InitializePeripheral();

        }

        public CoreDispatcher GetDispatcher()
        {
            return Dispatcher;
        }

        public MainPage GetMainPage()
        {
            return parent;
        }

        public void ShowVideo(byte[] data, int witdth, int height)
        {
            DjiClient_FrameArived(WindowsRuntimeBufferExtensions.AsBuffer(data, 0, data.Length), (uint)witdth, (uint)height, 0);
        }

        private void InitializeAIComponents()
        {
            //Initialize AI Model here

        }

        private void InitializePeripheral()
        {
            //Initialize your peripheral here;

        }

        public async void ShowMessagePopup(string message)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    NotifyPopup notifyPopup = new NotifyPopup(message);
                    notifyPopup.Show();
                }
                catch (Exception)
                {

                }
            });


        }


        public void CamMove(double pitch)
        {

            parent.windowsSDKManager.CamMove(pitch);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisepropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (Dispatcher.HasThreadAccess)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }

        private bool isConnected = false;
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                isConnected = value;
                RaisepropertyChanged();
                RaisepropertyChanged(nameof(ConnectedStatus));
                RaisepropertyChanged(nameof(ControlsVisible));
                GimbleAngle = 0;
            }
        }

        private bool isFlying = false;
        public bool IsFlying
        {
            get => isFlying;
            set
            {
                isFlying = value;
                RaisepropertyChanged();
            }
        }

        public string ConnectedStatus
        {
            get
            {
                if (IsConnected)
                {
                    return "Receiving...";
                }
                else
                {
                    return "Connecting...";
                }
            }
        }

        private int gimbleAngle = 0;
        private int gimbleDelay = 100;
        DateTime lastGimbleUpdate = DateTime.UtcNow.AddMinutes(-1);
        public int GimbleAngle
        {
            get
            {
                return gimbleAngle;
            }
            set
            {
                gimbleAngle = value;
                RaisepropertyChanged();
                // only send a gimble change every gimbleDelay ms to limit noise
                var diff = (int)(DateTime.UtcNow - lastGimbleUpdate).TotalMilliseconds;
                if (diff > gimbleDelay)
                {
                    lastGimbleUpdate = DateTime.UtcNow;
                    Task.Delay(gimbleDelay).ContinueWith((x) =>
                    {
                        parent.windowsSDKManager.SetGimbleAngle(gimbleAngle);
                    });
                }
            }
        }

        private double satelite;

        public double Satelite
        {
            get { return satelite; }
            set
            {
                satelite = value;
                RaisepropertyChanged();
            }
        }

        private double longitude;

        public double Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;
                RaisepropertyChanged();
            }
        }

        private double latitude;

        public double Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;
                RaisepropertyChanged();
            }
        }


        private double altitude;

        public double Altitude
        {
            get { return altitude; }
            set
            {
                altitude = value;
                RaisepropertyChanged();
            }
        }

        private double velocity;

        public double Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                RaisepropertyChanged();
            }
        }

        private double heading;

        public double Heading
        {
            get { return heading; }
            set
            {
                heading = value;
                RaisepropertyChanged();
            }
        }



        private double gpslevel;

        public double GPSLevel
        {
            get { return gpslevel; }
            set
            {
                gpslevel = value;
                RaisepropertyChanged();
            }
        }

        private string dronename;

        public string DroneName
        {
            get { return dronename; }
            set
            {
                dronename = value;
                RaisepropertyChanged();
            }
        }



        private string batteryremaining;

        public string BatteryRemaining
        {
            get { return batteryremaining; }
            set
            {
                batteryremaining = value;
                RaisepropertyChanged();
            }
        }

        private double distancelimit;

        public double DistanceLimit
        {
            get { return distancelimit; }
            set
            {
                distancelimit = value;
                RaisepropertyChanged();
            }
        }

        private double heightlimit;

        public double HeightLimit
        {
            get { return heightlimit; }
            set
            {
                heightlimit = value;
                RaisepropertyChanged();
            }
        }

        private string flightmode;

        public string FlightMode
        {
            get { return flightmode; }
            set
            {
                flightmode = value;
                RaisepropertyChanged();
            }
        }


        private string batterytemperature;

        public string BatteryTemperature
        {
            get { return batterytemperature; }
            set
            {
                batterytemperature = value;
                RaisepropertyChanged();
            }
        }

        private string videoresolution;

        public string VideoResolution
        {
            get { return videoresolution; }
            set
            {
                videoresolution = value;
                RaisepropertyChanged();
            }
        }

        private string framerate;

        public string FrameRate
        {
            get { return framerate; }
            set
            {
                framerate = value;
                RaisepropertyChanged();
            }
        }


     

        private string wifissid;

        public string WiFiSSID
        {
            get { return wifissid; }
            set
            {
                wifissid = value;
                RaisepropertyChanged();
            }
        }

        

        public Visibility ControlsVisible
        {
            get
            {
                if (IsConnected)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }





        private WriteableBitmap videoSource;

        public WriteableBitmap VideoSource
        {
            get { return videoSource; }
            set
            {
                if (videoSource != value)
                {
                    videoSource = value;
                    RaisepropertyChanged();
                }
            }
        }



        public void DjiClient_VelocityChanged(double X, double Y, double Z)
        {
            double airSpeed = X * X + Y * Y + Z * Z;
            airSpeed = Math.Abs(airSpeed) > 0.0001 ? Math.Sqrt(airSpeed) : 0;
            Velocity = airSpeed;
        }

        public void DjiClient_AttitudeChanged(double pitch, double yaw, double roll)
        {
            Heading = yaw;
        }

        public void DjiClient_AltitudeChanged(double newValue)
        {
            Altitude = newValue;
        }

        public void DjiClient_SateliteChanged(double newValue)
        {
            Satelite = newValue;
        }
        public void DjiClient_LongitudeChanged(double newValue)
        {
            Longitude = newValue;
        }
        public void DjiClient_LatitudeChanged(double newValue)
        {
            Latitude = newValue;
        }

        public void DjiClient_GSPLevelChanged(double newValue)
        {
            GPSLevel = newValue;
        }

        public void DjiClient_DroneNameChanged(string newValue)
        {
            DroneName = newValue;
        }


        public void DjiClient_BatteryRemainingChanged(string newValue)
        {
            BatteryRemaining = newValue;
        }

        public void DjiClient_BatteryTemperatureChanged(string newValue)
        {
            BatteryTemperature = newValue;
        }

        public void DjiClient_DistanceLimitChanged(double newValue)
        {
            DistanceLimit = newValue;
        }
        public void DjiClient_HeightLimitChanged(double newValue)
        {
            HeightLimit = newValue;
        }
        public void DjiClient_FlightModeChanged(string newValue)
        {
            FlightMode = newValue;
        }
        public void DjiClient_WiFiSSIDChanged(string newValue)
        {
            WiFiSSID = newValue;
        }
        public void DjiClient_VideoResolutionChanged(string newValue)
        {
            VideoResolution = newValue;
        }
        public void DjiClient_FrameRateChanged(string newValue)
        {
            FrameRate = newValue;
        }




        public async void DjiClient_FrameArived(IBuffer buffer, uint width, uint height, ulong timeStamp)
        {
            if (IsConnected == false) IsConnected = true;

            if (runProcessTask == null || runProcessTask.IsCompleted)
            {
                // Do not forget to dispose it! In this sample, we dispose it in ProcessSoftwareBitmap
                try
                {
                    SoftwareBitmap bitmapToProcess = SoftwareBitmap.CreateCopyFromBuffer(buffer,
                            BitmapPixelFormat.Bgra8, (int)width, (int)height, BitmapAlphaMode.Premultiplied);
                    runProcessTask = ProcessSoftwareBitmap(bitmapToProcess, timeStamp);
                }
                catch (Exception)
                {
                }
            }

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    if (VideoSource == null || VideoSource.PixelWidth != width || VideoSource.PixelHeight != height)
                    {
                        VideoSource = new WriteableBitmap((int)width, (int)height);
                    }

                    buffer.CopyTo(VideoSource.PixelBuffer);

                    VideoSource.Invalidate();
                }
                catch (Exception)
                {
                }
            });
        }

        public void PickPictureFromLocal()
        {
            ReadFromLocal.Loadpicture(this);
        }

        private static string GetStandTimeStr()
        {
            DateTime now = DateTime.Now;
            var ss = now.GetDateTimeFormats();


            string ret = String.Format
                (
                "{0,4:D4}{1,2:D2}{2,2:D2}T{3,2:D2}{4,2:D2}{5,2:D2}Z",
                now.Year,
                now.Month,
                now.Day,
                now.Hour,
                now.Minute,
                now.Second
                );
            return ret;
        }


        private async Task ProcessSoftwareBitmap(SoftwareBitmap bitmap, ulong timeStamp)
        {
            try
            {
                if (bitmap.PixelHeight != bitmap.PixelWidth)
                {
                    int destWidthAndHeight = 416;
                    using (var resourceCreator = CanvasDevice.GetSharedDevice())
                    using (var canvasBitmap = CanvasBitmap.CreateFromSoftwareBitmap(resourceCreator, bitmap))
                    using (var canvasRenderTarget = new CanvasRenderTarget(resourceCreator, destWidthAndHeight, destWidthAndHeight, canvasBitmap.Dpi))
                    using (var drawingSession = canvasRenderTarget.CreateDrawingSession())
                    using (var scaleEffect = new ScaleEffect())
                    {
                        scaleEffect.Source = canvasBitmap;
                        scaleEffect.Scale = new System.Numerics.Vector2((float)destWidthAndHeight / (float)bitmap.PixelWidth, (float)destWidthAndHeight / (float)bitmap.PixelHeight);
                        drawingSession.DrawImage(scaleEffect);
                        drawingSession.Flush();

                        var sbp = SoftwareBitmap.CreateCopyFromBuffer(canvasRenderTarget.GetPixelBytes().AsBuffer(), BitmapPixelFormat.Bgra8, destWidthAndHeight, destWidthAndHeight, BitmapAlphaMode.Premultiplied);

                        
                        await processWithONNX.ProcessSoftwareBitmap(sbp, timeStamp, this);
                    }
                }
                else
                {
                    await processWithONNX.ProcessSoftwareBitmap(bitmap, timeStamp, this);

                }



            }
            catch (Exception e)
            {
                string ss = e.Message;
                await Task.Delay(1000);
            }
            finally
            {

                bitmap.Dispose();
            }
        }
    }
}
