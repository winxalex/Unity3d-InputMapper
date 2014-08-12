package ws.winx.hid;

import java.nio.ByteBuffer;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Future;
import java.util.concurrent.Semaphore;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;

import android.hardware.usb.UsbDeviceConnection;
import android.hardware.usb.UsbEndpoint;
import android.hardware.usb.UsbRequest;
import android.util.Log;

public class ReadRunnable implements Runnable {

	
	private static final String TAG = "ReadRunnable";
	private UsbDeviceConnection _connection;
	private byte[] _inputBuffer;
	private UsbEndpoint _fromPoint;
	private IReadWriteListener _listener;
	Semaphore _endpointLock;
	ExecutorService _executor;
	int _timeout;
	ReadCallable _readCallable;

	public ReadRunnable(ExecutorService executor,Semaphore endpointLock,UsbDeviceConnection connection,UsbEndpoint fromPoint,byte[] into,int timeout,IReadWriteListener listener) {
		_connection=connection;
		_inputBuffer=into;
		_fromPoint=fromPoint;
		_listener=listener;
		_endpointLock=endpointLock;
		_executor=executor;
		_timeout=timeout;
		_readCallable=new ReadCallable(_connection, _fromPoint,_inputBuffer);
	}



	@Override
	public void run() {
		
		Log.d(TAG,"Try to aquire read");
		
		try {
			_endpointLock.acquire();
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			Log.e(TAG," Error:",e);
		}
		
		Log.d(TAG,"Continue read");
	Future<byte[]> future = null;
	
			try {
				future=_executor.submit(_readCallable);
				
				_inputBuffer=future.get(_timeout, TimeUnit.MILLISECONDS);
			} catch (InterruptedException e) {
				_readCallable.cancel();
				future.cancel(true);
			} catch (ExecutionException e) {
				Log.e(TAG," Error:",e);
			} catch (TimeoutException e) {
				future.cancel(true);
				_readCallable.cancel();
			}

     	 
     	 //new AsyncTaskExecutor(_listener).onPostExecute(_inputBuffer);
     	 _listener.onRead(_inputBuffer);
		
     	_endpointLock.release();
	}

}
