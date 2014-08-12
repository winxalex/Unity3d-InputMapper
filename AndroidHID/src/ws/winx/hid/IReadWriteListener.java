package ws.winx.hid;

import java.util.EventListener;

public interface IReadWriteListener extends EventListener {
  void onRead(byte[] data);
  void onWrite(boolean success);
}
