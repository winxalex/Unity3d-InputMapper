#if UNITY_WEBPLAYER	
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Timers;

namespace ws.winx.platform.web
{
    public class WebMessageArgs<T> : EventArgs
    {
        public readonly T RawMessage;

        public WebMessageArgs(T message)
        {
            this.RawMessage = message;
        }
    }



	public class WebHIDBehaviour: MonoBehaviour
	{

        public event EventHandler<WebMessageArgs<bool>> GamePadEventsSupportEvent;

		public event EventHandler<WebMessageArgs<GenericHIDDevice>> DeviceConnectedEvent;
		public event EventHandler<WebMessageArgs<int>> DeviceDisconnectedEvent;
        public event EventHandler<WebMessageArgs<WebHIDReport>> PositionUpdateEvent;

        protected bool _isInitialized = false;
        protected bool _hasEvents = false;

        public void HaveGamepadEvents()
        {
            Application.ExternalEval(
            "if('GamepadEvent' in window) UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onHaveGamepadEvents','1');" +
            "else UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onHaveGamepadEvents','0');"  
           );
        }


        public void onHaveGamepadEvents(string message)
        {
			bool hasGamepadEvents = Convert.ToBoolean(message);
             Log("onHaveGamepadEvents:" + (message=="1" ? "true":"false"));

            if (GamePadEventsSupportEvent!=null)
            {
                GamePadEventsSupportEvent(this, new WebMessageArgs<bool>(hasGamepadEvents));
            }



          


            if (hasGamepadEvents)
            {
                _hasEvents=true;
 Application.ExternalEval(
   //  "window.addEventListener('gamepadconnected',function(e){  var buttons = [];  for (var i = 0; i < e.gamepad.buttons.length; i++)   buttons[i] = e.gamepad.buttons[i].value; UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onDeviceConnectedEvent',JSON.stringify({ id: e.gamepad.id, axes: e.gamepad.axes, buttons: buttons, index: e.gamepad.index }))});" +
   //  "window.addEventListener('gamepaddisconnected',function(e){ UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onDeviceDisconnectedEvent',e.gamepad.index.toString())});"
      "window.addEventListener('gamepadconnected',function(e){   UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onDeviceConnectedEvent',JSON.stringify({ id: e.gamepad.id,  numButtons:e.gamepad.buttons.length, numAxes:e.gamepad.axes.length,  index: e.gamepad.index }))});" +
     "window.addEventListener('gamepaddisconnected',function(e){ UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onDeviceDisconnectedEvent',e.gamepad.id)});"
           
           );



            }
            else//manuall check
            {
               

                Timer enumeratorTimer = new Timer(1000);
                enumeratorTimer.Elapsed += new ElapsedEventHandler(EnumerateGamepadsTimedEventHandler);
                enumeratorTimer.Enabled = true;
            }
               
           
        }


        public void onDeviceConnectedEvent(string message)
        {
            if (DeviceConnectedEvent != null)
            {
                _isInitialized = true;
				GenericHIDDevice info=Json.Deserialize<GenericHIDDevice>(message) as GenericHIDDevice;
				int PID;
				int VID;
				string Name;
				IDToDeviceData (info.id,out PID,out VID,out Name);

				info.PID=PID;
				info.VID=VID;
				info.Name=Name;
                DeviceConnectedEvent(this, new WebMessageArgs<GenericHIDDevice>(info));
            }
        }

        public void onDeviceDisconnectedEvent(string message)
        {
            if (DeviceDisconnectedEvent != null)
            {
				int PID;
				int VID;
				string Name;
				IDToDeviceData (message,out PID,out VID,out Name);
                DeviceDisconnectedEvent(this, new WebMessageArgs<int>(PID));
            }
        }


