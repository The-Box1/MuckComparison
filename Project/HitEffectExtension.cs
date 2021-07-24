using UnityEngine;

internal static class HitEffectExtension
{
	public static Color GetColor(HitEffect effect)
	{
		return effect switch
		{
			HitEffect.Normal => Color.white, 
			HitEffect.Crit => Color.yellow, 
			HitEffect.Big => Color.red, 
			HitEffect.Falling => Color.cyan, 
			HitEffect.Electro => Color.yellow, 
			_ => Color.white, 
		};
	}

	public static string GetColorName(HitEffect effect)
	{
		return effect switch
		{
			HitEffect.Normal => "white", 
			HitEffect.Crit => "yellow", 
			HitEffect.Big => "red", 
			HitEffect.Falling => "#" + ColorUtility.ToHtmlStringRGB(Color.cyan), 
			HitEffect.Electro => "#" + ColorUtility.ToHtmlStringRGB(Color.yellow), 
			_ => "white", 
		};
	}
}
