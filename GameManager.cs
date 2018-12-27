using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	[SerializeField] private Transform gameBoard;
	[SerializeField] private GameObject gridCellPrefab;
	[SerializeField] private int numCellsPerRow = 3;
	[SerializeField] private AudioClip gameOverSound;

	private List<GridCell> gridCells = new List<GridCell>();
	private int[,] boardMatrix;
	private int turnCount = 1;
	private Dictionary<int, PlayerMove> moveHistory = new Dictionary<int, PlayerMove>();
	private string winMessage = "";


	private GameManager() { }

	private void Awake()
	{
		if(Instance == null)
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if(Instance != this)
		{
			Destroy(gameObject);
		}
	}

	public int GetPlayerNumber()
	{
		return ((turnCount + 1) % 2) + 1;
	}

	public void SetNumCellsPerRow(int num)
	{
		numCellsPerRow = num;
	}

	private void CreateGameBoard(int numCellsToWin)
	{
		GridLayoutGroup layout = gameBoard.GetComponent<GridLayoutGroup>();
		layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		layout.constraintCount = numCellsToWin;

		boardMatrix = new int[numCellsToWin, numCellsToWin];

		for(int i = 0; i < boardMatrix.GetLength(0); i++)
		{
			for(int j = 0; j < boardMatrix.GetLength(1); j++)
			{
				Vector2 position = new Vector2(i, j);
				GameObject newCell = Instantiate(gridCellPrefab, gameBoard);
				GridCell newCellGridCell = newCell.GetComponent<GridCell>();
				newCellGridCell.SetPosition(position);
				gridCells.Add(newCellGridCell);
			}
		}
	}

	public void EndTurn(GridCell cell)
	{
		boardMatrix[(int)cell.GetPosition().x, (int)cell.GetPosition().y] = GetPlayerNumber();

		PlayerMove newMove = new PlayerMove(turnCount, GetPlayerNumber(), cell.GetPosition());
		moveHistory.Add(turnCount, newMove);

		if(CheckForWinner())
		{
			GameOver();
		}
		else
		{
			turnCount++;
		}
	}

	private bool CheckForWinner()
	{
		for(int i = 0; i < boardMatrix.GetLength(0); i++) //We can assume that the number of columns is equal to the number of rows. Check all here instead of a loop for both.
		{
			if(CheckWinRow(i))
			{
				winMessage = "Player " + GetPlayerNumber() + " has won!";
				return true;
			}

			if (CheckWinCol(i))
			{
				winMessage = "Player " + GetPlayerNumber() + " has won!";
				return true;
			}
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

	private bool CheckWinRow(int rowNum)
	{
		if(rowNum >= boardMatrix.GetLength(0) || rowNum < 0)
		{
			Debug.Log("Invalid row index!");
			return false;
		}

		for(int i = 0; i < boardMatrix.GetLength(1) - 1; i++)
		{
			if(boardMatrix[rowNum, i] != boardMatrix[rowNum, i + 1] || boardMatrix[rowNum, i] == 0)
			{
				return false;
			}
		}

		return true;
	}

	private bool CheckWinCol(int colNum)
	{
		if (colNum >= boardMatrix.GetLength(1) || colNum < 0)
		{
			Debug.Log("Invalid col index!");
			return false;
		}

		for (int i = 0; i < boardMatrix.GetLength(0) - 1; i++)
		{
			if (boardMatrix[i, colNum] != boardMatrix[i + 1, colNum] || boardMatrix[i, colNum] == 0)
			{
				return false;
			}
		}

		return true;
	}

	private bool CheckWinDiagLeft()
	{
		for(int i = 0; i < boardMatrix.GetLength(0) - 1; i++)
		{
			if (boardMatrix[i, i] != boardMatrix[i + 1, i + 1] || boardMatrix[i, i] == 0)
			{
				return false;
			}
		}

		return true;
	}

	private bool CheckWinDiagRight()
	{
		for (int i = boardMatrix.GetLength(0) - 1, j = 0; i > 0; i--, j++)
		{
			if (boardMatrix[j, i] != boardMatrix[j + 1, i - 1] || boardMatrix[j, i] == 0)
			{
				return false;
			}
		}

		return true;
	}

	private bool CheckForDraw()
	{
		for(int i = 0; i < boardMatrix.GetLength(0); i++)
		{
			for(int j = 0; j < boardMatrix.GetLength(1); j++)
			{
				if(boardMatrix[i, j] == 0)
				{
					return false;
				}
			}
		}

		return true;
	}

	private void GameOver()
	{
		AudioManager.Instance.PlaySoundEffect(gameOverSound, true);

		foreach(GridCell cell in gridCells)
		{
			cell.SetInactive();
		}

		UIManager.Instance.ShowGameOverPanel(winMessage);
	}

	public void ResetGame()
	{
		turnCount = 1;

		foreach(GridCell cell in gridCells)
		{
			Destroy(cell.gameObject);
		}

		gridCells.Clear();
		moveHistory.Clear();
		boardMatrix = null;

		CreateGameBoard(numCellsPerRow);
	}

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

	public void PrintMoveHistory()
	{
		foreach(KeyValuePair<int, PlayerMove> move in moveHistory)
		{
			move.Value.PrintData();
		}
	}
}
