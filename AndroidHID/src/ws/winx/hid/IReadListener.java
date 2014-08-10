package ws.winx.hid;

import java.util.EventListener;

public interface IReadListener extends EventListener {
  void onRead(byte[] data);
}
