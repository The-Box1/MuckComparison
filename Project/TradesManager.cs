using UnityEngine;

public class TradesManager : MonoBehaviour
{
	public WoodmanTrades archerTrades;

	public WoodmanTrades chefTrades;

	public WoodmanTrades smithTrades;

	public WoodmanTrades woodTrades;

	public WoodmanTrades wildcardTrades;

	public static TradesManager Instance;

	private void Awake()
	{
		Instance = this;
	}
}
