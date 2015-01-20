using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ws.winx.devices
{
		public class DeviceProfiles : ScriptableObject,ISerializationCallbackReceiver
		{


				[HideInInspector]
				public List<string> pidvidShortTypeNamesKeys;

				[HideInInspector]
				public List<string> pidvidShortTypeNamesValues;

				[HideInInspector]
				public List<string> runtimePlatformDeviceProfileKeys;

		[HideInInspector]
		public List<RuntimePlatformListWrapper> runtimePlatfromKeys;

		[HideInInspector]
				public List<DeviceProfileListWrapper> deviceProfileValues;
				public Dictionary<string,string>  pidvidShortTypeNames = new Dictionary<string,string> ();
				public Dictionary<string,Dictionary<RuntimePlatform,DeviceProfile>>  runtimePlatformDeviceProfileDict = new Dictionary<string,Dictionary<RuntimePlatform,DeviceProfile>> ();
			
				public DeviceProfile currentProfile;

		#region ISerializationCallbackReceiver implementation
				public void OnBeforeSerialize ()
				{
						
						pidvidShortTypeNamesKeys.Clear ();
						pidvidShortTypeNamesValues.Clear ();
						foreach (var kvp in pidvidShortTypeNames) {
								pidvidShortTypeNamesKeys.Add (kvp.Key);
								pidvidShortTypeNamesValues.Add (kvp.Value);
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
						pidvidShortTypeNames = new Dictionary<string,string> ();
						for (i=0; i!= Math.Min(pidvidShortTypeNamesKeys.Count,pidvidShortTypeNamesValues.Count); i++)
								pidvidShortTypeNames.Add (pidvidShortTypeNamesKeys [i], pidvidShortTypeNamesValues [i]);

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

