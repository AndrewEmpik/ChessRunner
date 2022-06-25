using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

	[SerializeField] GameObject _kingMesh;
	[SerializeField] GameObject _pawnMesh;
	[SerializeField] GameObject _knightMesh;
	[SerializeField] GameObject _bishopMesh;
	[SerializeField] GameObject _rookMesh;
	[SerializeField] GameObject _queenMesh;

	private Vector2 _startTapPosition;
	private Vector2 _tapOffset;

	bool _freeToAct = true;

	int _curPositionNumber = 0;

	[SerializeField] GUIStyle style;

	public GameObject PlayerHitCursorPrefab;

	public List<GameObject> PlayerHitCursorList = new List<GameObject>();

	private PiecesManager _piecesManager;

	public Vector2Int PlayerCellAddress;

	Management _management;

	public override void Start()
    {
		base.Start();

		_piecesManager = FindObjectOfType<PiecesManager>();
		_management = FindObjectOfType<Management>();

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

			if (Input.GetMouseButtonDown(0) && !IsPointerOverGameObject())
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
							Beat();
						}
						else
						{
							SomeActionAtDucking();
						}
					}
				}
			}


#if UNITY_EDITOR
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
				StrafeLeft();
			else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
				StrafeRight();

			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
				Beat();

			if (Input.GetKey(KeyCode.Alpha0))
				ChangePieceType(PieceType.King);
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

#endif

		}

		// temp teleportation
		if (transform.position.z > 75.2f)
		transform.position = new Vector3(transform.position.x, transform.position.y, 0.2f);

	}

	public static bool IsPointerOverGameObject()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return true;

		if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
		{
			if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
				return true;
		}

		return false;
	}

	void PlaceHitCursors()
	{
		PlayerCellAddress = GlobalManagement.GetCellAddressByPosition(transform.position.x, transform.position.z);
		Vector2Int newHitCursorCoords;

		for (int i = 0; i < PlayerHitCursorList.Count; i++)
		{
			newHitCursorCoords = PlayerCellAddress + HitCursorPrototypes[i];

			if (Mathf.Abs(newHitCursorCoords.x) <= GlobalManagement.PathRadius)
			{
				PlayerHitCursorList[i].SetActive(true);
				PlayerHitCursorList[i].transform.position = GlobalManagement.GetPositionByCellAddress(newHitCursorCoords);
			}
			else
				PlayerHitCursorList[i].SetActive(false);
		}
	}

	Enemy FindEnemyUnderCursors()
	{
		for (int i = 0; i < PlayerHitCursorList.Count; i++)
		{
			Vector2 CursorCellAddress = GlobalManagement.GetCellAddressByPosition(PlayerHitCursorList[i].transform.position);
			for (int j = 0; j < _piecesManager.Enemies.Count; j++)
			{
				if (_piecesManager.Enemies[j].transform.position.z < PlayerHitCursorList[i].transform.position.z - GlobalManagement.CellSize/2)
					continue;
				else if (_piecesManager.Enemies[j].transform.position.z > PlayerHitCursorList[i].transform.position.z + GlobalManagement.CellSize/2)
					break;
				else
				{
					Vector2 EnemyCellAddress = GlobalManagement.GetCellAddressByPosition(_piecesManager.Enemies[j].transform.position);
					if (EnemyCellAddress == CursorCellAddress)
						return _piecesManager.Enemies[j];
				}
			}
		}

		return null;
	}


	public override void ChangePieceType(PieceType type)
	{
		base.ChangePieceType(type);

		_kingMesh.SetActive(false);
		_pawnMesh.SetActive(false);
		_knightMesh.SetActive(false);
		_bishopMesh.SetActive(false);
		_rookMesh.SetActive(false);
		_queenMesh.SetActive(false);

		switch (type)
		{
			case PieceType.King:
				_kingMesh.SetActive(true);
				break;
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
		}

		foreach (GameObject C in PlayerHitCursorList)
			Destroy(C);
		PlayerHitCursorList.Clear();

		for (int i = 0; i < HitCursorPrototypes.Count; i++)
		{
			PlayerHitCursorList.Add(Instantiate(PlayerHitCursorPrefab));
		}
	}

	public void UpgradePiece()
	{
		if ((int)CurrentType == 5)
			CurrentType = (PieceType)0;
		else
			CurrentType++;

		ChangePieceType(CurrentType);
	}
	public int DegradePiece()
	{
		int returnValue = 0;
		if ((int)CurrentType == 0)
		{
			CurrentType = (PieceType)5;
			returnValue = -1;
			_management.Lose();
		}
		else
			CurrentType--;

		ChangePieceType(CurrentType);

		return returnValue;
	}

	void StrafeRight()
	{
		//Debug.Log("StrafeRight");
		if (_curPositionNumber >= 2)
		{
			return; // можно в будущем сделать сход с трассы
		}
		StartCoroutine(StrafeCoroutine(_curPositionNumber + 1));
	}

	void StrafeLeft()
	{
		//Debug.Log("StrafeLeft");
		if (_curPositionNumber <= -2)
		{
			return; // можно в будущем сделать сход с трассы
		}
		StartCoroutine(StrafeCoroutine(_curPositionNumber - 1));
	}

	void RushAtPosition(Enemy TargetEnemy)
	{
		StartCoroutine(BeatRushCoroutine(TargetEnemy));
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

	IEnumerator BeatRushCoroutine(Enemy TargetEnemy)
	{
		_freeToAct = false;
		Vector3 startPosition = transform.position;

		for (float t = 0; t < _strafeDuration; t += Time.deltaTime * 1f)
		{
			transform.position = new Vector3(	Mathf.Lerp(startPosition.x, TargetEnemy.transform.position.x, t / _strafeDuration), 
												transform.position.y,
												Mathf.Lerp(startPosition.z, TargetEnemy.transform.position.z, t / _strafeDuration) );
			yield return null;
		}
		transform.position = TargetEnemy.transform.position;
		_curPositionNumber = GlobalManagement.GetCellAddressByPosition(transform.position).x;

		_piecesManager.Enemies.Remove(TargetEnemy);
		Destroy(TargetEnemy.gameObject);

		UpgradePiece();

		_freeToAct = true;
	}

	//IEnumerator StrafeCoroutine(int newPositionNumber)


	float GetXByPositionNumber(int positionNumber)
	{
		positionNumber = Mathf.Clamp(positionNumber, -2, 2);
		return (_maxXPosition/2) * positionNumber;
	}

	void Beat()
	{
		Enemy TargetEnemy = FindEnemyUnderCursors();
		if (TargetEnemy)
		{
			Debug.Log("Beat!");
			RushAtPosition(TargetEnemy);

			//WaitForSeconds(1f);

		}
	}

	void Empty()
	{
	}

	void SomeActionAtDucking() // реализовать что-то, ну или не надо, решай там уж сам
	{
		Debug.Log("Duck (???)");
	}

}
