package ws.winx.hid;

import java.nio.ByteBuffer;
import java.util.UUID;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.Future;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;


import android.hardware.usb.UsbRequest;
import android.util.Log;

public class ReadRunnable implements Runnable {

	
	private static final String TAG = "ReadRunnable";

	private byte[] _inputBuffer;
	
	private IReadWriteListener _listener;
	private HIDDeviceWrapper _device;
	private boolean _isReady=false;
	
	
	int _timeout;
	ReadCallable _readCallable;

	public ReadRunnable(HIDDeviceWrapper device) {
	
		_device=device;
		_isReady=true;
		//_readCallable=new ReadCallable(device);
	}

	public ReadRunnable timeout(int timeInMilliSeconds){
		_timeout=timeInMilliSeconds;
		return this;
		
	}
	
	
	public ReadRunnable addEventListener(IReadWriteListener listener){
		_listener=listener;
		return this;
		
	}
	
	public ReadRunnable read(byte[] into){
		_inputBuffer=into;
		return this;
		
	}
	

	@Override
	public void run() {
		
	
		 this._isReady=false;
		 
		 try {
			HIDDeviceWrapper.getEndPointlock().acquire();
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		 ByteBuffer buffer = ByteBuffer.wrap(_inputBuffer);
		 UUID uid=UUID.randomUUID();
		 
	        UsbRequest request = new UsbRequest();
	        if(!request.initialize(_device.get_connection(), _device.get_readEndpoint())){
	        	
		        	   Log.e(TAG,"Cant queue request");
		        	   return;
		          
	        }
	        
	        
	        
	       // while (true) {
	            // queue a request on the interrupt endpoint
	           if(!request.queue(buffer, _inputBuffer.length)){
	        	   
	        	   Log.e(TAG,"Cant queue request");
	        	   
	        	   request.close();
	        	   return;
	           }
	            
	           
	           
	            // wait for status event
	            if (_device.get_connection().requestWait() == request) {
	            	
	            	 Log.d(TAG, uid+"Request succeded");
	            	 _listener.onRead(_inputBuffer);
	            	 request.close();
	            } else {
	                Log.e(TAG, uid+"RequestWait failed, exiting");
	                //!!! don't close when failed
	             //   break;
	            }
	      //  }
		
	            this._isReady=true;
	            
	            
	           // HIDDeviceWrapper.getEndPointlock().release();
		
	/*	UUID uid=UUID.randomUUID();
		
		Log.d(TAG,uid+" Try to aquire read");
		
		try {
			HIDDeviceWrapper.getEndPointlock().acquire();
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			Log.e(TAG,uid+" Error:",e);
		}
		
		Log.d(TAG,uid+"Continue read");
	Future<byte[]> future = null;
	
			try {
				future=HIDDeviceWrapper.get__executor().submit(_readCallable.read(_inputBuffer));
				
				_inputBuffer=future.get(_timeout, TimeUnit.MILLISECONDS);
			} catch (InterruptedException e) {
				_readCallable.despose();
				future.cancel(true);
			} catch (ExecutionException e) {
				Log.e(TAG," Error:",e);
			} catch (TimeoutException e) {
				Log.d(TAG,uid+"Timeout ");
				_readCallable.despose();
				future.cancel(true);
				
			}

			
			Log.d(TAG,uid+" Released ");
     	 HIDDeviceWrapper.getEndPointlock().release();
     	 
     	 //new AsyncTaskExecutor(_listener).onPostExecute(_inputBuffer);
     	 _listener.onRead(_inputBuffer);
		
     	*/
	}

	public boolean is_isReady() {
		return _isReady;
	}



}
