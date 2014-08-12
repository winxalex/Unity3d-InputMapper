package ws.winx.hid;

import android.os.Bundle;
import android.util.Log;
import android.app.Activity;

public class AndroidHIDActivity extends Activity {
//public class AndroidHIDActivity extends UnityPlayerActivity {

	private static final String TAG = "AndroidHIDActivity";
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
	}
	
	
	 @Override
	    protected void onResume() {
		 
	        //this.mUnityPlayer.UnitySendMessage(arg0, arg1, arg2)
		// UnitySendMessage("GameObjectName1", "MethodName1", "Message to send");
	            Log.d(TAG, "onResume");
	        super.onResume();
	    }
	    @Override
	    protected void onPause()
	    {
	    	 Log.d(TAG, "onPause");
	        super.onPause();
	    }
	    @Override
	    protected void onStop() {
	       
	            Log.d(TAG, "onStop");
	        super.onStop();
	    }
	    



}
