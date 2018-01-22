using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidSharp;

namespace MMazeBehavior
{
    /// <summary>
    /// Handles interaction with the MSP430 microcontroller
    /// </summary>
    public class Microcontroller
    {
        #region Private data members

        private int VendorID = 8263;
        private int ProductID = 769;
        private HidStream MicrocontrollerStream = null;

        #endregion

        #region Singleton

        private static Microcontroller _instance = null;
        private static Object _instance_lock = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        private Microcontroller()
        {
            //constructor is private
        }

        /// <summary>
        /// Gets the one and only instance of this class that is allowed to exist.
        /// </summary>
        /// <returns>Instance of Microcontroller class</returns>
        public static Microcontroller GetInstance()
        {
            if (_instance == null)
            {
                lock (_instance_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Microcontroller();
                    }
                }
            }

            return _instance;
        }

        #endregion

        #region Methods

        public void ConnectToMicrocontroller ()
        {
            HidDeviceLoader device_loader = new HidDeviceLoader();
            HidDevice device = device_loader.GetDeviceOrDefault(VendorID, ProductID);
            if (device != null)
            {
                bool success = device.TryOpen(out MicrocontrollerStream);
                if (!success)
                {
                    MMazeMessaging.GetInstance().AddMessage("Unable to connect to microcontroller!");
                }
                else
                {
                    //Disable the read and write timeouts
                    //MicrocontrollerStream.ReadTimeout = System.Threading.Timeout.Infinite;
                    //MicrocontrollerStream.WriteTimeout = System.Threading.Timeout.Infinite;
                    MicrocontrollerStream.ReadTimeout = 30;
                    MicrocontrollerStream.WriteTimeout = 30;
                }
            }
            else
            {
                MMazeMessaging.GetInstance().AddMessage("Unable to find microcontroller device!");
            }
        }

        /// <summary>
        /// Read incoming data from the microcontroller stream
        /// </summary>
        public MMazeEvent ReadStream ()
        {
            if (MicrocontrollerStream != null && MicrocontrollerStream.CanRead)
            {
                //Create a 64-byte buffer to read data from the microcontroller
                Byte[] arr = new Byte[64];

                //Read data from the stream
                try
                {
                    int bytes_read = MicrocontrollerStream.Read(arr, 0, arr.Length);

                    //Byte 2 should indicate what kind of data is coming in
                    byte data_type = arr[2];

                    if (data_type == 'S')
                    {
                        //Byte 3 indicates the kind of event that occurred
                        MMazeEventNames e = MMazeEventNamesConverter.ConvertShortDescriptionToMMazeEventType(Convert.ToChar(arr[3]));

                        //Return the new event that we have read in
                        return new MMazeEvent(DateTime.Now, e);
                    }
                    else if (data_type == 'D')
                    {
                        //What is the D command?
                    }
                    else if (data_type == 'J')
                    {
                        //What is the J commmand?
                    }
                }
                catch (TimeoutException e)
                {
                    return new MMazeEvent() { EventType = MMazeEventNames.Undefined };
                }
            }

            //Return null in the case that no event was read from the stream
            return null;
        }

        #endregion
    }
}
