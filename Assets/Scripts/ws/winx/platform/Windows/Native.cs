#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;

namespace ws.winx.platform.windows
{
    public static class Native
    {
        

        /// <summary>
        ///     Class of devices. This structure is a DEV_BROADCAST_DEVICEINTERFACE structure.
        /// </summary>
        public const uint DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;

        public const int DBT_DEVICEARRIVAL = 0x8000; // system detected a new device        
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // device is gone      
        public const int WM_DEVICECHANGE = 0x0219; // device change event  
        public const int ERROR_SUCCESS = 0;
        public const int ERROR_IO_PENDING = 0X3E5;



        public static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);
        public static UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);
        public const int KEY_READ = 0x20019;
        public const string REGSTR_VAL_JOYOEMNAME = "OEMName";

        internal const int FILE_FLAG_OVERLAPPED = 0x40000000;
        internal const short FILE_SHARE_READ = 0x1;
        internal const short FILE_SHARE_WRITE = 0x2;
        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;
        internal const int ACCESS_NONE = 0;
        internal const int INVALID_HANDLE_VALUE = -1;
        internal const short OPEN_EXISTING = 3;
        internal const int WAIT_TIMEOUT = 0x102;
        internal const uint WAIT_OBJECT_0 = 0;
        internal const uint WAIT_FAILED = 0xffffffff;
        internal const int WAIT_INFINITE = 0xffff;
        internal const int WAIT_ABANDONED = 0x80;

		internal const UInt32 WM_CLOSE          = 0x0010;
        internal const UInt32 WM_DESTROY        = 0x0002;

        #region NativeStructures

        public enum RawInputDeviceType : uint
        {
            Mouse = 0,
            Keyboard = 1,
            HumanInterfaceDevice = 2
        }

        public enum RawInputDeviceInfoCommand : uint
        {
            PreparsedData = 0x20000005,
            DeviceName = 0x20000007,
            DeviceInfo = 0x2000000b,
        }






        [StructLayout(LayoutKind.Explicit)]
        public struct DeviceInfo
        {
            [FieldOffset(0)]
            public int Size;
            [FieldOffset(4)]
            public int Type;
            [FieldOffset(8)]
            public DeviceInfoMouse MouseInfo;
            [FieldOffset(8)]
            public DeviceInfoKeyboard KeyboardInfo;
            [FieldOffset(8)]
            public DeviceInfoHID HIDInfo;
        }

        public struct DeviceInfoMouse
        {
            public uint ID;
            public uint NumberOfButtons;
            public uint SampleRate;
        }

        public struct DeviceInfoKeyboard
        {
            public uint Type;
            public uint SubType;
            public uint KeyboardMode;
            public uint NumberOfFunctionKeys;
            public uint NumberOfIndicators;
            public uint NumberOfKeysTotal;
        }

        public struct DeviceInfoHID
        {
            public uint VendorID;
            public uint ProductID;
            public uint VersionNumber;
            public ushort UsagePage;
            public ushort Usage;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RawInputDeviceList
        {
            public IntPtr DeviceHandle;
            public RawInputDeviceType DeviceType;
        }


        // Struct for parameters of the WM_DEVICECHANGE message
        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_VOLUME
        {
            public int dbcv_size;
            public int dbcv_devicetype;
            public int dbcv_reserved;
            public int dbcv_unitmask;
        }



        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_HDR
        {
            internal Int32 dbch_size;
            internal Int32 dbch_devicetype;
            internal Int32 dbch_reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class DEV_BROADCAST_DEVICEINTERFACE
        {
            internal Int32 dbcc_size;
            internal Int32 dbcc_devicetype;
            internal Int32 dbcc_reserved;
            [MarshalAs(UnmanagedType.ByValArray,
           ArraySubType = UnmanagedType.U1,
           SizeConst = 16)]
            internal Byte[] dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
            internal Char[] dbcc_name;
        }




        [System.Runtime.InteropServices.StructLayout(
              System.Runtime.InteropServices.LayoutKind.Sequential,
              CharSet = System.Runtime.InteropServices.CharSet.Unicode
              )]
        public struct WNDCLASS
        {
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string lpszMenuName;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string lpszClassName;
        }



        [StructLayout(LayoutKind.Sequential)]
        internal struct OVERLAPPED
        {
            public int Internal;
            public int InternalHigh;
            public int Offset;
            public int OffsetHigh;
            public int hEvent;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        #endregion


        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegCloseKey(
            UIntPtr hKey);

        // This signature will not get an entire REG_BINARY value. It will stop at the first null byte.
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
        public static extern int RegQueryValueEx(
            UIntPtr hKey,
            string lpValueName,
            int lpReserved,
            out uint lpType,
            System.Text.StringBuilder lpData,
            ref uint lpcbData);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int RegOpenKeyEx(
          UIntPtr hKey,
          string subKey,
          int ulOptions,
          int samDesired,
          out UIntPtr hkResult);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern System.UInt16 RegisterClassW(
            [System.Runtime.InteropServices.In] ref WNDCLASS lpWndClass
            );

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowExW(
             UInt32 dwExStyle,
             [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
			string lpClassName,
             [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
			string lpWindowName,
             UInt32 dwStyle,
             Int32 x,
             Int32 y,
             Int32 nWidth,
             Int32 nHeight,
             IntPtr hWndParent,
             IntPtr hMenu,
             IntPtr hInstance,
             IntPtr lpParam
             );

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern System.IntPtr DefWindowProcW(
            IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam
            );

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyWindow(
            IntPtr hWnd
            );


        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
       public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("user32", EntryPoint = "SetWindowsHookEx")]
        public static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hmod, IntPtr dwThreadId);

        [DllImport("user32", EntryPoint = "UnhookWindowsHookEx")]
        public static extern int UnhookWindowsHookEx(IntPtr hHook);

        [DllImport("user32", EntryPoint = "CallNextHookEx")]
        public static extern int CallNextHook(IntPtr hHook, int ncode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThreadId();

        [DllImport("user32.dll")]
        public static extern System.IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        public static extern bool UnregisterDeviceNotification(IntPtr handle);




        [DllImport("User32.dll", SetLastError = true)]
        public static extern uint GetRawInputDeviceList(
            IntPtr pRawInputDeviceList,
            ref uint uiNumDevices,
            uint cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetRawInputDeviceInfo(
            IntPtr hDevice,
            RawInputDeviceInfoCommand uiCommand,
            IntPtr data,
            ref uint size);


        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        static internal extern bool CancelIo(IntPtr hFile);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        static internal extern bool CancelIoEx(IntPtr hFile, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        static internal extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        static internal extern bool CancelSynchronousIo(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static internal extern IntPtr CreateEvent(ref SECURITY_ATTRIBUTES securityAttributes, int bManualReset, int bInitialState, string lpName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static internal extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, ref SECURITY_ATTRIBUTES lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        static internal extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, [In] ref System.Threading.NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll")]
        static internal extern uint WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        [DllImport("kernel32.dll")]
        static internal extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, [In] ref System.Threading.NativeOverlapped lpOverlapped);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern bool HidD_GetProductString(IntPtr HidDeviceObject, byte[] Buffer, ulong BufferLength);



		
		[Flags]
		internal enum PovType
		{
			None = 0x0,
			Exists = 0x1,
			Discrete = 0x2,
			Continuous = 0x4
		}
		
		
		internal enum JoystickError : uint
		{
			NoError = 0,
			InvalidParameters = 165,
			NoCanDo = 166,
			Unplugged = 167
			//MM_NoDriver = 6,
			//MM_InvalidParameter = 11
		}
		
		[Flags]
		internal enum JoystCapsFlags
		{
			HasZ = 0x1,
			HasR = 0x2,
			HasU = 0x4,
			HasV = 0x8,
			HasPov = 0x16,
			HasPov4Dir = 0x32,
			HasPovContinuous = 0x64
		}
		
		
		
		internal struct JoyCaps
		{
			public ushort VID;
			public ushort PID;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string
			ProductName;
			public int XMin;
			public int XMax;
			public int YMin;
			public int YMax;
			public int ZMin;
			public int ZMax;
			public int NumButtons;
			public int PeriodMin;
			public int PeriodMax;
			public int RMin;
			public int RMax;
			public int UMin;
			public int UMax;
			public int VMin;
			public int VMax;
			public JoystCapsFlags Capabilities;
			public int MaxAxes;
			public int NumAxes;
			public int MaxButtons;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string
			RegKey;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string
			OemVxD;
			public static readonly int SizeInBytes;
			
			static JoyCaps()
			{
				SizeInBytes = Marshal.SizeOf(default(JoyCaps));
			}
		}
		
		[Flags]
		internal enum JoystickFlags//:uint
		{
			X = 0x1,
			Y = 0x2,
			Z = 0x4,
			R = 0x8,
			U = 0x10,
			V = 0x20,
			Pov = 0x40,
			Buttons = 0x80,
			//			JOY_RETURNCENTERED=0x00000400,
			All = X | Y | Z | R | U | V | Pov | Buttons
		}
		
		
		

		internal struct JoyInfoEx
		{
			
			public int Size;
			[MarshalAs(UnmanagedType.I4)]
			public JoystickFlags Flags;
			public int XPos;
			public int YPos;
			public int ZPos;
			public int RPos;
			public int UPos;
			public int VPos;
			public uint Buttons;
			public uint ButtonNumber;
			public ushort Pov;
			uint Reserved1;
			uint Reserved2;
			public static readonly int SizeInBytes;
			
			static JoyInfoEx()
			{
				
				SizeInBytes = Marshal.SizeOf(default(JoyInfoEx));
				
			}
		}
		

			[DllImport("Winmm.dll"), SuppressUnmanagedCodeSecurity]
			internal static extern JoystickError joyGetPosEx(int uJoyID, ref JoyInfoEx pji);
			
			[DllImport("Winmm.dll"), SuppressUnmanagedCodeSecurity]
			internal static extern JoystickError joyGetDevCaps(int uJoyID, out JoyCaps pjc, int cbjc);
			
			[DllImport("Winmm.dll"), SuppressUnmanagedCodeSecurity]
			internal static extern int joyGetNumDevs();




            /// <summary>
            /// Get Value of the Registry Key
            /// </summary>
            /// <param name="rootKey"></param>
            /// <param name="keyPath"></param>
            /// <param name="valueName"></param>
            /// <returns></returns>
            public static string ReadRegKey(UIntPtr rootKey, string keyPath, string valueName)
            {
                UIntPtr hKey;

                if (Native.RegOpenKeyEx(rootKey, keyPath, 0, Native.KEY_READ, out hKey) == 0)
                {
                    uint size = 1024;
                    uint type;
                    string keyValue = null;
                    StringBuilder keyBuffer = new StringBuilder((int)size);

                    if (Native.RegQueryValueEx(hKey, valueName, 0, out type, keyBuffer, ref size) == 0)
                        keyValue = keyBuffer.ToString();

                    Native.RegCloseKey(hKey);

                    return (keyValue);
                }

                return String.Empty;  // Return null if the value could not be read
            }


            public static string TruncateZeroTerminatedString(string input)
            {
                if (input == null)
                {
                    return null;
                }
                return input.Substring(0, input.IndexOf('\0'));
            }

		
	}
}
#endif