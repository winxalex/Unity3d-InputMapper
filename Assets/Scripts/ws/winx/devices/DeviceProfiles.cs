using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ws.winx.devices
{
		public class DeviceProfiles : ScriptableObject,ISerializationCallbackReceiver
		{

				//lists used for serialization of pidvidProfileNameDict
				[HideInInspector]
				public List<string>
						vidpidProfileNameKeys;
				[HideInInspector]
				public List<string>
						vidpidProfileNameValues;
				public Dictionary<string,string>  vidpidProfileNameDict = new Dictionary<string,string> ();

				//lists used for serialization of runtimePlatformDeviceProfileDict
				[HideInInspector]
				public List<string>
						runtimePlatformDeviceProfileKeys;
				[HideInInspector]
				public List<RuntimePlatformListWrapper>
						runtimePlatfromKeys;
				[HideInInspector]
				public List<DeviceProfileListWrapper>
						deviceProfileValues;
				public Dictionary<string,Dictionary<RuntimePlatform,DeviceProfile>>  runtimePlatformDeviceProfileDict = new Dictionary<string,Dictionary<RuntimePlatform,DeviceProfile>> ();
				public DeviceProfile currentProfile;

		#region ISerializationCallbackReceiver implementation
				public void OnBeforeSerialize ()
				{
						
						vidpidProfileNameKeys.Clear ();
						vidpidProfileNameValues.Clear ();
						foreach (var kvp in vidpidProfileNameDict) {
								vidpidProfileNameKeys.Add (kvp.Key);
								vidpidProfileNameValues.Add (kvp.Value);
						}

						int i;
						for (i=0; i<deviceProfileValues.Count; i++) {
								if (deviceProfileValues [i].list != null)
										deviceProfileValues [i].list.Clear ();
						}

						deviceProfileValues.Clear ();


						for (i=0; i<runtimePlatfromKeys.Count; i++) {
								if (runtimePlatfromKeys [i].list != null)
										runtimePlatfromKeys [i].list.Clear ();
						}
						runtimePlatfromKeys.Clear ();
						runtimePlatformDeviceProfileKeys.Clear ();

						foreach (var kvp in runtimePlatformDeviceProfileDict) {
								runtimePlatformDeviceProfileKeys.Add (kvp.Key);

								RuntimePlatformListWrapper runtimePlatformKeyList = new RuntimePlatformListWrapper ();
								DeviceProfileListWrapper runtimePlatformValueList = new DeviceProfileListWrapper ();

								foreach (var kvp1 in kvp.Value) {


										runtimePlatformValueList.list.Add (kvp1.Value);
										runtimePlatformKeyList.list.Add (kvp1.Key);
								}


								deviceProfileValues.Add (runtimePlatformValueList);
								runtimePlatfromKeys.Add (runtimePlatformKeyList);
						}




				}

				public void OnAfterDeserialize ()
				{
						int i, j;

						vidpidProfileNameDict = new Dictionary<string,string> ();
						for (i=0; i!= Math.Min(vidpidProfileNameKeys.Count,vidpidProfileNameValues.Count); i++)
								vidpidProfileNameDict.Add (vidpidProfileNameKeys [i], vidpidProfileNameValues [i]);

						runtimePlatformDeviceProfileDict = new Dictionary<string,Dictionary<RuntimePlatform,DeviceProfile>> ();
						for (i=0; i<Math.Min(runtimePlatformDeviceProfileKeys.Count,Math.Min(runtimePlatfromKeys.Count,deviceProfileValues.Count)); i++) {

								Dictionary<RuntimePlatform,DeviceProfile> tempDict = new Dictionary<RuntimePlatform, DeviceProfile> ();

								RuntimePlatformListWrapper runtimePlatformKeyList = runtimePlatfromKeys [i];
								DeviceProfileListWrapper runtimePlatformValueList = deviceProfileValues [i];

								for (j=0; j<Math.Min (runtimePlatformValueList.list.Count,runtimePlatformValueList.list.Count); j++) {
										tempDict.Add (runtimePlatformKeyList.list [j], runtimePlatformValueList.list [j]);
								}

								runtimePlatformDeviceProfileDict.Add (runtimePlatformDeviceProfileKeys [i], tempDict);
								
						}
				}
		#endregion
		}



}

