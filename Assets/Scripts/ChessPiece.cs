using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
	public enum PieceType
	{
		Pawn,
		Knight,
		Bishop,
		Rook,
		Queen,
		King
	}

	public PieceType CurrentType;

	public List<Vector2Int> HitCursorPrototypes = new List<Vector2Int>();

	public virtual void Start()
	{
		InitHitCursorsList();
	}

	public void InitHitCursorsList()
	{
		if (HitCursorPrototypes.Count > 0)
			HitCursorPrototypes.Clear();

		switch (CurrentType)
		{
			case PieceType.Pawn:
				HitCursorPrototypes.Add(new Vector2Int(-1, 1));
				HitCursorPrototypes.Add(new Vector2Int(1, 1));
				break;

			case PieceType.Knight:
				HitCursorPrototypes.Add(new Vector2Int(-2, 1));
				HitCursorPrototypes.Add(new Vector2Int(-1, 2));
				HitCursorPrototypes.Add(new Vector2Int(1, 2));
				HitCursorPrototypes.Add(new Vector2Int(2, 1));
				break;

			case PieceType.Bishop:
				HitCursorPrototypes.Add(new Vector2Int(-2, 2));
				HitCursorPrototypes.Add(new Vector2Int(-1, 1));
				HitCursorPrototypes.Add(new Vector2Int(1, 1));
				HitCursorPrototypes.Add(new Vector2Int(2, 2));
				break;

			case PieceType.Rook:
				HitCursorPrototypes.Add(new Vector2Int(-2, 0));
				HitCursorPrototypes.Add(new Vector2Int(-1, 0));
				HitCursorPrototypes.Add(new Vector2Int(0, 2));
				HitCursorPrototypes.Add(new Vector2Int(0, 1));
				HitCursorPrototypes.Add(new Vector2Int(1, 0));
				HitCursorPrototypes.Add(new Vector2Int(2, 0));
				break;

			case PieceType.Queen:
				HitCursorPrototypes.Add(new Vector2Int(-2, 0));
				HitCursorPrototypes.Add(new Vector2Int(-1, 0));
				HitCursorPrototypes.Add(new Vector2Int(0, 2));
				HitCursorPrototypes.Add(new Vector2Int(0, 1));
				HitCursorPrototypes.Add(new Vector2Int(1, 0));
				HitCursorPrototypes.Add(new Vector2Int(2, 0));
				HitCursorPrototypes.Add(new Vector2Int(-2, 2));
				HitCursorPrototypes.Add(new Vector2Int(-1, 1));
				HitCursorPrototypes.Add(new Vector2Int(1, 1));
				HitCursorPrototypes.Add(new Vector2Int(2, 2));
				break;

			case PieceType.King:
				HitCursorPrototypes.Add(new Vector2Int(-1, 0));
				HitCursorPrototypes.Add(new Vector2Int(-1, 1));
				HitCursorPrototypes.Add(new Vector2Int(0, 1));
				HitCursorPrototypes.Add(new Vector2Int(1, 1));
				HitCursorPrototypes.Add(new Vector2Int(1, 0));
				break;
		}
	}

	public virtual void ChangePieceType(PieceType type)
	{
		CurrentType = type;
		InitHitCursorsList();
	}
}
