//////////////////////////////////////////////////////////////////////////////////
//	
//  Original code by Written by Brian Peek (http://www.brianpeek.com/)
//	for MSDN's Coding4Fun (http://msdn.microsoft.com/coding4fun/)
//	Visit http://blogs.msdn.com/coding4fun/archive/2007/03/14/1879033.aspx
//  and http://www.codeplex.com/WiimoteLib
//	for more information
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using ws.winx.devices;
using System.Collections.Generic;


namespace ws.winx.platform.windows
{
    /// <summary>
    /// Implementation of WiiDriver
    /// </summary>
    public class WiiDriver : IJoystickDriver, IDisposable
    {

        internal struct AsyncPackage{
            FileStream stream=null;
            SafeFileHandle handle=null;
            WiimoteDevice device=null;
            bool isReady=true;
            IAsyncResult result=null;

        }


        protected static Dictionary<WiimoteDevice, AsyncPackage> _AsyncPackages;
       // protected static Dictionary<WiimoteDevice, FileStream> _FileStreams;




        ///// <summary>
        ///// Event raised when Wiimote state is changed
        ///// </summary>
        //public event EventHandler<WiimoteChangedEventArgs> WiimoteChanged;

        ///// <summary>
        ///// Event raised when an extension is inserted or removed
        ///// </summary>
        //public event EventHandler<WiimoteExtensionChangedEventArgs> WiimoteExtensionChanged;

        // VID = Nintendo, PID = Wiimote
        private const int VID = 0x057e;
        private const int PID = 0x0306;

        // sure, we could find this out the hard way using HID, but trust me, it's 22
        private const int REPORT_LENGTH = 22;



        // Wiimote output commands
        private enum OutputReport : byte
        {
            LEDs = 0x11,
            Type = 0x12,
            IR = 0x13,
            Status = 0x15,
            WriteMemory = 0x16,
            ReadMemory = 0x17,
            IR2 = 0x1a,
        };

        // Wiimote registers
        private const int REGISTER_IR = 0x04b00030;
        private const int REGISTER_IR_SENSITIVITY_1 = 0x04b00000;
        private const int REGISTER_IR_SENSITIVITY_2 = 0x04b0001a;
        private const int REGISTER_IR_MODE = 0x04b00033;

        private const int REGISTER_EXTENSION_INIT = 0x04a40040;
        private const int REGISTER_EXTENSION_TYPE = 0x04a400fe;
        private const int REGISTER_EXTENSION_CALIBRATION = 0x04a40020;

        // read/write handle to the device
       // private SafeFileHandle mHandle;

        // a pretty .NET stream to read/write from/to
        //private FileStream mStream;

        // report buffer
        private readonly byte[] mBuff = new byte[REPORT_LENGTH];

        // read data buffer
        private byte[] mReadBuff;

        // address to read from
        private int mAddress;

        // size of requested read
        private short mSize;

        private WiimoteDevice __currentDevice;


        // event for read data processing
        private readonly AutoResetEvent mReadDone = new AutoResetEvent(false);

        // use a different method to write reports
        private bool mAltWriteMethod;

        /// <summary>
        /// Default constructor
        /// </summary>
        public WiiDriver()
        {
        }


        public void Update(IJoystickDevice joystick)
        {
            //Read from joystick stream

            //FileStream stream = ((WiimoteDevice)joystick).mStream;

            //// if the stream is valid and ready
            //if (stream != null && stream.CanRead)
            //{
            //    // setup the read and the callback
            //    byte[] buff = new byte[REPORT_LENGTH];
            //   // stream.BeginRead(buff, 0, REPORT_LENGTH, new AsyncCallback(OnReadData), buff);

              // IAsyncResult result = stream.BeginRead(buff, 0, REPORT_LENGTH, null, null);
            //result.
            //    try
            //    {
            //        // end the current read
            //        stream.EndRead(result);

            //        // parse it
            //        if (ParseInputReport(buff))
            //        {
            //            // post an event
            //           // if (WiimoteChanged != null)
            //            //    WiimoteChanged(this, new WiimoteChangedEventArgs(mWiimoteState));
            //        }

                    
            //    }
            //    catch (OperationCanceledException)
            //    {
            //        Debug.WriteLine("OperationCanceledException");
            //    }

            //}
        }


        public IJoystickDevice ResolveDevice(IHIDDeviceInfo info)
        {
          

            if (info.VID == VID && info.PID == PID)
            {  
                 

                // open a read/write handle to our device using the DevicePath returned
                SafeFileHandle handle = UnsafeNativeMethods.CreateFile(info.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, EFileAttributes.Overlapped, IntPtr.Zero);

                WiimoteDevice joystick;
                int inx = 0;

               
                // create a nice .NET FileStream wrapping the handle abov
                FileStream stream = new FileStream(handle, FileAccess.ReadWrite, REPORT_LENGTH, true);

              

                // create new Device
                joystick = new WiimoteDevice(info.id, 16, 10,4,4);

                WiiDriver._FileStreams[joystick] = stream;
                WiiDriver._AsyncPackages[joystick] = handle;

               
                             


                //inti button structure
                for (; inx < 10; inx++)
                {
                    joystick.Buttons[inx] = new ButtonDetails();
                }

                AxisDetails axisDetails;


                //AccX
                axisDetails = new AxisDetails();
                joystick.Axis[JoystickAxis.AxisAccX] = axisDetails;

                //AccY
                axisDetails = new AxisDetails();
                joystick.Axis[JoystickAxis.AxisAccY] = axisDetails;

                //AccZ
                axisDetails = new AxisDetails();
                joystick.Axis[JoystickAxis.AxisAccZ] = axisDetails;


                //LX
                axisDetails = new AxisDetails();
              //  axisDetails.max = 32767;
              //  axisDetails.min = -32767;
                joystick.Axis[JoystickAxis.AxisX] = axisDetails;

                //LY
                axisDetails = new AxisDetails();
            //    axisDetails.max = 32767;
            //    axisDetails.min = -32767;
                joystick.Axis[JoystickAxis.AxisY] = axisDetails;

                //RX
                axisDetails = new AxisDetails();
         //       axisDetails.max = 32767;
        //        axisDetails.min = -32767;
                joystick.Axis[JoystickAxis.AxisZ] = axisDetails;

                //RY
                axisDetails = new AxisDetails();
          //      axisDetails.max = 32767;
          //      axisDetails.min = -32767;
                joystick.Axis[JoystickAxis.AxisR] = axisDetails;


                //TRIGGERS
                axisDetails = new AxisDetails();
                axisDetails.max = 255;
                axisDetails.min = 0;
                joystick.Axis[JoystickAxis.AxisU] = axisDetails;

                axisDetails = new AxisDetails();
                axisDetails.max = 255;
                axisDetails.min = 0;
                joystick.Axis[JoystickAxis.AxisV] = axisDetails;

                //POV
                axisDetails = new AxisDetails();
                axisDetails.isHat = true;
                joystick.Axis[JoystickAxis.AxisPovX] = axisDetails;
                axisDetails = new AxisDetails();
                axisDetails.isHat = true;
                joystick.Axis[JoystickAxis.AxisPovY] = axisDetails;

                __currentDevice = joystick;



                // start an async read operation on it
               //BeginAsyncRead();

                // read the calibration info from the controller
                try
                {
                    ReadCalibration(joystick);
                }
                catch
                {
                    // if we fail above, try the alternate HID writes
                    mAltWriteMethod = true;
                    ReadCalibration(joystick);
                }

                // force a status check to get the state of any extensions plugged in at startup
                GetStatus(joystick);

                __currentDevice = null;

                return joystick;

            }
          

            return null;
        }

        /// <summary>
        /// Connect to a Wiimote paired to the PC via Bluetooth
        /// </summary>
        //public void Connect()
        //{
        //    int index = 0;
        //    bool found = false;
        //    Guid guid;

        //    // get the GUID of the HID class
        //    HIDImports.HidD_GetHidGuid(out guid);

        //    // get a handle to all devices that are part of the HID class
        //    // Fun fact:  DIGCF_PRESENT worked on my machine just fine.  I reinstalled Vista, and now it no longer finds the Wiimote with that parameter enabled...
        //    IntPtr hDevInfo = HIDImports.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, HIDImports.DIGCF_DEVICEINTERFACE);// | HIDImports.DIGCF_PRESENT);

        //    // create a new interface data struct and initialize its size
        //    HIDImports.SP_DEVICE_INTERFACE_DATA diData = new HIDImports.SP_DEVICE_INTERFACE_DATA();
        //    diData.cbSize = Marshal.SizeOf(diData);

