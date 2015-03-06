using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using ws.winx.unity.attributes;

namespace ws.winx.unity.drawers
{
		[CustomPropertyDrawer(typeof(EnumAttribute))]
		public class EnumPropertyDrawer : PropertyDrawer
		{

				Enum _selected;

				public new EnumAttribute attribute{ get { return (EnumAttribute)base.attribute; } }

				public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
				{
						if (_selected == null) {
			
								if (Enum.IsDefined (attribute.GetEnumType (), property.intValue)) {
										_selected = (Enum)Enum.ToObject (this.attribute.GetEnumType (), property.intValue);
								} else
										_selected = this.attribute.GetEnumValue ();


						}
			
						EditorGUI.BeginProperty (position, label, property);
						_selected = EditorGUI.EnumPopup (position, _selected);
						property.intValue = (int)Convert.ChangeType (_selected, _selected.GetTypeCode ());
						property.serializedObject.ApplyModifiedProperties ();
		

						EditorGUI.EndProperty ();
				}

		}
}

