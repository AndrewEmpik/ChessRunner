using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	int _screenSizeX;
	public float Speed = 10f;
	public float StrafeSpeed = 1f;
	[SerializeField] float _maxXPosition = 1.84f;


	void Start()
    {
		_screenSizeX = Screen.width;
    }

    void Update()
    {
		transform.position += Vector3.forward * Speed * Time.deltaTime;
		if (Input.GetMouseButton(0))
		{
			transform.position += Vector3.right * StrafeSpeed * Time.deltaTime * (Input.mousePosition.x >= _screenSizeX/2 ? 1 : -1);
			transform.position = new Vector3(Mathf.Clamp(transform.position.x, -_maxXPosition, _maxXPosition), transform.position.y, transform.position.z);
		}

		// temp
		if (transform.position.z > 42.7f)
			transform.position = new Vector3(transform.position.x, transform.position.y, -7.3f);

	}
}
