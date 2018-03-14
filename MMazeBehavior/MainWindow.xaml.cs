using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace MMazeBehavior
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private data members

        private int _camera_frame_width = 1280;
        private int _camera_frame_height = 720;
        private int _overlay_height = 200;
        private List<int> _box_centers = new List<int>() { 213, 426, 639, 853, 1066 };
        private List<int> _box_lefts = new List<int>() { 163, 376, 589, 803, 1016 };
        private int _box_width = 100;
        private int _box_height = 100;
        private int _box_top = 50;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            //Load in the configuration file and the list of stages
            MMazeConfiguration.GetInstance().LoadConfigurationFile();
            MMazeConfiguration.GetInstance().LoadStages();

            //Look for the camera in the list of available cameras
            Accord.Video.DirectShow.FilterInfoCollection j = new Accord.Video.DirectShow.FilterInfoCollection(
                Accord.Video.DirectShow.FilterCategory.VideoInputDevice);
            List<Accord.Video.DirectShow.FilterInfo> my_list = j.Cast<Accord.Video.DirectShow.FilterInfo>().ToList();
            Accord.Video.DirectShow.FilterInfo my_camera = my_list.Where(x => x.Name.Equals("HD USB Camera")).FirstOrDefault();
            //Accord.Video.DirectShow.FilterInfo my_camera = my_list.Where(x => x.Name.Contains("Logitech")).FirstOrDefault();
            if (my_camera != null)
            {
                //Set up the camera as the capture devices
                var camera = new Accord.Video.DirectShow.VideoCaptureDevice(my_camera.MonikerString);
                camera.NewFrame += Camera_NewFrame;

                //Set camera properties
                camera.SetCameraProperty(Accord.Video.DirectShow.CameraControlProperty.Exposure, -6, Accord.Video.DirectShow.CameraControlFlags.Manual);

                //Select the proper video mode
                for (int i = 0; i < camera.VideoCapabilities.Length; i++)
                {
                    var selected_video_capabilities = camera.VideoCapabilities[i];
                    if (selected_video_capabilities.FrameSize.Width == _camera_frame_width && selected_video_capabilities.FrameSize.Height == _camera_frame_height)
                    {
                        camera.VideoResolution = selected_video_capabilities;
                    }
                }
                
                CameraVideoSourcePlayer.VideoSource = camera;
            }
            else
            {
                //Make sure the video source is null, indicating we do not have a camera connected
                CameraVideoSourcePlayer.VideoSource = null;
            }

            //Set the data context for this window
            //DataContext = new MMazeBehaviorViewModel(CameraVideoSourcePlayer);
            DataContext = MMazeBehaviorViewModel.GetInstance();
            MMazeBehaviorViewModel.GetInstance().SetupVideoSource(CameraVideoSourcePlayer);
            
            //Start the camera
            MMazeBehaviorViewModel vm = DataContext as MMazeBehaviorViewModel;
            if (vm != null)
            {
                vm.StartCamera();
            }
        }
        
        private void Camera_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            //Grab the video frame itself
            var video_frame = eventArgs.Frame;

            //Create a graphics object
            using (Graphics g = Graphics.FromImage(video_frame))
            {
                //Grab a separate copy of the video frame to save to the file
                var video_frame_copy = video_frame.Clone() as System.Drawing.Bitmap;

                //Make a semi-transparent rectangle near the top of the image
                SolidBrush b = new SolidBrush(System.Drawing.Color.FromArgb(128, System.Drawing.Color.CornflowerBlue));
                g.FillRectangle(b, new RectangleF(0, 0, _camera_frame_width, _overlay_height));

                //Draw each box
                SolidBrush b2 = new SolidBrush(System.Drawing.Color.FromArgb(128, System.Drawing.Color.Red));
                for (int i = 0; i < _box_lefts.Count; i++)
                {
                    g.DrawRectangle(new System.Drawing.Pen(b2, 4), new System.Drawing.Rectangle(_box_lefts[i], _box_top, _box_width, _box_height));
                }

                //Draw some text under each box
                List<string> _box_strings = new List<string>()
                {
                    "Left NP",
                    "Left Prox",
                    "Right Prox",
                    "Right NP",
                    "Sound Cue"
                };

                for (int i = 0; i < _box_lefts.Count; i++)
                {
                    g.DrawString(_box_strings[i], System.Drawing.SystemFonts.DefaultFont, b2, _box_lefts[i], _box_top + 110);
                }

                //Now let's draw some graphics based on the current state of the session.
                var model = MMazeBehaviorSession.GetInstance();
                var vm = MMazeBehaviorViewModel.GetInstance();
                if (model != null && vm != null)
                {
                    if (model.LeftNosepokeState)
                    {
                        g.FillRectangle(b2, _box_lefts[0], _box_top, _box_width, _box_height);
                    }
                    if (model.LeftProxState)
                    {
                        g.FillRectangle(b2, _box_lefts[1], _box_top, _box_width, _box_height);
                    }
                    if (model.RightProxState)
                    {
                        g.FillRectangle(b2, _box_lefts[2], _box_top, _box_width, _box_height);
                    }
                    if (model.RightNosepokeState)
                    {
                        g.FillRectangle(b2, _box_lefts[3], _box_top, _box_width, _box_height);
                    }
                    if (model.IsSoundPlaying)
                    {
                        g.FillRectangle(b2, _box_lefts[4], _box_top, _box_width, _box_height);
                    }
                    
                    //Create blank canvas for saving image to video file
                    Bitmap resized_canvas = new Bitmap(video_frame_copy.Width, video_frame_copy.Height + _overlay_height);

                    //Create a new graphics object with the blank canvas
                    using (Graphics g2 = Graphics.FromImage(resized_canvas))
                    {
                        //Paste the video frame on the blank canvas
                        g2.DrawImage(video_frame_copy, 0, _overlay_height, video_frame_copy.Width, video_frame_copy.Height);

                        //Draw the overlay on the top of the image
                        g2.FillRectangle(b, new RectangleF(0, 0, _camera_frame_width, _overlay_height));
                        for (int i = 0; i < _box_lefts.Count; i++)
                        {
                            g2.DrawRectangle(new System.Drawing.Pen(b2, 4), new System.Drawing.Rectangle(_box_lefts[i], _box_top, _box_width, _box_height));
                            g2.DrawString(_box_strings[i], System.Drawing.SystemFonts.DefaultFont, b2, _box_lefts[i], _box_top + 110);
                        }

                        if (model.LeftNosepokeState)
                        {
                            g2.FillRectangle(b2, _box_lefts[0], _box_top, _box_width, _box_height);
                        }
                        if (model.LeftProxState)
                        {
                            g2.FillRectangle(b2, _box_lefts[1], _box_top, _box_width, _box_height);
                        }
                        if (model.RightProxState)
                        {
                            g2.FillRectangle(b2, _box_lefts[2], _box_top, _box_width, _box_height);
                        }
                        if (model.RightNosepokeState)
                        {
                            g2.FillRectangle(b2, _box_lefts[3], _box_top, _box_width, _box_height);
                        }
                        if (model.IsSoundPlaying)
                        {
                            g2.FillRectangle(b2, _box_lefts[4], _box_top, _box_width, _box_height);
                        }

                        //Now let's take the current video frame and write it out to a saved video for the session
                        vm.WriteVideoFrame(resized_canvas);
                    }

                    //Release some resouces
                    resized_canvas.Dispose();
                }

                //Release some resources
                video_frame_copy.Dispose();
            }
            
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            MMazeBehaviorViewModel vm = DataContext as MMazeBehaviorViewModel;
            if (vm != null)
            {
                vm.HandleStartButtonClick();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MMazeBehaviorViewModel vm = DataContext as MMazeBehaviorViewModel;
            if (vm != null)
            {
                vm.StopCamera();
                vm.StopSession();
            }
        }
    }
}
