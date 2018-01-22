using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace MMazeBehavior
{
    /// <summary>
    /// This class represents the current session being run (or about to be run) for the MMaze
    /// </summary>
    public class MMazeBehaviorSession : NotifyPropertyChangedObject
    {
        #region Singleton class

        private static MMazeBehaviorSession _instance = null;
        private static Object _instance_lock = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        private MMazeBehaviorSession()
        {
            //Set the list of available stages.
            //Stages should already be loaded in at this point in the program.
            AvailableStages = MMazeConfiguration.GetInstance().LoadedStages;
            if (AvailableStages.Count > 0)
            {
                CurrentStage = AvailableStages[0];
            }
        }

        /// <summary>
        /// Returns the singleton instance of the MMazeBehaviorSession class
        /// </summary>
        /// <returns></returns>
        public static MMazeBehaviorSession GetInstance()
        {
            if (_instance == null)
            {
                lock(_instance_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MMazeBehaviorSession();
                    }
                }
            }

            return _instance;
        }

        #endregion

        #region Private Properties
        
        private int _right_feed_count = 0;
        private int _left_feed_count = 0;
        private MMazeTrialState _trial_state = MMazeTrialState.NoTrialStarted;

        private string _rat_name = string.Empty;
        private MMazeStage _current_stage = null;
        private List<MMazeStage> _available_stages = new List<MMazeStage>();
        private string _booth_name = string.Empty;
        private int _seconds_elapsed = 0;
        private bool _session_running = false;

        private MMazeFileWriter _session_file_writer = null;
        private MMazeVideoFileWriter _session_video_file_writer = null;
        private object _session_video_file_writer_lock = new object();

        private BackgroundWorker _background_thread = null;
        private ConcurrentBag<string> _background_properties_to_update = new ConcurrentBag<string>();

        private bool _left_nosepoke_state = false;
        private bool _left_prox_state = false;
        private bool _right_prox_state = false;
        private bool _right_nosepoke_state = false;
        private bool _is_sound_playing = false;

        private Stopwatch timer = new Stopwatch();
        private System.Timers.Timer _timer1 = null;
        private System.Timers.Timer _timer2 = null;

        private List<int> _this_session_timer_map = new List<int>();
        private List<int> _this_session_max_timer_map = new List<int>();

        private bool _playSoundFlag = false;

        private DateTime _most_recent_feed = DateTime.MinValue;

        #endregion

        #region Properties
        
        /// <summary>
        /// The rat name for this session
        /// </summary>
        public string RatName
        {
            get
            {
                return _rat_name;
            }
            set
            {
                _rat_name = value;
                NotifyPropertyChanged("RatName");
            }
        }
        
        /// <summary>
        /// The current stage being run
        /// </summary>
        public MMazeStage CurrentStage
        {
            get
            {
                return _current_stage;
            }
            set
            {
                _current_stage = value;
                NotifyPropertyChanged("CurrentStage");
            }
        }

        /// <summary>
        /// All the available stages
        /// </summary>
        public List<MMazeStage> AvailableStages
        {
            get
            {
                return _available_stages;
            }
            set
            {
                _available_stages = value;
                NotifyPropertyChanged("AvailableStages");
            }
        }

        /// <summary>
        /// The booth name
        /// </summary>
        public string BoothName
        {
            get
            {
                return _booth_name;
            }
            set
            {
                _booth_name = value;
                NotifyPropertyChanged("BoothName");
            }
        }

        /// <summary>
        /// The amount of time elapsed in the session
        /// </summary>
        public int SecondsElapsed
        {
            get
            {
                return _seconds_elapsed;
            }
            set
            {
                _seconds_elapsed = value;
                _background_properties_to_update.Add("SecondsElapsed");
            }
        }
        
        /// <summary>
        /// Whether or not the session is running
        /// </summary>
        public bool SessionIsRunning
        {
            get
            {
                return _session_running;
            }
            set
            {
                _session_running = value;
                NotifyPropertyChanged("SessionIsRunning");
            }
        }

        /// <summary>
        /// The number of feeds on the left side during this session
        /// </summary>
        public int LeftFeedCount
        {
            get
            {
                return _left_feed_count;
            }
            private set
            {
                _left_feed_count = value;
                _background_properties_to_update.Add("LeftFeedCount");
            }
        }

        /// <summary>
        /// The number of feeds on the right side during this session
        /// </summary>
        public int RightFeedCount
        {
            get
            {
                return _right_feed_count;
            }
            private set
            {
                _right_feed_count = value;
                _background_properties_to_update.Add("RightFeedCount");
            }
        }

        /// <summary>
        /// The state of the current trial during this session
        /// </summary>
        public MMazeTrialState TrialState
        {
            get
            {
                return _trial_state;
            }
            private set
            {
                _trial_state = value;
                _background_properties_to_update.Add("TrialState");
            }
        }

        /// <summary>
        /// The state of the left nosepoke
        /// </summary>
        public bool LeftNosepokeState
        {
            get
            {
                return _left_nosepoke_state;
            }
            private set
            {
                _left_nosepoke_state = value;
                _background_properties_to_update.Add("LeftNosepokeState");
            }
        }

        /// <summary>
        /// The state of the right nosepoke
        /// </summary>
        public bool RightNosepokeState
        {
            get
            {
                return _right_nosepoke_state;
            }
            private set
            {
                _right_nosepoke_state = value;
                _background_properties_to_update.Add("RightNosepokeState");
            }
        }

        /// <summary>
        /// The state of the left proximity sensor
        /// </summary>
        public bool LeftProxState
        {
            get
            {
                return _left_prox_state;
            }
            private set
            {
                _left_prox_state = value;
                _background_properties_to_update.Add("LeftProxState");
            }
        }

        /// <summary>
        /// The state of the right proximity sensor
        /// </summary>
        public bool RightProxState
        {
            get
            {
                return _right_prox_state;
            }
            private set
            {
                _right_prox_state = value;
                _background_properties_to_update.Add("RightProxState");
            }
        }

        /// <summary>
        /// Indicates whether a sound cue is currently playing or not
        /// </summary>
        public bool IsSoundPlaying
        {
            get
            {
                return _is_sound_playing;
            }
            private set
            {
                _is_sound_playing = value;
                _background_properties_to_update.Add("IsSoundPlaying");
            }
        }

        public List<int> FeedList_TimeSincePreviousFeed = null;
        public List<int> FeedList_TimeSinceSessionStart = null;
        private List<int> SoundList_TimeSinceSessionStart = null;
        private object sound_list_lock = new object();

        public List<int> GetSoundList ()
        {
            List<int> result = null;

            lock (sound_list_lock)
            {
                result = SoundList_TimeSinceSessionStart.ToList();
            }

            return result;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Starts a new session
        /// </summary>
        public void StartSession ()
        {
            //Set the session state
            SessionIsRunning = true;

            //Start a background thread that handles all booth/microcontroller interaction
            if (_background_thread == null)
            {
                _background_thread = new BackgroundWorker();
                _background_thread.WorkerSupportsCancellation = true;
                _background_thread.WorkerReportsProgress = true;
                _background_thread.DoWork += _background_thread_DoWork;
                _background_thread.ProgressChanged += _background_thread_ProgressChanged;
                _background_thread.RunWorkerCompleted += _background_thread_RunWorkerCompleted;
            }

            //Run the background thread
            _background_thread.RunWorkerAsync();
        }

        /// <summary>
        /// Stops the session that is currently running
        /// </summary>
        public void StopSession ()
        {
            //Set the session state
            SessionIsRunning = false;

            //Cancel the background thread
            if (_background_thread != null)
            {
                _background_thread.CancelAsync();
            }
        }

        private void _background_thread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //empty
        }

        private void _background_thread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string item = string.Empty;
            while (!_background_properties_to_update.IsEmpty)
            {
                _background_properties_to_update.TryTake(out item);
                NotifyPropertyChanged(item);
            }
        }

        private void _background_thread_DoWork(object sender, DoWorkEventArgs e)
        {
            //If a stage has not been defined, return immediately. We cannot proceed.
            if (CurrentStage == null)
            {
                return;
            }

            //Get an instance of the microcontroller that is connected to this computer
            var microcontroller = Microcontroller.GetInstance();
            microcontroller.ConnectToMicrocontroller();

            //Initialize variables for this behavior session
            LeftFeedCount = 0;
            RightFeedCount = 0;
            TrialState = MMazeTrialState.NoTrialStarted;
            IsSoundPlaying = false;

            //Open a file for this session
            _session_file_writer = new MMazeFileWriter();
            _session_file_writer.CreateFile(RatName);
            _session_file_writer.WriteFileHeader(
                RatName, 
                DateTime.Now.ToFileTime(),
                CurrentStage.StageName, 
                MMazeConfiguration.GetInstance().BoothName
            );

            //Make hard copies of the timer lists for the current stage so we can use them this session
            _this_session_timer_map = CurrentStage.StageRefractoryPeriods.ToList();
            _this_session_max_timer_map = CurrentStage.StageMaxSoundDelays.ToList();

            //Start a timer for the session.
            timer.Restart();

            //Start timer 1 - the refractory period timer
            StartTimer1();

            //Set the initial value of the "most recent feed" to be the current time
            _most_recent_feed = DateTime.Now;

            //Initialize the "feed lists"
            FeedList_TimeSincePreviousFeed = new List<int>();
            FeedList_TimeSinceSessionStart = new List<int>();

            lock (sound_list_lock)
            {
                SoundList_TimeSinceSessionStart = new List<int>();
            }
            
            //Loop until the session is ended
            while (!_background_thread.CancellationPending)
            {
                //Get the total seconds elapsed so far during this session
                SecondsElapsed = Convert.ToInt32(timer.Elapsed.TotalSeconds);

                //Read in the most recent event from the microcontroller
                var latest_event = microcontroller.ReadStream();
                if (latest_event != null && latest_event.EventType != MMazeEventNames.Undefined)
                {
                    //Write the latest event to the file
                    _session_file_writer.WriteEvent(latest_event);

                    //If it was a feeder event, update the feed lists we are maintaining (which will also be used to update
                    //the plot)
                    if (latest_event.EventType == MMazeEventNames.LeftFeederTriggered ||
                        latest_event.EventType == MMazeEventNames.RightFeederTriggered)
                    {
                        var time_since_last_feed = DateTime.Now - _most_recent_feed;
                        _most_recent_feed = DateTime.Now;

                        FeedList_TimeSincePreviousFeed.Add(Convert.ToInt32(time_since_last_feed.TotalSeconds));
                        FeedList_TimeSinceSessionStart.Add(Convert.ToInt32(timer.Elapsed.TotalSeconds));
                        _background_properties_to_update.Add("FeedList");
                    }

                    //Set the user-facing nosepoke and prox-sensor state variables based upon the most recent event
                    //that we have received from the microcontroller
                    switch (latest_event.EventType)
                    {
                        case MMazeEventNames.LeftNosepokeEnter:
                            LeftNosepokeState = true;
                            break;
                        case MMazeEventNames.LeftNosepokeLeave:
                            LeftNosepokeState = false;
                            break;
                        case MMazeEventNames.RightNosepokeEnter:
                            RightNosepokeState = true;
                            break;
                        case MMazeEventNames.RightNosepokeLeave:
                            RightNosepokeState = false;
                            break;
                        case MMazeEventNames.LeftProxEnter:
                            LeftProxState = true;
                            break;
                        case MMazeEventNames.LeftProxLeave:
                            LeftProxState = false;
                            break;
                        case MMazeEventNames.RightProxEnter:
                            RightProxState = true;
                            break;
                        case MMazeEventNames.RightProxLeave:
                            RightProxState = false;
                            break;
                    }

                    //Handle the trial state based upon the most recent event we received from the microcontroller
                    switch (TrialState)
                    {
                        case MMazeTrialState.NoTrialStarted:

                            //Set the trial state based upon the nosepoke that has been exited
                            if (latest_event.EventType == MMazeEventNames.LeftNosepokeLeave)
                            {
                                TrialState = MMazeTrialState.LeftToRight;
                            }
                            else if (latest_event.EventType == MMazeEventNames.RightNosepokeLeave)
                            {
                                TrialState = MMazeTrialState.RightToLeft;
                            }

                            break;
                        case MMazeTrialState.LeftToRight:

                            //This is a "left to right" trial, handle a beam break in the left prox area
                            if (latest_event.EventType == MMazeEventNames.LeftProxEnter)
                            {
                                TrialState = MMazeTrialState.LeftToRight_EnterLeftProx;
                                HandleBeamBreak();
                            }

                            break;
                        case MMazeTrialState.RightToLeft:

                            //This is a "right to left" trial, handle a beam break in the right prox area
                            if (latest_event.EventType == MMazeEventNames.RightProxEnter)
                            {
                                TrialState = MMazeTrialState.RightToLeft_EnterRightProx;
                                HandleBeamBreak();
                            }

                            break;
                        case MMazeTrialState.LeftToRight_EnterLeftProx:

                            //This is the end of a "left to right" trial, reset the trial state
                            if (latest_event.EventType == MMazeEventNames.RightNosepokeEnter)
                            {
                                TrialState = MMazeTrialState.NoTrialStarted;
                            }

                            break;
                        case MMazeTrialState.RightToLeft_EnterRightProx:

                            //This is the end of a "right to left" trial, reset the trial state
                            if (latest_event.EventType == MMazeEventNames.LeftNosepokeEnter)
                            {
                                TrialState = MMazeTrialState.NoTrialStarted;
                            }

                            break;
                    }
                }

                //Update the GUI based on what is happening in the background thread
                _background_properties_to_update.Add("SoundList");
                _background_thread.ReportProgress(0);

                //Sleep the thread
                Thread.Sleep(33);
            }

            //Close the file for this session
            _session_file_writer.WriteFileFooter(DateTime.Now.ToFileTime());
            _session_file_writer.CloseFile();
            
            //Stop the session timer
            timer.Stop();

            //Stop the sound timers
            if (_timer1 != null)
            {
                _timer1.Stop();
                _timer1.Close();
            }

            if (_timer2 != null)
            {
                _timer2.Stop();
                _timer2.Close();
            }

            //Close up
            SecondsElapsed = 0;
            _background_thread.ReportProgress(0);

            //Report the thread being canceled
            e.Cancel = true;
        }

        private void StartTimer1 ()
        {
            //"Timer 1" is a "refractory period" timer in which no sound can be played.
            //When timer 1 elapses, a flag is set that allows sound to be played.
            //This does not mean that sound automatically plays when timer 1 elapses.
            if (_this_session_timer_map.Count > 0)
            {
                int time_delay = _this_session_timer_map[0];
                _this_session_timer_map.RemoveAt(0);

                if (_timer1 != null)
                {
                    _timer1.Stop();
                    _timer1.Close();
                }

                _timer1 = new System.Timers.Timer(time_delay);
                _timer1.AutoReset = false;
                _timer1.Elapsed += Timer1Finish;
                _timer1.Start();
            }
        }

        private void Timer1Finish(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Set the flag indicating that sound is enabled
            _playSoundFlag = true;

            _timer1.Stop();
            _timer1.Close();

            //"Timer 2" is a timer that runs out after the maximum amount of time has passed in which we allow no
            //sound to be played. Therefore, when timer 2 runs out, a sound WILL be played.
            if (_timer2 != null && _timer2.Enabled)
            {
                _timer2.Stop();
                _timer2.Close();
            }

            if (_this_session_max_timer_map.Count > 0)
            {
                var d = _this_session_max_timer_map[0];
                _this_session_max_timer_map.RemoveAt(0);
                
                _timer2 = new System.Timers.Timer(d);
                _timer2.AutoReset = false;
                _timer2.Elapsed += Timer2Finish;
                _timer2.Start();
            }
        }

        private void Timer2Finish (object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer2.Stop();
            _timer2.Close();

            if (_playSoundFlag)
            {
                PlaySound();
            }
        }

        private void HandleBeamBreak ()
        {
            if (_playSoundFlag)
            {
                //Stop timer 2
                if (_timer2 != null && _timer2.Enabled)
                {
                    _timer2.Stop();
                    _timer2.Close();
                }

                //Play the sound
                PlaySound();
            }
        }

        private void PlaySound ()
        {
            //Reset the flag to play a sound
            _playSoundFlag = false;
            
            lock (sound_list_lock)
            {
                SoundList_TimeSinceSessionStart.Add(Convert.ToInt32(timer.Elapsed.TotalSeconds));
            }
            
            //Play the sound
            if (CurrentStage.SoundFiles.Count > 0)
            {
                //Get the sound file name
                var sound_file_name = CurrentStage.SoundFiles[0].Item1;
                var sound_file_path = MMazeConfiguration.GetInstance().SoundPath_Base;
                var fully_qualified_path = sound_file_path + sound_file_name;

                //Write an event to the file indicating we are playing a sound at this time
                MMazeEvent sound_event = new MMazeEvent(DateTime.Now, MMazeEventNames.SoundCue);
                _session_file_writer.WriteEvent(sound_event, sound_file_name);

                FileInfo finfo = new FileInfo(fully_qualified_path);
                if (finfo.Exists)
                {
                    //Set the variable in the model indicating that the sound is playing
                    IsSoundPlaying = true;

                    //Load the sound into memory
                    //SoundPlayer k = new SoundPlayer(finfo.FullName);

                    //Play the sound (asynchronously)
                    //k.Play();

                    //Play the sound
                    PlaySoundAsync(finfo.FullName);
                }
            }

            //Restart timer 1
            StartTimer1();
        }

        private async void PlaySoundAsync (string filename)
        {
            using (var player = new System.Media.SoundPlayer(filename))
            {
                await Task.Run(() => { player.Load(); player.PlaySync(); });
                IsSoundPlaying = false;
            }
        }

        /// <summary>
        /// Writes a video frame to the video of the current session.
        /// </summary>
        /// <param name="frame">The new video frame</param>
        public void WriteVideoFrame (System.Drawing.Bitmap frame)
        {
            lock (_session_video_file_writer_lock)
            {
                if (_session_video_file_writer != null)
                {
                    _session_video_file_writer.WriteVideoFrame(frame);
                }
            }
        }

        #endregion
    }
}
