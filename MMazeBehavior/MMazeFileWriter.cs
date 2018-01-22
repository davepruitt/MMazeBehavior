using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MMazeBehavior
{
    public class MMazeFileWriter
    {
        #region Private data members

        StreamWriter _writer = null;

        #endregion

        #region Constructor

        public MMazeFileWriter ()
        {
            //empty
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new file that will be used to store an M-Maze behavior session
        /// </summary>
        /// <param name="rat_name">The rat name that is being run in the M-Maze</param>
        public void CreateFile ( string rat_name )
        {
            //If no rat name is defined, we will define a default name here
            if (string.IsNullOrEmpty(rat_name))
            {
                rat_name = "_UNDEFINED_ANIMAL_NAME_";
            }

            //Figure out the path to the saved file
            var path = MMazeConfiguration.GetInstance().SavePath;
            var rat_path = rat_name + "/";
            var fully_qualified_path = path + rat_path;

            //Create the path if it does not already exist
            Directory.CreateDirectory(fully_qualified_path);
            
            //Create a file name for this new session
            var file_time = DateTime.Now.ToFileTime();
            string file_name = rat_name + "-" + file_time.ToString();
            string file_name_with_extension = file_name + ".PTSD";

            //Combine the path and file name to get a full path
            string fully_qualified_path_and_file = path + rat_path + file_name_with_extension;

            //Create the new handle to the new file
            _writer = new StreamWriter(fully_qualified_path_and_file);
        }

        /// <summary>
        /// Writes a header at the top of the data file
        /// </summary>
        public void WriteFileHeader (string rat_name, Int64 timestamp, string stage, string booth)
        {
            if (_writer != null)
            {
                //Write all of the header information
                _writer.WriteLine("BEGIN HEADER");

                _writer.WriteLine("TIMESTAMP");
                _writer.WriteLine(timestamp.ToString());

                _writer.WriteLine("ANIMAL NAME");
                _writer.WriteLine(rat_name);

                _writer.WriteLine("BOOTH NUMBER");
                _writer.WriteLine(booth);

                _writer.WriteLine("END HEADER");
                _writer.WriteLine("BEGIN DATA");
                _writer.WriteLine("TIMESTAMP\tEVENT_LABEL");
            }
        }

        /// <summary>
        /// Writes a footer at the bottom of the data file
        /// </summary>
        public void WriteFileFooter (Int64 timestamp)
        {
            if (_writer != null)
            {
                _writer.WriteLine("END DATA");
                _writer.WriteLine("BEGIN HEADER");
                _writer.WriteLine("TIMESTAMP");
                _writer.WriteLine(timestamp.ToString());
                _writer.WriteLine("END HEADER");
            }
        }

        /// <summary>
        /// Closes an open file
        /// </summary>
        public void CloseFile ()
        {
            if (_writer != null)
            {
                _writer.Close();
            }
        }

        /// <summary>
        /// Writes an individual event to the data file
        /// </summary>
        public void WriteEvent (MMazeEvent e, string sound_file_name = "")
        {
            if (_writer != null)
            {
                string event_name = MMazeEventNamesConverter.ConvertToDescription(e.EventType);

                //Change the event name to the sound file name if this is a sound event
                if (e.EventType == MMazeEventNames.SoundCue)
                {
                    event_name = sound_file_name;
                }
                
                string output_text = e.EventTime.ToFileTime().ToString() + "\t" + event_name;

                _writer.WriteLine(output_text);
            }
        }

        #endregion
    }
}
