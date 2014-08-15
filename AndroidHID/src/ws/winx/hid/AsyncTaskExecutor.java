package ws.winx.hid;



import android.os.AsyncTask;

public class AsyncTaskExecutor extends AsyncTask<Void, Void, ReadData> {

	
	    private IReadWriteListener listener;
	  

	    public AsyncTaskExecutor(IReadWriteListener listener){
	        this.listener=listener;
	       
	    }

	    // required methods
	    
	    @Override protected void onPostExecute(ReadData data) {  listener.onRead(data);};

	

		@Override
		protected ReadData doInBackground(Void... params) {
			return null;
		}
	}
