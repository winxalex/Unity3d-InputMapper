#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ws.winx.platform.android
{
    public class AndroidMessageArgs<T> : EventArgs
    {
         public readonly T data;

         public AndroidMessageArgs(T data)
        {
            this.data = data;

        }
    }


	public class AndroidHIDBehaviour:MonoBehaviour
	{
      
        AndroidHIDListenerProxy listener = new AndroidHIDListenerProxy();

        public event EventHandler<AndroidMessageArgs<AndroidJavaObject>> DeviceConnectedEvent
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

        public event EventHandler<AndroidMessageArgs<int>> DeviceDisconnectedEvent
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
            UnityEngine.Debug.Log("AndroidHIDBehaviour >>>> Start");

            AndroidJNI.AttachCurrentThread();
            var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            UnityEngine.Debug.Log("AndroidHIDBehaviour >>>> Acitivty:" + activity);
            
			//call Android Native Pluggin written in JAVA
            droidHID = new AndroidJavaClass("ws.winx.hid.AndroidHID");
            droidHID.CallStatic("Init", activity, listener);

            UnityEngine.Debug.Log("AndroidHIDBehaviour >>>>> AndroidHID" + droidHID);  
                //new AndroidJavaObject("ws.winx.hid.AndroidHID", activity, listener);

			//call Enumerate inside AndoridHID Native pluggin
            //droidHID.CallStatic("Enumerate");
        }


		/// <summary>
		/// Enumerate inside AndoridHID Native pluggin
		/// </summary>
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