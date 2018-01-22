using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    public class MMazeVideoFileWriter
    {
        private Accord.Video.FFMPEG.VideoFileWriter _writer = null;
        private DateTime _last_call = DateTime.Now;
        private object _writer_object_lock = new object();

        #region Constructor

        public MMazeVideoFileWriter ()
        {
            //empty
        }

        #endregion

        /// <summary>
        /// Creates a new file that will be used to store an M-Maze behavior session
        /// </summary>
        /// <param name="rat_name">The rat name that is being run in the M-Maze</param>
        public void CreateFile(string rat_name, int width, int height)
        {
            lock (_writer_object_lock)
            {
                //If no rat name is defined, we will define a default name here
                if (string.IsNullOrEmpty(rat_name))
                {
                    rat_name = "_UNDEFINED_ANIMAL_NAME_";
                }

                //Figure out the path to the saved file
                var path = MMazeConfiguration.GetInstance().SavePath;
                var rat_path = rat_name + "/videos/";
                var fully_qualified_path = path + rat_path;

                //Create the path if it does not already exist
                Directory.CreateDirectory(fully_qualified_path);

                //Create a file name for this new session
                var file_time = DateTime.Now.ToFileTime();
                string file_name = rat_name + "-" + file_time.ToString();
                string file_name_with_extension = file_name + ".avi";

                //Combine the path and file name to get a full path
                string fully_qualified_path_and_file = path + rat_path + file_name_with_extension;

                //Create the new handle to the new file
                _writer = new Accord.Video.FFMPEG.VideoFileWriter();
                _writer.Open(fully_qualified_path_and_file, width, height);

                //Keep track of when the last frame was written to this video
                //Initialize this to the current time for the new video being written.
                _last_call = DateTime.Now;
            }   
        }

        /// <summary>
        /// Closes an open file
        /// </summary>
        public void CloseFile()
        {
            lock (_writer_object_lock)
            {
                if (_writer != null)
                {
                    _writer.Close();
                }
            }
        }

        /// <summary>
        /// Writes a frame to the video
        /// </summary>
        /// <param name="frame">The new image to write to the video</param>
        public void WriteVideoFrame (Bitmap frame)
        {
            lock (_writer_object_lock)
            {
                var ts = DateTime.Now - _last_call;

                if (_writer != null && _writer.IsOpen)
                {
                    try
                    {
                        _writer.WriteVideoFrame(frame, ts);
                    }
                    catch (Exception)
                    {
                        System.Console.WriteLine("Error encoding frame");
                    }
                }
            }
        }
    }
}
