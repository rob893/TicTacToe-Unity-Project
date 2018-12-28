using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	/*
	 * The GameManager class sets up new games and enforces the game's rules.
	 */

	//Singleton
	public static GameManager Instance;

	public delegate void OnTurnComplete();
	public OnTurnComplete onTurnCompleteCallback;

	[SerializeField] private Transform gameBoard;
	[SerializeField] private GameObject gridCellPrefab; 
	[SerializeField] private AudioClip gameOverSound;

	private int numCellsPerRow = 3;
	private int turnCount = 1;
	private GridCell[,] boardMatrix;
	private Dictionary<int, PlayerMove> moveHistory = new Dictionary<int, PlayerMove>();
	private string winMessage = "";


	/// <summary>
	/// Private constructor to enforce singleton.
	/// </summary>
	private GameManager() { }

	/// <summary>
	/// Enforce the singleton.
	/// </summary>
	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		else if(Instance != this)
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Get the player who is currently making a move.
	/// </summary>
	/// <returns>The player number. Either 1 or 2.</returns>
	public int GetPlayerNumber()
	{
		return ((turnCount + 1) % 2) + 1;
	}

	/// <summary>
	/// Set the number of cells per row. Only useful right before creating a new game board.
	/// </summary>
	/// <param name="num">The number of cells per row (and also per column).</param>
	public void SetNumCellsPerRow(int num)
	{
		numCellsPerRow = num;
	}

	/// <summary>
	/// Creates a new game board.
	/// </summary>
	/// <param name="numCellsToWin">The number of cells per row (and also per column). Example: 3 will create a 3x3 board. 10 will create a 10x10 board.</param>
	private void CreateGameBoard(int numCellsToWin)
	{
		GridLayoutGroup layout = gameBoard.GetComponent<GridLayoutGroup>();
		layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		layout.constraintCount = numCellsToWin;

		boardMatrix = new GridCell[numCellsToWin, numCellsToWin];

		for(int i = 0; i < boardMatrix.GetLength(0); i++)
		{
			for(int j = 0; j < boardMatrix.GetLength(1); j++)
			{
				GridPosition position = new GridPosition(i, j);
				GameObject newCell = Instantiate(gridCellPrefab, gameBoard);
				GridCell newCellGridCell = newCell.GetComponent<GridCell>();
				newCellGridCell.SetPosition(position);

				boardMatrix[i, j] = newCellGridCell;
			}
		}
	}

	/// <summary>
	/// Ends the turn.
	/// </summary>
	/// <param name="cell">The GridCell that was just played.</param>
	public void EndTurn(GridCell cell)
	{
		PlayerMove newMove = new PlayerMove(turnCount, GetPlayerNumber(), cell.GetPosition());
		moveHistory.Add(turnCount, newMove);

		if(CheckForWinner(cell))
		{
			GameOver();
		}
		else
		{
			turnCount++;

			if(onTurnCompleteCallback != null)
			{
				onTurnCompleteCallback.Invoke();
			}
		}
	}

	/// <summary>
	/// Check the win/draw conditions of the game.
	/// </summary>
	/// <param name="cell">The GridCell that was just played.</param>
	/// <returns>True if the game is won or if it is a draw, false if otherwise.</returns>
	private bool CheckForWinner(GridCell cell)
	{
		if(turnCount < (numCellsPerRow * 2) - 1) //It is impossible to win if there has not been enough turns. No point in wasting time checking.
		{
			return false;
		}

		if(CheckWinRow(cell.GetPosition().row))
		{
			winMessage = "Player " + GetPlayerNumber() + " has won!";
			return true;
		}

		if (CheckWinCol(cell.GetPosition().column))
		{
			winMessage = "Player " + GetPlayerNumber() + " has won!";
			return true;
		}

		if(CheckWinDiagLeft())
		{
			winMessage = "Player " + GetPlayerNumber() + " has won!";
			return true;
		}

		if (CheckWinDiagRight())
		{
			winMessage = "Player " + GetPlayerNumber() + " has won!";
			return true;
		}

		if(CheckForDraw())
		{
			winMessage = "The game is a draw!";
			return true;
		}

		return false;
	}

	/// <summary>
	/// Check if the passed in row number is a win.
	/// </summary>
	/// <param name="rowNum">The row number to check for a win.</param>
	/// <returns>True if there is a win, false otherwise.</returns>
	private bool CheckWinRow(int rowNum)
	{
		if(rowNum >= boardMatrix.GetLength(0) || rowNum < 0)
		{
			Debug.Log("Invalid row index!");
			return false;
		}

		int playerNumber = GetPlayerNumber();

		for(int i = 0; i < boardMatrix.GetLength(0); i++)
		{
			if(boardMatrix[rowNum, i].GetPlayerWhoUsed() != playerNumber)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Check if the passed in column number is a win.
	/// </summary>
	/// <param name="colNum">The column number to check for a win.</param>
	/// <returns>True if there is a win, false otherwise.</returns>
	private bool CheckWinCol(int colNum)
	{
		if (colNum >= boardMatrix.GetLength(1) || colNum < 0)
		{
			Debug.Log("Invalid col index!");
			return false;
		}

		int playerNumber = GetPlayerNumber();

		for (int i = 0; i < boardMatrix.GetLength(1); i++)
		{
			if (boardMatrix[i, colNum].GetPlayerWhoUsed() != playerNumber)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Check if the left diagnal (top left to bottom right) is a win.
	/// </summary>
	/// <returns>True if there is a win, false otherwise.</returns>
	private bool CheckWinDiagLeft()
	{
		int playerNumber = GetPlayerNumber();

		for(int i = 0; i < boardMatrix.GetLength(0); i++)
		{
			if (boardMatrix[i, i].GetPlayerWhoUsed() != playerNumber)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Check if the right diagnal (top right to bottom left) is a win.
	/// </summary>
	/// <returns>True if there is a win, false otherwise.</returns>
	private bool CheckWinDiagRight()
	{
		int playerNumber = GetPlayerNumber();

		for (int i = boardMatrix.GetLength(0) - 1, j = 0; i >= 0; i--, j++)
		{
			if (boardMatrix[j, i].GetPlayerWhoUsed() != playerNumber)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Check if the game is a draw.
	/// </summary>
	/// <returns>True if there is a draw, false otherwise.</returns>
	private bool CheckForDraw()
	{
		//A draw is checked after every win condition. Instead of going through the entire matrix to see if there are any moves left, we can simply check the turn count against the number of possible moves.
		if (turnCount < numCellsPerRow * numCellsPerRow)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// End the game.
	/// </summary>
	private void GameOver()
	{
		AudioManager.Instance.PlaySoundEffect(gameOverSound, true);

		foreach(GridCell cell in boardMatrix)
		{
			cell.SetInactive();
		}

		UIManager.Instance.ShowGameOverPanel(winMessage);
	}

	/// <summary>
	/// Reset the game to their initial values and build a new gameboard.
	/// </summary>
	public void ResetGame()
	{
		turnCount = 1;

		if(boardMatrix != null)
		{
			foreach (GridCell cell in boardMatrix)
			{
				Destroy(cell.gameObject);
			}
		}

		moveHistory.Clear();
		boardMatrix = null;

		if(onTurnCompleteCallback != null)
		{
			onTurnCompleteCallback.Invoke();
		}

		CreateGameBoard(numCellsPerRow);
	}

	/// <summary>
	/// Print all of the values in the gameboard matrix to the console.
	/// </summary>
	private void PrintGameMatrix()
	{
		if(boardMatrix == null)
		{
			return;
		}

		for (int i = 0; i < boardMatrix.GetLength(0); i++)
		{
			for (int j = 0; j < boardMatrix.GetLength(1); j++)
			{
				Debug.Log(boardMatrix[i, j]);
			}
		}
	}

	/// <summary>
	/// Print the move history of the current game to the console.
	/// </summary>
	public void PrintMoveHistory()
	{
		foreach(KeyValuePair<int, PlayerMove> move in moveHistory)
		{
			move.Value.PrintData();
		}
	}
}
