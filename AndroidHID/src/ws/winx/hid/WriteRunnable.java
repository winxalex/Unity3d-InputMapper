package ws.winx.hid;

import android.util.Log;

public class WriteRunnable implements Runnable {

	
	private static final String TAG = "WriteRunnable";

	private byte[] __outputBuffer;
	private IReadWriteListener _listener;
	private HIDDeviceWrapper __device;
	int _timeout;

	private boolean _isReady;
	

	public WriteRunnable(HIDDeviceWrapper device) {
		__device=device;
	
			_isReady=true;
		
	}
	
	
	public WriteRunnable addEventListener(IReadWriteListener listener){
		_listener=listener;
		return this;
		
	}
	
	public WriteRunnable read(byte[] from){
		__outputBuffer=from;
		return this;
		
	}
	
	public WriteRunnable timeout(int timeInMilliSeconds){
		_timeout=timeInMilliSeconds;
		return this;
		
	}



	@Override
	public void run() {
		
		_isReady=false;
	//UUID uid=new java.util.UUID.randomUUID()
			//Log.d(TAG,"Write");
		
		try {
			HIDDeviceWrapper.getEndPointlock().acquire();
			
			//Log.d(TAG,"Try to acquire write");
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			Log.e(TAG," Error:",e);
		}
		
		//Log.d(TAG,"Try continue write");
		
		
		boolean success= __device.get_connection().controlTransfer(0x21, 0x09, 0x0240, 0, __outputBuffer, __outputBuffer.length, 0)>-1;
		
	//	Log.d(TAG,"Write success "+success);
		
     	 //_listener.onWrite(_connection.bulkTransfer(_fromPoint, _inputBuffer, _inputBuffer.length, 50)>-1);
		if(_listener!=null){
			_listener.onWrite(success);
		
			_listener=null;
		}
		
		HIDDeviceWrapper.getEndPointlock().release();
		
		_isReady=false;
		
	//	Log.d(TAG,"Release lock");
	}
	
	public boolean is_isReady() {
		return _isReady;
	}
	
	

}
