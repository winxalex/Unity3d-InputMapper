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
	

	private byte[] _inputBuffer;
	private UsbRequest __request;
	private UsbEndpoint _enf;
	

	public ReadCallable(HIDDeviceWrapper device) {
		
		 __request = new UsbRequest();
 		
		 _connection=device.get_connection();
		 _enf=device.get_readEndpoint();
		 
		 
    	 __request.initialize(_connection, device.get_readEndpoint());
	}
	
	public ReadCallable read(byte[] into){
		_inputBuffer=into;
		return this;
	}
	
	
	

	/*@Override
	protected void finalize() throws Throwable {
		// TODO Auto-generated method stub
		
		//__request.cancel();
		//__request.close();
		
		 Log.d(TAG, "Finalize Cancel Close request");
		super.finalize();
	}*/

	@Override
	public byte[] call() throws Exception {
		
    	 
    	 final ByteBuffer buf = ByteBuffer.wrap(_inputBuffer);
		   try{
			   __request = new UsbRequest();
		 		
			
		    	 __request.initialize(_connection, _enf);
		    	 
			  // Log.d(TAG, "Cancel request succeded "+__request.cancel());  
			   Log.d(TAG, "Try to queue request");
			   
    	  if (!__request.queue(buf, _inputBuffer.length)) {
             Log.e(TAG,"Error queueing request.");
            
             return _inputBuffer;
           }
    
    	    	                 
           // wait for status event (requestWait() blocks further execution)
    	       if (_connection.requestWait() == __request) {
	        	   Log.d(TAG, "Read requestWait succeded, exiting");
	           } else {
	              Log.e(TAG, "Read requestWait failed, exiting");
	      
	           }
    	   }catch(Exception e){
    		   Log.e(TAG, "Read requestWait failed, exiting",e);
    	   }finally{
    		   //__request.cancel();
    		   	__request.close();
    		   Log.d(TAG, "Request closed");
    		   
    	   }

		
		return _inputBuffer;
	}

	public void despose() {
		__request.cancel();
		   __request.close();
		   Log.d(TAG, "Dispose Cancel Close request");
	}

	public void cancel() {
	   Log.d(TAG, "Cancel request succeded "+__request.cancel());

	}

}
