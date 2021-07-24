using TMPro;
using UnityEngine;

public class GameoverUI : MonoBehaviour
{
	public TextMeshProUGUI daysText;

	public TextMeshProUGUI header;

	private void Awake()
	{
		int winnerId = GameManager.instance.winnerId;
		switch (winnerId)
		{
		case -3:
			header.text = "Victory!";
			daysText.text = "<size=80%>Muck escaped after " + GameManager.instance.currentDay + " days!";
			break;
		case -2:
			daysText.text = "Survived for " + GameManager.instance.currentDay + " days.";
			break;
		case -1:
			daysText.text = "Draw...";
			break;
		default:
		{
			string username = GameManager.players[winnerId].username;
			daysText.text = username + " won the game!";
			break;
		}
		}
	}
}
