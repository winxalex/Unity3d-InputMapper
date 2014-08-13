//#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ws.winx.platform.android
{
    public class AndroidMessageArgs : EventArgs
    {
         public readonly object data;

         public AndroidMessageArgs(object data)
        {
            this.data = data;

        }
    }


	public class AndroidHIDBehaviour:MonoBehaviour
	{
        public event EventHandler<AndroidMessageArgs> DeviceConnectedEvent;
        public event EventHandler<AndroidMessageArgs> DeviceDisconnectedEvent;
        HIDListenerProxy listener = new HIDListenerProxy();

        public event EventHandler DeviceConnectedEvent
        {
            add
            {
                listener.DeviceConnectedEvent += value;

            }
            remove
            {
                listener.DeviceConnectedEvent -= value;
            }
        }

        public event EventHandler DeviceDisconnectedEvent
        {
            add
            {
                listener.DeviceDisconnectedEvent += value;

            }
            remove
            {
                listener.DeviceDisconnectedEvent -= value;
            }
        }

        AndroidJavaClass pluginTutorialActivityJavaClass;
      

      

        void Start()
        {
            AndroidJNI.AttachCurrentThread();
            var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            pluginTutorialActivityJavaClass = new AndroidJavaClass("ws.winx.hid.AndroidHID");
            pluginTutorialActivityJavaClass.CallStatic("Init", activity, listener);
                
                //new AndroidJavaObject("ws.winx.hid.AndroidHID", activity, listener);

                
        }

        public void Read()
        {
           //pluginTutorialActivityJavaClass.Call
        }

        public void Write(byte[] data, AndroidJavaObject device)
        {
            //pluginTutorialActivityJavaClass.Call
        }

        internal void Log(string tag,string message)
        {
            pluginTutorialActivityJavaClass.CallStatic("Log", tag, message);
        }
    }
}
//#endif