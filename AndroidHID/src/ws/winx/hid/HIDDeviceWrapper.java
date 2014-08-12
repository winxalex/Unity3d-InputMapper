package ws.winx.hid;


import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Semaphore;
import java.util.concurrent.locks.ReentrantLock;


import android.hardware.usb.UsbConstants;
import android.hardware.usb.UsbDevice;
import android.hardware.usb.UsbDeviceConnection;
import android.hardware.usb.UsbEndpoint;
import android.hardware.usb.UsbInterface;
import android.hardware.usb.UsbManager;
import android.util.Log;

public class HIDDeviceWrapper {

	protected static final String TAG = null;
	UsbDeviceConnection _connection;
	UsbDevice _device;
	UsbEndpoint _readEndpoint;
	UsbEndpoint _writeEndpoint;

	UsbManager _usbManager;

	int _inputBufferSize = 8;
	int _outputBufferSize = 8;
	boolean _lockEndpoint=false;
	
	public final int index;
	public final int VID;
	public final int PID;
	private UsbInterface _controlInterface;
	
	private static ExecutorService __executor=Executors.newCachedThreadPool();
	private static final ReentrantLock __lock = new ReentrantLock();
	private static final Semaphore __enpointLock=new Semaphore(1);
	private static final ReadRunnable _readRunnable;
	private static final WriteRunnable _writeRunnable;

	public HIDDeviceWrapper(UsbDevice device, UsbManager usbManager) {

		this.index=device.getDeviceId();
		this.VID=device.getVendorId();
		this.PID=device.getProductId();
		
		
		_device = device;
		_usbManager = usbManager;
	}

	public boolean open() {

		_connection = _usbManager.openDevice(_device);

		if (_device.getInterfaceCount() < 0) {
			Log.e(TAG, "Could not claim control interface.");
			return false;
		}

		_controlInterface = _device.getInterface(0);

		if (!_connection.claimInterface(_controlInterface, true)) {
			Log.e(TAG, "Could not claim control interface.");
			return false;
		}

		UsbEndpoint endPoint;

		try{
		   
			
			
					if (_controlInterface.getEndpointCount() == 1)
						_writeEndpoint = _readEndpoint = _controlInterface.getEndpoint(0);
					else {
						endPoint = _controlInterface.getEndpoint(0);
						if (endPoint.getDirection() == UsbConstants.USB_DIR_IN)
							_readEndpoint = endPoint;
						else if (endPoint.getDirection() == UsbConstants.USB_DIR_OUT)
							_writeEndpoint = endPoint;
			
						endPoint = _controlInterface.getEndpoint(1);
						if (endPoint.getDirection() == UsbConstants.USB_DIR_IN)
							_readEndpoint = endPoint;
						else if (endPoint.getDirection() == UsbConstants.USB_DIR_OUT)
							_writeEndpoint = endPoint;
			
						if (_readEndpoint == null || _writeEndpoint == null)
							_writeEndpoint = _readEndpoint = _controlInterface.getEndpoint(0);
									
					}
					
					
					 Log.d(TAG, "Number Interfaces=" + _device.getInterfaceCount());
			            Log.d(TAG, "Number Endpoints=" + _device.getInterface(0).getEndpointCount());
			            Log.d(TAG, "End point Address" + _readEndpoint.getAddress());
			            Log.d(TAG, "End point Type" + _readEndpoint.getType());//type=3 USB_ENDPOINT_XFER_INT (Interupt)
			            Log.d(TAG, "End point Direction " + _readEndpoint.getDirection());//type=128 USB_DIR_IN
		}catch(Exception e){
			Log.e(TAG, "Error obtaining endpoints",e);
		}
		
		
		return true;

	}

	public void closeDevice() {
		if (_connection != null){
			
			if(_controlInterface!=null)
				_connection.releaseInterface(_controlInterface);
			
		}
		
		
	}

	public void Dispose() {

	}

	public void write(final byte[] from, IReadWriteListener listener, int timeout) {

		
			__executor.execute(new WriteRunnable(__enpointLock,_connection,from,listener));
		


	}

	public void read(byte[] from,IReadWriteListener listener, int timeout) {

		 __executor.execute(new ReadRunnable(__executor,__enpointLock,_connection,_readEndpoint,from,timeout,listener ));
			
		/* if (__lock.tryLock() && __lock.isHeldByCurrentThread()) {
             try {
            	 __executor.execute(new ReadRunnable(_connection,_readEndpoint,into,listener ));
             } catch (Exception e) {   
            	 Log.e(TAG,"Error:",e);
             } finally {
            	 __lock.unlock();
             }               
         } else {
        	 Log.d(TAG, " already Reading...");
         }*/
		
		
	}
}
