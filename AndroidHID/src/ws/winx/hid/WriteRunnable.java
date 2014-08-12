package ws.winx.hid;

import java.util.concurrent.Semaphore;

import android.hardware.usb.UsbDeviceConnection;
import android.util.Log;

public class WriteRunnable implements Runnable {

	
	private static final String TAG = "ReadRunnable";
	private UsbDeviceConnection _connection;
	private byte[] _inputBuffer;
	private IReadWriteListener _listener;
	Semaphore __endpointLock;

	public WriteRunnable(Semaphore endPointLock,UsbDeviceConnection connection,byte[] into,IReadWriteListener listener) {
		_connection=connection;
		_inputBuffer=into;
		_listener=listener;
		__endpointLock=endPointLock;
	}



	@Override
	public void run() {
		
			Log.d(TAG,"Try to aquire write");
		
		try {
			__endpointLock.acquire();
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			Log.e(TAG," Error:",e);
		}
		
		Log.d(TAG,"Try continue write");
		
		
		
		
     	 //_listener.onWrite(_connection.bulkTransfer(_fromPoint, _inputBuffer, _inputBuffer.length, 50)>-1);
		_listener.onWrite(_connection.controlTransfer(0x21, 0x09, 0x0240, 0, _inputBuffer, _inputBuffer.length, 0)>-1);
		
		__endpointLock.release();
	}

}
