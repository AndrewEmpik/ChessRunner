using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalManagement
{
	public static Vector2 WorldZeroPoint = Vector2.zero;
	public static int PathWidthInCells = 5; // we don't change this for now
	public static int PathRadius = (PathWidthInCells - 1) / 2; // we assume that PathWidthInCells is odd
	public static float CellSize = 1f;

	public static bool FirstLoad = true;
	public static bool MusicEnabled = true;
	public static float Volume = 1.0f;

	public static Vector2Int GetCellAddressByPosition(float x, float y)
	{
		float xRelative = x - WorldZeroPoint.x;
		int _x = (int)((Mathf.Abs(xRelative) + CellSize / 2) / CellSize * Mathf.Sign(xRelative));
		int _y = (int)((y-WorldZeroPoint.y	+ CellSize / 2) / CellSize);
		return new Vector2Int(_x,_y);
	}

	public static Vector2Int GetCellAddressByPosition(Vector3 point)
	{
		return GetCellAddressByPosition(point.x, point.z);
	}
	public static Vector2Int GetCellAddressByPosition(Vector2 point)
	{
		return GetCellAddressByPosition(point.x, point.y);
	}

	public static Vector3 GetPositionByCellAddress(Vector2Int cellAddress)
	{
		return GetPositionByCellAddress(cellAddress.x, cellAddress.y);
	}

	public static Vector3 GetPositionByCellAddress(int x, int y)
	{
		Vector2 flatPosition = new Vector2(x, y) * CellSize + WorldZeroPoint;
		return new Vector3(flatPosition.x, 0, flatPosition.y);
	}
/*
	public static void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit(); 
#endif
	}
	*/
}
