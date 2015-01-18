

using UnityEditor;
using UnityEngine;
using ws.winx.devices;
using System;
using System.IO;


namespace ws.winx.editor
{

	
	[CustomEditor(typeof(DeviceProfiles))]
	[CanEditMultipleObjects]
	public class DeviceProfileEditor : Editor
	{
		private DeviceProfiles D; //Make an easy shortcut to the Dialogue your editing

		private DeviceProfile[] __profiles;

		void Awake()
		{
			//D=(DeviceProfiles)target;
		}

	

		void OnEnable () {
			// Setup the SerializedProperties


		//	__profiles=serializedObject.FindProperty ("profiles");
//			damageProp = serializedObject.FindProperty ("damage");
//			armorProp = serializedObject.FindProperty ("armor");
//			gunProp = serializedObject.FindProperty ("gun");
		}



		
		public override void OnInspectorGUI()
		{
			this.DrawDefaultInspector ();
			GUILayout.Button ("mile");


			// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
			//serializedObject.ApplyModifiedProperties ();
		}
	}
}