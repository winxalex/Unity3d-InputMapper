package ws.winx.hid;

import java.io.IOException;
import java.nio.ByteBuffer;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.FutureTask;

import android.hardware.usb.UsbConstants;
import android.hardware.usb.UsbDevice;
import android.hardware.usb.UsbDeviceConnection;
import android.hardware.usb.UsbEndpoint;
import android.hardware.usb.UsbInterface;
import android.hardware.usb.UsbManager;

import android.util.Log;

public class GenericHIDDevice {

	protected static final String TAG = null;
	UsbDeviceConnection _connection;
	UsbDevice _device;
	UsbEndpoint _readEndpoint;
	UsbEndpoint _writeEndpoint;

	UsbManager _usbManager;

	private int _inputBufferSize = 8;
	private int _outputBufferSize = 8;

	public GenericHIDDevice(UsbDevice device, UsbManager usbManager) {

		_device = device;
		_usbManager = usbManager;
	}

	public void open() {

		_connection = _usbManager.openDevice(_device);

		if (_device.getInterfaceCount() < 0) {
			Log.e(TAG, "Could not claim control interface.");
			return;
		}

		UsbInterface mControlInterface = _device.getInterface(0);

		if (!_connection.claimInterface(mControlInterface, true)) {
			Log.e(TAG, "Could not claim control interface.");
		}

		UsbEndpoint endPoint;

		if (mControlInterface.getEndpointCount() == 1)
			_writeEndpoint = _readEndpoint = mControlInterface.getEndpoint(0);
		else {
			endPoint = mControlInterface.getEndpoint(0);
			if (endPoint.getDirection() == UsbConstants.USB_DIR_IN)
				_readEndpoint = endPoint;
			else if (endPoint.getDirection() == UsbConstants.USB_DIR_OUT)
				_writeEndpoint = endPoint;

			endPoint = mControlInterface.getEndpoint(1);
			if (endPoint.getDirection() == UsbConstants.USB_DIR_IN)
				_readEndpoint = endPoint;
			else if (endPoint.getDirection() == UsbConstants.USB_DIR_OUT)
				_writeEndpoint = endPoint;

			if (_readEndpoint == null || _writeEndpoint == null)
				_writeEndpoint = _readEndpoint = mControlInterface
						.getEndpoint(0);
		}

	}

	public void closeDevice() {
		if (_connection != null)
			_connection.close();
	}

	public void Dispose() {

	}

	public void write(byte[] data, int timeout) {

		synchronized (this) {

			_connection
					.bulkTransfer(_writeEndpoint, data, data.length, timeout);
		}

	}

	public void read(IReadListener listener, int timeout) {

		ExecutorService executor = Executors.newCachedThreadPool();

		synchronized (this) {
			Future<byte[]> ft = executor.submit(new ReadCallable(_connection,
					_readEndpoint, _inputBufferSize));

			try {
				listener.onRead(ft.get());
			} catch (Exception e) {
				Log.e(TAG, " Error:", e);
			}
		}

	}

	int get_inputBufferSize() {
		return _inputBufferSize;
	}

	void set_inputBufferSize(int _inputBufferSize) {
		this._inputBufferSize = _inputBufferSize;
	}

	int get_outputBufferSize() {
		return _outputBufferSize;
	}

	void set_outputBufferSize(int _outputBufferSize) {
		this._outputBufferSize = _outputBufferSize;
	}

}
