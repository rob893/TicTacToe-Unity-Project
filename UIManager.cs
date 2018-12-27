using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager Instance;

	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject gameOverPanel;
	[SerializeField] private GameObject chooseGameTypePanel;
	[SerializeField] private Text gameInfoText;
	[SerializeField] private Text playerTurnText;
	[SerializeField] private Text gameOverText;
	[SerializeField] private AudioClip selectSound;


	private UIManager() { }

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

	private void Start()
	{
		GameManager.Instance.onTurnCompleteCallback += UpdatePlayerTurnText;
	}

	public void UpdatePlayerTurnText()
	{
		playerTurnText.text = "Turn: player " + GameManager.Instance.GetPlayerNumber(); 
	}

	public void ShowGameOverPanel(string gameOverText)
	{
		this.gameOverText.text = gameOverText;
		gameOverPanel.SetActive(true);
		gameInfoText.enabled = false;
		playerTurnText.enabled = false;
	}

	public void HideGameOverPanel()
	{
		gameOverText.text = "";
		gameOverPanel.SetActive(false);
		gameInfoText.enabled = true;
		playerTurnText.enabled = true;
	}

	public void TogglePausePanel()
	{
		AudioManager.Instance.PlaySoundEffect(selectSound);
		pausePanel.SetActive(!pausePanel.activeSelf);
		gameInfoText.enabled = !gameInfoText.enabled;
		playerTurnText.enabled = !playerTurnText.enabled;
	}

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

	public void Choose3x3()
	{
		AudioManager.Instance.PlaySoundEffect(selectSound);
		chooseGameTypePanel.SetActive(false);
		playerTurnText.enabled = true;
		gameInfoText.enabled = true;
		GameManager.Instance.SetNumCellsPerRow(3);
		GameManager.Instance.ResetGame();
	}

	public void Choose4x4()
	{
		AudioManager.Instance.PlaySoundEffect(selectSound);
		chooseGameTypePanel.SetActive(false);
		playerTurnText.enabled = true;
		gameInfoText.enabled = true;
		GameManager.Instance.SetNumCellsPerRow(4);
		GameManager.Instance.ResetGame();
	}

	public void PrintGameMoveHistory()
	{
		AudioManager.Instance.PlaySoundEffect(selectSound);
		GameManager.Instance.PrintMoveHistory();
	}

	public void QuitGame()
	{
		AudioManager.Instance.PlaySoundEffect(selectSound);
		Application.Quit();
	}
}
