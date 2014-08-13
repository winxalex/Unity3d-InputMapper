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
         public ReadWriteListenerProxy(int index ) : base("ws.winx.hid.IReadWriteListener") {_ownerIndex=index;}

        
        void onRead(byte[] data) {  _readCallback.Invoke(new HIDReport(_ownerIndex,data,HIDReport.ReadStatus.Success));}
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