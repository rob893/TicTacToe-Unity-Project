using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPosition  {
	/*
	 * This class simply keeps the position of a grid cell in a readable manner. Similiar to a vector2, but without the extra functions and instead of x and y floats, we have row
	 * and column ints.
	 */

	public int row;
	public int column;


	/// <summary>
	/// Create a new GridPosition with passed in row and column values.
	/// </summary>
	/// <param name="row">The row.</param>
	/// <param name="column">The column.</param>
	public GridPosition(int row, int column)
	{
		this.row = row;
		this.column = column;
	}
}
