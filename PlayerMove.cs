using System;
using UnityEngine;

public class PlayerMove {

	private int turnNumber;
	private int playerNumber;
	private GridPosition gridCellChosen;
	private DateTime timeStamp;


	/// <summary>
	/// Constructor. Creates a new PlayerMove and sets its values.
	/// </summary>
	/// <param name="turnNumber">The turn the move took place.</param>
	/// <param name="playerNumber">The player who made the move (1 or 2)</param>
	/// <param name="gridCellChosen">The GridPosition of the chosen cell.</param>
	public PlayerMove(int turnNumber, int playerNumber, GridPosition gridCellChosen)
	{
		this.turnNumber = turnNumber;
		this.playerNumber = playerNumber;
		this.gridCellChosen = gridCellChosen;
		timeStamp = DateTime.Now;
	}

	/// <summary>
	/// Prints the data of a move in a readable format.
	/// </summary>
	public void PrintData()
	{
		Debug.Log("For turn " + turnNumber + ", player " + playerNumber + " chose the grid located at (" + gridCellChosen.row
			+ ", " + gridCellChosen.column + "). The move took place at " + timeStamp);
	}
}
