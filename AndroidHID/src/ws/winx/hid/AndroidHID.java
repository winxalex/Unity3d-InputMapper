package ws.winx.hid;

import java.io.IOException;
import java.util.HashMap;
import java.util.Iterator;

import android.R.string;
import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.hardware.usb.UsbDevice;
import android.hardware.usb.UsbDeviceConnection;
import android.hardware.usb.UsbEndpoint;
import android.hardware.usb.UsbInterface;
import android.hardware.usb.UsbManager;

import android.util.Log;
import android.util.SparseArray;

public class AndroidHID {

	private static final String ACTION_USB_PERMISSION =
		    "com.android.example.USB_PERMISSION";
	protected static final String TAG = "AndroidHID";
	
	private static UsbManager _usbManager;
	private static PendingIntent mPermissionIntent;
	private static IHIDListener mIHIDLIstener;
	//private static Context mContext;
	private static SparseArray<GenericHIDDevice> _Generics;
	
	
	public AndroidHID(Context context,IHIDListener listener) {

		_usbManager = (UsbManager) context.getSystemService(Context.USB_SERVICE);
		
		mPermissionIntent = PendingIntent.getBroadcast(context, 0, new Intent(ACTION_USB_PERMISSION), 0);
		IntentFilter filter = new IntentFilter();
		filter.addAction(ACTION_USB_PERMISSION);
		filter.addAction(UsbManager.ACTION_USB_DEVICE_ATTACHED);
		filter.addAction(UsbManager.ACTION_USB_DEVICE_DETACHED);
		context.registerReceiver(mUsbReceiver, filter);
		
		_Generics=new SparseArray<GenericHIDDevice>();
	}
	
	
	

	public static void write(byte[] data,int pid){
		
	}
	
	public static void read(int pid,IReadListener listener){
		_Generics.get(pid).read(listener,0);
	}
	
	
	public static void Enumerate(){
		
		UsbDevice device;
			Iterator<UsbDevice> deviceIterator = _usbManager.getDeviceList().values().iterator();
			while(deviceIterator.hasNext()){
				device=deviceIterator.next();
				if(_usbManager.hasPermission(device))
					mIHIDLIstener.onAttached(device);
				else{
					_usbManager.requestPermission(device, mPermissionIntent);
				}
			}
		
	}
	
	
	
	public void Dispose(){
		
	}
	
	
	BroadcastReceiver mUsbReceiver = new BroadcastReceiver() {
	    public void onReceive(Context context, Intent intent) {
	        String action = intent.getAction(); 
	        if (UsbManager.ACTION_USB_DEVICE_ATTACHED.equals(action)) {
	            UsbDevice device = (UsbDevice)intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);
	            if (device != null) {
	            	 if(!_usbManager.hasPermission(device)){
	            	
	            	 }else{
	            		 Log.d(TAG, "Device PID:" + device.getProductId()+" VID:"+device.getProductId()+" Connected!");
	            		 mIHIDLIstener.onAttached(device);
	            		 
	            	 }
	                // call your method that cleans up and closes communication with the device
	            }
	        }
	        else if (UsbManager.ACTION_USB_DEVICE_DETACHED.equals(action)) {
	            UsbDevice device = (UsbDevice)intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);
	            if (device != null) {
	            	 mIHIDLIstener.onDetached(device.getProductId());
	            	 Log.d(TAG, "Device PID:" + device.getProductId()+" VID:"+device.getProductId()+" Disconnected!");
	                // call your method that cleans up and closes communication with the device
	            }
	        }
	      else  if (ACTION_USB_PERMISSION.equals(action)) {
	            synchronized (this) {
	                UsbDevice device = (UsbDevice)intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);

	                if (intent.getBooleanExtra(UsbManager.EXTRA_PERMISSION_GRANTED, false)) {
	                    if(device != null){
	                    	 Log.d(TAG, "Device PID:" + device.getProductId()+" VID:"+device.getProductId()+" Permission Granted!");
	                     
	                    	 mIHIDLIstener.onAttached(device);
	                    	 
	                   }
	                } 
	                else {
	                    Log.d(TAG, "permission denied for device " + device);
	                }
	            }
	        }
	    }
	};

}
