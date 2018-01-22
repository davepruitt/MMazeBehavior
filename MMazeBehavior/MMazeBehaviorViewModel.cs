using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;
using System.ComponentModel;
using System.Windows;

namespace MMazeBehavior
{
    /// <summary>
    /// View-model class for the MMaze Behavior GUI
    /// </summary>
    public class MMazeBehaviorViewModel : NotifyPropertyChangedObject
    {
        #region Private data members

        private Accord.Controls.VideoSourcePlayer _video_player = null;
        private MMazeBehaviorSession _session = MMazeBehaviorSession.GetInstance();
        private PlotModel _session_plot_model = new PlotModel();
        private MMazeVideoFileWriter _session_video_file_writer = null;

        #endregion

        #region Constructors

        private static MMazeBehaviorViewModel _instance = null;
        private static Object _instance_lock = new object();
        
        /// <summary>
        /// Returns the singleton instance of the MMazeBehaviorSession class
        /// </summary>
        /// <returns></returns>
        public static MMazeBehaviorViewModel GetInstance()
        {
            if (_instance == null)
            {
                lock (_instance_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MMazeBehaviorViewModel();
                    }
                }
            }

            return _instance;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player">The video player in the GUI</param>
        private MMazeBehaviorViewModel (  )
        {
            //Setup the plot
            SetupPlot();

            //Subscribe to event notifications from the behavior session
            MMazeBehaviorSession.GetInstance().PropertyChanged += ExecuteReactionsToModelPropertyChanged;
        }

        protected override void ExecuteReactionsToModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Check if we need to update the plot
            if (e.PropertyName.Equals("FeedList"))
            {
                if (_session.FeedList_TimeSincePreviousFeed != null && 
                    _session.FeedList_TimeSinceSessionStart != null)
                {
                    //Get the datapoints that need to be plotted
                    var x_datapoints = _session.FeedList_TimeSinceSessionStart.ToList();
                    var y_datapoints = _session.FeedList_TimeSincePreviousFeed.ToList();

                    List<ScatterPoint> datapoints = new List<ScatterPoint>();
                    for (int i = 0; i < x_datapoints.Count && i < y_datapoints.Count; i++)
                    {
                        ScatterPoint j = new ScatterPoint(x_datapoints[i], y_datapoints[i]);
                        datapoints.Add(j);
                    }

                    //Grab the scatter series from the plot
                    var series = _session_plot_model.Series.Select(x => x as ScatterSeries).FirstOrDefault();
                    if (series != null)
                    {
                        //Update the plot
                        series.Points.Clear();
                        series.Points.AddRange(datapoints);
                        _session_plot_model.InvalidatePlot(true);
                    }
                }
            }
            else if (e.PropertyName.Equals("SoundList"))
            {
                int ymax = 10;
                if (_session.FeedList_TimeSincePreviousFeed != null && _session.FeedList_TimeSincePreviousFeed.Count > 0)
                {
                    ymax = _session.FeedList_TimeSincePreviousFeed.ToList().Max();
                }
                
                List<int> sound_list = _session.GetSoundList();

                _session_plot_model.Annotations.Clear();

                if (sound_list != null)
                {
                    var y_axis = _session_plot_model.Axes.Where(j => j.Position == AxisPosition.Left).FirstOrDefault();
                    if (y_axis != null)
                    {
                        for (int i = 0; i < sound_list.Count; i++)
                        {
                            //Create the annotation
                            LineAnnotation new_annotation = new LineAnnotation()
                            {
                                Type = LineAnnotationType.Vertical,
                                LineStyle = LineStyle.Dash,
                                StrokeThickness = 2,
                                Color = OxyColor.FromRgb(255, 0, 0),
                                MinimumY = 0,
                                MaximumY = ymax,
                                X = sound_list[i]
                            };

                            //Add this annotation
                            _session_plot_model.Annotations.Add(new_annotation);

                            //Invalidate the plot
                            _session_plot_model.InvalidatePlot(true);
                        }
                    }
                }
            }

            //Call the base function to handle anything else
            base.ExecuteReactionsToModelPropertyChanged(sender, e);
        }

        public void SetupVideoSource ( Accord.Controls.VideoSourcePlayer player )
        {
            _video_player = player;
        }

        public void SetupPlot ()
        {
            //Create an x-axis and a y-axis for the plot
            LinearAxis x_axis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                MinimumRange = 10,
                MaximumRange = 1000
            };

