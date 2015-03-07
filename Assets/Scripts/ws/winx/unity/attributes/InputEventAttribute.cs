using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

namespace ws.winx.unity.attributes
{
	public class InputEventAttribute : PropertyAttribute
	{
		Type _type;
		
		public Type GetEnumType(){
			
			return _type;
		}
		
		public Enum GetEnumValue(){
			
			return Enum.GetValues (_type).GetValue(0) as Enum;
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.unity.attributes.InputEventAttribute"/> class.
		/// </summary>
		/// <param name="enumTypeName">Enum type name.Enum containing Input States constants Wave,Forward,Backward...</param>
		public InputEventAttribute(string enumTypeName){
			_type = Type.GetType (enumTypeName);
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ws.winx.unity.attributes.InputEventAttribute"/> class.
		/// </summary>
		/// <param name="enumType">Enum type.Enum containing Input States constants Wave,Forward,Backward...</param>
		public InputEventAttribute(Type enumType){
			_type = enumType;
			
		}
			
	}
}

