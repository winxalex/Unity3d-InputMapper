using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using ws.winx.unity.attributes;
using UnityEditorInternal;

namespace ws.winx.editor.drawers
{
		[CustomPropertyDrawer(typeof(InputEventAttribute))]
		public class InputEventAttributePropertyDrawer : PropertyDrawer
		{

				
				float onUPSerializedPropertyHeight;
				float onDOWNSerializedPropertyHeight;
				float onHOLDSerializedPropertyHeight;

				public new InputEventAttribute attribute{ get { return (InputEventAttribute)base.attribute; } }

				public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
				{
						SerializedProperty onUPSerialized = property.FindPropertyRelative ("onUP");
						SerializedProperty onDOWNSerialized = property.FindPropertyRelative ("onDOWN");
						SerializedProperty onHOLDSerialized = property.FindPropertyRelative ("onHOLD");
			
		
					
						SerializedProperty elements;
						ReorderableList list;
						
						elements = onUPSerialized.FindPropertyRelative ("m_PersistentCalls.m_Calls");
						list = new ReorderableList (onUPSerialized.serializedObject, elements, false, true, true, true);
						
						list.elementHeight = 43f;//MAGIC NUMBER (see UnityEventDrawer onGUI and GetState)
						onUPSerializedPropertyHeight = list.GetHeight ();

					

						elements = onDOWNSerialized.FindPropertyRelative ("m_PersistentCalls.m_Calls");
						list = new ReorderableList (onDOWNSerialized.serializedObject, elements, false, true, true, true);
						list.elementHeight = 43f;

						onDOWNSerializedPropertyHeight = list.GetHeight ();


						elements = onHOLDSerialized.FindPropertyRelative ("m_PersistentCalls.m_Calls");
						list = new ReorderableList (onHOLDSerialized.serializedObject, elements, false, true, true, true);
						
						list.elementHeight = 43f;
						onHOLDSerializedPropertyHeight = list.GetHeight ();


						return onUPSerializedPropertyHeight + onDOWNSerializedPropertyHeight + onHOLDSerializedPropertyHeight
								+ 5 * 2f + 16f;//offset + EnumPopup
				}

				public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
				{
						

						SerializedProperty stateHashSerialized = property.FindPropertyRelative ("state");

						Enum stateSelected=null;
			
								if (Enum.IsDefined (attribute.GetEnumType (), stateHashSerialized.intValue)) {
										stateSelected = (Enum)Enum.ToObject (this.attribute.GetEnumType (), stateHashSerialized.intValue);
								} else//get first(default) available state
										stateSelected = this.attribute.GetEnumValue ();


						

						

						SerializedProperty onUPSerialized = property.FindPropertyRelative ("onUP");
						SerializedProperty onDOWNSerialized = property.FindPropertyRelative ("onDOWN");
						SerializedProperty onHOLDSerialized = property.FindPropertyRelative ("onHOLD");




			
						EditorGUI.BeginProperty (position, label, property);


						
						position.y += 2f;
						position.height = 16f;
						EditorGUI.BeginChangeCheck ();
					
						stateSelected = EditorGUI.EnumPopup (position, stateSelected);

						if (EditorGUI.EndChangeCheck ()) {
								stateHashSerialized.intValue = (int)Convert.ChangeType (stateSelected, stateSelected.GetTypeCode ());
								//stateHashSerialized.serializedObject.ApplyModifiedProperties ();
						}

					
						position.y += 2f + position.height;
						position.height = onUPSerializedPropertyHeight;
		
						EditorGUI.PropertyField (position, onUPSerialized);

						position.y += 2f + position.height;
						position.height = onDOWNSerializedPropertyHeight;

						EditorGUI.PropertyField (position, onDOWNSerialized);

						position.y += 2f + position.height;
						position.height = onDOWNSerializedPropertyHeight;

						EditorGUI.PropertyField (position, onHOLDSerialized);

		

						
		
					
		

						EditorGUI.EndProperty ();
				}

		}
}

