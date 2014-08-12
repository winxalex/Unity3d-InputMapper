package ws.winx.hid;

import android.os.AsyncTask;

public class AsyncTaskExecutor extends AsyncTask<Void, Void, byte[]> {

	
	    private IReadWriteListener listener;
	  

	    public AsyncTaskExecutor(IReadWriteListener listener){
	        this.listener=listener;
	       
	    }

	    // required methods
	    
	    @Override protected void onPostExecute(byte[] data) {  listener.onRead(data);};

	

		@Override
		protected byte[] doInBackground(Void... params) {
			return null;
		}
	}
