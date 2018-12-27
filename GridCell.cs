using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GridCell : MonoBehaviour {

	[SerializeField] private AudioClip chooseSound;
	[SerializeField] private AudioClip errorSound;

	private bool isUsed = false;
	private Vector2 position;
	private Button button;
	private Text text;


	private void Start()
	{
		button = GetComponent<Button>();
		text = GetComponentInChildren<Text>();
	}

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

	public void SetPosition(Vector2 pos)
	{
		position = pos;
	}

	public Vector2 GetPosition()
	{
		return position;
	}

	public void SetInactive()
	{
		button.interactable = false;
	}
}
