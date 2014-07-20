using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ws.winx.platform.windows
{
    public class CustomWindow : IDisposable
    {
        delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);



        [System.Runtime.InteropServices.StructLayout(
            System.Runtime.InteropServices.LayoutKind.Sequential,
            CharSet = System.Runtime.InteropServices.CharSet.Unicode
            )]
        struct WNDCLASS
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

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern System.UInt16 RegisterClassW(
            [System.Runtime.InteropServices.In] ref WNDCLASS lpWndClass
            );

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CreateWindowExW(
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
        static extern System.IntPtr DefWindowProcW(
            IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam
            );

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyWindow(
            IntPtr hWnd
            );

        private const int ERROR_CLASS_ALREADY_EXISTS = 1410;


        public IntPtr m_hwnd;
        public const int WmDevicechange = 0x0219; // device change event   
        public const int DbtDevicearrival = 0x8000; // system detected a new device        
        public const int DbtDeviceremovecomplete = 0x8004; // device is gone     


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {

            UnityEngine.Debug.Log("Dispose USBNotifier");
            if (disposing)
            {
                // Dispose managed resources
            }

            // Dispose unmanaged resources
            if (m_hwnd != IntPtr.Zero)
            {
                DestroyWindow(m_hwnd);
                m_hwnd = IntPtr.Zero;
            }


        }

        public CustomWindow(string class_name)
        {

            if (class_name == null) throw new System.Exception("class_name is null");
            if (class_name == String.Empty) throw new System.Exception("class_name is empty");

            m_wnd_proc_delegate = CustomWndProc;

            // Create WNDCLASS
            WNDCLASS wind_class = new WNDCLASS();
            wind_class.lpszClassName = class_name;
            wind_class.lpfnWndProc = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(m_wnd_proc_delegate);

            UInt16 class_atom = RegisterClassW(ref wind_class);

            int last_error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();

            if (class_atom == 0 && last_error != ERROR_CLASS_ALREADY_EXISTS)
            {
                throw new System.Exception("Could not register window class");
            }

            // Create window
            m_hwnd = CreateWindowExW(
                0,
                class_name,
                String.Empty,
                0,
                0,
                0,
                0,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
                );
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_HDR
        {
            internal Int32 dbch_size;
            internal Int32 dbch_devicetype;
            internal Int32 dbch_reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class DEV_BROADCAST_DEVICEINTERFACE_1
        {
            internal Int32 dbcc_size;
            internal Int32 dbcc_devicetype;
            internal Int32 dbcc_reserved;
            [MarshalAs(UnmanagedType.ByValArray,
           ArraySubType = UnmanagedType.U1,
           SizeConst = 16)]
            internal Byte[] dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray,
            SizeConst = 255)]
            internal Char[] dbcc_name;
        }

        private static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            int devType = 0;

            if (msg == WmDevicechange)
            {

                if (lParam != IntPtr.Zero)
                    devType = Marshal.ReadInt32(lParam, 4);

                switch ((int)wParam)
                {
                    case DbtDeviceremovecomplete:

                        if (devType == WinHIDInterface.DBT_DEVTYP_DEVICEINTERFACE)
                        {
                            try
                            {
                                DEV_BROADCAST_DEVICEINTERFACE_1 devBroadcastDeviceInterface =
                                    new DEV_BROADCAST_DEVICEINTERFACE_1();
                                DEV_BROADCAST_HDR devBroadcastHeader = new DEV_BROADCAST_HDR();
                                Marshal.PtrToStructure(lParam, devBroadcastHeader);

                                Int32 stringSize = Convert.ToInt32((devBroadcastHeader.dbch_size - 32) / 2);
                                Array.Resize(ref devBroadcastDeviceInterface.dbcc_name, stringSize);
                                Marshal.PtrToStructure(lParam, devBroadcastDeviceInterface);
                                String deviceNameString = new String(devBroadcastDeviceInterface.dbcc_name, 0, stringSize);
							UnityEngine.Debug.Log("Removed " + deviceNameString);
						}
						catch (Exception e)
                            {
							UnityEngine.Debug.LogException(e);
                            }
                        }







                        break;
                    case DbtDevicearrival:
                        if (devType == WinHIDInterface.DBT_DEVTYP_DEVICEINTERFACE)
                        {
                            try
                            {
                                DEV_BROADCAST_DEVICEINTERFACE_1 devBroadcastDeviceInterface =
                                new DEV_BROADCAST_DEVICEINTERFACE_1();
                                DEV_BROADCAST_HDR devBroadcastHeader = new DEV_BROADCAST_HDR();
                                Marshal.PtrToStructure(lParam, devBroadcastHeader);

                                Int32 stringSize = Convert.ToInt32((devBroadcastHeader.dbch_size - 32) / 2);
                                Array.Resize(ref devBroadcastDeviceInterface.dbcc_name, stringSize);
								Marshal.PtrToStructure(lParam, devBroadcastDeviceInterface);
                                String deviceNameString = new String(devBroadcastDeviceInterface.dbcc_name, 0, stringSize);




							UnityEngine.Debug.Log("Connected " + deviceNameString);
						}
						catch (Exception e)
                            {
							UnityEngine.Debug.LogException(e);
                            }
                        }

                        break;
                }
            }

            return DefWindowProcW(hWnd, msg, wParam, lParam);
        }

        private WndProc m_wnd_proc_delegate;
    }
}