        //    // get a device interface to a single device (enumerate all devices)
        //    while (HIDImports.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guid, index, ref diData))
        //    {
        //        UInt32 size;

        //        // get the buffer size for this device detail instance (returned in the size parameter)
        //        HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, IntPtr.Zero, 0, out size, IntPtr.Zero);

        //        // create a detail struct and set its size
        //        HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA();

        //        // yeah, yeah...well, see, on Win x86, cbSize must be 5 for some reason.  On x64, apparently 8 is what it wants.
        //        // someday I should figure this out.  Thanks to Paul Miller on this...
        //        diDetail.cbSize = (uint)(IntPtr.Size == 8 ? 8 : 5);

        //        // actually get the detail struct
        //        if (HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out size, IntPtr.Zero))
        //        {
        //            Debug.WriteLine(index + " " + diDetail.DevicePath + " " + Marshal.GetLastWin32Error());

        //            // open a read/write handle to our device using the DevicePath returned
        //            mHandle = HIDImports.CreateFile(diDetail.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

        //            // create an attributes struct and initialize the size
        //            HIDImports.HIDD_ATTRIBUTES attrib = new HIDImports.HIDD_ATTRIBUTES();
        //            attrib.Size = Marshal.SizeOf(attrib);

        //            // get the attributes of the current device
        //            if (HIDImports.HidD_GetAttributes(mHandle.DangerousGetHandle(), ref attrib))
        //            {
        //                // if the vendor and product IDs match up
        //                if (attrib.VendorID == VID && attrib.ProductID == PID)
        //                {
        //                    Debug.WriteLine("Found it!");
        //                    found = true;

        //                    // create a nice .NET FileStream wrapping the handle above
        //                    mStream = new FileStream(mHandle, FileAccess.ReadWrite, REPORT_LENGTH, true);

        //                    // start an async read operation on it
        //                    BeginAsyncRead();

        //                    // read the calibration info from the controller
        //                    try
        //                    {
        //                        ReadCalibration();
        //                    }
        //                    catch
        //                    {
        //                        // if we fail above, try the alternate HID writes
        //                        mAltWriteMethod = true;
        //                        ReadCalibration();
        //                    }

        //                    // force a status check to get the state of any extensions plugged in at startup
        //                    GetStatus();

        //                    break;
        //                }
        //                else
        //                {
        //                    // otherwise this isn't the controller, so close up the file handle
        //                    mHandle.Close();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            // failed to get the detail struct
        //            throw new WiimoteException("SetupDiGetDeviceInterfaceDetail failed on index " + index);
        //        }

        //        // move to the next device
        //        index++;
        //    }

        //    // clean up our list
        //    HIDImports.SetupDiDestroyDeviceInfoList(hDevInfo);

        //    // if we didn't find a Wiimote, throw an exception
        //    if (!found)
        //        throw new WiimoteException("Wiimote not found in HID device list.");
        //}

   

        internal struct AsyncronPackage{
        }

        ///// <summary>
        ///// Start reading asynchronously from the controller
        ///// </summary>
        new AsyncronPackage(device, buff)
            new AsyncCallback(OnReadData)
        private void BeginAsyncRead(WiimoteDevice device )
        {
            FileStream stream = WiiDriver._FileStreams[device];

            // if the stream is valid and ready
            if (stream != null && stream.CanRead)
            {
                // setup the read and the callback
                byte[] buff = new byte[REPORT_LENGTH];
                stream.BeginRead(buff, 0, REPORT_LENGTH, , );
            }
        }

        /// <summary>
        /// Callback when data is ready to be processed
        /// </summary>
        /// <param name="ar">State information for the callback</param>
        private void OnReadData(IAsyncResult ar)
        {
            AsyncronPackage package = ((AsyncronPackage)ar.AsyncState);

            WiimoteDevice device = package.device;

            //ar.AsyncWaitHandle.WaitOne(

            FileStream mStream = WiiDriver._FileStreams[device];

            // grab the byte buffer
            byte[] buff = (byte[])package.buff;

            try
            {
                // end the current read
                mStream.EndRead(ar);

                // parse it
                ParseInputReport(package, buff);
                //if (ParseInputReport(device,buff))
               // {
                    // post an event
                    //if (WiimoteChanged != null)
                       // WiimoteChanged(this, new WiimoteChangedEventArgs(mWiimoteState));
               // }

                // start reading again
                BeginAsyncRead(package);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("OperationCanceledException");
            }
        }

