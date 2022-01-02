using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
	[Header("Bools")]
	public bool isLevelStart;
	public bool isLevelDone;
	public bool isLevelFail;

	[Header("Tap To Start")]
	public GameObject PanelTapToStart;

	[Header("In Game Panel")]
	public GameObject PanelInGame;
	public TextMeshProUGUI TextCoin;
	public Slider ProgressBar;
	public TextMeshProUGUI TextLevel;

	[Header("End Game Panel")]
	public GameObject PanelEndGame;
	public TextMeshProUGUI TextEndGame;
	public TextMeshProUGUI TextEndGameButton;
	public Button ButtonEndGame;
	public TextMeshProUGUI TextFinalCoin;

	[Header("Strings")]
	public List<string> TextsEndGameWin;
	public List<string> TextsEndGameFail;

	GameController GC;
	Transform Player;
	Transform FT;
	int level;
	public static UIController instance;

	private void Awake()
	{
		if (!instance)
		{
			instance = this;
		}
	}
	// Start is called before the first frame update
	void Start()
	{
		StartMethods();
	}
	#region StartMethods

	void StartMethods()
	{
		GC = GameController.instance;
		Player = PlayerController.instance.transform;
		FT = FinishTrigger.instance.transform;
		GetLevel();
		ShowTapToStartPanel();
	}

	void GetLevel()
    {
		level = GC.level;
    }


	#endregion

	#region TapToStart

	void ShowTapToStartPanel()
	{
		PanelTapToStart.SetActive(true);
	}

	void CloseTapToStart()
	{
		PanelTapToStart.SetActive(false);
	}

	public void ButtonActionTapToStart()
	{
		CloseTapToStart();
		GC.TapToStartActions();
		ShowInGamePanel();
	}

    #endregion
    private void Update()
    {
        if(isLevelStart && !isLevelFail && !isLevelDone)
        {
			UpdateeProgreesBar();
        }
    }
    #region InGamePanel
    void ShowInGamePanel()
	{
		PanelInGame.SetActive(true);
		SetProgressBar();
		FillTextLevel();
	}
	void SetProgressBar()
    {
		ProgressBar.maxValue = Vector3.Distance(Player.position, FT.position);
	}
	void FillTextLevel()
    {
		TextLevel.text = level.ToString();
    }
   void UpdateeProgreesBar()
    {
		ProgressBar.value = ProgressBar.maxValue - Vector3.Distance(Player.position, FT.position);
	}
    void CloseInGamePanel()
	{
		PanelInGame.SetActive(false);
	}

	public void UpdateCoinText(string scoreText)
	{
		TextCoin.text = scoreText;
	}

	#endregion

	#region EndGamePanel

	public void ShowEndGamePanel()
	{
		CloseInGamePanel();
		PanelEndGame.SetActive(true);
		FillEndGameTexts();
	}

	public void ButtonActionEndGame()
	{
		ButtonEndGame.gameObject.SetActive(false);
		GC.EndGameButtonAction();
	}

	void FillEndGameTexts()
	{
		if (isLevelDone)
		{
			TextEndGame.text = TextsEndGameWin[Random.Range(0, TextsEndGameWin.Count)];
			TextEndGameButton.text = "Next Level";
		}
		else if (isLevelFail)
		{
			TextEndGame.text = TextsEndGameFail[Random.Range(0, TextsEndGameFail.Count)];
			TextEndGameButton.text = "Retry";
		}
		TextFinalCoin.text = PlayerPrefs.GetInt("Coin").ToString();
	}

	#endregion
}
