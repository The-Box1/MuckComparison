public class GameSettings
{
	public enum GameMode
	{
		Survival,
		Versus,
		Creative
	}

	public enum FriendlyFire
	{
		Off,
		On
	}

	public enum Difficulty
	{
		Easy,
		Normal,
		Gamer
	}

	public enum Respawn
	{
		OnNewDay,
		Never
	}

	public enum GameLength
	{
		Short = 3,
		Medium = 8,
		Long = 14
	}

	public enum Multiplayer
	{
		Off,
		On
	}

	public int Seed;

	public GameMode gameMode { get; set; }

	public FriendlyFire friendlyFire { get; set; }

	public Difficulty difficulty { get; set; }

	public Respawn respawn { get; set; }

	public GameLength gameLength { get; set; }

	public Multiplayer multiplayer { get; set; }

	public GameSettings(int seed, GameMode gameMode = GameMode.Survival, FriendlyFire friendlyFire = FriendlyFire.Off, Difficulty difficulty = Difficulty.Normal, GameLength gameLength = GameLength.Short, Multiplayer multiplayer = Multiplayer.On)
	{
		Seed = seed;
		this.gameMode = gameMode;
		this.friendlyFire = friendlyFire;
		this.difficulty = difficulty;
		this.gameLength = gameLength;
		this.multiplayer = multiplayer;
	}

	public GameSettings(int seed, int gameMode, int friendlyFire, int difficulty, int gameLength, int multiplayer)
	{
		Seed = seed;
		this.gameMode = (GameMode)gameMode;
		this.friendlyFire = (FriendlyFire)friendlyFire;
		this.difficulty = (Difficulty)difficulty;
		this.gameLength = (GameLength)gameLength;
		this.multiplayer = (Multiplayer)multiplayer;
	}

	public int BossDay()
	{
		return difficulty switch
		{
			Difficulty.Easy => 6, 
			Difficulty.Normal => 4, 
			Difficulty.Gamer => 3, 
			_ => 5, 
		};
	}

	public float GetChestPriceMultiplier()
	{
		return difficulty switch
		{
			Difficulty.Easy => 8f, 
			Difficulty.Normal => 6f, 
			Difficulty.Gamer => 5f, 
			_ => 5f, 
		};
	}

	public int DayLength()
	{
		return difficulty switch
		{
			Difficulty.Easy => 56, 
			Difficulty.Normal => 54, 
			Difficulty.Gamer => 52, 
			_ => 5, 
		};
	}
}
