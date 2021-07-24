using UnityEngine;

[CreateAssetMenu]
public class Powerup : ScriptableObject
{
	public enum PowerTier
	{
		White,
		Blue,
		Orange
	}

	public new string name;

	public string description;

	public int id;

	public PowerTier tier;

	public Mesh mesh;

	public Material material;

	public Sprite sprite;

	public Color GetOutlineColor()
	{
		return tier switch
		{
			PowerTier.White => Color.white, 
			PowerTier.Blue => Color.cyan, 
			PowerTier.Orange => Color.yellow, 
			_ => Color.white, 
		};
	}

	public string GetColorName()
	{
		return tier switch
		{
			PowerTier.White => "white", 
			PowerTier.Blue => "#00C0FF", 
			PowerTier.Orange => "orange", 
			_ => "white", 
		};
	}
}