            LinearAxis y_axis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                MinimumRange = 10,
                Maximum = double.NaN,
                MaximumRange = double.NaN
            };

            //Clear all annotations
            _session_plot_model.Annotations.Clear();

            //Clear the set of axes on the plot and add the axes we just created
            _session_plot_model.Axes.Clear();
            _session_plot_model.Axes.Add(x_axis);
            _session_plot_model.Axes.Add(y_axis);

            //Clear all the series on this plot
            _session_plot_model.Series.Clear();

            //Add a scatter series to the plot
            var scatter_series = new ScatterSeries()
            {
                MarkerType = MarkerType.Triangle,
                MarkerStroke = OxyColor.FromRgb(0, 0, 255),
                MarkerFill = OxyColor.FromRgb(128, 128, 255)
            };

            //Add the series to the plot
            _session_plot_model.Series.Add(scatter_series);

            //Update the plot
            _session_plot_model.InvalidatePlot(true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the rat being run
        /// </summary>
        public string RatName
        {
            get
            {
                return _session.RatName;
            }
            set
            {
                string input = value;
                input = HelperMethods.CleanInput(input.Trim()).ToUpper();
                _session.RatName = input;
                NotifyPropertyChanged("RatName");
            }
        }

        /// <summary>
        /// The name of the booth we are connected to
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "BoothName" })]
        public string BoothName
        {
            get
            {
                return _session.BoothName;
            }
        }

        /// <summary>
        /// How much time has elapsed for this session
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "SecondsElapsed" })]
        public string SessionTimerText
        {
            get
            {
                return TimeSpan.FromSeconds(_session.SecondsElapsed).ToString(@"hh\:mm\:ss");
            }
        }

        /// <summary>
        /// Indicates whether the user can edit the rat name or the stage selected
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "SessionIsRunning" })]
        public bool IsEditingEnabled
        {
            get
            {
                return !_session.SessionIsRunning;
            }
        }

        /// <summary>
        /// The list of available stages the user can choose from
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "AvailableStages" })]
        public List<string> StageList
        {
            get
            {
                if (_session.AvailableStages != null && _session.AvailableStages.Count > 0)
                {
                    List<string> result = _session.AvailableStages.Select(x => x.StageName).ToList();
                    return result;
                }
                else
                {
                    List<string> result = new List<string>() { "No stages found" };
                    return result;
                }
            }
        }

        /// <summary>
        /// The index of the selected stage in the list of available stages
        /// </summary>
        public int SelectedStageIndex
        {
            get
            {
                if (_session.AvailableStages != null && _session.AvailableStages.Count > 0)
                {
                    int index = _session.AvailableStages.IndexOf(_session.CurrentStage);
                    return index;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                int selected_index = value;
                if (_session.AvailableStages != null && _session.AvailableStages.Count > 0)
                {
                    if (selected_index >= 0 && selected_index < _session.AvailableStages.Count)
                    {
                        _session.CurrentStage = _session.AvailableStages[selected_index];
                    }
                }
                
                NotifyPropertyChanged("SelectedStageIndex");
            }
        }

        /// <summary>
        /// The string that appears on the start button
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "SessionIsRunning" })]
        public string StartButtonContent
        {
            get
            {
                if (_session.SessionIsRunning)
                {
                    return "Stop";
                }
                else
                {
                    return "Start";
                }
            }
        }

        /// <summary>
        /// The color of the start button
        /// </summary>
        [ReactToModelPropertyChanged(new string[] { "SessionIsRunning" })]
        public SolidColorBrush StartButtonColor
        {
            get
            {
                if (_session.SessionIsRunning)
                {
                    return new SolidColorBrush(Colors.Red);
                }
                else
                {
                    return new SolidColorBrush(Colors.Green);
                }
            }
        }

        [ReactToModelPropertyChanged(new string[] { "LeftProxState" })]
        public bool LeftProxState
        {
            get
            {
                return _session.LeftProxState;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "LeftNosepokeState" })]
        public bool LeftNosepokeState
        {
            get
            {
                return _session.LeftNosepokeState;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "RightProxState" })]
        public bool RightProxState
        {
            get
            {
                return _session.RightProxState;
            }
        }

        [ReactToModelPropertyChanged(new string[] { "RightNosepokeState" })]
        public bool RightNosepokeState
        {
            get
            {
                return _session.RightNosepokeState;
            }
        }

        /// <summary>
        /// Indicates whether the start button is enabled
        /// </summary>
        public bool IsStartButtonEnabled
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the plot model used for plotting
        /// </summary>
        public PlotModel SessionPlotModel
        {
            get
            {
                return _session_plot_model;
            }
        }

        /// <summary>
        /// Determines whether the WindowsFormHost that holds the camera player is visible to the user
        /// </summary>
        public Visibility CameraVisibility
        {
            get
            {
                if (_video_player != null && _video_player.VideoSource != null)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Determines whether the TextBlock that tells the user that no camera is attached is visible to the user
        /// </summary>
        public Visibility CameraNotAvailableVisibility
        {
            get
            {
                if (_video_player != null && _video_player.VideoSource != null)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reacts to a click of the start button
        /// </summary>
        public void HandleStartButtonClick ()
        {
            if (_session.SessionIsRunning)
            {
                StopSession();
            }
            else
            {
                StartSession();
            }
        }

        /// <summary>
        /// Starts a new session
        /// </summary>
        public void StartSession ()
        {
            //Clear the plot and set up a new plot
            SetupPlot();

            //Start the new behavior session
            _session.StartSession();

            //Create a file to write the video recording of the session.
            _session_video_file_writer = new MMazeVideoFileWriter();
            _session_video_file_writer.CreateFile(_session.RatName, 1280, 920);
        }

        /// <summary>
        /// Stops the current session
        /// </summary>
        public void StopSession ()
        {
            _session.StopSession();

            if (_session_video_file_writer != null)
            {
                _session_video_file_writer.CloseFile();
                _session_video_file_writer = null;
            }
        }

        /// <summary>
        /// Starts the webcam
        /// </summary>
        public void StartCamera ()
        {
            if (_video_player != null && _video_player.VideoSource != null)
            {
                _video_player.Start();
            }
        }

        /// <summary>
        /// Stops the webcam
        /// </summary>
        public void StopCamera ()
        {
            if (_video_player != null)
            {
                //Signal the video source to stop its background thread
                _video_player.SignalToStop();

                //Wait for the video source's background thread to exit, then return from this function
                _video_player.WaitForStop();
            }
        }

        public void WriteVideoFrame(System.Drawing.Bitmap frame)
        {
            if (_session_video_file_writer != null)
            {
                _session_video_file_writer.WriteVideoFrame(frame);
            }
        }

        #endregion
    }
}
