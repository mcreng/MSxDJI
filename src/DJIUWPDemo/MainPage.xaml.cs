using DJIDemo.Controls;
//using DJISDK;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace DJIDemo
{
    public sealed partial class MainPage : Page
    {

        private WriteableBitmap videoSource;

        public WriteableBitmap VideoSource
        {
            get { return videoSource; }
            set
            {
                if (videoSource != value)
                {
                    videoSource = value;
                    // RaisepropertyChanged();
                }
            }
        }


        public WindowsSDKManager windowsSDKManager = new WindowsSDKManager();

        public MainPage()
        {        
            this.InitializeComponent();
            viewModel = new MainPageViewModel(Dispatcher, this);
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        
       


        public Canvas GetCanvas()
        {       
            return MLResult;
        }
        public TextBlock GetTextBlock()
        {
            return mlModelResult;
        }

        public int GetWidth()
        {
            return (int)videoElement.ActualWidth;
        }

        public int GetHeight()
        {
            return (int)videoElement.ActualHeight;
        }


      


        private MainPageViewModel viewModel;
        public MainPageViewModel ViewModel => viewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Setup Joystick listener
            Task.Run(() => { ListenForJoystick(); });      
        }

        

        #region Joystick Controls
        // Take off button
        private async void ButtonTakeOff_Click(object sender, RoutedEventArgs e)
        {
            SetJoyStickValue(new JoyStickValues(-1, -1, -1, 1));
            await Task.Delay(1000);
            SetJoyStickValue(new JoyStickValues(0, 0, 0, 0));
            await Task.Delay(1000);
            SetJoyStickValue(new JoyStickValues(0.3, 0, 0, 0));
            await Task.Delay(800);
            SetJoyStickValue(new JoyStickValues(0, 0, 0, 0));

        }

        // Take off button in Landing mode
        private void ButtonLand_Click(object sender, RoutedEventArgs e)
        {
            SetJoyStickValue(new JoyStickValues(-1, 0, 0, 0));
        }

        // Go home button
        private void ButtonGoHome_Click(object sender, RoutedEventArgs e)
        {
            // todo:add gohome from djiclient
            bool goingHome = ((sender as CircularToggleButton).IsChecked == true);
        }

        private void JoystickLeft_OnJoystickReleased(object sender, JoystickEventArgs e)
        {
            Debug.WriteLine("JSL Released");
            SetJoyStickValue(new JoyStickValues(0, null, null, 0));
        }

        private void JoystickLeft_OnJoystickMoved(object sender, JoystickEventArgs e)
        {
            Debug.WriteLine("JSL Moved");
            SetJoyStickValue(new JoyStickValues(e.YValue, null, null, e.XValue));
        }

        private void JoystickRight_OnJoystickReleased(object sender, JoystickEventArgs e)
        {
            Debug.WriteLine("JSR Released");
            SetJoyStickValue(new JoyStickValues(null, 0, 0, null));
        }

        private void JoystickRight_OnJoystickMoved(object sender, JoystickEventArgs e)
        {
            Debug.WriteLine("JSR Moved");
            SetJoyStickValue(new JoyStickValues(null, e.XValue, e.YValue, null));
        }

        static BlockingCollection<JoyStickValues> JoyStickValuesQueue = new BlockingCollection<JoyStickValues>();

        private void SetJoyStickValue(JoyStickValues newValues)
        {
            JoyStickValuesQueue.TryAdd(newValues);
        }

        private void ListenForJoystick()
        {
            JoyStickValues current = new JoyStickValues(0, 0, 0, 0);
            foreach (var joystickItem in JoyStickValuesQueue.GetConsumingEnumerable())
            {
                current.throttle = joystickItem.throttle ?? current.throttle;
                current.roll = joystickItem.roll ?? current.roll;
                current.pitch = joystickItem.pitch ?? current.pitch;
                current.yaw = joystickItem.yaw ?? current.yaw;
                windowsSDKManager.SetJoyStickValue((float)current.throttle, (float)current.roll, (float)current.pitch, (float)current.yaw);
            }
        }
        #endregion //Joystick Controls
        int CamMoveValue = 0;


        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {

            if (args.VirtualKey == Windows.System.VirtualKey.Down)
            {
                //down
                viewModel.CamMove(--CamMoveValue);

            }
            else if (args.VirtualKey == Windows.System.VirtualKey.Up)
            {
                //up
                viewModel.CamMove(++CamMoveValue);

            }
            //else if (args.VirtualKey == Windows.System.VirtualKey.F)
            //{
            //    viewModel.PickPictureFromLocal();
            //}
           

        }
       
    }

    static class Extensions
    {
        public static float? ConvertJoystickValue(this double? value)
        {
            if (value.HasValue)
                return (float?)(value * 100);
            return null;
        }
    }

    public class JoyStickValues
    {
        public JoyStickValues(double? throttle, double? roll, double? pitch, double? yaw)
        {
            this.throttle = throttle.ConvertJoystickValue();
            this.roll = roll.ConvertJoystickValue();
            this.pitch = pitch.ConvertJoystickValue();
            this.yaw = yaw.ConvertJoystickValue();
        }

        public float? throttle;
        public float? roll;
        public float? pitch;
        public float? yaw;
    }

}
