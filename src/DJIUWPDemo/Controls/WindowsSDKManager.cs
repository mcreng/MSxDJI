using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DJIVideoParser;
using DJI.WindowsSDK;
using System.Runtime.InteropServices.WindowsRuntime;
using DJI.WindowsSDK.Components;

namespace DJIDemo.Controls
{
    public class WindowsSDKManager
    {
        ComponentManager componentManager = null;
        FlightControllerHandler flightControllerHandler = null;
        Parser videoParser = null;
        GimbalHandler gimbalHandler = null;
        ProductHandler productHandler = null;
        FlightAssistantHandler flightAssistantHandler = null;
        VirtualRemoteController virtualRemoteController = null;
        Parser parser = null;
        double alnum = 0;
        static MainPageViewModel viewModel = null;
        BatteryHandler batteryHandler = null;
        WiFiHandler wiFiHandler = null;
        CameraHandler cameraHandler = null;
        public void InitializeWindowsSDK(MainPageViewModel viewmodel)
        {
            viewModel = viewmodel;
            InitializeSDK();
            InitializeParser();
           
        }

        public void SetJoyStickValue(float throttle, float roll, float pitch, float yaw)
        {
            if (viewModel.IsConnected)
            {
                if (virtualRemoteController != null)
                {
                    virtualRemoteController.UpdateJoystickValue(throttle, roll, pitch, yaw);
                }
            }
        }

        public void SetGimbleAngle(int deg)
        {
            if (viewModel.IsConnected)
            {
                //Tobe implement
            }
        }

        private int InitializeSDK()
        {
            int ret = -1;
            DJISDKManager.Instance.SDKRegistrationStateChanged += OnSDKRegistrationStateChanged;
            DJISDKManager.Instance.RegisterApp("<input your DJI key here>");

            virtualRemoteController = DJISDKManager.Instance.VirtualRemoteController;
            componentManager = DJISDKManager.Instance.ComponentManager;

            if (componentManager != null)
            {
                flightAssistantHandler = componentManager.GetFlightAssistantHandler(0, 0);
                flightControllerHandler = componentManager.GetFlightControllerHandler(0, 0);
                if (flightControllerHandler != null)
                {
                    flightControllerHandler.ConnectionChanged += OnConnectStatusChanges;
                    flightControllerHandler.IsFlyingChanged += OnFlyingStatusChanges;
                    flightControllerHandler.VelocityChanged += OnVelocityChanges;
                    flightControllerHandler.AttitudeChanged += OnAttitudeChanges;
                    flightControllerHandler.AltitudeChanged += OnAltitudeChanges;
                    flightControllerHandler.SatelliteCountChanged += OnSatelliteCountChanged;
                    flightControllerHandler.GPSSignalLevelChanged += OnGPSSignalLevelChanged;
                    flightControllerHandler.AircraftLocationChanged += OnAircraftLocationChanged;
                    flightControllerHandler.AircraftNameChanged += OnAircraftNameChanged;
                    flightControllerHandler.FlightModeChanged += OnFlightModeChanged;
                    flightControllerHandler.HeightLimitChanged += OnHeightLimitChanged;
                    flightControllerHandler.DistanceLimitChanged += OnDistanceLimitChanged;
                    flightControllerHandler.AircraftNameChanged += OnAircraftNameChanged;
                    
                }

                gimbalHandler = componentManager.GetGimbalHandler(0, 0);
                productHandler = componentManager.GetProductHandler(0);
                batteryHandler = componentManager.GetBatteryHandler(0, 0);
                wiFiHandler = componentManager.GetWiFiHandler(0, 0);
                cameraHandler = componentManager.GetCameraHandler(0, 0);
                


                if (batteryHandler != null)
                {

                    batteryHandler.ChargeRemainingInPercentChanged += OnChargeRemainingInPercentChanged;
                    batteryHandler.BatteryTemperatureChanged += OnBatteryTemperatureChanged;
                }
                if (wiFiHandler != null)
                {
                    wiFiHandler.WiFiSSIDChanged += OnWiFiSSIDChanged;
                }
                if (cameraHandler != null)
                {
                    cameraHandler.VideoResolutionAndFrameRateChanged += OnVideoResolutionAndFrameRateChanged;
                    VideoResolutionAndFrameRate vf = new VideoResolutionAndFrameRate();
                    vf.frameRate = VideoFrameRate.RATE_24FPS;
                    vf.resolution = VideoResolution.RESOLUTION_1280x720;
                    cameraHandler.SetVideoResolutionAndFrameRateAsync(vf);
                   
                }

            }
            ret = CheckSDKIntializeStatsu();

          


            return ret;
        }

        private  void OnSDKRegistrationStateChanged(SDKRegistrationState state, SDKError errorCode)
        {
            if (state != SDKRegistrationState.Succeeded)
            {
                viewModel.ShowMessagePopup(errorCode.ToString());
            }
        }

        private void OnVideoResolutionAndFrameRateChanged(object sender, VideoResolutionAndFrameRate? value)
        {
            if (value != null)
            {
                viewModel.DjiClient_VideoResolutionChanged(value.Value.resolution.ToString());
                viewModel.DjiClient_FrameRateChanged(value.Value.frameRate.ToString());
                cameraHandler.VideoResolutionAndFrameRateChanged += OnVideoResolutionAndFrameRateChanged;
            }
        }

