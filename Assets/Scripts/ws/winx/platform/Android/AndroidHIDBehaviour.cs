#if UNITY_ANDROID
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
      
        HIDListenerProxy listener = new HIDListenerProxy();

        public event EventHandler<AndroidMessageArgs> DeviceConnectedEvent
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

        public event EventHandler<AndroidMessageArgs> DeviceDisconnectedEvent
        {
            add
            {
                listener.DeviceDisconnectedEvent +=  value;

            }
            remove
            {
                listener.DeviceDisconnectedEvent -= value;
            }
        }

        AndroidJavaClass droidHID;
      

      

        void Start()
        {
            AndroidJNI.AttachCurrentThread();
            var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            droidHID = new AndroidJavaClass("ws.winx.hid.AndroidHID");
            droidHID.CallStatic("Init", activity, listener);
                
                //new AndroidJavaObject("ws.winx.hid.AndroidHID", activity, listener);

                
        }


          public void Enumerate(){
 droidHID.CallStatic("Enumerate");
}

       

        internal void Log(string tag,string message)
        {
            droidHID.CallStatic("Log", tag, message);
        }
    }
}
#endif