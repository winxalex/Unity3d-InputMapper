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

       HIDDevice.ReadCallback _readCallback;
       HIDDevice.WriteCallback _writeCallback;
        int _ownerIndex;
        public ReadWriteListenerProxy(int index) : base("ws.winx.hid.IReadWriteListener") { _ownerIndex = index; }

        
        void onRead(AndroidJavaObject jo) {
          //  Debug.Log("ReadWriteListenerProxy>>onRead:");
           //   Debug.Log("ReadWriteListenerProxy>>onRead rawObject:" + bufferObject);

            AndroidJavaObject bufferObject = jo.Get<AndroidJavaObject>("Buffer");

        

            byte[] buffer = AndroidJNIHelper.ConvertFromJNIArray<byte[]>(bufferObject.GetRawObject());
          //  Debug.Log("ReadWriteListenerProxy>>Call array succeded:");

         //   Debug.Log("ReadWriteListenerProxy>>onRead:" + BitConverter.ToString(buffer));
            _readCallback.Invoke(new HIDReport(_ownerIndex,buffer,HIDReport.ReadStatus.Success));}
      
        
        
        /// <summary>
        /// onWrite Handler
        /// </summary>
        /// <param name="success"></param>
        void onWrite(bool success){ if(_writeCallback!=null) _writeCallback.Invoke(success); }
	
        internal void addReadCallback(HIDDevice.ReadCallback callback)
        {
 	        _readCallback=callback;

        }

          internal void addWriteCallback(HIDDevice.WriteCallback callback)
        {
 	        _writeCallback=callback;

        }
    
    }
}
#endif