        private void OnWiFiSSIDChanged(object sender, StringMsg? value)
        {
            if (value != null)
            {
                viewModel.DjiClient_WiFiSSIDChanged(value.Value.value);
            }
            
        }

        private void OnBatteryTemperatureChanged(object sender, DoubleMsg? value)
        {
            if (value != null)
            {
                string str = value.Value.value.ToString() + "℃";
                viewModel.DjiClient_BatteryTemperatureChanged(str);
            }
            
        }

        private void OnDistanceLimitChanged(object sender, IntMsg? value)
        {
            if (value != null)
                viewModel.DjiClient_DistanceLimitChanged(value.Value.value);
           
        }

        private void OnHeightLimitChanged(object sender, IntMsg? value)
        {
            if (value != null)
                viewModel.DjiClient_HeightLimitChanged(value.Value.value);
            
        }

        private void OnFlightModeChanged(object sender, FCFlightModeMsg? value)
        {
            if (value != null)
                viewModel.DjiClient_FlightModeChanged(value.Value.value.ToString());
            
        }

        private void OnChargeRemainingInPercentChanged(object sender, IntMsg? value)
        {
            if (value != null)
            {
                string str = value.Value.value.ToString() + "%";
                viewModel.DjiClient_BatteryRemainingChanged(str);

            }
        }

        private void OnAircraftNameChanged(object sender, StringMsg? value)
        {
            if(value !=null)
            viewModel.DjiClient_DroneNameChanged(value.Value.value);
        }

        private void OnAircraftLocationChanged(object sender, LocationCoordinate2D? value)
        {
            if (value != null)
            {
                viewModel.DjiClient_LatitudeChanged(value.Value.latitude);
                viewModel.DjiClient_LongitudeChanged(value.Value.longitude);
            }
        }

        private void OnGPSSignalLevelChanged(object sender, FCGPSSignalLevelMsg? value)
        {
            if (value != null)
            {
                double level = -1;
                switch (value.Value.value)
                {
                    case FCGPSSignalLevel.LEVEL_0: level = 0; break;
                    case FCGPSSignalLevel.LEVEL_1: level = 1; break;
                    case FCGPSSignalLevel.LEVEL_2: level = 2; break;
                    case FCGPSSignalLevel.LEVEL_3: level = 3; break;
                    case FCGPSSignalLevel.LEVEL_4: level = 4; break;
                    case FCGPSSignalLevel.LEVEL_5: level = 5; break;
                    default:
                     break;

                }
                viewModel.DjiClient_GSPLevelChanged(level);
            }
        }

      

        private void OnSatelliteCountChanged(object sender, IntMsg? value)
        {
            if (value != null)
                viewModel.DjiClient_SateliteChanged(value.Value.value);
        }

        void OnVideoPush(VideoFeed sender, [ReadOnlyArray] ref byte[] bytes)
        {
            videoParser.PushVideoData(0, 0, bytes, bytes.Length);
        }



        public async void CamMove(double pitch)
        {
            //GimbalAngleRotation
            GimbalAngleRotation value = new GimbalAngleRotation();
            value.mode = GimbalAngleRotationMode.ABSOLUTE_ANGLE;

            value.roll = 0;
            value.yaw = 0;
            value.pitch = pitch;
            value.pitchIgnored = false;
            value.rollIgnored = false;
            value.yawIgnored = false;
            value.duration = 2;
            await gimbalHandler.RotateByAngleAsync(value);

        }


        private void OnAltitudeChanges(object sender, DoubleMsg? value)
        {
            if (value != null)
            {
                alnum += value.Value.value;
                viewModel.DjiClient_AltitudeChanged(alnum);
            }
        }

        private void OnAttitudeChanges(object sender, Attitude? value)
        {
            if (value != null)
                viewModel.DjiClient_AttitudeChanged(value.Value.pitch, value.Value.yaw, value.Value.roll);
        }

        private void OnFlyingStatusChanges(object sender, BoolMsg? value)
        {
            if (value != null)
                viewModel.IsFlying = value.Value.value;
        }

        private void OnConnectStatusChanges(object sender, BoolMsg? value)
        {
            if (value != null)
                viewModel.IsConnected = value.Value.value;
          
        }

        private void OnVelocityChanges(object sender, Velocity3D? value)
        {
            if (value != null)
                viewModel.DjiClient_VelocityChanged(value.Value.x, value.Value.y, value.Value.z);
            
        }

        private int CheckSDKIntializeStatsu()
        {
            if (videoParser != null && virtualRemoteController != null && flightAssistantHandler != null && flightControllerHandler != null && gimbalHandler != null && productHandler != null)
                return 0;
            else
                return -1;
        }

        private int InitializeParser()
        {
            int ret = -1;
            videoParser = new Parser();
            videoParser.Initialize();
            videoParser.SetVideoDataCallack(0, 0, viewModel.ShowVideo);
            if (DJISDKManager.Instance.SDKRegistrationResultCode == SDKError.NO_ERROR)
            {
                DJISDKManager.Instance.VideoFeeder.GetPrimaryVideoFeed(0).VideoDataUpdated += OnVideoPush;
                ret = 0;
            }


            return ret;
        }

        private void VideoCallBack(byte[] bytes, uint length)
        {
            //byte[] bytes, uint length
            try
            {
                parser.PushVideoData(0, 0, bytes, (int)length);
            }
            catch (Exception)
            {
            }
        }
    }
}
