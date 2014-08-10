using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ws.winx.platform.android
{
	public class AndroidHIDBehaviour:MonoBehaviour
	{
        public event EventHandler DeviceConnectedEvent;
        public event EventHandler DeviceDisconnectedEvent;
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

        AndroidJavaObject pluginTutorialActivityJavaClass;
      

      

        void Start()
        {
            AndroidJNI.AttachCurrentThread();
            var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            pluginTutorialActivityJavaClass = new AndroidJavaObject("ws.winx.hid.AndroidHID", activity, listener);

                
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
            pluginTutorialActivityJavaClass.Call("Log", tag, message);
        }
    }
}