        /// <summary>
        /// Parse a report sent by the Wiimote
        /// </summary>
        /// <param name="buff">Data buffer to parse</param>
        /// <returns>Returns a boolean noting whether an event needs to be posted</returns>
        private bool ParseInputReport(WiimoteDevice device, byte[] buff)
        {
            InputReport type = (InputReport)buff[0];

            switch (type)
            {
                case InputReport.Buttons:
                    ParseButtons(device,buff);
                    break;
                case InputReport.ButtonsAccel:
                    ParseButtons(device,buff);
                    ParseAccel(device,buff);
                    break;
                case InputReport.ButtonsIRAccel:
                    ParseButtons(device,buff);
                    ParseAccel(device,buff);
                    ParseIR(device,buff);
                    break;
                case InputReport.ButtonsExtension:
                    ParseButtons(device,buff);
                    ParseExtension(device,DecryptBuffer(buff), 4);
                    break;
                case InputReport.ButtonsExtensionAccel:
                    ParseButtons(device,buff);
                    ParseAccel(device,buff);
                    ParseExtension(device,DecryptBuffer(buff), 6);
                    break;
                case InputReport.IRExtensionAccel:
                    ParseButtons(device,buff);
                    ParseAccel(device,buff);
                    ParseIR(device,buff);
                    ParseExtension(device,DecryptBuffer(buff), 16);
                    break;
                case InputReport.Status:
                    ParseButtons(device,buff);
                    device.Battery = buff[6];

                    // get the real LED values in case the values from SetLEDs() somehow becomes out of sync, which really shouldn't be possible
                    

                     device.LED[0] = (buff[3] & 0x10) != 0;
                     device.LED[1] = (buff[3] & 0x20) != 0;
                      device.LED[2] = (buff[3] & 0x40) != 0;
                     device.LED[3]= (buff[3] & 0x80) != 0;

                    //mWiimoteState.LEDState.LED1 = (buff[3] & 0x10) != 0;
                    //mWiimoteState.LEDState.LED2 = (buff[3] & 0x20) != 0;
                    //mWiimoteState.LEDState.LED3 = (buff[3] & 0x40) != 0;
                    //mWiimoteState.LEDState.LED4 = (buff[3] & 0x80) != 0;

                    // extension connected?
                    bool extension = (buff[3] & 0x02) != 0;
                    //Debug.WriteLine("Extension: " + extension);
                    UnityEngine.Debug.Log("Extension: " + extension);


                    //if (mWiimoteState.Extension != extension)
                    if (device.Extension != extension)
                    {
                        device.Extension = extension;

                        if (extension)
                        {


                            // start reading again
                            BeginAsyncRead(device);

                            InitializeExtension(device);
                        }
                        else
                            device.ExtensionType = WiiExtensionType.None;

                      //  if (WiimoteExtensionChanged != null)
                       //     WiimoteExtensionChanged(this, new WiimoteExtensionChangedEventArgs(mWiimoteState.ExtensionType, mWiimoteState.Extension));
                    }

                    break;
                case InputReport.ReadData:
                    ParseButtons(device,buff);
                    ParseReadData(device,buff);
                    break;
                default:
                    Debug.WriteLine("Unknown report type: " + type.ToString("x"));
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Handles setting up an extension when plugged in
        /// </summary>
        private void InitializeExtension(WiimoteDevice device)
        {
            WriteData(device, REGISTER_EXTENSION_INIT, 0x00);

            // doing a read too quickly after a write seems to kill the guitar controller
            Thread.Sleep(50);

            byte[] buff = ReadData(device, REGISTER_EXTENSION_TYPE, 2);

            if (buff[0] == (byte)WiiExtensionType.Nunchuk && buff[1] == (byte)WiiExtensionType.Nunchuk)
                device.ExtensionType = WiiExtensionType.Nunchuk;
            else if (buff[0] == (byte)WiiExtensionType.ClassicController && buff[1] == (byte)WiiExtensionType.ClassicController)
                device.ExtensionType = WiiExtensionType.ClassicController;
            else if (buff[0] == (byte)WiiExtensionType.ClassicController && buff[1] == (byte)WiiExtensionType.Guitar)
                device.ExtensionType = WiiExtensionType.Guitar;
            else if (buff[0] == 0xff)	// partially inserted case...reset back to nothing inserted
            {
                device.Extension = false;
                device.ExtensionType = WiiExtensionType.None;
                return;
            }
            else
                throw new WiimoteException("Unknown extension controller found: " + buff[0]);

            buff = DecryptBuffer(ReadData(device,REGISTER_EXTENSION_CALIBRATION, 16));

            AxisDetails axisDetails;

            switch (device.ExtensionType)
            {
                case WiiExtensionType.Nunchuk:


                    //X,Y
                    axisDetails = device.Axis[JoystickAxis.AxisX] as AxisDetails;
                    axisDetails.max = buff[8];
                    axisDetails.min = buff[9];
                    axisDetails.mid = buff[10];

                    axisDetails = device.Axis[JoystickAxis.AxisY] as AxisDetails;
                    axisDetails.max = buff[11];
                    axisDetails.min = buff[12];
                    axisDetails.mid = buff[13];



                   // Acceleration
                      axisDetails = device.Axis[JoystickAxis.AxisAccR] as AxisDetails;
                      axisDetails.max = buff[4];
                      axisDetails.min = buff[0];
                  

                    axisDetails = device.Axis[JoystickAxis.AxisAccU] as AxisDetails;
                    axisDetails.max = buff[5];
                      axisDetails.min = buff[1];
                   

                     axisDetails = device.Axis[JoystickAxis.AxisAccV] as AxisDetails;
                      axisDetails.max = buff[6];
                      axisDetails.min = buff[2];
                   

                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.X0 = buff[0];
                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Y0 = buff[1];
                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Z0 = buff[2];
                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.XG = buff[4];
                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.YG = buff[5];
                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.ZG = buff[6];

                    //mWiimoteState.NunchukState.CalibrationInfo.MaxX = buff[8];
                    //mWiimoteState.NunchukState.CalibrationInfo.MinX = buff[9];
                    //mWiimoteState.NunchukState.CalibrationInfo.MidX = buff[10];
                    //mWiimoteState.NunchukState.CalibrationInfo.MaxY = buff[11];
                    //mWiimoteState.NunchukState.CalibrationInfo.MinY = buff[12];
                    //mWiimoteState.NunchukState.CalibrationInfo.MidY = buff[13];

              



                    break;
                case WiiExtensionType.ClassicController:

                    //Left Stick
                    axisDetails = device.Axis[JoystickAxis.AxisX] as AxisDetails;
                    axisDetails.max = (byte)(buff[0] >> 2);
                    axisDetails.min = (byte)(buff[1] >> 2);
                    axisDetails.mid = (byte)(buff[2] >> 2);

                    axisDetails = device.Axis[JoystickAxis.AxisY] as AxisDetails;
                     axisDetails.max = (byte)(buff[3] >> 2);
                    axisDetails.min = (byte)(buff[4] >> 2);
                    axisDetails.mid = (byte)(buff[5] >> 2);

                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXL = (byte)(buff[0] >> 2);
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MinXL = (byte)(buff[1] >> 2);
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MidXL = (byte)(buff[2] >> 2);
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYL = (byte)(buff[3] >> 2);
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MinYL = (byte)(buff[4] >> 2);
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MidYL = (byte)(buff[5] >> 2);

                    //Rigth Stick
                    axisDetails = device.Axis[JoystickAxis.AxisZ] as AxisDetails;
                    axisDetails.max = (byte)(buff[6] >> 3);
                    axisDetails.min = (byte)(buff[7] >> 3);
                    axisDetails.mid = (byte)(buff[8] >> 3);

                    axisDetails = device.Axis[JoystickAxis.AxisR] as AxisDetails;
                     axisDetails.max = (byte)(buff[9] >> 3);
                    axisDetails.min = (byte)(buff[10] >> 3);
                    axisDetails.mid = (byte)(buff[11] >> 3);

                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXR = (byte)(buff[6] >> 3);
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MinXR = (byte)(buff[7] >> 3);
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MidXR = (byte)(buff[8] >> 3);
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYR = (byte)(buff[9] >> 3);
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MinYR = (byte)(buff[10] >> 3);
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MidYR = (byte)(buff[11] >> 3);

                    // this doesn't seem right...
                    //					mWiimoteState.ClassicControllerState.AccelCalibrationInfo.MinTriggerL = (byte)(buff[12] >> 3);
                    //					mWiimoteState.ClassicControllerState.AccelCalibrationInfo.MaxTriggerL = (byte)(buff[14] >> 3);
                    //					mWiimoteState.ClassicControllerState.AccelCalibrationInfo.MinTriggerR = (byte)(buff[13] >> 3);
                    //					mWiimoteState.ClassicControllerState.AccelCalibrationInfo.MaxTriggerR = (byte)(buff[15] >> 3);
                    
                     //mWiimoteState.ClassicControllerState.CalibrationInfo.MinTriggerL = 0;
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerL = 31;
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MinTriggerR = 0;
                    //mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerR = 31;

                    //Left Trigger
                     axisDetails = device.Axis[JoystickAxis.AxisU] as AxisDetails;
                    axisDetails.max = (byte)(buff[14] >> 3);
                    axisDetails.min = (byte)(buff[12] >> 3);
                   
                    //Rigth Trigger
                    axisDetails = device.Axis[JoystickAxis.AxisV] as AxisDetails;
                     axisDetails.max = (byte)(buff[15] >> 3);
                    axisDetails.min = (byte)(buff[13] >> 3);
                   

                    break;
             
            }
        }

        /// <summary>
        /// Decrypts data sent from the extension to the Wiimote
        /// </summary>
        /// <param name="buff">Data buffer</param>
        /// <returns>Byte array containing decoded data</returns>
        private byte[] DecryptBuffer(byte[] buff)
        {
            for (int i = 0; i < buff.Length; i++)
                buff[i] = (byte)(((buff[i] ^ 0x17) + 0x17) & 0xff);

            return buff;
        }

        /// <summary>
        /// Parses a standard button report into the ButtonState struct
        /// </summary>
        /// <param name="buff">Data buffer</param>
        private void ParseButtons(WiimoteDevice device, byte[] buff)
        {
           
            //mWiimoteState.ButtonState.A = (buff[2] & 0x08) != 0;
            //mWiimoteState.ButtonState.B = (buff[2] & 0x04) != 0;
            //mWiimoteState.ButtonState.Minus = (buff[2] & 0x10) != 0;
            //mWiimoteState.ButtonState.Home = (buff[2] & 0x80) != 0;
            //mWiimoteState.ButtonState.Plus = (buff[1] & 0x10) != 0;
            //mWiimoteState.ButtonState.One = (buff[2] & 0x02) != 0;
            //mWiimoteState.ButtonState.Two = (buff[2] & 0x01) != 0;

            device.Buttons[0].value = (float)(buff[2] & 0x08);  
            device.Buttons[1].value = (float)(buff[2] & 0x04);
            device.Buttons[2].value = (float)(buff[2] & 0x10);
            device.Buttons[3].value = (float)(buff[2] & 0x80);
            device.Buttons[4].value = (float)(buff[1] & 0x10);
            device.Buttons[5].value = (float)(buff[2] & 0x02);
            device.Buttons[6].value = (float)(buff[2] & 0x01);

            //mWiimoteState.ButtonState.Up = (buff[1] & 0x08) != 0;
            //mWiimoteState.ButtonState.Down = (buff[1] & 0x04) != 0;
            //mWiimoteState.ButtonState.Left = (buff[1] & 0x01) != 0;
            //mWiimoteState.ButtonState.Right = (buff[1] & 0x02) != 0;

            //POV
            AxisDetails axisDetails;

            axisDetails = device.Axis[JoystickAxis.AxisPovY] as AxisDetails;

            axisDetails.value = 0;

            if ((buff[1] & 0x08) != 0)
                axisDetails.value = 1f;
            else if((buff[1] & 0x04) != 0)
                axisDetails.value = -1f;

            axisDetails = device.Axis[JoystickAxis.AxisPovX] as AxisDetails;
            axisDetails.value = 0;

            if ((buff[1] & 0x02) != 0)
                axisDetails.value = 1f;
            else if ((buff[1] & 0x01) != 0)
                axisDetails.value = -1f;

          


        }

        /// <summary>
        /// Parse accelerometer data
        /// </summary>
        /// <param name="buff">Data buffer</param>
        private void ParseAccel(WiimoteDevice device, byte[] buff)
        {
            AxisDetails axisDetails;
            axisDetails = device.Axis[JoystickAxis.AxisAccX] as AxisDetails;
            axisDetails.value = (float)buff[3] - axisDetails.min / (axisDetails.max - axisDetails.min);
           

            axisDetails = device.Axis[JoystickAxis.AxisAccY] as AxisDetails;
            axisDetails.value = (float)buff[4] - axisDetails.min / (axisDetails.max - axisDetails.min);

            axisDetails = device.Axis[JoystickAxis.AxisAccZ] as AxisDetails;
            axisDetails.value = (float)buff[5] - axisDetails.min / (axisDetails.max - axisDetails.min);


            //mWiimoteState.AccelState.RawValues.X = buff[3];
            //mWiimoteState.AccelState.RawValues.Y = buff[4];
            //mWiimoteState.AccelState.RawValues.Z = buff[5];

            //mWiimoteState.AccelState.Values.X = (float)((float)mWiimoteState.AccelState.RawValues.X - mWiimoteState.AccelCalibrationInfo.X0) /
            //                                ((float)mWiimoteState.AccelCalibrationInfo.XG - mWiimoteState.AccelCalibrationInfo.X0);
            //mWiimoteState.AccelState.Values.Y = (float)((float)mWiimoteState.AccelState.RawValues.Y - mWiimoteState.AccelCalibrationInfo.Y0) /
            //                                ((float)mWiimoteState.AccelCalibrationInfo.YG - mWiimoteState.AccelCalibrationInfo.Y0);
            //mWiimoteState.AccelState.Values.Z = (float)((float)mWiimoteState.AccelState.RawValues.Z - mWiimoteState.AccelCalibrationInfo.Z0) /
            //                                ((float)mWiimoteState.AccelCalibrationInfo.ZG - mWiimoteState.AccelCalibrationInfo.Z0);
        }

        /// <summary>
        /// Parse IR data from report
        /// </summary>
        /// <param name="buff">Data buffer</param>
        private void ParseIR(WiimoteDevice device, byte[] buff)
        {
          //  mWiimoteState.IRState.IRSensors[0].RawPosition.X = buff[6] | ((buff[8] >> 4) & 0x03) << 8;
         //   mWiimoteState.IRState.IRSensors[0].RawPosition.Y = buff[7] | ((buff[8] >> 6) & 0x03) << 8;


            float sensor1RawX;
            float sensor1RawY;

           
           

           AxisDetails irAxisDetails;
               
              var sensor=device.IR_SENSORS[0];

              

            switch (device.Mode)
            {
                case IRMode.Basic:
                  
                     if(sensor.Found=!(buff[6] == 0xff && buff[7] == 0xff))
                     {
                          sensor.Axis[JoystickAxis.AxisX].value = (float) ( buff[6] | ((buff[8] >> 4) & 0x03) << 8)/1023.5f;
                          sensor.Axis[JoystickAxis.AxisY].value = (float) (buff[7] | ((buff[8] >> 6) & 0x03) << 8)/767.5f;
                     }
                                        
                      sensor= device.IR_SENSORS[1];

                     if(sensor.Found=!(buff[9] == 0xff && buff[10] == 0xff))
                     {
                          sensor.Axis[JoystickAxis.AxisX].value = (float) ( buff[9] | ((buff[8] >> 0) & 0x03) << 8) /1023.5f;
                          sensor.Axis[JoystickAxis.AxisY].value = (float) (buff[10] | ((buff[8] >> 2) & 0x03) << 8)/767.5f;
                     }


                  
                    //mWiimoteState.IRState.IRSensors[1].RawPosition.X = buff[9]  | ((buff[8] >> 0) & 0x03) << 8;
                    //mWiimoteState.IRState.IRSensors[1].RawPosition.Y = buff[10] | ((buff[8] >> 2) & 0x03) << 8;


                    //mWiimoteState.IRState.IRSensors[0].Size = 0x00;
                    //mWiimoteState.IRState.IRSensors[1].Size = 0x00;

                    //mWiimoteState.IRState.IRSensors[0].Found = !(buff[6] == 0xff && buff[7] == 0xff);
                    //mWiimoteState.IRState.IRSensors[1].Found = !(buff[9] == 0xff && buff[10] == 0xff);
                    break;
                case IRMode.Extended:

                     if(sensor.Found=!(buff[6] == 0xff && buff[7] == 0xff && buff[8] == 0xff))
                     {
                          sensor.Axis[JoystickAxis.AxisX].value = (float) ( buff[6] | ((buff[8] >> 4) & 0x03) << 8)/1023.5f;
                          sensor.Axis[JoystickAxis.AxisY].value = (float) (buff[7] | ((buff[8] >> 6) & 0x03) << 8)/767.5f;
                          sensor.Size=buff[8] & 0x0f;
                     }

                                     
                      sensor= device.IR_SENSORS[1];

                     if(sensor.Found=!(buff[9] == 0xff && buff[10] == 0xff && buff[11] == 0xff))
                     {
                          sensor.Axis[JoystickAxis.AxisX].value = (float) ( buff[9] | ((buff[11] >> 4) & 0x03) << 8) /1023.5f;
                          sensor.Axis[JoystickAxis.AxisY].value = (float) (buff[10] | ((buff[11] >> 6) & 0x03) << 8)/767.5f;
                         sensor.Size=buff[11] & 0x0f;
                     }

                     sensor= device.IR_SENSORS[2];

                     if(sensor.Found=!(buff[12] == 0xff && buff[13] == 0xff && buff[14] == 0xff))
                     {
                          sensor.Axis[JoystickAxis.AxisX].value = (float) (buff[12] | ((buff[14] >> 4) & 0x03) << 8) /1023.5f;
                          sensor.Axis[JoystickAxis.AxisY].value = (float) (buff[13] | ((buff[14] >> 6) & 0x03) << 8)/767.5f;
                         sensor.Size=buff[14] & 0x0f;
                     }


                      sensor= device.IR_SENSORS[2];

                     if(sensor.Found= !(buff[15] == 0xff && buff[16] == 0xff && buff[17] == 0xff))
                     {
                          sensor.Axis[JoystickAxis.AxisX].value = (float) (buff[15] | ((buff[17] >> 4) & 0x03) << 8) /1023.5f;
                          sensor.Axis[JoystickAxis.AxisY].value = (float) (buff[16] | ((buff[17] >> 6) & 0x03) << 8)/767.5f;
                         sensor.Size=buff[17] & 0x0f;
                     }

                   





                   // mWiimoteState.IRState.IRSensors[1].RawPosition.X = buff[9] | ((buff[11] >> 4) & 0x03) << 8;
                  //  mWiimoteState.IRState.IRSensors[1].RawPosition.Y = buff[10] | ((buff[11] >> 6) & 0x03) << 8;
                  //  mWiimoteState.IRState.IRSensors[2].RawPosition.X = buff[12] | ((buff[14] >> 4) & 0x03) << 8;
                 //   mWiimoteState.IRState.IRSensors[2].RawPosition.Y = buff[13] | ((buff[14] >> 6) & 0x03) << 8;
                   // mWiimoteState.IRState.IRSensors[3].RawPosition.X = buff[15] | ((buff[17] >> 4) & 0x03) << 8;
                  //  mWiimoteState.IRState.IRSensors[3].RawPosition.Y = buff[16] | ((buff[17] >> 6) & 0x03) << 8;

                   // mWiimoteState.IRState.IRSensors[0].Size = buff[8] & 0x0f;
                   // mWiimoteState.IRState.IRSensors[1].Size = buff[11] & 0x0f;
                  //  mWiimoteState.IRState.IRSensors[2].Size = buff[14] & 0x0f;
                  //  mWiimoteState.IRState.IRSensors[3].Size = buff[17] & 0x0f;

                   // mWiimoteState.IRState.IRSensors[0].Found = !(buff[6] == 0xff && buff[7] == 0xff && buff[8] == 0xff);
                  //  mWiimoteState.IRState.IRSensors[1].Found = !(buff[9] == 0xff && buff[10] == 0xff && buff[11] == 0xff);
                  //  mWiimoteState.IRState.IRSensors[2].Found = !(buff[12] == 0xff && buff[13] == 0xff && buff[14] == 0xff);
                 //   mWiimoteState.IRState.IRSensors[3].Found = !(buff[15] == 0xff && buff[16] == 0xff && buff[17] == 0xff);
                    break;
            }

            //mWiimoteState.IRState.IRSensors[0].Position.X = (float)(mWiimoteState.IRState.IRSensors[0].RawPosition.X / 1023.5f);
            //mWiimoteState.IRState.IRSensors[1].Position.X = (float)(mWiimoteState.IRState.IRSensors[1].RawPosition.X / 1023.5f);
            //mWiimoteState.IRState.IRSensors[2].Position.X = (float)(mWiimoteState.IRState.IRSensors[2].RawPosition.X / 1023.5f);
            //mWiimoteState.IRState.IRSensors[3].Position.X = (float)(mWiimoteState.IRState.IRSensors[3].RawPosition.X / 1023.5f);

            
            

            //mWiimoteState.IRState.IRSensors[0].Position.Y = (float)(mWiimoteState.IRState.IRSensors[0].RawPosition.Y / 767.5f);
            //mWiimoteState.IRState.IRSensors[1].Position.Y = (float)(mWiimoteState.IRState.IRSensors[1].RawPosition.Y / 767.5f);
            //mWiimoteState.IRState.IRSensors[2].Position.Y = (float)(mWiimoteState.IRState.IRSensors[2].RawPosition.Y / 767.5f);
            //mWiimoteState.IRState.IRSensors[3].Position.Y = (float)(mWiimoteState.IRState.IRSensors[3].RawPosition.Y / 767.5f);

           
           // if (mWiimoteState.IRState.IRSensors[0].Found && mWiimoteState.IRState.IRSensors[1].Found)
            //if (device.IR_SENSORS[0].Found && device.IR_SENSORS[1].Found)
            //{
            //    mWiimoteState.IRState.RawMidpoint.X = (mWiimoteState.IRState.IRSensors[1].RawPosition.X + mWiimoteState.IRState.IRSensors[0].RawPosition.X) / 2;
            //    mWiimoteState.IRState.RawMidpoint.Y = (mWiimoteState.IRState.IRSensors[1].RawPosition.Y + mWiimoteState.IRState.IRSensors[0].RawPosition.Y) / 2;

            //    mWiimoteState.IRState.Midpoint.X = (mWiimoteState.IRState.IRSensors[1].Position.X + mWiimoteState.IRState.IRSensors[0].Position.X) / 2.0f;
            //    mWiimoteState.IRState.Midpoint.Y = (mWiimoteState.IRState.IRSensors[1].Position.Y + mWiimoteState.IRState.IRSensors[0].Position.Y) / 2.0f;
            //}
            //else
            //  device.IR_POINT.x = device.IR_POINT.y = 0.0f;
        }

        /// <summary>
        /// Parse data from an extension controller
        /// </summary>
        /// <param name="buff">Data buffer</param>
        /// <param name="offset">Offset into data buffer</param>
        private void ParseExtension(WiimoteDevice device, byte[] buff, int offset)
        {
            switch (device.ExtensionType)
            {
                case WiiExtensionType.Nunchuk:
                      AxisDetails axisDetails;

                     axisDetails = device.Axis[JoystickAxis.AxisX] as AxisDetails;
                   
                    if(axisDetails.max>0f){
                        axisDetails.value=((float)buff[offset]-axisDetails.mid)/(axisDetails.max-axisDetails.min);
                    }


                     axisDetails = device.Axis[JoystickAxis.AxisY] as AxisDetails;
                   
                    if(axisDetails.max>0f){
                        axisDetails.value=((float)buff[offset + 1]-axisDetails.mid)/(axisDetails.max-axisDetails.min);
                    }


                  //  mWiimoteState.NunchukState.RawJoystick.X = buff[offset];
                  //  mWiimoteState.NunchukState.RawJoystick.Y = buff[offset + 1];



                    mWiimoteState.NunchukState.AccelState.RawValues.X = buff[offset + 2];
                    mWiimoteState.NunchukState.AccelState.RawValues.Y = buff[offset + 3];
                    mWiimoteState.NunchukState.AccelState.RawValues.Z = buff[offset + 4];

                    mWiimoteState.NunchukState.C = (buff[offset + 5] & 0x02) == 0;
                    mWiimoteState.NunchukState.Z = (buff[offset + 5] & 0x01) == 0;

                    mWiimoteState.NunchukState.AccelState.Values.X = (float)((float)mWiimoteState.NunchukState.AccelState.RawValues.X - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.X0) /
                                                    ((float)mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.XG - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.X0);
                    mWiimoteState.NunchukState.AccelState.Values.Y = (float)((float)mWiimoteState.NunchukState.AccelState.RawValues.Y - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Y0) /
                                                    ((float)mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.YG - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Y0);
                    mWiimoteState.NunchukState.AccelState.Values.Z = (float)((float)mWiimoteState.NunchukState.AccelState.RawValues.Z - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Z0) /
                                                    ((float)mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.ZG - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Z0);


                    if (mWiimoteState.NunchukState.CalibrationInfo.MaxX != 0x00)
                        mWiimoteState.NunchukState.Joystick.X = (float)((float)mWiimoteState.NunchukState.RawJoystick.X - mWiimoteState.NunchukState.CalibrationInfo.MidX) /
                                                ((float)mWiimoteState.NunchukState.CalibrationInfo.MaxX - mWiimoteState.NunchukState.CalibrationInfo.MinX);

                    if (mWiimoteState.NunchukState.CalibrationInfo.MaxY != 0x00)
                        mWiimoteState.NunchukState.Joystick.Y = (float)((float)mWiimoteState.NunchukState.RawJoystick.Y - mWiimoteState.NunchukState.CalibrationInfo.MidY) /
                                                ((float)mWiimoteState.NunchukState.CalibrationInfo.MaxY - mWiimoteState.NunchukState.CalibrationInfo.MinY);

                    break;

                case WiiExtensionType.ClassicController:
                    mWiimoteState.ClassicControllerState.RawJoystickL.X = (byte)(buff[offset] & 0x3f);
                    mWiimoteState.ClassicControllerState.RawJoystickL.Y = (byte)(buff[offset + 1] & 0x3f);
                    mWiimoteState.ClassicControllerState.RawJoystickR.X = (byte)((buff[offset + 2] >> 7) | (buff[offset + 1] & 0xc0) >> 5 | (buff[offset] & 0xc0) >> 3);
                    mWiimoteState.ClassicControllerState.RawJoystickR.Y = (byte)(buff[offset + 2] & 0x1f);

                    mWiimoteState.ClassicControllerState.RawTriggerL = (byte)(((buff[offset + 2] & 0x60) >> 2) | (buff[offset + 3] >> 5));
                    mWiimoteState.ClassicControllerState.RawTriggerR = (byte)(buff[offset + 3] & 0x1f);

                    mWiimoteState.ClassicControllerState.ButtonState.TriggerR = (buff[offset + 4] & 0x02) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.Plus = (buff[offset + 4] & 0x04) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.Home = (buff[offset + 4] & 0x08) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.Minus = (buff[offset + 4] & 0x10) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.TriggerL = (buff[offset + 4] & 0x20) == 0;

                    mWiimoteState.ClassicControllerState.ButtonState.Down = (buff[offset + 4] & 0x40) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.Right = (buff[offset + 4] & 0x80) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.Up = (buff[offset + 5] & 0x01) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.Left = (buff[offset + 5] & 0x02) == 0;

                    mWiimoteState.ClassicControllerState.ButtonState.ZR = (buff[offset + 5] & 0x04) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.X = (buff[offset + 5] & 0x08) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.A = (buff[offset + 5] & 0x10) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.Y = (buff[offset + 5] & 0x20) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.B = (buff[offset + 5] & 0x40) == 0;
                    mWiimoteState.ClassicControllerState.ButtonState.ZL = (buff[offset + 5] & 0x80) == 0;

                    if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXL != 0x00)
                        mWiimoteState.ClassicControllerState.JoystickL.X = (float)((float)mWiimoteState.ClassicControllerState.RawJoystickL.X - mWiimoteState.ClassicControllerState.CalibrationInfo.MidXL) /
                        (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXL - mWiimoteState.ClassicControllerState.CalibrationInfo.MinXL);

                    if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYL != 0x00)
                        mWiimoteState.ClassicControllerState.JoystickL.Y = (float)((float)mWiimoteState.ClassicControllerState.RawJoystickL.Y - mWiimoteState.ClassicControllerState.CalibrationInfo.MidYL) /
                        (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYL - mWiimoteState.ClassicControllerState.CalibrationInfo.MinYL);

                    if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXR != 0x00)
                        mWiimoteState.ClassicControllerState.JoystickR.X = (float)((float)mWiimoteState.ClassicControllerState.RawJoystickR.X - mWiimoteState.ClassicControllerState.CalibrationInfo.MidXR) /
                        (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXR - mWiimoteState.ClassicControllerState.CalibrationInfo.MinXR);

                    if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYR != 0x00)
                        mWiimoteState.ClassicControllerState.JoystickR.Y = (float)((float)mWiimoteState.ClassicControllerState.RawJoystickR.Y - mWiimoteState.ClassicControllerState.CalibrationInfo.MidYR) /
                        (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYR - mWiimoteState.ClassicControllerState.CalibrationInfo.MinYR);

