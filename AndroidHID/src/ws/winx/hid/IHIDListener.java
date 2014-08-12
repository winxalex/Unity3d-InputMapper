package ws.winx.hid;

import java.util.EventListener;

public interface IHIDListener extends EventListener {
	void onAttached(HIDDeviceWrapper deviceWrapper);
    void onDetached(int pid);
    void onError(String error);
}

;