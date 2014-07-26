using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Timers;

namespace ws.winx.platform.web
{
    public class WebMessageArgs : EventArgs
    {
        public readonly string Message;

        public WebMessageArgs(string message)
        {
            this.Message = message;
        }
    }



	public class WebHIDBehaviour: MonoBehaviour
	{

        public event EventHandler<WebMessageArgs> GamePadEventsSupportEvent;

        public event EventHandler<WebMessageArgs> DeviceConnectedEvent;
        public event EventHandler<WebMessageArgs> DeviceDisconnectedEvent;
        public event EventHandler<WebMessageArgs> PositionUpdateEvent;

        protected bool _isInitialized = false;

        public void HaveGamepadEvents()
        {
            Application.ExternalEval(
            "if('GamepadEvent' in window) UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onHaveGamepadEvents','1');" +
            "else UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onHaveGamepadEvents','0');"  
           );
        }


        public void onHaveGamepadEvents(string message)
        {

             UnityEngine.Debug.Log("onHaveGamepadEvents:" + message);

            if (GamePadEventsSupportEvent!=null)
            {
                GamePadEventsSupportEvent(this, new WebMessageArgs(message));
            }



          


            if (message== "1")
            {
               
 Application.ExternalEval(
     "window.addEventListener('gamepadconnected',function(e){ UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onDeviceConnectedEvent',JSON.stringify(e.gamepad))});" +
     "window.addEventListener('gamepaddisconnected',function(e){ UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onDeviceDisconnectedEvent',e.gamepad.index.toString())});"
             
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
                DeviceConnectedEvent(this, new WebMessageArgs(message));
            }
        }

        public void onDeviceDisconnectedEvent(string message)
        {
            if (DeviceDisconnectedEvent != null)
            {
                DeviceDisconnectedEvent(this, new WebMessageArgs(message));
            }
        }


        private string ENUMERATE_COMMAND = "var gamepads = navigator.getGamepads ? navigator.getGamepads() : (navigator.webkitGetGamepads ? navigator.webkitGetGamepads() : []);" +
                                              "for (var i = 0; i < gamepads.length; i++) {"+
                                                "if (gamepads[i]) {"+
                                              
                                                  "UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onDeviceConnectedEvent',JSON.stringify(gamepads[i]));"+
                                               " }"+
                                              "}";
                                          




        private string GAMEPAD_COMMAND = "var gamepads = navigator.getGamepads ? navigator.getGamepads() : (navigator.webkitGetGamepads ? navigator.webkitGetGamepads() : []);" +
                                          "if(gamepads.length>0 && gamepads[{0}]){{" +
                                          "UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onJoyGetPosEx',JSON.stringify(gamepads[{0}]));" +
                                          "}}else{{"+
                                          "UnityObject2.instances[0].getUnity().SendMessage('WebHIDBehaviourGO','onDeviceDisconnectedEvent','{0}');"+
                                          "}}";
       
        
        
        void EnumerateGamepadsTimedEventHandler(object sender, EventArgs e)
        {
           // UnityEngine.Debug.Log(ENUMERATE_COMMAND);
                Application.ExternalEval(ENUMERATE_COMMAND);
           
  
        }



           


            public void joyGetPosEx(int index)
            {
               // UnityEngine.Debug.Log(String.Format(GAMEPAD_COMMAND, index));
                Application.ExternalEval(String.Format(GAMEPAD_COMMAND, index));
            }


            public void onJoyGetPosEx(string message){
                //Debug.Log("onJoyGetPos");
                if (PositionUpdateEvent!=null)
                {
                    //Debug.Log("Send Event");
                    PositionUpdateEvent(this, new WebMessageArgs(message));

                }
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
