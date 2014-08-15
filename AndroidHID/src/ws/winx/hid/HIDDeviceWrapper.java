package ws.winx.hid;


import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Semaphore;



import android.app.Activity;
import android.hardware.usb.UsbConstants;
import android.hardware.usb.UsbDevice;
import android.hardware.usb.UsbDeviceConnection;
import android.hardware.usb.UsbEndpoint;
import android.hardware.usb.UsbInterface;
import android.hardware.usb.UsbManager;
import android.util.Log;

public class HIDDeviceWrapper {

	protected static final String TAG = "Unity";//"HIDDeviceWrapper"
	private UsbDeviceConnection __connection;
	UsbDevice _device;
	private UsbEndpoint _readEndpoint;


	UsbManager _usbManager;


	
	boolean _lockEndpoint=false;
	public final int index;
	public final int VID;
	public final int PID;
	private UsbInterface __controlInterface;
	public final String path;
	
	private static ExecutorService __executor=Executors.newCachedThreadPool();
	//private static final ReentrantLock __lock = new ReentrantLock();
	private static final Semaphore __endPointLock=new Semaphore(1);
	private static ReadRunnable __readRunnable;
	private static  WriteRunnable __writeRunnable;
	

	public HIDDeviceWrapper(UsbDevice device, UsbManager usbManager) {

		this.index=device.getDeviceId();
		this.VID=device.getVendorId();
		this.PID=device.getProductId();
		this.path=device.getDeviceName();
		
		_device = device;
		_usbManager = usbManager;
	}

	public boolean open() {
		
		 Log.d(TAG, "Try to open device");
		 Log.d(TAG, "Number Interfaces=" + _device.getInterfaceCount());
         Log.d(TAG, "Number Endpoints=" + _device.getInterface(0).getEndpointCount());
        
		
		__connection=_usbManager.openDevice(_device);

		if (_device.getInterfaceCount() < 0) {
			Log.e(TAG, "No interface to be clamed.");
			return false;
		}

		__controlInterface = _device.getInterface(0);

		if (!get_connection().claimInterface(__controlInterface, true)) {
			Log.e(TAG, "Could not claim control interface.");
			return false;
		}

		   _readEndpoint = __controlInterface.getEndpoint(0);
	        if (_readEndpoint.getDirection()!=UsbConstants.USB_DIR_IN || _readEndpoint.getType() != UsbConstants.USB_ENDPOINT_XFER_INT) {
	            Log.e(TAG, "Endpoint is not interrupt type");
	            return false;
	        }
	        
	        Log.d(TAG, "End point Address" + _readEndpoint.getAddress());
	         Log.d(TAG, "End point Type" + _readEndpoint.getType());//type=3 USB_ENDPOINT_XFER_INT (Interupt)
	         Log.d(TAG, "End point Direction " + _readEndpoint.getDirection());//type=128 USB_DIR_IN
			
		
		
		
		return true;

	}



	public void Dispose() {
		if (__connection != null){
					
					if(__controlInterface!=null)
						get_connection().releaseInterface(__controlInterface);
					
				}
		
		HIDDeviceWrapper.__executor.shutdownNow();
		
	}
	
	public void write(byte[] from, int timeout) {

		if(__writeRunnable==null) __writeRunnable=new WriteRunnable(this);
		
		Log.d(TAG,"write "+HexDump.dumpHexString((byte[]) from)+" size"+((byte[]) from).length);
		
			//HIDDeviceWrapper.__executor.execute(__writeRunnable.read(from).timeout(timeout).addEventListener(listener));
		((Activity)AndroidHID.context).runOnUiThread(__writeRunnable.read(from).timeout(timeout));


	}

	public void write(byte[] from, IReadWriteListener listener, int timeout) {

		if(__writeRunnable==null) __writeRunnable=new WriteRunnable(this);
		
		
		Log.d(TAG,"write with listener:"+HexDump.dumpHexString((byte[]) from)+" size"+((byte[]) from).length);
		
			//HIDDeviceWrapper.__executor.execute(__writeRunnable.read(from).timeout(timeout).addEventListener(listener));
		((Activity)AndroidHID.context).runOnUiThread(__writeRunnable.read(from).timeout(timeout).addEventListener(listener));


	}

	public void read(Object into,IReadWriteListener listener, int timeout) {

		if(__readRunnable==null)
		    __readRunnable=new ReadRunnable(this);
		
/*		Log.d(TAG,"Received: "+into.toString()+"  "+into.getClass().getName());
		Log.d(TAG,"Values"+HexDump.dumpHexString((byte[]) into)+" size"+((byte[]) into).length);
		//return;
		
		final byte[] _inputBuffer=(byte[]) into;
		
		final UsbDeviceConnection connection=__connection;
		final UsbEndpoint endPoint=_readEndpoint;
		final IReadWriteListener _listener=listener;
		
		if(__readRunnable.is_isReady())
			((Activity)AndroidHID.context).runOnUiThread(
					new Runnable() {
						
						@Override
						public void run() {
							// this._isReady=false;
							 
							 try {
								HIDDeviceWrapper.getEndPointlock().acquire();
								 Log.d(TAG,"Lock aquired");
							} catch (InterruptedException e) {
								// TODO Auto-generated catch block
								  Log.e(TAG,"Lock error",e);
							}
							
							 ByteBuffer buffer = ByteBuffer.wrap(_inputBuffer);
							 
							 UsbRequest request = new UsbRequest();
						        if(!request.initialize(connection, endPoint)){
						        	
							        	   Log.e(TAG,"Cant queue request");
							        	   return;
							          
						        }
						        
						        Log.d(TAG,"Request"+request);
						        
						           if(!request.queue(buffer, _inputBuffer.length)){
						        	   
						        	   Log.e(TAG,"Cant queue request");
						        	   
						        	   request.close();
						        	   return;
						           }
						            
						           Log.e(TAG,"Queue request");
						        
						        
						        // wait for status event
					            if (connection.requestWait() == request) {
					            	
					            	 Log.d(TAG, "Request succeded");
					            		Log.d(TAG,"Values"+HexDump.dumpHexString(_inputBuffer)+" size"+( _inputBuffer).length);
					            	 _listener.onRead(new ReadData(_inputBuffer));
					            	 
					            	 Log.d(TAG, "Read sent");
					            	 request.close();
					            	 
					            	 Log.d(TAG, "Close req");
					            } else {
					                Log.e(TAG, "RequestWait failed, exiting");
					                //!!! don't close when failed
					             //   break;
					            }
						        
						        
							 HIDDeviceWrapper.getEndPointlock().release();
							 Log.d(TAG,"Lock released");
							 //this._isReady=true;
							
						}
					}
					
					
					);
			*/
			
			
			
		((Activity)AndroidHID.context).runOnUiThread(__readRunnable.read((byte[])into).timeout(timeout).addEventListener(listener));
			 //  HIDDeviceWrapper.__executor().execute(__readRunnable.read(into).timeout(timeout).addEventListener(listener));
		
		 

		
	}

	public static Semaphore getEndPointlock() {
		return __endPointLock;
	}

	public UsbDeviceConnection get_connection() {
		return __connection;
	}

	public static ExecutorService get__executor() {
		return __executor;
	}

	public UsbEndpoint get_readEndpoint() {
		return _readEndpoint;
	}





	
}
