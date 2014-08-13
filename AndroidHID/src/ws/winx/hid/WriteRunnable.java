package ws.winx.hid;

import android.util.Log;

public class WriteRunnable implements Runnable {

	
	private static final String TAG = "WriteRunnable";

	private byte[] __outputBuffer;
	private IReadWriteListener _listener;
	private HIDDeviceWrapper __device;
	int _timeout;

	public WriteRunnable(HIDDeviceWrapper device) {
		__device=device;
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
		
		
	//	java.util.UUID.randomUUID()
			Log.d(TAG,"Try to aquire write");
		
		try {
			HIDDeviceWrapper.getEndPointlock().acquire();
			
			Log.d(TAG,"Try to aquire write");
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			Log.e(TAG," Error:",e);
		}
		
		Log.d(TAG,"Try continue write");
		
		
		boolean sucess= __device.get_connection().controlTransfer(0x21, 0x09, 0x0240, 0, __outputBuffer, __outputBuffer.length, 0)>-1;
		
		HIDDeviceWrapper.getEndPointlock().release();
		
     	 //_listener.onWrite(_connection.bulkTransfer(_fromPoint, _inputBuffer, _inputBuffer.length, 50)>-1);
		_listener.onWrite(sucess);
		
		
	}

}
