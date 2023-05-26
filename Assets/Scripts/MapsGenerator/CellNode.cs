using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellNode : Node
{
	public CellNode(Vector2Int bottomLeftCorner, Vector2Int topRightCorner, Node parent, int index) : base(parent)
	{
		BottomLeftCorner= bottomLeftCorner;
		TopLeftCorner = new Vector2Int(bottomLeftCorner.x, topRightCorner.y);
		BottomRightCorner = new Vector2Int(topRightCorner.x, bottomLeftCorner.y);
		TopRightCorner = topRightCorner;

		LayerIndex=index;
	}

	//Dimensiones de la celda
	public int Width() { return TopRightCorner.x - TopLeftCorner.x; }
	public int Length() { return TopRightCorner.y - BottomRightCorner.y; }
}