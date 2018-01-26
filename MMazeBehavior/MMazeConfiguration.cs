using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    /// <summary>
    /// A class for handling configuration and initialization of the MMaze program
    /// </summary>
    public class MMazeConfiguration
    {
        #region Private data members

        private string _config_file_name = "config.txt";
        private string _stage_file_name = "stages.tsv";
        private string _stage_spreadsheet_uri = string.Empty;
        private string _booth_name = string.Empty;
        private string _save_path = "./Saved Sessions/";
        private List<MMazeStage> _loaded_stages = new List<MMazeStage>();

        public string SoundPath_Base = "./sounds/";


        #endregion

        #region Singleton class

        private static MMazeConfiguration _instance = null;
        private static Object _instance_lock = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        private MMazeConfiguration()
        {
            //empty
        }

        /// <summary>
        /// Returns the singleton instance of the MMazeConfiguration class
        /// </summary>
        /// <returns></returns>
        public static MMazeConfiguration GetInstance()
        {
            if (_instance == null)
            {
                lock (_instance_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MMazeConfiguration();
                    }
                }
            }

            return _instance;
        }

        #endregion

        #region Public methods

        public void LoadConfigurationFile ()
        {
            //Load the configuration file
            var file_lines = TxBDC_Common.ConfigurationFileLoader.LoadConfigurationFile(_config_file_name);

            //Iterate over each line of the config file and load in the parameters
            foreach (var f in file_lines)
            {
                var line_parts = f.Split(new char[] { ':' }, 2);
                if (line_parts.Length >= 2)
                {
                    string first_part = line_parts[0].Trim();
                    string second_part = line_parts[1].Trim();

                    if (first_part.Equals("Booth", StringComparison.InvariantCultureIgnoreCase))
                    {
                        BoothName = second_part;
                    }
                    else if (first_part.Equals("Stages"))
                    {
                        StageSpreadsheetURI = second_part;
                    }
                    else if (first_part.Equals("Save Path"))
                    {
                        SavePath = second_part;
                    }
                }
            }
        }

        public void LoadStages ()
        {
            //Load the stage spreadsheet
            //Uri stage_spreadsheet_uri = new Uri(_stage_spreadsheet_uri);
            var stage_spreadsheet_data = TxBDC_Common.ReadGoogleSpreadsheet.ReadLocalFile(_stage_file_name);
            
            //After loading the spreadsheet, parse through the data. Each row is a stage. 
            //Skip row 0 (the first row), because it is a header row.
            for (int r = 1; r < stage_spreadsheet_data.Count; r++)
            {
                //Grab this row
                var this_row = stage_spreadsheet_data[r];

                //Create a new stage object
                MMazeStage new_stage = new MMazeStage()
                {
                    StageName = this_row[0].Trim(),
                    StageDescription = this_row[1].Trim()
                };

                var s1_file = this_row[2].Trim();
                var s1_name = this_row[3].Trim();
                var s2_file = this_row[4].Trim();
                var s2_name = this_row[5].Trim();
                var refractory_list = this_row[6].Trim();
                var max_delay_list = this_row[7].Trim();
                var cue_type = this_row[8].Trim();

                var actual_cue_type = MMazeStageCueTypeConverter.ConvertFullDescriptionToMMazeCueType(cue_type);

                //Add the first sound file, if it is defined
                if (!string.IsNullOrEmpty(s1_file) && !string.IsNullOrEmpty(s1_name))
                {
                    Tuple<string, string> s1 = new Tuple<string, string>(s1_file, s1_name);
                    new_stage.SoundFiles.Add(s1);
                }

                //Add the second sound file, if it is defined
                if (!string.IsNullOrEmpty(s2_file) && !string.IsNullOrEmpty(s2_name))
                {
                    Tuple<string, string> s2 = new Tuple<string, string>(s2_file, s2_name);
                    new_stage.SoundFiles.Add(s2);
                }

                //Parse out the refractory periods and max delay periods.
                List<int> refractory_periods = HelperMethods.ParseNumberList(refractory_list);
                List<int> max_delay_periods = HelperMethods.ParseNumberList(max_delay_list);

                //Change numbers from units of seconds to units of milliseconds
                refractory_periods = refractory_periods.Select(x => x * 1000).ToList();
                max_delay_periods = max_delay_periods.Select(x => x * 1000).ToList();

                //Set the variables within the stage object
                new_stage.StageRefractoryPeriods = refractory_periods;
                new_stage.StageMaxSoundDelays = max_delay_periods;
                new_stage.StageCueType = actual_cue_type;

                //Add this new stage to the list of loaded stages
                LoadedStages.Add(new_stage);
            }
        }

        #endregion

        #region Public data members

        /// <summary>
        /// The name of the current booth
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
            }
        }

        /// <summary>
        /// The string that contains the URI for where the stage spreadsheet is located
        /// </summary>
        public string StageSpreadsheetURI
        {
            get
            {
                return _stage_spreadsheet_uri;
            }
            private set
            {
                _stage_spreadsheet_uri = value;
            }
        }

        /// <summary>
        /// A list of all loaded stages
        /// </summary>
        public List<MMazeStage> LoadedStages
        {
            get
            {
                return _loaded_stages;
            }
            set
            {
                _loaded_stages = value;
            }
        }

        /// <summary>
        /// The base path for saving session files
        /// </summary>
        public string SavePath
        {
            get
            {
                return _save_path;
            }
            set
            {
                _save_path = value;
            }
        }

        #endregion
    }
}
