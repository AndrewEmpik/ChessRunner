using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerMove : ChessPiece
{
	int _screenSizeX;
	int _screenCenterX;
	const int _targetScreenSizeX = 1080; // the constant for proportions
	float _screenCoefficient;
	//int _screenSizeY;
	public float Speed = 10f;
	//public float StrafeSpeed = 1f;
	[SerializeField] float _maxXPosition = 2f;
	[SerializeField] float _strafeDuration = 0.25f;
	[SerializeField] float _thresholdForShortTap = 70f;
	[SerializeField] float _offsetForRewind = 80f;

	[SerializeField] GameObject _kingMesh;
	[SerializeField] GameObject _pawnMesh;
	[SerializeField] GameObject _knightMesh;
	[SerializeField] GameObject _bishopMesh;
	[SerializeField] GameObject _rookMesh;
	[SerializeField] GameObject _queenMesh;

	private Vector2 _startTapPosition;
	private Vector2 _tapOffset;

	public bool FreeToAct = true;

	int _curPositionNumber = 0;

	[SerializeField] GUIStyle style;

	public GameObject PlayerHitCursorPrefab;

	public List<GameObject> PlayerHitCursorList = new List<GameObject>();

	private PiecesManager _piecesManager;

	public Vector2Int PlayerCellAddress;

	Management _management;

	public UnityEvent OnPlayerRush;
	public UnityEvent OnCrash;

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
		// this used to be needed for debug
		//Vector2Int cell = GlobalManagement.GetCellAddressByPosition(transform.position.x, transform.position.z);
		//GUI.Label(new Rect(_screenCenterX, 10, 100, 100), "dbg: " + _tapOffset.magnitude.ToString() + " (" + _thresholdForShortTap.ToString("0") + ")" + "\n" +
		//													"cell: " + cell.x + "," + cell.y, style);
	}

	void Update()
    {
		transform.position += Vector3.forward * Speed * Time.deltaTime;

		PlayerCellAddress = GlobalManagement.GetCellAddressByPosition(transform.position.x, transform.position.z);
		// TODO optimize it by adding the event of cell changing
		PlaceHitCursors();

		if (FreeToAct)
		{
			Enemy enemyInThisCell = CheckIfInEnemyCell();
			if (enemyInThisCell)
			{
				OnCrash.Invoke();
				if (DegradePiece() == 0)
				{
					_piecesManager.Enemies.Remove(enemyInThisCell);
					Destroy(enemyInThisCell.gameObject);
				}
				else
					this.gameObject.SetActive(false);
			}

			if (Input.GetMouseButtonDown(0) && !IsPointerOverGameObject())
			{
				_startTapPosition = Input.mousePosition;
			}

			if (Input.GetMouseButtonUp(0))
			{
				_tapOffset = (Vector2)Input.mousePosition - _startTapPosition;

				// if the short tap
				if (_tapOffset.magnitude <= _thresholdForShortTap)
				{
					if (Input.mousePosition.x >= _screenCenterX)
						StrafeRight();
					else
						StrafeLeft();
				}
				else // if swipe
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
		if (transform.position.z > _offsetForRewind)
			//transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
			_management.Win();

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

	Enemy CheckIfInEnemyCell()
	{
		for (int j = 0; j < _piecesManager.Enemies.Count; j++)
		{
			if (_piecesManager.Enemies[j].transform.position.z < transform.position.z - GlobalManagement.CellSize / 2)
				continue;
			else if (_piecesManager.Enemies[j].transform.position.z > transform.position.z + GlobalManagement.CellSize / 2)
				break;
			else
			{
				Vector2 EnemyCellAddress = GlobalManagement.GetCellAddressByPosition(_piecesManager.Enemies[j].transform.position);
				//Debug.Log("Enemy:  " + EnemyCellAddress.x + ", " + EnemyCellAddress.y);
				//Debug.Log("Player: " + PlayerCellAddress.x + ", " + PlayerCellAddress.y);
				if (EnemyCellAddress == PlayerCellAddress && !_piecesManager.Enemies[j].IsInRush)
					return _piecesManager.Enemies[j];
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
		{ /* do nothing, because we do not jump higher than the queen is */ } // CurrentType = (PieceType)0;
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
		if (_curPositionNumber >= 2)
		{
			return; // we may add an off-track exit in the future
		}
		StartCoroutine(StrafeCoroutine(_curPositionNumber + 1));
	}

	void StrafeLeft()
	{
		if (_curPositionNumber <= -2)
		{
			return; // we may add an off-track exit in the future
		}
		StartCoroutine(StrafeCoroutine(_curPositionNumber - 1));
	}

	void RushAtPosition(Enemy TargetEnemy)
	{
		OnPlayerRush.Invoke();
		StartCoroutine(BeatRushCoroutine(TargetEnemy));
	}

	IEnumerator StrafeCoroutine(int newPositionNumber)
	{
		FreeToAct = false;
		float startPosition = transform.position.x;
		float endPosition = GetXByPositionNumber(newPositionNumber);

		for (float t = 0; t < _strafeDuration; t += Time.deltaTime * 1f)
		{
			transform.position = new Vector3(Mathf.Lerp(startPosition, endPosition, t/ _strafeDuration), transform.position.y, transform.position.z);
			yield return null;
		}
		transform.position = new Vector3(endPosition, transform.position.y, transform.position.z);
		_curPositionNumber = newPositionNumber;
		FreeToAct = true;
	}

	IEnumerator BeatRushCoroutine(Enemy TargetEnemy)
	{
		FreeToAct = false;
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

		_management.AddScore(TargetEnemy.PieceCost);

		FreeToAct = true;
	}

	float GetXByPositionNumber(int positionNumber)
	{
		positionNumber = Mathf.Clamp(positionNumber, -2, 2);
		return (_maxXPosition/2) * positionNumber;
	}

	void Beat()
	{
		Enemy TargetEnemy = FindEnemyUnderCursors();
		if (TargetEnemy && !TargetEnemy.IsInRush)
			RushAtPosition(TargetEnemy);
	}

}
