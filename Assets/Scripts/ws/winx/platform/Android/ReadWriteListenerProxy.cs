#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ws.winx.platform.android
{
	public class ReadWriteListenerProxy:AndroidJavaProxy
	{
       
       
        public HIDDevice.ReadCallback ReadComplete;

         public HIDDevice.WriteCallback WriteComplete;
        
       
        public ReadWriteListenerProxy() : base("ws.winx.hid.IReadWriteListener") 
        {
           
        }

        
        void onRead(AndroidJavaObject jo) {
          //  Debug.Log("ReadWriteListenerProxy>>onRead:");
           //   Debug.Log("ReadWriteListenerProxy>>onRead rawObject:" + bufferObject);

            AndroidJavaObject bufferObject = jo.Get<AndroidJavaObject>("Buffer");

        

            byte[] buffer = AndroidJNIHelper.ConvertFromJNIArray<byte[]>(bufferObject.GetRawObject());
          //  Debug.Log("ReadWriteListenerProxy>>Call array succeded:");

         //   Debug.Log("ReadWriteListenerProxy>>onRead:" + BitConverter.ToString(buffer));
            if (ReadComplete != null)
                ReadComplete.Invoke(buffer);
        }
      
        
        
        /// <summary>
        /// onWrite Handler
        /// </summary>
        /// <param name="success"></param>
        void onWrite(bool success) { if (WriteComplete != null) WriteComplete.Invoke(success); }
	
    
    }
}
#endif