//////////////////////////////////////////////////////////////////////////////////
//	
//  Original code by Written by Brian Peek (http://www.brianpeek.com/)
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using ws.winx.devices;
using System.Collections.Generic;
using ws.winx.platform;


namespace ws.winx.drivers
{
    /// <summary>
    /// Implementation of WiiDriver
    /// 
  

    /// </summary>
    public class WiiDriver : IDriver, IDisposable
    {






        protected static IHIDInterface _hidInterface;

       

        // VID = Nintendo, PID = Wiimote
        private const int VID =0x057e;
        private const int PID =0x0306;
        private const int PID_MOTION_PLUS_INSIDE = 0x0330;

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

        // wiimote registers
        const int REGISTER_CALIBRATION = 0x0016;
        const int REGISTER_IR = 0x4b00030;
        const int REGISTER_IR_SENSITIVITY_1 = 0x4b00000;
        const int REGISTER_IR_SENSITIVITY_2 = 0x4b0001a;
        const int REGISTER_IR_MODE = 0x4b00033;
        const int REGISTER_EXTENSION_INIT = 0x04a40040;
        const int REGISTER_EXTENSION_INIT1 = 0x4a400f0;
        const int REGISTER_EXTENSION_INIT2 = 0x4a400fb;
        const int REGISTER_EXTENSION_TYPE = 0x4a400fa;
        const int REGISTER_EXTENSION_CALIBRATION = 0x4a40020;
        const int REGISTER_BALANCE_CALIBRATION = 0x4a40024;
        const int REGISTER_MOTIONPLUS_DETECT = 0x4a600fa;//0x(4)a600fa
        const int REGISTER_MOTIONPLUS_INIT = 0x4a600f0;//0x4a600f0
        const int REGISTER_MODE = 0x4a600fe;//0x04a600fe


      

        /// <summary>
        /// Default constructor
        /// </summary>
        public WiiDriver()
        {
        }


