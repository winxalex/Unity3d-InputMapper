using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ws.winx.input;
using ws.winx.devices;
using System.Runtime.Serialization;

namespace ws.winx.input{




	#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
	[DataContract]
	#endif
	public class InputPlayer:System.IDisposable
	{

		public enum Player:int{
			Player0,
			Player1,
			Player2,
			Player3,
			Player4,
			Player5,
			Player6,
			Player7
			
			
			
		}

		#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[DataMember(Order=1)]
		#endif
		 Dictionary<string,Dictionary<int,InputState>> _DeviceStateInputs;


		public Dictionary<string, Dictionary<int, InputState>> DeviceProfileStateInputs {
			get {
				if(_DeviceStateInputs==null) _DeviceStateInputs=new Dictionary<string, Dictionary<int, InputState>>();
				return _DeviceStateInputs;
			}
			set {
				_DeviceStateInputs=value;

			}
		}


		public IDevice _Device;

        public IDevice Device{
            get{ 
                
                
               
            //assign deserialized Device to Player
            if (_Device == null && _deviceID != null)
            {
                if (InputEx.Devices.ContainsID(_deviceID))
                    _Device = InputEx.Devices[_deviceID];

            }
            
            return _Device;
            
            }
            set { _Device = value; if (_Device != null) _deviceID = _Device.ID; else _deviceID = null; }
        }


		#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[DataMember(Order=2)]
		#endif
		protected string _deviceID;

		public string DeviceID {
			get {
					return _deviceID;
			}
			
		}

		#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[DataMember(Order=3)]
		#endif
		public string Name;




        public Dictionary<int, InputEvent> stateEvents = new Dictionary<int, InputEvent>();

		public InputPlayer Clone(){
			InputPlayer newInputPlayer = new InputPlayer ();
			Dictionary<int,InputState> stateInputs;

			foreach (var DeviceHashStateInputPair in this.DeviceProfileStateInputs) {

				stateInputs=newInputPlayer.DeviceProfileStateInputs[DeviceHashStateInputPair.Key]=new Dictionary<int,InputState>();

				foreach(var HashStateInputPair in DeviceHashStateInputPair.Value){
					stateInputs[HashStateInputPair.Key]=HashStateInputPair.Value.Clone();
				}


			}


			return newInputPlayer;

		}






        internal InputEvent GetEvent(int stateNameHash)
        {
            if (stateEvents.ContainsKey(stateNameHash))
            {
                stateEvents[stateNameHash] = new InputEvent(stateNameHash);

            }


            return stateEvents[stateNameHash];
        }

        public void Dispose()
        {

            foreach (var stateEventsPair in this.stateEvents)
            {

                stateEventsPair.Value.Dispose();

            }

            stateEvents.Clear();

            foreach (var DeviceHashStateInputPair in this.DeviceProfileStateInputs)
            {

                DeviceHashStateInputPair.Value.Clear();

            }

            this.DeviceProfileStateInputs.Clear();
        }
    }
}

