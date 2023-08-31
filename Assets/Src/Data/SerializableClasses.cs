using System;
using UnityEngine;

[Serializable]
public class SerializableVector3
{
	public float x,y,z;
		
	public static implicit operator SerializableVector3(Vector3 value)
	{
		return new SerializableVector3()
		{
			x = value.x, y = value.y, z = value.z
		};
	}
		
	public static implicit operator Vector3(SerializableVector3 value)
	{
		return new Vector3()
		{
			x = value.x, y = value.y, z = value.z
		};
	}
}