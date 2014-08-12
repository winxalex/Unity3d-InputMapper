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
	
	private UsbEndpoint _fromPoint;
	private byte[] _inputBuffer;
	private UsbRequest request;

	public ReadCallable(UsbDeviceConnection connection,UsbEndpoint fromPoint,byte[] inputBuffer) {
		_connection=connection;
		_inputBuffer=inputBuffer;
		_fromPoint=fromPoint;
		 request = new UsbRequest();
 		
			
    	 request.initialize(_connection, _fromPoint);
	}

	@Override
	protected void finalize() throws Throwable {
		// TODO Auto-generated method stub
		
		request.cancel();
		request.close();
		
		 Log.d(TAG, "Close request");
		super.finalize();
	}

	@Override
	public byte[] call() throws Exception {
		
    	 
    	 final ByteBuffer buf = ByteBuffer.wrap(_inputBuffer);
		
    	  if (!request.queue(buf, _inputBuffer.length)) {
             Log.e(TAG,"Error queueing request.");
            
             return _inputBuffer;
           }
    
    	    	                 
           // wait for status event (requestWait() blocks further execution)
    	   try{
    		   
    	   
	           if (_connection.requestWait() == request) {
	        	   Log.d(TAG, "Read requestWait succeded, exiting");
	           } else {
	              Log.e(TAG, "Read requestWait failed, exiting");
	      
	           }
    	   }catch(Exception e){
    		  
    	   }finally{
    		   request.cancel();
    		  
    		   
    		   
    	   }

		
		return _inputBuffer;
	}

	public void despose() {
		request.cancel();
		   request.close();
		   Log.d(TAG, "Close request");
	}

	public void cancel() {
	    request.cancel();
	
		
	}

}
