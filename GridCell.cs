using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GridCell : MonoBehaviour {
	/*
	 * This class represents an individual grid cell. For example, a 3x3 board has 9 instances of this class.
	 */

	[SerializeField] private AudioClip chooseSound;
	[SerializeField] private AudioClip errorSound;

	private bool isUsed = false;
	private GridPosition position;
	private Button button;
	private Text text;


	/// <summary>
	/// Set initial references.
	/// </summary>
	private void Start()
	{
		button = GetComponent<Button>();
		text = GetComponentInChildren<Text>();
	}

	/// <summary>
	/// Called when the cell is clicked on, this method will mark the cell as used, enable the text (either to "X" or "O" depending on the player number), and then call
	/// GameManager's EndTurn function by passing a reference to itself. This function will play an error noise and return before doing anything if the cell has already been used.
	/// </summary>
	public void Choose()
	{
		if(isUsed)
		{
			AudioManager.Instance.PlaySoundEffect(errorSound, true);
			return;
		}

		isUsed = true;
		text.enabled = true;

		AudioManager.Instance.PlaySoundEffect(chooseSound, true);

		text.text = GameManager.Instance.GetPlayerNumber() == 1 ? "X" : "O";
		GameManager.Instance.EndTurn(this);
	}

	/// <summary>
	/// Sets the position of the cell.
	/// </summary>
	/// <param name="pos">The GridPosition associated with this cell.</param>
	public void SetPosition(GridPosition pos)
	{
		position = pos;
	}

	/// <summary>
	/// Get the position of this cell.
	/// </summary>
	/// <returns>The GridPosition of the cell.</returns>
	public GridPosition GetPosition()
	{
		return position;
	}

	/// <summary>
	/// Set this cell to be uninteractable.
	/// </summary>
	public void SetInactive()
	{
		button.interactable = false;
	}
}
