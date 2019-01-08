using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour {

	[Header("Testing Tool")]

	[Header("Set Expected Winner (Does nothing if \"Draw\" is checked)")]
	[SerializeField]
	private Player playerToWin = Player.PlayerOne;

	[Header("Test Draw")]
	[SerializeField]
	private bool draw = true;

	[Header("Test Row Win")]
	[SerializeField]
	private bool rowWin;
	[Tooltip("Must be between 0 (inclusive) and board size (inclusive).")]
	[SerializeField]
	private int rowWinNum;

	[Header("Test Column Win")]
	[SerializeField]
	private bool colWin;
	[Tooltip("Must be between 1 (inclusive) and board size (inclusive).")]
	[SerializeField]
	private int colWinNum;

	[Header("Test Diagonal Win")]
	[SerializeField]
	private bool leftDiagWin;
	[SerializeField]
	private bool rightDiagWin;

	[Header("Set The Board Size")]
	[Tooltip("Must be greater than 2. The created game board will have this many rows and columns.")]
	[SerializeField]
	private int boardSize = 3;

	[Header("Set Time Between Moves")]
	[Tooltip("Must be greater than 0.")]
	[SerializeField]
	private float moveSpeed = 0.25f;

	private GridCell[,] boardMatrix;
	private enum Player { PlayerOne = 1, PlayerTwo = 2 }


	private void Start()
	{
		if (!((rowWin || colWin || leftDiagWin) ^ (rightDiagWin || draw)))
		{
			if (!rowWin && !colWin && !leftDiagWin && !rightDiagWin && !draw)
			{
				Debug.LogError("You must select a test!");
			}
			else
			{
				Debug.LogError("You can only test one type of win or draw!\nPlease ensure only one test is checked.");
			}

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
			return;
		}

		if (boardSize <= 2)
		{
			Debug.LogError("The board size must be greater than 2!");

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
			return;
		}

		if(boardSize > 4)
		{
			Debug.LogWarning("The selected board size is greater than the required use cases (3x3 and 4x4)!\n" +
				"The board will look strange in the game view. However, the game is still fully functional and testable with very large boards.");
		}

		if (moveSpeed <= 0f)
		{
			Debug.LogError("Move speed must be greater than 0!");

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
			return;
		}

		if (colWin && (colWinNum <= 0 || colWinNum > boardSize))
		{
			Debug.LogError("Column win number must be between 1 (inclusive) and " + boardSize + " (inclusive)!");

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
			return;
		}

		if (rowWin && (rowWinNum <= 0 || rowWinNum > boardSize))
		{
			Debug.LogError("Row win number must be between 1 (inclusive) and " + boardSize + " (inclusive)!");

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
			return;
		}

		colWinNum--;
		rowWinNum--;
		
		UIManager.Instance.ChooseGameType(boardSize);
		UIManager.Instance.HideChooseIconPanel();
		boardMatrix = GameManager.Instance.GetBoardMatrix();

		StartCoroutine(PlayGame());
	}

	private IEnumerator PlayGame()
	{
		yield return new WaitForSeconds(0.25f);

		while (!GameManager.Instance.GetIsGameOver())
		{
			if (colWin)
			{
				MakeColMove(colWinNum, (int)playerToWin);
			}
			else if(rowWin)
			{
				MakeRowMove(rowWinNum, (int)playerToWin);
			}
			else if(rightDiagWin)
			{
				MakeRightDiagMove((int)playerToWin);
			}
			else if(leftDiagWin)
			{
				MakeLeftDiagMove((int)playerToWin);
			}
			else if(draw)
			{
				MakeDrawMove();
			}
			else
			{
				Debug.LogError("Something went wrong.");
				break;
			}

			yield return new WaitForSeconds(moveSpeed);
		}

		if((int)playerToWin == GameManager.Instance.GetWinner() || (draw && GameManager.Instance.GetWinner() == 0))
		{
			if(draw)
			{
				Debug.Log("Test passed! The game is a draw as expected!");
			}
			else
			{
				Debug.Log("Test passed! " + playerToWin + " has won as expected!");
			}
		}
		else
		{
			Debug.LogError("Test Failed!");
		}

		yield return new WaitForSeconds(1);

		Debug.Log("Test complete. Exiting...");

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}

	private void MakeRowMove(int row, int playerToWin)
	{
		for(int i = 0; i < boardMatrix.GetLength(0); i++)
		{
			if(boardMatrix[row, i].GetPlayerWhoUsed() != 0)
			{
				continue;
			}

			if(GameManager.Instance.GetPlayerNumber() == playerToWin)
			{
				boardMatrix[row, i].Choose();
				return;
			}
			else
			{
				int j = GameManager.Instance.GetTurnCount() % boardMatrix.GetLength(0);

				j = j == row ? (j + 1) % boardMatrix.GetLength(0) : j;

				boardMatrix[j, i].Choose();
				return;
			}
		}
	}

	private void MakeColMove(int col, int playerToWin)
	{
		for (int i = 0; i < boardMatrix.GetLength(1); i++)
		{
			if (boardMatrix[i, col].GetPlayerWhoUsed() != 0)
			{
				continue;
			}

			if (GameManager.Instance.GetPlayerNumber() == playerToWin)
			{
				boardMatrix[i, col].Choose();
				return;
			}
			else
			{
				int j = GameManager.Instance.GetTurnCount() % boardMatrix.GetLength(1);

				j = j == col ? (j + 1) % boardMatrix.GetLength(1) : j;

				boardMatrix[i, j].Choose();
				return;
			}
		}
	}

	private void MakeLeftDiagMove(int playerToWin)
	{
		for (int i = 0; i < boardMatrix.GetLength(0); i++)
		{
			if(boardMatrix[i, i].GetPlayerWhoUsed() != 0)
			{
				continue;
			}

			if (GameManager.Instance.GetPlayerNumber() == playerToWin)
			{
				boardMatrix[i, i].Choose();
				return;
			}
			else
			{
				int j = GameManager.Instance.GetTurnCount() % boardMatrix.GetLength(1);

				j = j == i ? (j + 1) % boardMatrix.GetLength(1) : j;

				boardMatrix[i, j].Choose();
				return;
			}
		}
	}

	private void MakeRightDiagMove(int playerToWin)
	{
		for (int i = boardMatrix.GetLength(0) - 1, k = 0; i >= 0; i--, k++)
		{
			if (boardMatrix[k, i].GetPlayerWhoUsed() != 0)
			{
				continue;
			}

			if (GameManager.Instance.GetPlayerNumber() == playerToWin)
			{
				boardMatrix[k, i].Choose();
				return;
			}
			else
			{
				int j = GameManager.Instance.GetTurnCount() % boardMatrix.GetLength(1);

				j = j == i ? (j + 1) % boardMatrix.GetLength(1) : j;

				boardMatrix[k, j].Choose();
				return;
			}
		}
	}

	private void MakeDrawMove()
	{
		for(int i = 0; i < boardMatrix.GetLength(0); i++)
		{
			for(int j = 0; j < boardMatrix.GetLength(1); j++)
			{
				int row;

				if(boardSize % 2 == 0)
				{
					row = (i + GameManager.Instance.GetPlayerNumber() + GameManager.Instance.GetTurnCount()) % boardMatrix.GetLength(0);
				}
				else
				{
					row = (i + GameManager.Instance.GetTurnCount()) % boardMatrix.GetLength(0);
				}

				if (boardMatrix[row, j].GetPlayerWhoUsed() != 0)
				{
					continue;
				}

				boardMatrix[row, j].Choose();
				return;
			}
		}
	}

	public void RunTest()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = true;
#endif
	}
}
