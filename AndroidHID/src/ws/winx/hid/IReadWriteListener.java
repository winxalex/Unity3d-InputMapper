package ws.winx.hid;


import java.util.EventListener;

public interface IReadWriteListener extends EventListener {
  //08-15 10:34:45.129: I/Unity(23086): AndroidJavaException: java.lang.NoSuchMethodError: no method with name='getLength' signature='(L[B;)I' in class Ljava/lang/reflect/Array;
  //void onRead(byte[] data);
	void onRead(ReadData data);
  void onWrite(boolean success);
}
