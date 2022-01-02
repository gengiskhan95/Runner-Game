using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	[Header("Bools")]
	public bool isLevelStart;
	public bool isLevelDone;
	public bool isLevelFail;

	[Space(15)]
	public int level;

	UIController UI;
	PlayerController Player;
	CameraController Camera;

	[Header("Tags")]
	public string TagObstacle;
	public string TagFinish;
	public string TagCollectable;
	public string TagWallTrigger;

	public static GameController instance;

	private void Awake()
	{
		if (!instance)
		{
			instance = this;
		}
		GetLevel();
		CheckLevel();
	}
	void GetLevel()
	{
		if (PlayerPrefs.GetInt("Level") == 0)
		{
			level = 1;
			PlayerPrefs.SetInt("Level", 1);
		}
		else
		{
			level = PlayerPrefs.GetInt("Level");
		}
	}
	void CheckLevel()
	{
		if (SceneManager.GetActiveScene().buildIndex != level - 1)
		{
			SceneManager.LoadScene(level - 1);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		StartMethods();
	}
	#region Start Methods
	void StartMethods()
	{
		UI = UIController.instance;
		Player = PlayerController.instance;
		Camera = CameraController.instance;
	}


	#endregion

	#region TapToStart

	public void TapToStartActions()
	{
		SendLevelStart();
		Player.TapToStartActions();
	}
	void SendLevelStart()
	{
		UI.isLevelStart = true;
		isLevelStart = true;
		Player.isLevelStart = true;
		Camera.isLevelStart = true;
	}


	#endregion
	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.D))
        {
			PlayerPrefs.DeleteAll();
        }
	}


	#region EndGame

	public void LevelComplete()
	{
		isLevelDone = true;
		Player.isLevelDone = true;
		UI.isLevelDone = true;
		Camera.isLevelDone = true;
		SetFinalCoin();
		UI.ShowEndGamePanel();
		Player.ActionLevelDone();
	}
	public void LevelFail()
	{
		isLevelFail = true;
		Player.isLevelFail = true;
		UI.isLevelFail = true;
		Camera.isLevelFail = true;
		SetFinalCoin();
		UI.ShowEndGamePanel();
		Player.ActionLevelFail();
	}
	void SetFinalCoin()
    {
		PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + Player.coin);
    }

	public void EndGameButtonAction()
	{
		if (isLevelDone)
		{
			if (level == SceneManager.sceneCountInBuildSettings)
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
			else
			{
				PlayerPrefs.SetInt("Level", level + 1);
				SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1));
			}
		}
		else if (isLevelFail)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
	#endregion
}
