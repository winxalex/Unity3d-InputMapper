package ws.winx.hid;


import java.util.Iterator;


import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.hardware.usb.UsbDevice;

import android.hardware.usb.UsbManager;

import android.util.Log;
import android.util.SparseArray;

public class AndroidHID {

	private static final String ACTION_USB_PERMISSION =
		    "com.android.example.USB_PERMISSION";
	protected static final String TAG = "AndroidHID";
	
	private static UsbManager __usbManager;
	private static PendingIntent __permissionIntent;
	private static IHIDListener __IHIDLIstener;

	private static SparseArray<HIDDeviceWrapper> _Generics;
	private static Context _context;
	
	
	public static void Init(Context context,IHIDListener listener) {

		__usbManager = (UsbManager) context.getSystemService(Context.USB_SERVICE);
		__IHIDLIstener=listener;
		__permissionIntent = PendingIntent.getBroadcast(context, 0, new Intent(ACTION_USB_PERMISSION), 0);
		IntentFilter filter = new IntentFilter();
		filter.addAction(ACTION_USB_PERMISSION);
		filter.addAction(UsbManager.ACTION_USB_DEVICE_ATTACHED);
		filter.addAction(UsbManager.ACTION_USB_DEVICE_DETACHED);
		context.registerReceiver(__usbReceiver, filter);
		_context=context;
		
		_Generics=new SparseArray<HIDDeviceWrapper>();
	}
	
	
	

	public static void write(byte[] data,int pid,IReadWriteListener listener){
		HIDDeviceWrapper device=_Generics.get(pid);
		
		if(device!=null)
			device.write(data,listener, 0);
		else
			Enumerate();
	}
	
	public static void read(byte[] into,int pid,IReadWriteListener listener){
		HIDDeviceWrapper device=_Generics.get(pid);
		
		if(device!=null)
		  device.read(into,listener,500);
		else
		 Enumerate();
	}
	
	
	public static void Enumerate(){
		
		UsbDevice device;
		
			Iterator<UsbDevice> deviceIterator = __usbManager.getDeviceList().values().iterator();
			while(deviceIterator.hasNext()){
				device=deviceIterator.next();
				
				
				if(__usbManager.hasPermission(device)){
				
				//Fancy
				/*	   intent = new Intent(UsbManager.ACTION_USB_DEVICE_ATTACHED).putExtra(UsbManager.EXTRA_DEVICE, device);
		        _context.sendBroadcast(intent);*/
					
					 AndroidHID.attachDevice(device);
				}else{
					__usbManager.requestPermission(device, __permissionIntent);
				}
				
			}
		
	}
	
	public static void Log(String tag,String message){
		Log.d(tag,message);
	}
	
	public static void Dispose(){
		int i = _Generics.size()-1;
		int key;
		
		if(__usbReceiver!=null)
		_context.unregisterReceiver(__usbReceiver);
		
		while( i > -1) {
		   key = _Generics.keyAt(i);
		   // get the object by the key.
		   _Generics.get(key).Dispose();
		   _Generics.remove(key);
		   i--;
		}
		
	}
	
	private static void attachDevice(UsbDevice device){
		 HIDDeviceWrapper wrapper=new HIDDeviceWrapper(device, __usbManager);
    	 wrapper.open();
    	 
		 _Generics.append(device.getProductId(), wrapper);
		 __IHIDLIstener.onAttached(wrapper);
		 
		 Log.d(TAG, "Device id:"+device.getDeviceId()+" PID:" + device.getProductId()+" VID:"+device.getVendorId()+" Connected!");
         
	}
	
	private static BroadcastReceiver __usbReceiver = new BroadcastReceiver() {
	    public void onReceive(Context context, Intent intent) {
	        String action = intent.getAction(); 
	        if (UsbManager.ACTION_USB_DEVICE_ATTACHED.equals(action)) {
	            UsbDevice device = (UsbDevice)intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);
	            if (device != null) {
	            	 if(!__usbManager.hasPermission(device)){
	            		 __usbManager.requestPermission(device,__permissionIntent);
	            	 }else{
	            		 Log.d(TAG, "Device id:"+device.getDeviceId()+" PID:" + device.getProductId()+" VID:"+device.getVendorId()+" Connected!");
	            		 Log.d(TAG, device.toString());
	            		
	            		 AndroidHID.attachDevice(device);
	            	 }
	                
	            }
	        }
	        else if (UsbManager.ACTION_USB_DEVICE_DETACHED.equals(action)) {
	            UsbDevice device = (UsbDevice)intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);
	            if (device != null) {
	            	 __IHIDLIstener.onDetached(device.getProductId());
	            	 Log.d(TAG, "Device id:"+device.getDeviceId()+" PID:" + device.getProductId()+" VID:"+device.getVendorId()+" Disconnected!");
                      _Generics.get(device.getProductId()).Dispose();
                      _Generics.remove(device.getProductId());
	            	
	            }
	        }
	      else  if (ACTION_USB_PERMISSION.equals(action)) {
	            synchronized (this) {
	                UsbDevice device = (UsbDevice)intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);

	                if (intent.getBooleanExtra(UsbManager.EXTRA_PERMISSION_GRANTED, false)) {
	                    if(device != null){
	                    	 Log.d(TAG, "Device id:"+device.getDeviceId()+" PID:" + device.getProductId()+" VID:"+device.getVendorId()+" Permission Granted!");
	                    	 
	                    	 AndroidHID.attachDevice(device);
	                    	 
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
