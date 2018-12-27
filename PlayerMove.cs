using System;
using UnityEngine;

public class PlayerMove {

	private int turnNumber;
	private int playerNumber;
	private Vector2 gridCellChosen;
	private DateTime timeStamp;


	public PlayerMove(int turnNumber, int playerNumber, Vector2 gridCellChosen)
	{
		this.turnNumber = turnNumber;
		this.playerNumber = playerNumber;
		this.gridCellChosen = gridCellChosen;
		timeStamp = DateTime.Now;
	}

	public void PrintData()
	{
		Debug.Log("For turn " + turnNumber + ", player " + playerNumber + " chose the grid located at " + gridCellChosen + ". The move took place at " + timeStamp);
	}
}
