using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	int _screenSizeX;
	int _screenCenterX;
	const int _targetScreenSizeX = 1080; // константа для пропорций
	float _screenCoefficient;
	//int _screenSizeY;
	public float Speed = 10f;
	//public float StrafeSpeed = 1f;
	[SerializeField] float _maxXPosition = 2f;
	[SerializeField] float _strafeDuration = 0.25f;
	[SerializeField] float _thresholdForShortTap = 70f;

	private Management _management;

	private Vector2 _startTapPosition;
	private Vector2 _tapOffset;

	bool _freeToAct = true;

	int _curPositionNumber = 0;

	[SerializeField] GUIStyle style;

	void Start()
    {
		_screenSizeX = Screen.width;
		_screenCenterX = _screenSizeX / 2;

		_screenCoefficient = (float)_screenSizeX / _targetScreenSizeX;
		_thresholdForShortTap = _thresholdForShortTap * _screenCoefficient;

		_management = FindObjectOfType<Management>();

		//_screenSizeY = Screen.height;
#if UNITY_EDITOR
		style.fontSize = (int)(style.fontSize / 2.5f);
#endif
	}

	void OnGUI()
	{
		Vector2Int cell = _management.GetCellAddressByPosition(transform.position.x, transform.position.z);
		GUI.Label(new Rect(_screenCenterX, 10, 100, 100), "dbg: " + _tapOffset.magnitude.ToString() + " (" + _thresholdForShortTap.ToString("0") + ")" + "\n" +
															"cell: " + cell.x + "," + cell.y, style);
	}

	void Update()
    {
		transform.position += Vector3.forward * Speed * Time.deltaTime;

		if (_freeToAct)
		{

			if (Input.GetMouseButtonDown(0))
			{
				_startTapPosition = Input.mousePosition;
			}

			if (Input.GetMouseButtonUp(0))
			{
				_tapOffset = (Vector2)Input.mousePosition - _startTapPosition;



				// если короткое нажатие
				if (_tapOffset.magnitude <= _thresholdForShortTap)
				{
					if (Input.mousePosition.x >= _screenCenterX)
						StrafeRight();
					else
						StrafeLeft();
				}
				else // если свайп
				{
					if (Mathf.Abs(_tapOffset.x) >= Mathf.Abs(_tapOffset.y))
					{
						if (_tapOffset.x > 0)
							StrafeRight();
						else
							StrafeLeft();
					}
					else
					{
						if (_tapOffset.y > 0)
						{
							Jump();
						}
						else
						{
							Duck();
						}
					}
				}
			}


#if UNITY_EDITOR
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
				StrafeLeft();
			else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
				StrafeRight();
#endif

		}

		// temp teleportation
		if (transform.position.z > 50.2f)
		transform.position = new Vector3(transform.position.x, transform.position.y, 0.2f);

	}

	void StrafeRight()
	{
		Debug.Log("StrafeRight");
		if (_curPositionNumber >= 2)
		{
			return; // можно в будущем сделать сход с трассы
		}
		StartCoroutine(StrafeCoroutine(_curPositionNumber + 1));
	}

	void StrafeLeft()
	{
		Debug.Log("StrafeLeft");
		if (_curPositionNumber <= -2)
		{
			return; // можно в будущем сделать сход с трассы
		}
		StartCoroutine(StrafeCoroutine(_curPositionNumber - 1));
	}

	IEnumerator StrafeCoroutine(int newPositionNumber)
	{
		_freeToAct = false;
		float startPosition = transform.position.x;
		float endPosition = GetXByPositionNumber(newPositionNumber);

		for (float t = 0; t < _strafeDuration; t += Time.deltaTime * 1f)
		{
			transform.position = new Vector3(Mathf.Lerp(startPosition, endPosition, t/ _strafeDuration), transform.position.y, transform.position.z);
			yield return null;
		}
		transform.position = new Vector3(endPosition, transform.position.y, transform.position.z);
		_curPositionNumber = newPositionNumber;
		_freeToAct = true;
	}

	float GetXByPositionNumber(int positionNumber)
	{
		positionNumber = Mathf.Clamp(positionNumber, -2, 2);
		return (_maxXPosition/2) * positionNumber;
	}

	void Jump()
	{

		Debug.Log("Jump");
	}
	void Duck()
	{

		Debug.Log("Duck");
	}

}