		internal void IDToDeviceData(String value,out int PID,out int VID,out string Name){
			
			String[] parts;
			int inx;


			parts = value.Split('-');
			//044f-b653-NAME FF
			//Name (Vendor: 044f Product: b653) Chrome

			PID = 0x0;
			VID = 0x0;


			if (parts.Length > 2)
			{
				Name = parts[2];
				PID = Convert.ToInt32(parts[1].Trim(), 16);
				VID = Convert.ToInt32(parts[0].Trim(), 16);
			}
			else
				if ((inx = value.IndexOf("(")) > -1)
			{
				Name = value.Substring(0, inx - 1);
				parts = value.Substring(inx, value.Length - inx - 1).Replace("Product", "").Split(':');
				
				PID = Convert.ToInt32(parts[2].Trim(), 16);
				VID = Convert.ToInt32(parts[1].Trim(), 16);
			}
			else
				Name = value;
			
		}


        private string ENUMERATE_COMMAND = "var gamepads = navigator.getGamepads ? navigator.getGamepads() : (navigator.webkitGetGamepads ? navigator.webkitGetGamepads() : []);" +
                                              "for (var i = 0; i < gamepads.length; i++) {"+
                                              " var gamepad=gamepads[i]; " +
                                              
                                                "if (gamepad) {"+
                                                   
                                                  "UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onDeviceConnectedEvent',JSON.stringify({ id: gamepad.id, numButtons:gamepad.buttons.length, numAxes:gamepad.axes.length,  index: gamepad.index }));" +
                                               " }"+
                                              "}";
                                          




        private string GAMEPAD_COMMAND = "var gamepads = navigator.getGamepads ? navigator.getGamepads() : (navigator.webkitGetGamepads ? navigator.webkitGetGamepads() : []);" +
                                          "if(gamepads.length>0 && gamepads[{0}]){{" +
                                          "UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onRead',JSON.stringify(gamepads[{0}]));" +
                                          "}}else{{"+
                                          "UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onDeviceDisconnectedEvent','{0}');"+
                                          "}}";

        //FF JSON doesn't work well
        private string GAMEPAD_COMMAND_COMPLEX = "var gamepads = navigator.getGamepads ? navigator.getGamepads() : (navigator.webkitGetGamepads ? navigator.webkitGetGamepads() : []);" +
                                        "if(gamepads.length>0 && gamepads[{0}]){{" +
                                        "var gamepad=gamepads[{0}]; var buttons = [];  for (var i = 0; i < gamepad.buttons.length; i++)   buttons[i] = gamepad.buttons[i].value;"+
                                        "UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onRead',JSON.stringify({{ id: gamepad.id, axes: gamepad.axes, buttons: buttons, index: gamepad.index }}));" +
                                        "}}";
       
        
        
        void EnumerateGamepadsTimedEventHandler(object sender, EventArgs e)
        {
           
            //UnityEngine.Debug.Log(ENUMERATE_COMMAND);
                Application.ExternalEval(ENUMERATE_COMMAND);
           
  
        }



           


            public void Read(int index)
            {
               // UnityEngine.Debug.Log(String.Format(GAMEPAD_COMMAND, index));
                if(_hasEvents)
                    Application.ExternalEval(String.Format(GAMEPAD_COMMAND_COMPLEX, index));
                else
                    Application.ExternalEval(String.Format(GAMEPAD_COMMAND, index));
            }


            public void onRead(string message){
                
                if (PositionUpdateEvent!=null)
                {
					WebHIDReport report= (WebHIDReport)Json.Deserialize<WebHIDReport>(message);
                   // Log("onJoyGetPos" + message);
                    //Debug.Log("Send Event");
                    PositionUpdateEvent(this, new WebMessageArgs<WebHIDReport>(report));

                }
            }


            public void Log(string message)
            {
                Application.ExternalEval("console.log('" + message + "')");
            }

        void Start()
        {
            HaveGamepadEvents();
        }

        void Awake()
        {
            UnityEngine.Object.DontDestroyOnLoad(this);
        }
    }


}
#endif