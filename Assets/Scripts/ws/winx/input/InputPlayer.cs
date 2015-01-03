using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ws.winx.input;
using ws.winx.devices;

namespace ws.winx.input{





	public class InputPlayer 
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


		 Dictionary<string,Dictionary<int,InputState>> _DeviceStateInputs;


		public Dictionary<string, Dictionary<int, InputState>> DeviceStateInputs {
			get {
				if(_DeviceStateInputs==null) _DeviceStateInputs=new Dictionary<string, Dictionary<int, InputState>>();
				return _DeviceStateInputs;
			}
		}


		public IDevice Device;


		public InputPlayer Clone(){
			InputPlayer newInputPlayer = new InputPlayer ();
			Dictionary<int,InputState> stateInputs;

			foreach (var DeviceHashStateInputPair in this.DeviceStateInputs) {

				stateInputs=newInputPlayer.DeviceStateInputs[DeviceHashStateInputPair.Key]=new Dictionary<int,InputState>();

				foreach(var HashStateInputPair in DeviceHashStateInputPair.Value){
					stateInputs[HashStateInputPair.Key]=HashStateInputPair.Value.Clone();
				}


			}


			return newInputPlayer;

		}





	}
}

