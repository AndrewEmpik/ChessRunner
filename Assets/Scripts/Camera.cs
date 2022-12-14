using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
	[SerializeField] Transform _player;
	float _offsetZ;

	private void Start()
	{
		_offsetZ = transform.position.z - _player.position.z ;
	}

	void LateUpdate()
    {
		transform.position = new Vector3(_player.position.x/2, transform.position.y, _player.position.z + _offsetZ);
    }
}