        public void Update(IDevice device)
        {

            HIDDevice hidDevice;
           WiimoteDevice wDevice=device as WiimoteDevice;

            //Add states

            if(wDevice!=null){

          

            switch(wDevice.processingMode){
                case ProcessingMode.InProgress:
                    return;
                
                case ProcessingMode.Update:
                    wDevice.processingMode=ProcessingMode.InProgress;
                    _hidInterface.Read(wDevice,onRead);
                        
                break;

                case ProcessingMode.AccCalibration:
                       ReadAccCalibration(wDevice, REGISTER_CALIBRATION, 7);
                break;

                case ProcessingMode.ExtCheck:
                 // force a status check to get the state of any extensions plugged in at startup
                   GetStatus(wDevice);
                break;

                case ProcessingMode.MPlusCheck:
                    CheckMotionPlusCapabilities(wDevice);
                break;

                 case ProcessingMode.MPlusInit:
                   InitializeMotionPlus(wDevice);
                break;

                 case ProcessingMode.MPlusCalibration:
                  
                break;

            }
            
           
            }

          


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="address"></param>
        /// <param name="size"></param>
        internal void ReadAccCalibration(WiimoteDevice device, int address, short size)
        {
            device.processingMode=ProcessingMode.InProgress;

            HIDDevice hidDevice = _hidInterface.Generics[device];

            hidDevice.InputReportByteLength = REPORT_LENGTH;
            hidDevice.OutputReportByteLength = REPORT_LENGTH;

            //clean extension
            // WriteMemory(device,REGISTER_EXTENSION_INIT2, 0x00);
            // _hidInterface.Read(device, onReadCalibration);


            //request calibration
            ReadMemory(device, address, size, (suc) =>
            {
                // _hidInterface.Read(device, onReadCalibration);
                Debug.WriteLineIf(!suc, "Read Calibaration failed");
               
                _hidInterface.Read(device, onReadAccCalibration);
            });




        }


        /// <summary>
        /// handles calibration read information stored on Wiimote
        /// </summary>
        private void onReadAccCalibration(object data)
        {
            HIDReport report = data as HIDReport;

            if (report.Status == HIDReport.ReadStatus.Success)
            {
                 WiimoteDevice device = _hidInterface.Devices[report.index] as WiimoteDevice;
                byte[] buff = report.Data;

                // if (report != null || report.Status != HIDReport.ReadStatus.Success || report.Data == null) return;
                UnityEngine.Debug.Log(BitConverter.ToString(buff));

                ParseButtons(device, buff);

                buff = ParseReadData(REGISTER_CALIBRATION, buff);


                // AxisDetails[JoystickAxis.AxisX].
                AxisDetails axisDetails;

                axisDetails = device.Axis[JoystickAxis.AxisAccX] as AxisDetails;
                axisDetails.min = buff[0];
                axisDetails.max = buff[4];

                axisDetails = device.Axis[JoystickAxis.AxisAccY] as AxisDetails;
                axisDetails.min = buff[1];
                axisDetails.max = buff[5];

                axisDetails = device.Axis[JoystickAxis.AxisAccZ] as AxisDetails;
                axisDetails.min = buff[2];
                axisDetails.max = buff[6];

                //00-00-60-00-16-80-80-7F-17-99-99-98
                //mWiimoteState.AccelCalibrationInfo.X0 = buff[0];
                //mWiimoteState.AccelCalibrationInfo.Y0 = buff[1];
                //mWiimoteState.AccelCalibrationInfo.Z0 = buff[2];
                //mWiimoteState.AccelCalibrationInfo.XG = buff[4];
                //mWiimoteState.AccelCalibrationInfo.YG = buff[5];
                //mWiimoteState.AccelCalibrationInfo.ZG = buff[6];

               device.processingMode=ProcessingMode.ExtCheck;
            }




        }



        public void onAnyRead(object data)
        {
            HIDReport report = data as HIDReport;

            // if (report != null || report.Status != HIDReport.ReadStatus.Success || report.Data == null) return;
            UnityEngine.Debug.Log("Any Read"+BitConverter.ToString(report.Data));

        }

        public void onRead(object data) { 
            
            ParseInputReport(data as HIDReport); }

        public IDevice ResolveDevice(IHIDDevice hidDevice)
        {


            if ((hidDevice.VID == VID && hidDevice.PID == PID) || (hidDevice.VID == VID && hidDevice.PID == PID_MOTION_PLUS_INSIDE))
            {
                _hidInterface = hidDevice.hidInterface;

              

                WiimoteDevice device;
                int inx = 0;


                // create new Device
                device = new WiimoteDevice(((IHIDInterface)_hidInterface).Devices.Count, hidDevice.PID, hidDevice.VID, 16, 12, 4, 4, this);



                //inti button structure
                for (; inx < 12; inx++)
                {
                    device.Buttons[inx] = new ButtonDetails();
                }

                AxisDetails axisDetails;


                //AccX
                axisDetails = new AxisDetails();
                device.Axis[JoystickAxis.AxisAccX] = axisDetails;

                //AccY
                axisDetails = new AxisDetails();
                device.Axis[JoystickAxis.AxisAccY] = axisDetails;

                //AccZ
                axisDetails = new AxisDetails();
                device.Axis[JoystickAxis.AxisAccZ] = axisDetails;

                //AccR
                axisDetails = new AxisDetails();
                device.Axis[JoystickAxis.AxisAccR] = axisDetails;

                //AccU
                axisDetails = new AxisDetails();
                device.Axis[JoystickAxis.AxisAccU] = axisDetails;

                //AccV
                axisDetails = new AxisDetails();
                device.Axis[JoystickAxis.AxisAccV] = axisDetails;


                //LX
                axisDetails = new AxisDetails();
                //  axisDetails.max = 32767;
                //  axisDetails.min = -32767;
                device.Axis[JoystickAxis.AxisX] = axisDetails;

                //LY
                axisDetails = new AxisDetails();
                //    axisDetails.max = 32767;
                //    axisDetails.min = -32767;
                device.Axis[JoystickAxis.AxisY] = axisDetails;

                //RX
                axisDetails = new AxisDetails();
                //       axisDetails.max = 32767;
                //        axisDetails.min = -32767;
                device.Axis[JoystickAxis.AxisZ] = axisDetails;

                //RY
                axisDetails = new AxisDetails();
                //      axisDetails.max = 32767;
                //      axisDetails.min = -32767;
                device.Axis[JoystickAxis.AxisR] = axisDetails;


                //TRIGGERS
                axisDetails = new AxisDetails();
                axisDetails.max = 255;
                axisDetails.min = 0;
                axisDetails.isTrigger = true;
                device.Axis[JoystickAxis.AxisU] = axisDetails;


                axisDetails = new AxisDetails();
                axisDetails.max = 255;
                axisDetails.min = 0;
                axisDetails.isTrigger = true;
                device.Axis[JoystickAxis.AxisV] = axisDetails;

                //POV
                axisDetails = new AxisDetails();
                axisDetails.isHat = true;
                device.Axis[JoystickAxis.AxisPovX] = axisDetails;
                axisDetails = new AxisDetails();
                axisDetails.isHat = true;
                device.Axis[JoystickAxis.AxisPovY] = axisDetails;





                device.isReady = false;



                //((HIDDevice)hidDevice).InputReportByteLength = REPORT_LENGTH;
               // ((HIDDevice)hidDevice).OutputReportByteLength = 7;

            
                device.processingMode=ProcessingMode.AccCalibration;

                return device;

            }


            return null;
        }



      

        protected void onCalibrationRequestStatus(bool success)
        {
            UnityEngine.Debug.Log("onCalibrationRequestStatus:" + success);
        }




           

        /// <summary>
        /// Parse HIDReport
        /// </summary>
        /// <param name="report"></param>
        private void ParseInputReport(HIDReport report)
        {
            byte[] buff = report.Data;



            WiimoteDevice device = _hidInterface.Devices[report.index] as WiimoteDevice;
            device.isReady = true;

            InputReport type = (InputReport)buff[0];

            switch (type)
            {
                case InputReport.Buttons:
                    ParseButtons(device, buff);
                    break;
                case InputReport.ButtonsAccel:
                    ParseButtons(device, buff);
                    ParseAccel(device, buff);
                    break;
                case InputReport.ButtonsIRAccel:
                    ParseButtons(device, buff);
                    ParseAccel(device, buff);
                    ParseIR(device, buff);
                    break;
                case InputReport.ButtonsExtension:
                    ParseButtons(device, buff);
                    ParseExtension(device, buff, 4);
                    break;
                case InputReport.ButtonsExtensionAccel:
                    ParseButtons(device, buff);
                    ParseAccel(device, buff);
                    ParseExtension(device, buff, 6);
                    break;
                case InputReport.IRExtensionAccel:
                    ParseButtons(device, buff);
                    ParseAccel(device, buff);
                    ParseIR(device, buff);
                    ParseExtension(device, buff, 16);
                    break;
                case InputReport.Status:


                       

                  

					Debug.WriteLine("******** STATUS ********");
                    ParseButtons(device,buff);
					//device.BatteryRaw = buff[6];
					device.Battery = (((100.0f * 48.0f * (float)((int)buff[6] / 48.0f))) / 192.0f);

					// get the real LED values in case the values from SetLEDs() somehow becomes out of sync, which really shouldn't be possible
					device.LED[0] = (buff[3] & 0x10) != 0;
					device.LED[1] = (buff[3] & 0x20) != 0;
					device.LED[2] = (buff[3] & 0x40) != 0;
					device.LED[3] = (buff[3] & 0x80) != 0;

					
				

					// extension connected?
					bool extension = (buff[3] & 0x02) != 0;
					Debug.WriteLine("Extension, Old: " + (device.Extensions!=0x00) + ", New: " + extension);



                 

                        if (extension)
                        {
                             
                              // long extensionNumber;
           
                              // buff = ReadData(REGISTER_EXTENSION_TYPE, 6);
                             //  extensionNumber = ((long)buff[0] << 40) | ((long)buff[1] << 32) | ((long)buff[2]) << 24 | ((long)buff[3]) << 16 | ((long)buff[4]) << 8 | buff[5];

                              ReadExtension(device);


                      
                     }
                    else if(device.MotionPlusStatus == (byte)MotionPlusStatus.None)
                    {
                       device.processingMode=ProcessingMode.MPlusCheck;
                    }
                    
                    
					



                    //ParseButtons(device, buff);
                    //device.Battery = buff[6];

                    //// get the real LED values in case the values from SetLEDs() somehow becomes out of sync, which really shouldn't be possible


                    //device.LED[0] = (buff[3] & 0x10) != 0;
                    //device.LED[1] = (buff[3] & 0x20) != 0;
                    //device.LED[2] = (buff[3] & 0x40) != 0;
                    //device.LED[3] = (buff[3] & 0x80) != 0;

                    ////mWiimoteState.LEDState.LED1 = (buff[3] & 0x10) != 0;
                    ////mWiimoteState.LEDState.LED2 = (buff[3] & 0x20) != 0;
                    ////mWiimoteState.LEDState.LED3 = (buff[3] & 0x40) != 0;
                    ////mWiimoteState.LEDState.LED4 = (buff[3] & 0x80) != 0;

                    //// extension connected?
                    //bool extension = (buff[3] & 0x02) != 0;
                    ////Debug.WriteLine("Extension: " + extension);
                    //UnityEngine.Debug.Log("Extension: " + extension);


                    ////if (mWiimoteState.Extension != extension)
                    //if (device.hasExtensionDevice != extension)
                    //{
                    //    device.hasExtensionDevice = extension;

                    //    if (extension)
                    //    {
                    //        device.isReady = false;
                    //        InitializeExtension(device);
                           
                    //    }
                    //    else
                    //    {
                    //        device.ExtensionType = WiiExtensionType.None;
                           
                    //    }

                    //    //  if (WiimoteExtensionChanged != null)
                    //    //     WiimoteExtensionChanged(this, new WiimoteExtensionChangedEventArgs(mWiimoteState.ExtensionType, mWiimoteState.Extension));
                    //}

                    break;
                //case InputReport.ReadData:
                //    ParseButtons(device, buff);
                //    ParseReadData(device, buff);
                //    break;
                default:
                    UnityEngine.Debug.Log("Unknown report type: " + type.ToString("x"));
                break;
                   
            }

            
          
        }



        /// <summary>
		/// Initialize the MotionPlus extension
		/// </summary>
		public void InitializeMotionPlus(WiimoteDevice device)
		{

          
           
            if ((device.Extensions & (byte)ExtensionType.MotionPlus) == 0 )
            {
                Debug.WriteLine("InitializeMotionPlus");

                device.processingMode=ProcessingMode.InProgress;
                // Initialize it:
               // WriteData(REGISTER_MOTIONPLUS_INIT, 0x55);
                WriteMemory(device,REGISTER_MOTIONPLUS_INIT, 0x55);

                //WriteData(REGISTER_MOTIONPLUS_INIT, 0x04);//0xa600fe
                WriteMemory(device,REGISTER_MODE,0x04);
            }

		}


        
        /// <summary>
        /// 
        /// </summary>
        public void CheckMotionPlusCapabilities(WiimoteDevice device)
        {
            device.processingMode=ProcessingMode.InProgress;



                  Debug.WriteLine("Try:"+device.numMPlusChecks+" to MOTIONPLUS_DETECT");

               // ReadData(REGISTER_MOTIONPLUS_DETECT, 0x02);
                ReadMemory(device,REGISTER_MOTIONPLUS_DETECT, 0x02,(suc)=>{
                    _hidInterface.Read(device,onCheckMotionPlusCapabilities);
                });


            

          
        
        }




        public void onCheckMotionPlusCapabilities(object data){
            HIDReport report = data as HIDReport;
            byte[] buff=report.Data;

               WiimoteDevice device = _hidInterface.Devices[report.index] as WiimoteDevice;
                
            if(buff[1] == 0x05){
                device.MotionPlusStatus|=(byte)MotionPlusStatus.Exist;
                 device.processingMode=ProcessingMode.MPlusInit;
            }else{
                if(device.numMPlusChecks<4){
                     device.numMPlusChecks++;
                    device.processingMode=ProcessingMode.MPlusCheck;
                 }else{
                     device.processingMode=ProcessingMode.Update;
                }
                 
            }

        }

          /// <summary>
        /// 
        /// </summary>
        public void DisableMotionPlus(WiimoteDevice device)
        {
           
           
             //   WriteData(REGISTER_EXTENSION_INIT_1, 0x55);
            //    WriteData(REGISTER_EXTENSION_INIT_2, 0x00);

             WriteMemory(device,REGISTER_EXTENSION_INIT1, 0x55);
            WriteMemory(device,REGISTER_EXTENSION_INIT2, 0x00);

                device.Extensions &= (byte)0xDF;

            //Status Disabled
            device.MotionPlusStatus &=(byte) 0xD;//mask 0x1101

                Debug.WriteLineIf(((device.Extensions & (byte)ExtensionType.MotionPlus) == 0), "MotionPlus disabled");
        }



        

       

       private void ReadExtension(WiimoteDevice device){


          ReadMemory(device,REGISTER_EXTENSION_TYPE, 6,(suc)=>{

                 _hidInterface.Read(device,onReadExtension);
          });

        
       }

        /// <summary>
        /// Handles setting up an extension when plugged in
        /// </summary>
        private void onReadExtension(object data)
        {
            HIDReport report = data as HIDReport;


            WiimoteDevice device = _hidInterface.Devices[report.index] as WiimoteDevice;

            byte[] buff = report.Data;

            long type = ((long)buff[0] << 40) | ((long)buff[1] << 32) | ((long)buff[2]) << 24 | ((long)buff[3]) << 16 | ((long)buff[4]) << 8 | buff[5];


             if(type != (long)ExtensionNumber.Guitar && type!=(long)ExtensionNumber.Drums)
            type=type & 0x0000ffffffff;

            switch((ExtensionNumber)type)
			{
				case ExtensionNumber.None:
				case ExtensionNumber.ParitallyInserted:
				
					//mWiimoteState.ExtensionType = ExtensionNumber.None;
					return;
				case ExtensionNumber.Nunchuk:
                    if(device.PASS_THRU_MODE==(byte)PassThruMode.None)
                        device.Extensions = (byte)ExtensionType.Nunchuck;
                    else 
                        device.Extensions |= (byte)ExtensionType.Nunchuck;

                  //  Debug.WriteLine(mWiimoteState.NunchukState.CalibrationInfo.MaxX + " " + mWiimoteState.NunchukState.CalibrationInfo.MidX + " " + mWiimoteState.NunchukState.CalibrationInfo.MinX);
                 //   Debug.WriteLine(mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.X0 + " " + mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.XG);

                    break;
				case ExtensionNumber.ClassicController:
                    if(device.PASS_THRU_MODE==(byte)PassThruMode.None)
                          device.Extensions = (int)ExtensionType.ClassicController;
                      else
                         device.Extensions |= (int)ExtensionType.ClassicController;

                        break;
				case ExtensionNumber.Guitar:
                     device.Extensions = (int)ExtensionType.Guitar;
                break;
				case ExtensionNumber.BalanceBoard:
                    device.Extensions = (int)ExtensionType.BalancedBoard;
	            break;
				case ExtensionNumber.Drums:
                    device.Extensions = (int)ExtensionType.Drums;
                break;
				case ExtensionNumber.TaikoDrum:
                     device.Extensions = (byte)ExtensionType.TaikoDrums;
                break;
				case ExtensionNumber.MotionPlus:
                     if(device.PASS_THRU_MODE==(byte)PassThruMode.None)
                         device.Extensions = (byte)ExtensionType.MotionPlus;
                    else
                         device.Extensions |= (byte)ExtensionType.MotionPlus;

                  
                  
					break;
				default:
					throw new Exception("Unknown extension controller found: " + type.ToString("x"));
			}

            Debug.WriteLine("InitExtension " + (ExtensionNumber)type);



                           if (device.PASS_THRU_MODE!=PassThruMode.None)//NOT TESTED (PSEUDO)
                                {


                                    if ((type & 0x0000ffffffff) == (long)ExtensionNumber.Nunchuk)
                                    {
                                         WriteMemory(device,REGISTER_MODE,(byte)PassThruMode.Nunchuck);
                                        //WriteData(REGISTER_MODE, (byte)PassThruMode.Nunchuck);

                                    }
                                    else if ((type & 0x0000ffffffff) == (long)ExtensionNumber.ClassicController)
                                    {
                                        // DisableMotionPlus();/???
                                        WriteMemory(device,REGISTER_MODE, (byte)PassThruMode.ClassicController);
                                        //WriteData(REGISTER_MODE, (byte)PassThruMode.ClassicController);
                                    }
                                           

                                }




        
            
         
              ReadExtensionCalibaration(device,type);
           

          
        }


        protected void ReadExtensionCalibaration(WiimoteDevice device,long type){
              WriteMemory(device, REGISTER_EXTENSION_CALIBRATION, 16,(suc)=>{
                _hidInterface.Read(device, (dt)=>{ onExtensionCalibration(dt,type); }
                    );
           });
        }


        protected void onExtensionCalibration(object data,long type)
        {
            HIDReport report = data as HIDReport;
            WiimoteDevice device = _hidInterface.Devices[report.index] as WiimoteDevice;
            //byte[] buff=DecryptBuffer(report.Data);
            byte[] buff=ParseReadData(REGISTER_EXTENSION_CALIBRATION,report.Data);
                
            AxisDetails axisDetails;

            switch((ExtensionNumber)type)
			{
				
				case ExtensionNumber.Nunchuk:

            
                    //X0,Y0,Z0 are actully MID
                    //XG,YG,ZG are actually MAX
                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.X0 = buff[0];
                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Y0 = buff[1];
                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Z0 = buff[2];
                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.XG = buff[4];
                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.YG = buff[5];
                    //mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.ZG = buff[6];
                    //mWiimoteState.NunchukState.CalibrationInfo.MaxX = buff[8]; //seem this calibration isn't correct
                    //mWiimoteState.NunchukState.CalibrationInfo.MinX = buff[9];
                    //mWiimoteState.NunchukState.CalibrationInfo.MidX = buff[10];
                    //mWiimoteState.NunchukState.CalibrationInfo.MaxY = buff[11];
                    //mWiimoteState.NunchukState.CalibrationInfo.MinY = buff[12];
                    //mWiimoteState.NunchukState.CalibrationInfo.MidY = buff[13];
         


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
                case ExtensionNumber.ClassicController:

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


            SetReportType(device,InputReport.IRExtensionAccel,true);

            device.processingMode=ProcessingMode.Update;

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
            else if ((buff[1] & 0x04) != 0)
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


            var sensor = device.IR_SENSORS[0];



            switch (device.Mode)
            {
                case IRMode.Basic:

                    if (sensor.Found = !(buff[6] == 0xff && buff[7] == 0xff))
                    {
                        sensor.Axis[JoystickAxis.AxisX].value = (float)(buff[6] | ((buff[8] >> 4) & 0x03) << 8) / 1023.5f;
                        sensor.Axis[JoystickAxis.AxisY].value = (float)(buff[7] | ((buff[8] >> 6) & 0x03) << 8) / 767.5f;
                    }

                    sensor = device.IR_SENSORS[1];

                    if (sensor.Found = !(buff[9] == 0xff && buff[10] == 0xff))
                    {
                        sensor.Axis[JoystickAxis.AxisX].value = (float)(buff[9] | ((buff[8] >> 0) & 0x03) << 8) / 1023.5f;
                        sensor.Axis[JoystickAxis.AxisY].value = (float)(buff[10] | ((buff[8] >> 2) & 0x03) << 8) / 767.5f;
                    }



                    //mWiimoteState.IRState.IRSensors[1].RawPosition.X = buff[9]  | ((buff[8] >> 0) & 0x03) << 8;
                    //mWiimoteState.IRState.IRSensors[1].RawPosition.Y = buff[10] | ((buff[8] >> 2) & 0x03) << 8;


                    //mWiimoteState.IRState.IRSensors[0].Size = 0x00;
                    //mWiimoteState.IRState.IRSensors[1].Size = 0x00;

                    //mWiimoteState.IRState.IRSensors[0].Found = !(buff[6] == 0xff && buff[7] == 0xff);
                    //mWiimoteState.IRState.IRSensors[1].Found = !(buff[9] == 0xff && buff[10] == 0xff);
                    break;
                case IRMode.Extended:

                    if (sensor.Found = !(buff[6] == 0xff && buff[7] == 0xff && buff[8] == 0xff))
                    {
                        sensor.Axis[JoystickAxis.AxisX].value = (float)(buff[6] | ((buff[8] >> 4) & 0x03) << 8) / 1023.5f;
                        sensor.Axis[JoystickAxis.AxisY].value = (float)(buff[7] | ((buff[8] >> 6) & 0x03) << 8) / 767.5f;
                        sensor.Size = buff[8] & 0x0f;
                    }


                    sensor = device.IR_SENSORS[1];

                    if (sensor.Found = !(buff[9] == 0xff && buff[10] == 0xff && buff[11] == 0xff))
                    {
                        sensor.Axis[JoystickAxis.AxisX].value = (float)(buff[9] | ((buff[11] >> 4) & 0x03) << 8) / 1023.5f;
                        sensor.Axis[JoystickAxis.AxisY].value = (float)(buff[10] | ((buff[11] >> 6) & 0x03) << 8) / 767.5f;
                        sensor.Size = buff[11] & 0x0f;
                    }

                    sensor = device.IR_SENSORS[2];

                    if (sensor.Found = !(buff[12] == 0xff && buff[13] == 0xff && buff[14] == 0xff))
                    {
                        sensor.Axis[JoystickAxis.AxisX].value = (float)(buff[12] | ((buff[14] >> 4) & 0x03) << 8) / 1023.5f;
                        sensor.Axis[JoystickAxis.AxisY].value = (float)(buff[13] | ((buff[14] >> 6) & 0x03) << 8) / 767.5f;
                        sensor.Size = buff[14] & 0x0f;
                    }


                    sensor = device.IR_SENSORS[2];

                    if (sensor.Found = !(buff[15] == 0xff && buff[16] == 0xff && buff[17] == 0xff))
                    {
                        sensor.Axis[JoystickAxis.AxisX].value = (float)(buff[15] | ((buff[17] >> 4) & 0x03) << 8) / 1023.5f;
                        sensor.Axis[JoystickAxis.AxisY].value = (float)(buff[16] | ((buff[17] >> 6) & 0x03) << 8) / 767.5f;
                        sensor.Size = buff[17] & 0x0f;
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
            AxisDetails axisDetails;


             if  ((device.Extensions & (byte)ExtensionType.Nunchuck) !=0){
			
                    if(device.PASS_THRU_MODE == PassThruMode.Nunchuck){
                        if((buff[offset + 5] & 0x03)==0x00){
                                //interleave mode
                             //if (extension_data.size() >= 6 && !(extension_data[5] & 0x03))
                    //{
                    //    output.valid_data_flags |= dolphiimote_NUNCHUCK_VALID;

                    //    output.nunchuck.stick_x = extension_data[0];
                    //    output.nunchuck.stick_y = extension_data[1];
                    //    output.nunchuck.x = (extension_data[2] << 1) | (extension_data[5] & 0x10) >> 4;
                    //    output.nunchuck.y = (extension_data[3] << 1) | (extension_data[5] & 0x20) >> 5;
                    //    output.nunchuck.z = ((extension_data[4] & ~0x1) << 1) | (extension_data[5] & 0xC0) >> 6;

                    //    output.nunchuck.buttons = ~(extension_data[5] >> 2) & 0x3;
                    //}



                        //mWiimoteState.NunchukState.RawJoystick.X = buff[offset];
                        //mWiimoteState.NunchukState.RawJoystick.Y = buff[offset + 1];

                            axisDetails = device.Axis[JoystickAxis.AxisX] as AxisDetails;

                            if (axisDetails.max > 0f)
                            {
                                axisDetails.value = ((float)buff[offset] - axisDetails.mid) / (axisDetails.max - axisDetails.min);
                            }


                            axisDetails = device.Axis[JoystickAxis.AxisY] as AxisDetails;

                            if (axisDetails.max > 0f)
                            {
                                axisDetails.value = ((float)buff[offset + 1] - axisDetails.mid) / (axisDetails.max - axisDetails.min);
                            }


                        //mWiimoteState.NunchukState.AccelState.RawValues6b.X = buff[offset + 2];
                        //mWiimoteState.NunchukState.AccelState.RawValues6b.Y = buff[offset + 3];
                        //mWiimoteState.NunchukState.AccelState.RawValues6b.Z = buff[offset + 4];


                        //mWiimoteState.NunchukState.AccelState.RawValues8b.X = (buff[offset + 2]<<1) | (buff[offset + 5] & 0x10) >> 4;
                        //mWiimoteState.NunchukState.AccelState.RawValues8b.Y = (buff[offset + 3] << 1)| (buff[offset + 5] & 0x30) >> 4;;
                        //mWiimoteState.NunchukState.AccelState.RawValues8b.Z = ((buff[offset + 4]& ~0x1) << 1) | (buff[offset + 5] & 0xC0) >> 6;


                            //Acceleration axes
                            axisDetails = device.Axis[JoystickAxis.AxisAccR] as AxisDetails;

                            if (axisDetails.max > 0f)
                            {
                                axisDetails.value = ((float)((buff[offset + 2] << 1) | (buff[offset + 5] & 0x10) >> 4) - axisDetails.min) / (axisDetails.max - axisDetails.min);
                            }


                            axisDetails = device.Axis[JoystickAxis.AxisAccU] as AxisDetails;

                            if (axisDetails.max > 0f)
                            {
                                axisDetails.value = ((float)((buff[offset + 3] << 1)| (buff[offset + 5] & 0x30) >> 4) - axisDetails.min) / (axisDetails.max - axisDetails.min);
                            }

                            axisDetails = device.Axis[JoystickAxis.AxisAccV] as AxisDetails;

                            if (axisDetails.max > 0f)
                            {
                                axisDetails.value = ((float)(((buff[offset + 4]& ~0x1) << 1) | (buff[offset + 5] & 0xC0) >> 6) - axisDetails.min) / (axisDetails.max - axisDetails.min);
                            }




                        }else {return;}
                    }
          


                            axisDetails = device.Axis[JoystickAxis.AxisX] as AxisDetails;

                            if (axisDetails.max > 0f)
                            {
                                axisDetails.value = ((float)buff[offset] - axisDetails.mid) / (axisDetails.max - axisDetails.min);
                            }


                            axisDetails = device.Axis[JoystickAxis.AxisY] as AxisDetails;

                            if (axisDetails.max > 0f)
                            {
                                axisDetails.value = ((float)buff[offset + 1] - axisDetails.mid) / (axisDetails.max - axisDetails.min);
                            }


                    //  mWiimoteState.NunchukState.RawJoystick.X = buff[offset];
                    //  mWiimoteState.NunchukState.RawJoystick.Y = buff[offset + 1];


                    //Acceleration axes
                    axisDetails = device.Axis[JoystickAxis.AxisAccR] as AxisDetails;

                    if (axisDetails.max > 0f)
                    {
                        axisDetails.value = ((float)buff[offset + 2] - axisDetails.min) / (axisDetails.max - axisDetails.min);
                    }


                    axisDetails = device.Axis[JoystickAxis.AxisAccU] as AxisDetails;

                    if (axisDetails.max > 0f)
                    {
                        axisDetails.value = ((float)buff[offset + 3] - axisDetails.min) / (axisDetails.max - axisDetails.min);
                    }

                    axisDetails = device.Axis[JoystickAxis.AxisAccV] as AxisDetails;

                    if (axisDetails.max > 0f)
                    {
                        axisDetails.value = ((float)buff[offset + 4] - axisDetails.min) / (axisDetails.max - axisDetails.min);
                    }


                    //mWiimoteState.NunchukState.AccelState.RawValues.X = buff[offset + 2];
                    //mWiimoteState.NunchukState.AccelState.RawValues.Y = buff[offset + 3];
                    //mWiimoteState.NunchukState.AccelState.RawValues.Z = buff[offset + 4];

                    device.Buttons[7].value = (float)(buff[offset + 5] & 0x02);
                    device.Buttons[8].value = (float)((buff[offset + 5] & 0x01));

                    // mWiimoteState.NunchukState.C = (buff[offset + 5] & 0x02) == 0;
                    // mWiimoteState.NunchukState.Z = (buff[offset + 5] & 0x01) == 0;

                    //mWiimoteState.NunchukState.AccelState.Values.X = (float)((float)mWiimoteState.NunchukState.AccelState.RawValues.X - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.X0) /
                    //                                ((float)mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.XG - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.X0);
                    //mWiimoteState.NunchukState.AccelState.Values.Y = (float)((float)mWiimoteState.NunchukState.AccelState.RawValues.Y - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Y0) /
                    //                                ((float)mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.YG - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Y0);
                    //mWiimoteState.NunchukState.AccelState.Values.Z = (float)((float)mWiimoteState.NunchukState.AccelState.RawValues.Z - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Z0) /
                    //                                ((float)mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.ZG - mWiimoteState.NunchukState.CalibrationInfo.AccelCalibration.Z0);


                    //if (mWiimoteState.NunchukState.CalibrationInfo.MaxX != 0x00)
                    //    mWiimoteState.NunchukState.Joystick.X = (float)((float)mWiimoteState.NunchukState.RawJoystick.X - mWiimoteState.NunchukState.CalibrationInfo.MidX) /
                    //                            ((float)mWiimoteState.NunchukState.CalibrationInfo.MaxX - mWiimoteState.NunchukState.CalibrationInfo.MinX);

                    //if (mWiimoteState.NunchukState.CalibrationInfo.MaxY != 0x00)
                    //    mWiimoteState.NunchukState.Joystick.Y = (float)((float)mWiimoteState.NunchukState.RawJoystick.Y - mWiimoteState.NunchukState.CalibrationInfo.MidY) /
                    //                            ((float)mWiimoteState.NunchukState.CalibrationInfo.MaxY - mWiimoteState.NunchukState.CalibrationInfo.MinY);
                 //mWiimoteState.NunchukState.RawJoystick.X = buff[offset];
                 //       mWiimoteState.NunchukState.RawJoystick.Y = buff[offset + 1];

                    
                      
                 //       mWiimoteState.NunchukState.AccelState.RawValues6b.X = buff[offset + 2];
                 //       mWiimoteState.NunchukState.AccelState.RawValues6b.Y = buff[offset + 3];
                 //       mWiimoteState.NunchukState.AccelState.RawValues6b.Z = buff[offset + 4];

                 //        //    output.nunchuck.x = (extension_data[2] << 2) | (extension_data[5] & 0x0C) >> 2;
                 //   //    output.nunchuck.y = (extension_data[3] << 2) | (extension_data[5] & 0x30) >> 4;
                 //   //    output.nunchuck.z = (extension_data[4] << 2) | (extension_data[5] & 0xC0) >> 6;



                 //        //10bit precision but calibration data is 8bit
                 //        mWiimoteState.NunchukState.AccelState.RawValues8b.X = (buff[offset + 2]<<2) | (buff[offset + 5] & 0x0C) >> 2;
                 //       mWiimoteState.NunchukState.AccelState.RawValues8b.Y = (buff[offset + 3] << 2)| (buff[offset+5] & 0x30) >> 4;;
                 //       mWiimoteState.NunchukState.AccelState.RawValues8b.Z = (buff[offset + 4]<< 2)| (buff[offset + 5] & 0xC0) >> 6;

                 //       mWiimoteState.NunchukState.C = (buff[offset + 5] & 0x02) == 0;
                 //       mWiimoteState.NunchukState.Z = (buff[offset + 5] & 0x01) == 0;







                   }

             else if ((device.Extensions & (byte)ExtensionType.ClassicController) != 0)
             {

                 if (device.PASS_THRU_MODE == PassThruMode.ClassicController)
                 {

                     if ((buff[offset + 5] & 0x03) == 0)
                     {
                         //mWiimoteState.ClassicControllerState.RawJoystickL.X = (byte)(buff[offset] & 0x3E);
                         //mWiimoteState.ClassicControllerState.RawJoystickL.Y = (byte)(buff[offset + 1] & 0x3E);

                         //mWiimoteState.ClassicControllerState.RawJoystickR.X = (byte)((buff[offset] & 0xC0) >> 3) | ((buff[offset + 1] & 0xC0) >> 5) | ((buff[offset + 2] & 0xC0) >> 7);
                         //mWiimoteState.ClassicControllerState.RawJoystickR.Y = (byte)(buff[offset + 2] & 0x1F);

                         //mWiimoteState.ClassicControllerState.RawTriggerL = (byte)(((buff[offset + 2] & 0x60) >> 2) | ((buff[offset + 3] & 0xE0) >> 5));
                         //mWiimoteState.ClassicControllerState.RawTriggerR = (byte)(buff[offset + 3] & 0x1f);


                         ////TODO CHECK if buttons are the same as in normal mode
                         //mWiimoteState.ClassicControllerState.ButtonState.TriggerR = (buff[offset + 4] & 0x02) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.Plus = (buff[offset + 4] & 0x04) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.Home = (buff[offset + 4] & 0x08) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.Minus = (buff[offset + 4] & 0x10) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.TriggerL = (buff[offset + 4] & 0x20) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.Down = (buff[offset + 4] & 0x40) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.Right = (buff[offset + 4] & 0x80) == 0;

                         //mWiimoteState.ClassicControllerState.ButtonState.Up = (buff[offset + 5] & 0x01) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.Left = (buff[offset + 5] & 0x02) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.ZR = (buff[offset + 5] & 0x04) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.X = (buff[offset + 5] & 0x08) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.A = (buff[offset + 5] & 0x10) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.Y = (buff[offset + 5] & 0x20) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.B = (buff[offset + 5] & 0x40) == 0;
                         //mWiimoteState.ClassicControllerState.ButtonState.ZL = (buff[offset + 5] & 0x80) == 0;

                     }
                     else
                     {
                         return;


                     }
                 }
                 else
                 {


                     //AXES


                     //mWiimoteState.ClassicControllerState.RawJoystickL.X = (byte)(buff[offset] & 0x3f);
                     //mWiimoteState.ClassicControllerState.RawJoystickL.Y = (byte)(buff[offset + 1] & 0x3f);
                     // mWiimoteState.ClassicControllerState.RawJoystickR.X = (byte)((buff[offset + 2] >> 7) | (buff[offset + 1] & 0xc0) >> 5 | (buff[offset] & 0xc0) >> 3);
                     // mWiimoteState.ClassicControllerState.RawJoystickR.Y = (byte)(buff[offset + 2] & 0x1f);



                     axisDetails = device.Axis[JoystickAxis.AxisX] as AxisDetails;

                     if (axisDetails.max > 0f)
                     {
                         axisDetails.value = ((float)(buff[offset] & 0x3f) - axisDetails.mid) / (axisDetails.max - axisDetails.min);
                     }

                     axisDetails = device.Axis[JoystickAxis.AxisY] as AxisDetails;

                     if (axisDetails.max > 0f)
                     {
                         axisDetails.value = ((float)(buff[offset + 1] & 0x3f) - axisDetails.mid) / (axisDetails.max - axisDetails.min);
                     }


                     axisDetails = device.Axis[JoystickAxis.AxisZ] as AxisDetails;

                     if (axisDetails.max > 0f)
                     {
                         axisDetails.value = ((float)((buff[offset + 2] >> 7) | (buff[offset + 1] & 0xc0) >> 5 | (buff[offset] & 0xc0) >> 3) - axisDetails.mid) / (axisDetails.max - axisDetails.min);
                     }

                     axisDetails = device.Axis[JoystickAxis.AxisR] as AxisDetails;

                     if (axisDetails.max > 0f)
                     {
                         axisDetails.value = ((float)(buff[offset + 2] & 0x1f) - axisDetails.mid) / (axisDetails.max - axisDetails.min);
                     }



                     // mWiimoteState.ClassicControllerState.RawTriggerL = (byte)(((buff[offset + 2] & 0x60) >> 2) | (buff[offset + 3] >> 5));
                     //  mWiimoteState.ClassicControllerState.RawTriggerR = (byte)(buff[offset + 3] & 0x1f);

                     //  mWiimoteState.ClassicControllerState.ButtonState.TriggerL = (buff[offset + 4] & 0x20) == 0;
                     //  mWiimoteState.ClassicControllerState.ButtonState.TriggerR = (buff[offset + 4] & 0x02) == 0;


                     axisDetails = device.Axis[JoystickAxis.AxisU] as AxisDetails;

                     if (axisDetails.max > 0f)
                     {
                         axisDetails.value = ((float)(((buff[offset + 2] & 0x60) >> 2) | (buff[offset + 3] >> 5)) - axisDetails.mid) / (axisDetails.max - axisDetails.min);
                     }

                     axisDetails = device.Axis[JoystickAxis.AxisV] as AxisDetails;

                     if (axisDetails.max > 0f)
                     {
                         axisDetails.value = ((float)(buff[offset + 3] & 0x1f) - axisDetails.mid) / (axisDetails.max - axisDetails.min);
                     }

                     //mWiimoteState.ClassicControllerState.ButtonState.Plus = (buff[offset + 4] & 0x04) == 0;
                     //mWiimoteState.ClassicControllerState.ButtonState.Home = (buff[offset + 4] & 0x08) == 0;
                     //mWiimoteState.ClassicControllerState.ButtonState.Minus = (buff[offset + 4] & 0x10) == 0;

                     device.Buttons[4].value = (float)(buff[1] & 0x10);
                     device.Buttons[3].value = (float)(buff[2] & 0x80);
                     device.Buttons[2].value = (float)(buff[2] & 0x10);



                     //POV
                     //mWiimoteState.ClassicControllerState.ButtonState.Down = (buff[offset + 4] & 0x40) == 0;
                     //mWiimoteState.ClassicControllerState.ButtonState.Right = (buff[offset + 4] & 0x80) == 0;
                     //mWiimoteState.ClassicControllerState.ButtonState.Up = (buff[offset + 5] & 0x01) == 0;
                     //mWiimoteState.ClassicControllerState.ButtonState.Left = (buff[offset + 5] & 0x02) == 0;

                     //POV
                     axisDetails = device.Axis[JoystickAxis.AxisPovY] as AxisDetails;

                     axisDetails.value = 0;

                     if ((buff[offset + 5] & 0x01) == 0)
                         axisDetails.value = 1f;
                     else if ((buff[offset + 4] & 0x40) == 0)
                         axisDetails.value = -1f;

                     axisDetails = device.Axis[JoystickAxis.AxisPovX] as AxisDetails;
                     axisDetails.value = 0;

                     if ((buff[offset + 4] & 0x80) == 0)
                         axisDetails.value = 1f;
                     else if ((buff[offset + 5] & 0x02) == 0)
                         axisDetails.value = -1f;



                     //BUTTONS


                     device.Buttons[7].value = (float)(buff[offset + 5] & 0x04);
                     device.Buttons[8].value = (float)(buff[offset + 5] & 0x08);
                     device.Buttons[0].value = (float)(buff[offset + 5] & 0x10);
                     device.Buttons[9].value = (float)(buff[offset + 5] & 0x20);
                     device.Buttons[1].value = (float)(buff[offset + 5] & 0x40);
                     device.Buttons[10].value = (float)(buff[offset + 5] & 0x80);



                     //mWiimoteState.ClassicControllerState.ButtonState.ZR = (buff[offset + 5] & 0x04) == 0;
                     //mWiimoteState.ClassicControllerState.ButtonState.X = (buff[offset + 5] & 0x08) == 0;
                     //mWiimoteState.ClassicControllerState.ButtonState.A = (buff[offset + 5] & 0x10) == 0;
                     //mWiimoteState.ClassicControllerState.ButtonState.Y = (buff[offset + 5] & 0x20) == 0;
                     //mWiimoteState.ClassicControllerState.ButtonState.B = (buff[offset + 5] & 0x40) == 0;
                     //mWiimoteState.ClassicControllerState.ButtonState.ZL = (buff[offset + 5] & 0x80) == 0;

                     //if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXL != 0x00)
                     //    mWiimoteState.ClassicControllerState.JoystickL.X = (float)((float)mWiimoteState.ClassicControllerState.RawJoystickL.X - mWiimoteState.ClassicControllerState.CalibrationInfo.MidXL) /
                     //    (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXL - mWiimoteState.ClassicControllerState.CalibrationInfo.MinXL);

                     //if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYL != 0x00)
                     //    mWiimoteState.ClassicControllerState.JoystickL.Y = (float)((float)mWiimoteState.ClassicControllerState.RawJoystickL.Y - mWiimoteState.ClassicControllerState.CalibrationInfo.MidYL) /
                     //    (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYL - mWiimoteState.ClassicControllerState.CalibrationInfo.MinYL);

                     //if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXR != 0x00)
                     //    mWiimoteState.ClassicControllerState.JoystickR.X = (float)((float)mWiimoteState.ClassicControllerState.RawJoystickR.X - mWiimoteState.ClassicControllerState.CalibrationInfo.MidXR) /
                     //    (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxXR - mWiimoteState.ClassicControllerState.CalibrationInfo.MinXR);

                     //if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYR != 0x00)
                     //    mWiimoteState.ClassicControllerState.JoystickR.Y = (float)((float)mWiimoteState.ClassicControllerState.RawJoystickR.Y - mWiimoteState.ClassicControllerState.CalibrationInfo.MidYR) /
                     //    (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxYR - mWiimoteState.ClassicControllerState.CalibrationInfo.MinYR);

                     //if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerL != 0x00)
                     //    mWiimoteState.ClassicControllerState.TriggerL = (mWiimoteState.ClassicControllerState.RawTriggerL) /
                     //    (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerL - mWiimoteState.ClassicControllerState.CalibrationInfo.MinTriggerL);

                     //if (mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerR != 0x00)
                     //    mWiimoteState.ClassicControllerState.TriggerR = (mWiimoteState.ClassicControllerState.RawTriggerR) /
                     //    (float)(mWiimoteState.ClassicControllerState.CalibrationInfo.MaxTriggerR - mWiimoteState.ClassicControllerState.CalibrationInfo.MinTriggerR);

                 }

            }
        }

        /// <summary>
        /// Parse data returned from a read report
        /// </summary>
        /// <param name="buff">Data buffer</param>
        private byte[] ParseReadData(int address,byte[] buff)
        {
            if ((buff[3] & 0x08) != 0)
                throw new Exception("Error reading data from Wiimote: Bytes do not exist.");

            if ((buff[3] & 0x07) != 0)
                UnityEngine.Debug.LogWarning("Error reading data from Wiimote: Attempt to read from write-only registers.");

            // get our size and offset from the report
            int size = (buff[3] >> 4) + 1;
            int offset = (buff[4] << 8 | buff[5]);

           
            // add it to the buffer
          //  Array.Copy(buff, 6, mReadBuff, offset - mAddress, size);
            byte[] result = new byte[size];

             Array.Copy(buff, 6, result, offset - (address & 0xffff), size);

            // if we've read it all, set the event
            //if (mAddress + mSize == offset + size)
               // mReadDone.Set();

             return result;
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
        /// Set Wiimote reporting mode (if using an IR report type, IR sensitivity is set to WiiLevel3)
        /// </summary>
        /// <param name="type">Report type</param>
        /// <param name="continuous">Continuous data</param>
        void SetReportType(WiimoteDevice device, InputReport type, bool continuous)
        {
            SetReportType(device, type, IRSensitivity.Maximum, continuous);
        }

        /// <summary>
        /// Set Wiimote reporting mode
        /// </summary>
        /// <param name="type">Report type</param>
        /// <param name="irSensitivity">IR sensitivity</param>
        /// <param name="continuous">Continuous data</param>
        void SetReportType(WiimoteDevice device, InputReport type, IRSensitivity irSensitivity, bool continuous)
        {
            switch (type)
            {
                case InputReport.ButtonsIRAccel:
                    EnableIR(device, IRMode.Extended, irSensitivity);
                    break;
                case InputReport.IRExtensionAccel:
                    EnableIR(device, IRMode.Basic, irSensitivity);
                    break;
                default:
                    DisableIR(device);
                    break;
            }


            byte[] mBuff = new byte[REPORT_LENGTH];

            mBuff[0] = (byte)OutputReport.Type;
            // mBuff[1] = (byte)((continuous ? 0x04 : 0x00) | (byte)(mWiimoteState.Rumble ? 0x01 : 0x00));
            mBuff[1] = (byte)((continuous ? (uint)0x04 : (uint)0x00) | (uint)device.RumbleBit);
            mBuff[2] = (byte)type;

            _hidInterface.Write(mBuff, device);
        }

        /// <summary>
        /// Set the LEDs on the Wiimote
        /// </summary>
        /// <param name="led1">LED 1</param>
        /// <param name="led2">LED 2</param>
        /// <param name="led3">LED 3</param>
        /// <param name="led4">LED 4</param>
        void SetLEDs(WiimoteDevice device, bool led1, bool led2, bool led3, bool led4)
        {
            device.LED[0] = led1;
            device.LED[1] = led2;
            device.LED[2] = led3;
            device.LED[3] = led4;
            //mWiimoteState.LEDState.LED1 = led1;
            //mWiimoteState.LEDState.LED2 = led2;
            //mWiimoteState.LEDState.LED3 = led3;
            //mWiimoteState.LEDState.LED4 = led4;

            byte[] mBuff = new byte[REPORT_LENGTH];
            
            mBuff[0] = (byte)OutputReport.LEDs;
            mBuff[1] = (byte)(
                (led1 ? (uint)0x10 : (uint)0x00) |
                (led2 ? (uint)0x20 : (uint)0x00) |
                (led3 ? (uint)0x40 : (uint)0x00) |
                (led4 ? (uint)0x80 : (uint)0x00) |
                        device.RumbleBit);

            _hidInterface.Write(mBuff, device);
        }

        /// <summary>
        /// Set the LEDs on the Wiimote
        /// </summary>
        /// <param name="leds">The value to be lit up in base2 on the Wiimote</param>
        void SetLEDs(WiimoteDevice device, int leds)
        {




            device.LED[0] = (leds & 0x01) > 0;
            device.LED[1] = (leds & 0x02) > 0;
            device.LED[2] = (leds & 0x04) > 0;
            device.LED[3] = (leds & 0x08) > 0;


            //mWiimoteState.LEDState.LED1 = (leds & 0x01) > 0;
            //mWiimoteState.LEDState.LED2 = (leds & 0x02) > 0;
            //mWiimoteState.LEDState.LED3 = (leds & 0x04) > 0;
            //mWiimoteState.LEDState.LED4 = (leds & 0x08) > 0;


            byte[] mBuff = new byte[REPORT_LENGTH];

            mBuff[0] = (byte)OutputReport.LEDs;
            mBuff[1] = (byte)(
                ((leds & 0x01) > 0 ? (uint)0x10 : (uint)0x00) |
                ((leds & 0x02) > 0 ? (uint)0x20 : (uint)0x00) |
                ((leds & 0x04) > 0 ? (uint)0x40 : (uint)0x00) |
                ((leds & 0x08) > 0 ? (uint)0x80 : (uint)0x00) |
                        (uint)device.RumbleBit);

            _hidInterface.Write(mBuff, device);
        }

        /// <summary>
        /// Toggle rumble
        /// </summary>
        /// <param name="on">On or off</param>
        void SetRumble(WiimoteDevice device, bool on)
        {
            device.Rumble = on;

            // the LED report also handles rumble
            SetLEDs(device, device.LED[0],
                    device.LED[1],
                    device.LED[2],
                    device.LED[3]);
        }

        /// <summary>
        /// Retrieve the current status of the Wiimote and extensions.  Replaces GetBatteryLevel() since it was poorly named.
        /// </summary>
        void GetStatus(WiimoteDevice device)
        {
            device.processingMode=ProcessingMode.InProgress;

            HIDDevice hidDevice = _hidInterface.Generics[device];
            hidDevice.InputReportByteLength = REPORT_LENGTH;
            hidDevice.OutputReportByteLength = REPORT_LENGTH;

            byte[] mBuff = new byte[REPORT_LENGTH];

            mBuff[0] = (byte)OutputReport.Status;
            mBuff[1] = (byte)device.RumbleBit;
            //mBuff[1] = GetRumbleBit();


            _hidInterface.Write(mBuff, device, (suc) => {
                UnityEngine.Debug.Log("Status call:" + suc);
               // _hidInterface.Read(device, onRead);
            });
        }

        

       

        /// <summary>
        /// Turn on the IR sensor
        /// </summary>
        /// <param name="mode">The data report mode</param>
        /// <param name="irSensitivity">IR sensitivity</param>
        void EnableIR(WiimoteDevice device, IRMode mode, IRSensitivity irSensitivity)
        {
            device.Mode = mode;

            byte[] mBuff = new byte[REPORT_LENGTH];
            mBuff[0] = (byte)OutputReport.IR;
            mBuff[1] = (byte)(0x04 | device.RumbleBit);
            _hidInterface.Write(mBuff, device);

            mBuff = new byte[27];
            mBuff[0] = (byte)OutputReport.IR2;
            mBuff[1] = (byte)(0x04 | device.RumbleBit);
            _hidInterface.Write(mBuff, device);

            WriteMemory(device, REGISTER_IR, 0x08);

            switch (irSensitivity)
            {
                case IRSensitivity.WiiLevel1:
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x64, 0x00, 0xfe });
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0xfd, 0x05 });
                    break;
                case IRSensitivity.WiiLevel2:
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x96, 0x00, 0xb4 });
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0xb3, 0x04 });
                    break;
                case IRSensitivity.WiiLevel3:
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0xaa, 0x00, 0x64 });
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0x63, 0x03 });
                    break;
                case IRSensitivity.WiiLevel4:
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0xc8, 0x00, 0x36 });
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0x35, 0x03 });
                    break;
                case IRSensitivity.WiiLevel5:
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x07, 0x00, 0x00, 0x71, 0x01, 0x00, 0x72, 0x00, 0x20 });
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0x1, 0x03 });
                    break;
                case IRSensitivity.Maximum:
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_1, 9, new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x90, 0x00, 0x41 });
                    WriteMemory(device, REGISTER_IR_SENSITIVITY_2, 2, new byte[] { 0x40, 0x00 });
                    break;
                default:
                    throw new ArgumentOutOfRangeException("irSensitivity");
            }
            WriteMemory(device, REGISTER_IR_MODE, (byte)mode);
            WriteMemory(device, REGISTER_IR, 0x08);
        }

        /// <summary>
        /// Disable the IR sensor
        /// </summary>
        void DisableIR(WiimoteDevice device)
        {
            device.Mode = IRMode.Off;

            byte[] mBuff=new byte[27];
            mBuff[0] = (byte)OutputReport.IR;
            mBuff[1] = (byte)device.RumbleBit;

            _hidInterface.Write(mBuff, device);

            mBuff = new byte[27];
            mBuff[0] = (byte)OutputReport.IR2;
            mBuff[1] = (byte)device.RumbleBit;

            _hidInterface.Write(mBuff, device);
            
        }





        /// <summary>
        /// Read data or register from Wiimote
        /// </summary>
        /// <param name="address">Address to read</param>
        /// <param name="size">Length to read</param>

        void ReadMemory(WiimoteDevice device, int address, short size, HIDDevice.WriteCallback callback)
        {

            //Output Report 0x17 reads memory:
            //(52) 17 XX XX XX XX YY YY
            //XX XX XX XX is big-endian formatted address
            //YY YY is big-endian formatted size in bytes
            //LSB of first byte is rumble flag and is not part of address, should
            //be set to whatever state the rumble should be

            byte[] mBuff = new byte[REPORT_LENGTH];

            

            address = address & 0xffff;
            mBuff[0] = (byte)OutputReport.ReadMemory;
            mBuff[1] = (byte)(((address & 0xff000000) >> 24) | (uint)device.RumbleBit);
            mBuff[2] = (byte)((address & 0x00ff0000) >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00) >> 8);
            mBuff[4] = (byte)(address & 0x000000ff);

            mBuff[5] = (byte)((size & 0xff00) >> 8);
            mBuff[6] = (byte)(size & 0xff);

            _hidInterface.Write(mBuff,device, callback);


           

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="address"></param>
        /// <param name="size"></param>
        void ReadMemory(WiimoteDevice device, int address, short size)
        {

            byte[] mBuff = new byte[REPORT_LENGTH];

            mBuff[0] = (byte)OutputReport.ReadMemory;
            mBuff[1] = (byte)(((address & 0xff000000) >> 24) | (uint)device.RumbleBit);
            mBuff[2] = (byte)((address & 0x00ff0000) >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00) >> 8);
            mBuff[4] = (byte)(address & 0x000000ff);

            mBuff[5] = (byte)((size & 0xff00) >> 8);
            mBuff[6] = (byte)(size & 0xff);

            _hidInterface.Write(mBuff, device);


        }



        /// <summary>
        /// Write a single byte to the Wiimote
        /// </summary>
        /// <param name="address">Address to write</param>
        /// <param name="data">Byte to write</param>
        void WriteMemory(WiimoteDevice device, int address, byte data,HIDDevice.WriteCallback callback = null)
        {
            WriteMemory(device, address, 1, new byte[] { data },callback);
        }

        /// <summary>
        /// Write a byte array to a specified address
        /// </summary>
        /// <param name="address">Address to write</param>
        /// <param name="size">Length of buffer</param>
        /// <param name="buff">Data buffer</param>

        void WriteMemory(WiimoteDevice device, int address, byte size, byte[] buff, HIDDevice.WriteCallback callback = null)
        {

            byte[] mBuff = new byte[REPORT_LENGTH];
            
            mBuff[0] = (byte)OutputReport.WriteMemory;
            mBuff[1] = (byte)(((address & 0xff000000) >> 24) | device.RumbleBit);
            mBuff[2] = (byte)((address & 0x00ff0000) >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00) >> 8);
            mBuff[4] = (byte)(address & 0x000000ff);
            mBuff[5] = size;


            if (callback == null)
                _hidInterface.Write(mBuff, device);
            else
                _hidInterface.Write(mBuff, device, callback);

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


            }




        }
        #endregion








        #region AxisDetails
        public sealed class AxisDetails : IAxisDetails
        {

            #region Fields
            float _value;
            int _uid;
            int _min;
            int _max;
            int _mid;
            JoystickButtonState _buttonState;
            bool _isNullable;
            bool _isHat;
            bool _isTrigger;


            #region IAxisDetails implementation



            public bool isTrigger
            {
                get
                {
                    return _isTrigger;
                }
                set
                {
                    _isTrigger = value;
                }
            }






            public int mid
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

            public int min
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


            public int max
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
}
