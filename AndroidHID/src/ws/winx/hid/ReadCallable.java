package ws.winx.hid;

import java.nio.ByteBuffer;
import java.util.concurrent.Callable;

import android.hardware.usb.UsbDeviceConnection;
import android.hardware.usb.UsbEndpoint;
import android.hardware.usb.UsbRequest;
import android.util.Log;

public class ReadCallable implements Callable<byte[]> {

	
	private static final String TAG = "ReadCallable";
	private UsbDeviceConnection _connection;
	private int _inputBufferSize;
	private UsbEndpoint _fromPoint;

	public ReadCallable(UsbDeviceConnection connection,UsbEndpoint fromPoint,int bufferSize) {
		_connection=connection;
		_inputBufferSize=bufferSize;
		_fromPoint=fromPoint;
	}

	@Override
	public byte[] call() throws Exception {
		UsbRequest request = new UsbRequest();
    	
		
		byte[] inputBuffer=new byte[_inputBufferSize];
      	 request.initialize(_connection, _fromPoint);
      	 
      	 final ByteBuffer buf = ByteBuffer.wrap(inputBuffer);
		
      	 
      	 while (true) {
        	 
      	   if (!request.queue(buf, _inputBufferSize)) {
                 Log.e(TAG,"Error queueing request.");
                 break;
               }
      	    	                  
             // wait for status event
             if (_connection.requestWait() == request) {
          	    	                	   
                 final int nread = buf.get(0);
                 if (nread > 0) {
                  // Log.d(TAG, HexDump.dumpHexString(dest, 0, Math.min(8, dest.length)));
              	  break;
                 } 
                 
                 try {
                     Thread.sleep(1);
                 } catch (InterruptedException e) {
              	   Log.e(TAG, "Error ",e);
                         	   
              	   break;
                 }
                 
             } else {
                 Log.e(TAG, "Read requestWait failed, exiting");
                 break;
             }
 	
  
  }
      	 
      	 
      	 
		
		
		return inputBuffer;
	}

}
