package ws.winx.hid;
import android.hardware.usb.UsbDevice;
import java.util.EventListener;

public interface IHIDListener extends EventListener {
	void onAttached(UsbDevice device);
    void onDetached(int pid);
    void onError(String error);
}

;