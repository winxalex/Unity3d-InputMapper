using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ws.winx.input;
using ws.winx.devices;

namespace ws.winx.input{

	public class InputPlayer 
	{
		public enum Player:uint{
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







	}
}

