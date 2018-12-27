using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	/*
	 * The UIManager is the class that handles all the UI input. In the case of this game, it handles the various button presses (not the grid cell buttons though, as those are
	 * part of the game and not part of the UI). For example, this class handle the various menu buttons and call the appropriate methods in other classes when they are pressed.
	 */


	//Singleton
	public static UIManager Instance;

	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject gameOverPanel;
	[SerializeField] private GameObject chooseGameTypePanel;
	[SerializeField] private Text gameInfoText;
	[SerializeField] private Text playerTurnText;
	[SerializeField] private Text gameOverText;
	[SerializeField] private AudioClip selectSound;


	/// <summary>
	/// Private constructor to enforce the singleton.
	/// </summary>
	private UIManager() { }

	/// <summary>
	/// Enforces the singleton and sets all UI elements to their initial state.
	/// </summary>
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		chooseGameTypePanel.SetActive(true);
		gameInfoText.enabled = false;
		playerTurnText.enabled = false;
		pausePanel.SetActive(false);
		gameOverPanel.SetActive(false);
		gameOverText.text = "";
	}

	/// <summary>
	/// Called after awake. Subscribes to GameManager's onTurnCompleteCallback delegate.
	/// </summary>
	private void Start()
	{
		GameManager.Instance.onTurnCompleteCallback += UpdatePlayerTurnText;
	}

	/// <summary>
	/// Updates the player turn text.
	/// </summary>
	public void UpdatePlayerTurnText()
	{
		playerTurnText.text = "Turn: player " + GameManager.Instance.GetPlayerNumber(); 
	}

	/// <summary>
	/// Shows the game over panel with the custom game over text.
	/// </summary>
	/// <param name="gameOverText">The custom game over text.</param>
	public void ShowGameOverPanel(string gameOverText)
	{
		this.gameOverText.text = gameOverText;
		gameOverPanel.SetActive(true);
		gameInfoText.enabled = false;
		playerTurnText.enabled = false;
	}


	/// <summary>
	/// Hides the game over panel.
	/// </summary>
	public void HideGameOverPanel()
	{
		gameOverText.text = "";
		gameOverPanel.SetActive(false);
		gameInfoText.enabled = true;
		playerTurnText.enabled = true;
	}


	/// <summary>
	/// Toggles the pause panel and the game info text.
	/// </summary>
	public void TogglePausePanel()
	{
		AudioManager.Instance.PlaySoundEffect(selectSound);
		pausePanel.SetActive(!pausePanel.activeSelf);
		gameInfoText.enabled = !gameInfoText.enabled;
		playerTurnText.enabled = !playerTurnText.enabled;
	}


	/// <summary>
	/// Starts a new game by hiding the game over panel, pause panel, and by showing the game type panel.
	/// </summary>
	public void NewGame()
	{
		AudioManager.Instance.PlaySoundEffect(selectSound);
		if (pausePanel.activeSelf)
		{
			TogglePausePanel();
		}

		if(gameOverPanel.activeSelf)
		{
			HideGameOverPanel();
		}

		gameInfoText.enabled = false;
		playerTurnText.enabled = false;
		chooseGameTypePanel.SetActive(true);
	}

	/// <summary>
	/// This function will set the number of cells per row in the game board for the next game. It will then reset the game and start it.
	/// </summary>
	/// <param name="numCellsToWin">The number of cells per row for the new game. Ie 3 = a 3x3 gameboard, 5 = 5x5 etc.</param>
	public void ChooseGameType(int numCellsToWin)
	{
		AudioManager.Instance.PlaySoundEffect(selectSound);
		chooseGameTypePanel.SetActive(false);
		playerTurnText.enabled = true;
		gameInfoText.enabled = true;
		GameManager.Instance.SetNumCellsPerRow(numCellsToWin);
		GameManager.Instance.ResetGame();
	}

	/// <summary>
	/// Print the entire move history of the current game.
	/// </summary>
	public void PrintGameMoveHistory()
	{
		AudioManager.Instance.PlaySoundEffect(selectSound);
		GameManager.Instance.PrintMoveHistory();
	}

	/// <summary>
	/// Quit the application. Only works in stand alone builds.
	/// </summary>
	public void QuitGame()
	{
		AudioManager.Instance.PlaySoundEffect(selectSound);
		Application.Quit();
	}
}