                    if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerL != 0x00)
                        mWiimoteState.ClassicControllerState.TriggerL = (mWiimoteState.ClassicControllerState.RawTriggerL) /
                        (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerL - mWiimoteState.ClassicControllerState.CalibrationInfo.MinTriggerL);

                    if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerR != 0x00)
                        mWiimoteState.ClassicControllerState.TriggerR = (mWiimoteState.ClassicControllerState.RawTriggerR) /
                        (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerR - mWiimoteState.ClassicControllerState.CalibrationInfo.MinTriggerR);
                    break;

               
            }
        }

        /// <summary>
        /// Parse data returned from a read report
        /// </summary>
        /// <param name="buff">Data buffer</param>
        private void ParseReadData(WiimoteDevice device, byte[] buff)
        {
            if ((buff[3] & 0x08) != 0)
                throw new WiimoteException("Error reading data from Wiimote: Bytes do not exist.");

            if ((buff[3] & 0x07) != 0)
                throw new WiimoteException("Error reading data from Wiimote: Attempt to read from write-only registers.");

            // get our size and offset from the report
            int size = (buff[3] >> 4) + 1;
            int offset = (buff[4] << 8 | buff[5]);

            // add it to the buffer
            Array.Copy(buff, 6, mReadBuff, offset - mAddress, size);

            // if we've read it all, set the event
            if (mAddress + mSize == offset + size)
                mReadDone.Set();
        }

        ///// <summary>
        ///// Returns whether rumble is currently enabled.
        ///// </summary>
        ///// <returns>Byte indicating true (0x01) or false (0x00)</returns>
        //private byte GetRumbleBit(WiimoteDevice device)
        //{
        //    //return (byte)(mWiimoteState.Rumble ? 0x01 : 0x00);
        //    return (byte)(device.Rumble ? 0x01 : 0x00);
        //}

        /// <summary>
        /// Read calibration information stored on Wiimote
        /// </summary>
        private void ReadCalibration(WiimoteDevice device)
        {
            // this appears to change the report type to 0x31
            byte[] buff = ReadData(device,0x0016, 7);

           // AxisDetails[JoystickAxis.AxisX].
            AxisDetails axisDetails;

            axisDetails = device.Axis[JoystickAxis.AxisAccX] as AxisDetails;
            axisDetails.min=buff[0];
            axisDetails.max = buff[4];

            axisDetails = device.Axis[JoystickAxis.AxisAccY] as AxisDetails;
            axisDetails.min = buff[1];
            axisDetails.max = buff[5];

            axisDetails = device.Axis[JoystickAxis.AxisAccZ] as AxisDetails;
            axisDetails.min = buff[2];
            axisDetails.max = buff[6];

            //mWiimoteState.AccelCalibrationInfo.X0 = buff[0];
            //mWiimoteState.AccelCalibrationInfo.Y0 = buff[1];
            //mWiimoteState.AccelCalibrationInfo.Z0 = buff[2];
            //mWiimoteState.AccelCalibrationInfo.XG = buff[4];
            //mWiimoteState.AccelCalibrationInfo.YG = buff[5];
            //mWiimoteState.AccelCalibrationInfo.ZG = buff[6];

            
        }

        /// <summary>
        /// Set Wiimote reporting mode (if using an IR report type, IR sensitivity is set to WiiLevel3)
        /// </summary>
        /// <param name="type">Report type</param>
        /// <param name="continuous">Continuous data</param>
        public void SetReportType(InputReport type, bool continuous)
        {
            SetReportType(type, IRSensitivity.Maximum, continuous);
        }

        /// <summary>
        /// Set Wiimote reporting mode
        /// </summary>
        /// <param name="type">Report type</param>
        /// <param name="irSensitivity">IR sensitivity</param>
        /// <param name="continuous">Continuous data</param>
        public void SetReportType(WiimoteDevice device,InputReport type, IRSensitivity irSensitivity, bool continuous)
        {
            switch (type)
            {
                case InputReport.ButtonsIRAccel:
                    EnableIR(device,IRMode.Extended, irSensitivity);
                    break;
                case InputReport.IRExtensionAccel:
                    EnableIR(device,IRMode.Basic, irSensitivity);
                    break;
                default:
                    DisableIR(device);
                    break;
            }

            ClearReport();
            mBuff[0] = (byte)OutputReport.Type;
           // mBuff[1] = (byte)((continuous ? 0x04 : 0x00) | (byte)(mWiimoteState.Rumble ? 0x01 : 0x00));
            mBuff[1] = (byte)((continuous ? 0x04 : 0x00) | device.RumbleBit);
            mBuff[2] = (byte)type;

            WriteReport(device);
        }

        /// <summary>
        /// Set the LEDs on the Wiimote
        /// </summary>
        /// <param name="led1">LED 1</param>
        /// <param name="led2">LED 2</param>
        /// <param name="led3">LED 3</param>
        /// <param name="led4">LED 4</param>
        public void SetLEDs(WiimoteDevice device, bool led1, bool led2, bool led3, bool led4)
        {
            device.LED[0]=led1;
            device.LED[1]=led2;
            device.LED[2]=led3;
            device.LED[3]=led4;
            //mWiimoteState.LEDState.LED1 = led1;
            //mWiimoteState.LEDState.LED2 = led2;
            //mWiimoteState.LEDState.LED3 = led3;
            //mWiimoteState.LEDState.LED4 = led4;

            ClearReport();

            mBuff[0] = (byte)OutputReport.LEDs;
            mBuff[1] = (byte)(
                        (led1 ? 0x10 : 0x00) |
                        (led2 ? 0x20 : 0x00) |
                        (led3 ? 0x40 : 0x00) |
                        (led4 ? 0x80 : 0x00) |
                        device.RumbleBit);

            WriteReport(device);
        }

        /// <summary>
        /// Set the LEDs on the Wiimote
        /// </summary>
        /// <param name="leds">The value to be lit up in base2 on the Wiimote</param>
        public void SetLEDs(WiimoteDevice device, int leds)
        {

     


            device.LED[0] = (leds & 0x01) > 0;
            device.LED[1] = (leds & 0x02) > 0;
            device.LED[2] = (leds & 0x04) > 0;
            device.LED[3] = (leds & 0x08) > 0;


            //mWiimoteState.LEDState.LED1 = (leds & 0x01) > 0;
            //mWiimoteState.LEDState.LED2 = (leds & 0x02) > 0;
            //mWiimoteState.LEDState.LED3 = (leds & 0x04) > 0;
            //mWiimoteState.LEDState.LED4 = (leds & 0x08) > 0;

            ClearReport();

            mBuff[0] = (byte)OutputReport.LEDs;
            mBuff[1] = (byte)(
                        ((leds & 0x01) > 0 ? 0x10 : 0x00) |
                        ((leds & 0x02) > 0 ? 0x20 : 0x00) |
                        ((leds & 0x04) > 0 ? 0x40 : 0x00) |
                        ((leds & 0x08) > 0 ? 0x80 : 0x00) |
                        device.RumbleBit);

            WriteReport(device);
        }

        /// <summary>
        /// Toggle rumble
        /// </summary>
        /// <param name="on">On or off</param>
        public void SetRumble(WiimoteDevice device,bool on)
        {
            device.Rumble = on;

            // the LED report also handles rumble
            SetLEDs(device,device.LED[0],
                    device.LED[1],
                    device.LED[2],
                    device.LED[3]);
        }

        /// <summary>
        /// Retrieve the current status of the Wiimote and extensions.  Replaces GetBatteryLevel() since it was poorly named.
        /// </summary>
        public void GetStatus(WiimoteDevice device)
        {
            ClearReport();

            mBuff[0] = (byte)OutputReport.Status;
            mBuff[1] = device.RumbleBit;
            //mBuff[1] = GetRumbleBit();

            WriteReport(device);
        }

        /// <summary>
        /// Turn on the IR sensor
        /// </summary>
        /// <param name="mode">The data report mode</param>
        /// <param name="irSensitivity">IR sensitivity</param>
        private void EnableIR(WiimoteDevice device, IRMode mode, IRSensitivity irSensitivity)
        {
            device.Mode = mode;

            ClearReport();
            mBuff[0] = (byte)OutputReport.IR;
            mBuff[1] = (byte)(0x04 | device.RumbleBit);
            WriteReport(device);

            ClearReport();
            mBuff[0] = (byte)OutputReport.IR2;
            mBuff[1] = (byte)(0x04 | device.RumbleBit);
            WriteReport(device);

            WriteData(device,REGISTER_IR, 0x08);
            switch (irSensitivity)
            {
                case IRSensitivity.WiiLevel1:
                    WriteData(device,REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x64, 0x00, 0xfe });
                    WriteData(device,REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0xfd, 0x05 });
                    break;
                case IRSensitivity.WiiLevel2:
                    WriteData(device,REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x96, 0x00, 0xb4 });
                    WriteData(device,REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0xb3, 0x04 });
                    break;
                case IRSensitivity.WiiLevel3:
                    WriteData(device,REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0xaa, 0x00, 0x64 });
                    WriteData(device,REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0x63, 0x03 });
                    break;
                case IRSensitivity.WiiLevel4:
                    WriteData(device,REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0xc8, 0x00, 0x36 });
                    WriteData(device,REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0x35, 0x03 });
                    break;
                case IRSensitivity.WiiLevel5:
                    WriteData(device,REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x07, 0x00, 0x00, 0x71, 0x01, 0x00, 0x72, 0x00, 0x20 });
                    WriteData(device,REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0x1, 0x03 });
                    break;
                case IRSensitivity.Maximum:
                    WriteData(device,REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x90, 0x00, 0x41 });
                    WriteData(device,REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0x40, 0x00 });
                    break;
                default:
                    throw new ArgumentOutOfRangeException("irSensitivity");
            }
            WriteData(device,REGISTER_IR_MODE, (byte)mode);
            WriteData(device,REGISTER_IR, 0x08);
        }

        /// <summary>
        /// Disable the IR sensor
        /// </summary>
        private void DisableIR(WiimoteDevice device)
        {
            device.Mode = IRMode.Off;

            ClearReport();
            mBuff[0] = (byte)OutputReport.IR;
            mBuff[1] = device.RumbleBit;
            WriteReport(device);

            ClearReport();
            mBuff[0] = (byte)OutputReport.IR2;
            mBuff[1] = device.RumbleBit;
            WriteReport(device);
        }

        /// <summary>
        /// Initialize the report data buffer
        /// </summary>
        private void ClearReport()
        {
            Array.Clear(mBuff, 0, REPORT_LENGTH);
        }

        /// <summary>
        /// Write a report to the Wiimote
        /// </summary>
        private void WriteReport(WiimoteDevice device)
        {
            FileStream stream;
            if (mAltWriteMethod)
            {

                UnsafeNativeMethods.HidD_SetOutputReport(WiiDriver._AsyncPackages[device].DangerousGetHandle(), mBuff, (uint)mBuff.Length);
            }
            else if ((stream = WiiDriver._FileStreams[device]) != null)
            {
                stream.Write(mBuff, 0, REPORT_LENGTH);
            }

            Thread.Sleep(100);
        }

        /// <summary>
        /// Read data or register from Wiimote
        /// </summary>
        /// <param name="address">Address to read</param>
        /// <param name="size">Length to read</param>
        /// <returns>Data buffer</returns>
        public byte[] ReadData(WiimoteDevice device,int address, short size)
        {
            ClearReport();

            mReadBuff = new byte[size];
            mAddress = address & 0xffff;
            mSize = size;

            mBuff[0] = (byte)OutputReport.ReadMemory;
            mBuff[1] = (byte)(((address & 0xff000000) >> 24) | device.RumbleBit);
            mBuff[2] = (byte)((address & 0x00ff0000) >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00) >> 8);
            mBuff[4] = (byte)(address & 0x000000ff);

            mBuff[5] = (byte)((size & 0xff00) >> 8);
            mBuff[6] = (byte)(size & 0xff);

            WriteReport(device);

            if (!mReadDone.WaitOne(1000, false))
                throw new WiimoteException("Error reading data from Wiimote...is it connected?");

            return mReadBuff;
        }

        /// <summary>
        /// Write a single byte to the Wiimote
        /// </summary>
        /// <param name="address">Address to write</param>
        /// <param name="data">Byte to write</param>
        public void WriteData(WiimoteDevice device,int address, byte data)
        {
            WriteData(device,address, 1, new byte[] { data });
        }

        /// <summary>
        /// Write a byte array to a specified address
        /// </summary>
        /// <param name="address">Address to write</param>
        /// <param name="size">Length of buffer</param>
        /// <param name="buff">Data buffer</param>

        public void WriteData(WiimoteDevice device,int address, byte size, byte[] buff)
        {
            ClearReport();

            mBuff[0] = (byte)OutputReport.WriteMemory;
            mBuff[1] = (byte)(((address & 0xff000000) >> 24) | device.RumbleBit);
            mBuff[2] = (byte)((address & 0x00ff0000) >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00) >> 8);
            mBuff[4] = (byte)(address & 0x000000ff);
            mBuff[5] = size;
            Array.Copy(buff, 0, mBuff, 6, size);

            WriteReport(device);

            Thread.Sleep(100);
        }

      


        #region Enums

            /// <summary>
    /// Sensitivity of the IR camera on the Wiimote
    /// </summary>
    public enum IRSensitivity
    {
        /// <summary>
        /// Equivalent to level 1 on the Wii console
        /// </summary>
        WiiLevel1,
        /// <summary>
        /// Equivalent to level 2 on the Wii console
        /// </summary>
        WiiLevel2,
        /// <summary>
        /// Equivalent to level 3 on the Wii console (default)
        /// </summary>
        WiiLevel3,
        /// <summary>
        /// Equivalent to level 4 on the Wii console
        /// </summary>
        WiiLevel4,
        /// <summary>
        /// Equivalent to level 5 on the Wii console
        /// </summary>
        WiiLevel5,
        /// <summary>
        /// Maximum sensitivity
        /// </summary>
        Maximum
    }


        /// <summary>
        /// The extension plugged into the Wiimote
        /// </summary>
        [DataContract]
        public enum WiiExtensionType : byte
        {
            /// <summary>
            /// No extension
            /// </summary>
            None = 0x00,
            /// <summary>
            /// Nunchuk extension
            /// </summary>
            Nunchuk = 0xfe,
            /// <summary>
            /// Classic Controller extension
            /// </summary>
            ClassicController = 0xfd,

            // hmm...what's 0xfc?

            /// <summary>
            /// Guitar controller from Guitar Hero
            /// </summary>
            Guitar = 0xfb
        };

        /// <summary>
        /// The mode of data reported for the IR sensor
        /// </summary>
        [DataContract]
        public enum IRMode : byte
        {
            /// <summary>
            /// IR sensor off
            /// </summary>
            Off = 0x00,
            /// <summary>
            /// Basic mode
            /// </summary>
            Basic = 0x01,	// 10 bytes
            /// <summary>
            /// Extended mode
            /// </summary>
            Extended = 0x03,	// 12 bytes
            /// <summary>
            /// Full mode (unsupported)
            /// </summary>
            Full = 0x05,	// 16 bytes * 2 (format unknown)
        };

        /// <summary>
        /// The report format in which the Wiimote should return data
        /// </summary>	
        public enum InputReport : byte
        {
            /// <summary>
            /// Status report
            /// </summary>
            Status = 0x20,
            /// <summary>
            /// Read data from memory location
            /// </summary>
            ReadData = 0x21,
            /// <summary>
            /// Button data only
            /// </summary>
            Buttons = 0x30,
            /// <summary>
            /// Button and accelerometer data
            /// </summary>
            ButtonsAccel = 0x31,
            /// <summary>
            /// IR sensor and accelerometer data
            /// </summary>
            ButtonsIRAccel = 0x33,
            /// <summary>
            /// Button and extension controller data
            /// </summary>
            ButtonsExtension = 0x34,
            /// <summary>
            /// Extension and accelerometer data
            /// </summary>
            ButtonsExtensionAccel = 0x35,
            /// <summary>
            /// IR sensor, extension controller and accelerometer data
            /// </summary>
            IRExtensionAccel = 0x37,
        };


        [Flags]
        public enum EFileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }




        #endregion


        /// <summary>
        /// Win32 import information for use with the Wiimote library
        /// </summary>
        internal class UnsafeNativeMethods
        {
            //
            // Flags controlling what is included in the device information set built
            // by SetupDiGetClassDevs
            //
            public const int DIGCF_DEFAULT = 0x00000001; // only valid with DIGCF_DEVICEINTERFACE
            public const int DIGCF_PRESENT = 0x00000002;
            public const int DIGCF_ALLCLASSES = 0x00000004;
            public const int DIGCF_PROFILE = 0x00000008;
            public const int DIGCF_DEVICEINTERFACE = 0x00000010;

        
            [StructLayout(LayoutKind.Sequential)]
            public struct SP_DEVINFO_DATA
            {
                public uint cbSize;
                public Guid ClassGuid;
                public uint DevInst;
                public IntPtr Reserved;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SP_DEVICE_INTERFACE_DATA
            {
                public int cbSize;
                public Guid InterfaceClassGuid;
                public int Flags;
                public IntPtr RESERVED;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct SP_DEVICE_INTERFACE_DETAIL_DATA
            {
                public UInt32 cbSize;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
                public string DevicePath;
            }

            //[StructLayout(LayoutKind.Sequential)]
            //public struct HIDD_ATTRIBUTES
            //{
            //    public int Size;
            //    public short VendorID;
            //    public short ProductID;
            //    public short VersionNumber;
            //}

            //The HidD_GetHidGuid routine returns the device interfaceGUID for HIDClass devices.
            //[DllImport(@"hid.dll", CharSet=CharSet.Auto, SetLastError = true)]
            //public static extern void HidD_GetHidGuid(out Guid gHid);

            //[DllImport("hid.dll")]
            //public static extern Boolean HidD_GetAttributes(IntPtr HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

            [DllImport("hid.dll")]
            internal extern static bool HidD_SetOutputReport(
                IntPtr HidDeviceObject,
                byte[] lpReportBuffer,
                uint ReportBufferLength);

            //[DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
            //public static extern IntPtr SetupDiGetClassDevs(
            //    ref Guid ClassGuid,
            //    [MarshalAs(UnmanagedType.LPTStr)] string Enumerator,
            //    IntPtr hwndParent,
            //    UInt32 Flags
            //    );

            //[DllImport(@"setupapi.dll", CharSet=CharSet.Auto, SetLastError = true)]
            //public static extern Boolean SetupDiEnumDeviceInterfaces(
            //    IntPtr hDevInfo,
            //    //ref SP_DEVINFO_DATA devInfo,
            //    IntPtr devInvo,
            //    ref Guid interfaceClassGuid,
            //    Int32 memberIndex,
            //    ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
            //);

            //[DllImport(@"setupapi.dll", SetLastError = true)]
            //public static extern Boolean SetupDiGetDeviceInterfaceDetail(
            //    IntPtr hDevInfo,
            //    ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            //    IntPtr deviceInterfaceDetailData,
            //    UInt32 deviceInterfaceDetailDataSize,
            //    out UInt32 requiredSize,
            //    IntPtr deviceInfoData
            //);

            //[DllImport(@"setupapi.dll", SetLastError = true)]
            //public static extern Boolean SetupDiGetDeviceInterfaceDetail(
            //    IntPtr hDevInfo,
            //    ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            //    ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
            //    UInt32 deviceInterfaceDetailDataSize,
            //    out UInt32 requiredSize,
            //    IntPtr deviceInfoData
            //);

            ////distroy device list
            //[DllImport(@"setupapi.dll", CharSet=CharSet.Auto, SetLastError = true)]
            //public static extern UInt16 SetupDiDestroyDeviceInfoList( IntPtr hDevInfo );

            //Create file
            [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern SafeFileHandle CreateFile(
                string fileName,
                [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
                [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
                IntPtr securityAttributes,
                [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
                [MarshalAs(UnmanagedType.U4)] EFileAttributes flags,
                IntPtr template);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(IntPtr hObject);
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose Wiimote
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose wiimote
        /// </summary>
        /// <param name="disposing">Disposing?</param>
        protected virtual void Dispose(bool disposing)
        {
             // close up our handles
            if (disposing)
            {
                foreach (var handle in WiiDriver._AsyncPackages)
                {
                    handle.Value.Close();

                }

                WiiDriver._AsyncPackages.Clear();

                foreach (var stream in WiiDriver._FileStreams)
                {
                    stream.Value.Close();

                }

                WiiDriver._FileStreams.Clear();
            }


           
               
        }
        #endregion








            #region IAxisDetails implementation


            public byte mid
            {
                get
                {
                    return _mid;
                }
                set
                {
                    _mid = value;
                }
            }

            public byte min
            {
                get
                {
                    return _min;
                }
                set
                {
                    _min = value;
                }
            }


            public byte max
            {
                get
                {
                    return _max;
                }
                set
                {
                    _max = value;
                }
            }


            public bool isNullable
            {
                get
                {
                    return _isNullable;
                }
                set
                {
                    _isNullable = value;
                }
            }


            public bool isHat
            {
                get
                {
                    return _isHat;
                }
                set
                {
                    _isHat = value;
                }
            }

#endregion

        }
        #endregion

        #region AxisDetails
        public sealed class AxisDetails : IAxisDetails
        {

            #region Fields
            float _value;
            int _uid;
            byte _min;
            byte _max;
            byte _mid;
            JoystickButtonState _buttonState;
            bool _isNullable;
            bool _isHat;


            #region IAxisDetails implementation


            public byte mid
            {
                get
                {
                    return _mid;
                }
                set
                {
                    _mid = value;
                }
            }

            public byte min
            {
                get
                {
                    return _min;
                }
                set
                {
                    _min = value;
                }
            }


            public byte max
            {
                get
                {
                    return _max;
                }
                set
                {
                    _max = value;
                }
            }


            public bool isNullable
            {
                get
                {
                    return _isNullable;
                }
                set
                {
                    _isNullable = value;
                }
            }


            public bool isHat
            {
                get
                {
                    return _isHat;
                }
                set
                {
                    _isHat = value;
                }
            }


            #endregion


            #region IDeviceDetails implementation


            public uint uid
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }


            #endregion

            public JoystickButtonState buttonState
            {
                get { return _buttonState; }
            }
            public float value
            {
                get { return _value; }
                set
                {

                    if (value == 0)
                    {
                        if (_buttonState == JoystickButtonState.Down
                            || _buttonState == JoystickButtonState.Hold)
                        {

                            //axis float value isn't yet update so it have value before getting 0
                            if (_value > 0)//0 come after positive values
                                _buttonState = JoystickButtonState.PosToUp;
                            else
                                _buttonState = JoystickButtonState.NegToUp;

                        }
                        else
                        {//if(buttonState==JoystickButtonState.Up){
                            _buttonState = JoystickButtonState.None;
                        }


                    }
                    else
                    {
                        if (_buttonState == JoystickButtonState.None
                            || _buttonState == JoystickButtonState.Up)
                        {

                            _buttonState = JoystickButtonState.Down;

                        }
                        else
                        {
                            _buttonState = JoystickButtonState.Hold;
                        }


                    }

                    _value = value;



                }//set
            }

            #endregion

        }

        #endregion


        #region ButtonDetails
        public sealed class ButtonDetails : IButtonDetails
        {

            #region Fields

            float _value;
            uint _uid;
            JoystickButtonState _buttonState;

            #region IDeviceDetails implementation


            public uint uid
            {
                get
                {
                    return _uid;
                }
                set
                {
                    _uid = value;
                }
            }




            public JoystickButtonState buttonState
            {
                get { return _buttonState; }
            }



            public float value
            {
                get
                {
                    return _value;
                    //return (_buttonState==JoystickButtonState.Hold || _buttonState==JoystickButtonState.Down);
                }
                set
                {

                    _value = value;
                    //if pressed==TRUE
                    //TODO check the code with triggers
                    if (value > 0)
                    {
                        if (_buttonState == JoystickButtonState.None
                            || _buttonState == JoystickButtonState.Up)
                        {

                            _buttonState = JoystickButtonState.Down;



                        }
                        else
                        {
                            //if (buttonState == JoystickButtonState.Down)
                            _buttonState = JoystickButtonState.Hold;

                        }


                    }
                    else
                    { //
                        if (_buttonState == JoystickButtonState.Down
                            || _buttonState == JoystickButtonState.Hold)
                        {
                            _buttonState = JoystickButtonState.Up;
                        }
                        else
                        {//if(buttonState==JoystickButtonState.Up){
                            _buttonState = JoystickButtonState.None;
                        }

                    }
                }
            }
            #endregion
            #endregion

            #region Constructor
            public ButtonDetails(uint uid = 0) { this.uid = uid; }
            #endregion






        }

        #endregion






    }

    /// <summary>
    /// Represents errors that occur during the execution of the Wiimote library
    /// </summary>
    [Serializable]
    public class WiimoteException : ApplicationException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public WiimoteException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public WiimoteException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public WiimoteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected WiimoteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}