using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : ChessPiece
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

	[SerializeField] GameObject _pawnMesh;
	[SerializeField] GameObject _knightMesh;
	[SerializeField] GameObject _bishopMesh;
	[SerializeField] GameObject _rookMesh;
	[SerializeField] GameObject _queenMesh;
	[SerializeField] GameObject _kingMesh;

	private Vector2 _startTapPosition;
	private Vector2 _tapOffset;

	bool _freeToAct = true;

	int _curPositionNumber = 0;

	[SerializeField] GUIStyle style;

	public GameObject PlayerHitCursorPrefab;

	public List<GameObject> PlayerHitCursorList = new List<GameObject>();

	public override void Start()
    {
		base.Start();

		_screenSizeX = Screen.width;
		_screenCenterX = _screenSizeX / 2;

		_screenCoefficient = (float)_screenSizeX / _targetScreenSizeX;
		_thresholdForShortTap = _thresholdForShortTap * _screenCoefficient;

		for (int i=0; i<HitCursorPrototypes.Count; i++)
		{
			PlayerHitCursorList.Add(Instantiate(PlayerHitCursorPrefab));
		}

		//_screenSizeY = Screen.height;
#if UNITY_EDITOR
		style.fontSize = (int)(style.fontSize / 2.5f);
#endif
	}

	void OnGUI()
	{
		Vector2Int cell = GlobalManagement.GetCellAddressByPosition(transform.position.x, transform.position.z);
		GUI.Label(new Rect(_screenCenterX, 10, 100, 100), "dbg: " + _tapOffset.magnitude.ToString() + " (" + _thresholdForShortTap.ToString("0") + ")" + "\n" +
															"cell: " + cell.x + "," + cell.y, style);
	}

	void Update()
    {
		transform.position += Vector3.forward * Speed * Time.deltaTime;
		PlaceHitCursors();

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

			if (Input.GetKey(KeyCode.Alpha1))
				ChangePieceType(PieceType.Pawn);
			if (Input.GetKey(KeyCode.Alpha2))
				ChangePieceType(PieceType.Knight);
			if (Input.GetKey(KeyCode.Alpha3))
				ChangePieceType(PieceType.Bishop);
			if (Input.GetKey(KeyCode.Alpha4))
				ChangePieceType(PieceType.Rook);
			if (Input.GetKey(KeyCode.Alpha5))
				ChangePieceType(PieceType.Queen);
			if (Input.GetKey(KeyCode.Alpha0))
				ChangePieceType(PieceType.King);
#endif

		}

		// temp teleportation
		if (transform.position.z > 75.2f)
		transform.position = new Vector3(transform.position.x, transform.position.y, 0.2f);

	}

	void PlaceHitCursors()
	{
		Vector2Int playerCellAddress = GlobalManagement.GetCellAddressByPosition(transform.position.x, transform.position.z);
		Vector2Int newHitCursorCoords;

		for (int i = 0; i < PlayerHitCursorList.Count; i++)
		{
			newHitCursorCoords = playerCellAddress + HitCursorPrototypes[i];

			if (Mathf.Abs(newHitCursorCoords.x) <= GlobalManagement.PathRadius)
			{
				PlayerHitCursorList[i].SetActive(true);
				PlayerHitCursorList[i].transform.position = GlobalManagement.GetPositionByCellAddress(newHitCursorCoords);
			}
			else
				PlayerHitCursorList[i].SetActive(false);
		}
	}

	public override void ChangePieceType(PieceType type)
	{
		base.ChangePieceType(type);

		_pawnMesh.SetActive(false);
		_knightMesh.SetActive(false);
		_bishopMesh.SetActive(false);
		_rookMesh.SetActive(false);
		_queenMesh.SetActive(false);
		_kingMesh.SetActive(false);

		switch (type)
		{
			case PieceType.Pawn:
				_pawnMesh.SetActive(true);
				break;
			case PieceType.Knight:
				_knightMesh.SetActive(true);
				break;
			case PieceType.Bishop:
				_bishopMesh.SetActive(true);
				break;
			case PieceType.Rook:
				_rookMesh.SetActive(true);
				break;
			case PieceType.Queen:
				_queenMesh.SetActive(true);
				break;
			case PieceType.King:
				_kingMesh.SetActive(true);
				break;
		}

		foreach (GameObject C in PlayerHitCursorList)
			Destroy(C);
		PlayerHitCursorList.Clear();

		for (int i = 0; i < HitCursorPrototypes.Count; i++)
		{
			PlayerHitCursorList.Add(Instantiate(PlayerHitCursorPrefab));
		}
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
