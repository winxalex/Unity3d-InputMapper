package ws.winx.hid;

import java.nio.ByteBuffer;




import android.hardware.usb.UsbRequest;
import android.util.Log;

public class ReadRunnable implements Runnable {

	
	private static final String TAG = "ReadRunnable";

	private byte[] _inputBuffer;
	
	private IReadWriteListener _listener;
	private HIDDeviceWrapper _device;
	private boolean _isReady=false;
	
	
	int _timeout;
	

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
			
			  Log.e(TAG,"Lock error",e);
		}
		
		 ByteBuffer buffer = ByteBuffer.wrap(_inputBuffer);
		 
		//UUID uid=UUID.randomUUID();
		 
		//  Log.d(TAG,"Wrapper");
		 
	        UsbRequest request = new UsbRequest();
	        if(!request.initialize(_device.get_connection(), _device.get_readEndpoint())){
	        	
		       // 	   Log.e(TAG,"Cant queue request");
		        	   return;
		          
	        }
	        
	        Log.d(TAG,"Request"+request);
	     
	            // queue a request on the interrupt endpoint
	           if(!request.queue(buffer, _inputBuffer.length)){
	        	   
	        	   Log.e(TAG,"Cant queue request");
	        	   
	        	   request.close();
	        	   return;
	           }
	            
	          // Log.e(TAG,"Queue request");
	           
	            // wait for status event
	            if (_device.get_connection().requestWait() == request) {
	            	
	            //	 Log.d(TAG, uid+"Request succeded");
	            	 
	            	 if(_listener!=null)
	            	 _listener.onRead(new ReadData(_inputBuffer));
	            	 
	            	 request.close();
	            } else {
	              //  Log.e(TAG, uid+"RequestWait failed, exiting");
	                //!!! don't request.close() close when failed
	           
	            }
	     
	            this._isReady=true;
	            
	            
	            HIDDeviceWrapper.getEndPointlock().release();
		
	/*
	 * 
	 * option with time out not tested on UIThread
	 * 	UUID uid=UUID.randomUUID();
		
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
