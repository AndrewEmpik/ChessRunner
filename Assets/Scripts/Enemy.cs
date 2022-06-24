using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : ChessPiece
{

	public GameObject EnemyHitCursorPrefab;
	public List<GameObject> EnemyHitCursorList = new List<GameObject>();
	PlayerMove _player;
	Vector2Int _unitCellAddress;

	private bool _playerDetected = false;
	private Vector2Int _cellPlayerDetectedAt;
	private Vector2Int _targetCellToAttack;

	public override void Start()
    {
		base.Start();

		for (int i = 0; i < HitCursorPrototypes.Count; i++)
		{
			EnemyHitCursorList.Add(Instantiate(EnemyHitCursorPrefab));
		}

		PlaceHitCursors();

		_player = FindObjectOfType<PlayerMove>(); // bad practice?
	}

	private void Update()
	{
		if (!_playerDetected)
		{
			if (CheckPlayerUnderCursor())
			{
				_playerDetected = true;
				_cellPlayerDetectedAt = _player.PlayerCellAddress;
				_targetCellToAttack = _cellPlayerDetectedAt + Vector2Int.up;
			}
		}
		else // следим за игроком
		{
			if (_player.PlayerCellAddress != _cellPlayerDetectedAt)
			{
				if (_player.PlayerCellAddress == _targetCellToAttack)
					RushAtPlayer();
				else // эх, улизнул
					_playerDetected = false;
			}
		}
	}

	void PlaceHitCursors()
	{
		_unitCellAddress = GlobalManagement.GetCellAddressByPosition(transform.position.x, transform.position.z);
		Vector2Int newHitCursorCoords;

		for (int i = 0; i < EnemyHitCursorList.Count; i++)
		{
			newHitCursorCoords = _unitCellAddress + HitCursorPrototypes[i] * new Vector2Int(1,-1); // * -1 по y

			//Debug.Log(newHitCursorCoords.x + ", " + newHitCursorCoords.y + " (" + GlobalManagement.PathRadius + ")");

			if (Mathf.Abs(newHitCursorCoords.x) <= GlobalManagement.PathRadius)
			{
				EnemyHitCursorList[i].SetActive(true);
				EnemyHitCursorList[i].transform.position = GlobalManagement.GetPositionByCellAddress(newHitCursorCoords);
			}
			else
				EnemyHitCursorList[i].SetActive(false);
		}
	}

	bool CheckPlayerUnderCursor()
	{
		if (_player.PlayerCellAddress.y < _unitCellAddress.y - 2
			|| _player.PlayerCellAddress.y > _unitCellAddress.y + 2)
		{
			return false;
		}
		else if (_unitCellAddress.x != 0
			&& (_player.PlayerCellAddress.x < _unitCellAddress.x - 2
				|| _player.PlayerCellAddress.x > _unitCellAddress.x + 2))
		{
			return false;
		}
		else
		{
			for (int i = 0; i < EnemyHitCursorList.Count; i++)
			{
				// TODO запоминать CellAddress ещё раньше, чтобы не пересчитывать каждый раз
				if (GlobalManagement.GetCellAddressByPosition(	EnemyHitCursorList[i].transform.position.x, 
																EnemyHitCursorList[i].transform.position.z	) == _player.PlayerCellAddress)
					return true;
			}
		}
		return false;
	}

	void RushAtPlayer()
	{
		StartCoroutine(BeatRushCoroutine());
	}

	IEnumerator BeatRushCoroutine()
	{
		Vector3 startPosition = transform.position;

		float strafeDuration = 0.12f;

		// тут TODO strafeDuration прописать в переменную
		for (float t = 0; t < strafeDuration; t += Time.deltaTime * 1f)
		{
			transform.position = new Vector3(Mathf.Lerp(startPosition.x, _player.transform.position.x, t / strafeDuration),
												transform.position.y,
												Mathf.Lerp(startPosition.z, _player.transform.position.z, t / strafeDuration));
			yield return null;
		}
		transform.position = _player.transform.position;

		//_piecesManager.Enemies.Remove(TargetEnemy);
		//Destroy(TargetEnemy.gameObject);

		//UpgradePiece();

		//_freeToAct = true;
	}

	void OnDestroy()
    {
		foreach (GameObject C in EnemyHitCursorList)
			Destroy(C);
		EnemyHitCursorList.Clear();
    }
}
