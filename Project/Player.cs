using Steamworks;
using UnityEngine;

public class Player
{
	public int id;

	public string username;

	public bool ready;

	public bool joined;

	public bool loading;

	public Color color;

	public Vector3 pos;

	public float yOrientation;

	public float xOrientation;

	public bool running;

	public bool dead;

	public int ping;

	public ulong damageDone;

	public ulong mobsKilled;

	public ulong damageTaken;

	public float lastPingTime;

	public int[] powerups;

	public int[] armor;

	public int totalArmor;

	public SteamId steamId;

	public int currentHp;

	public Player(int id, string username, Color color)
	{
		this.id = id;
		this.username = username;
		currentHp = 100;
		dead = false;
		powerups = new int[ItemManager.Instance.allPowerups.Count];
		armor = new int[4];
		for (int i = 0; i < armor.Length; i++)
		{
			armor[i] = -1;
		}
	}

	public Player(int id, string username, Color color, SteamId steamId)
	{
		this.id = id;
		this.username = username;
		this.steamId = steamId;
		currentHp = 100;
		dead = false;
		powerups = new int[ItemManager.Instance.allPowerups.Count];
		armor = new int[4];
		for (int i = 0; i < armor.Length; i++)
		{
			armor[i] = -1;
		}
	}

	public void PingPlayer()
	{
		lastPingTime = Time.time;
	}

	public void UpdateArmor(int armorSlot, int itemId)
	{
		Debug.Log("slot: " + armorSlot + ", itemid: " + itemId);
		armor[armorSlot] = itemId;
		totalArmor = 0;
		int[] array = armor;
		foreach (int num in array)
		{
			if (num != -1)
			{
				totalArmor += ItemManager.Instance.allItems[num].armor;
			}
		}
	}

	public void Died()
	{
		currentHp = 0;
		dead = true;
	}

	public int Damage(int damageDone)
	{
		currentHp -= damageDone;
		if (currentHp < 0)
		{
			currentHp = 0;
		}
		return currentHp;
	}
}
