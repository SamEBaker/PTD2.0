using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;
using Button = UnityEngine.UI.Button;

public class UIManager : MonoBehaviour
{
    GameManager gameManager;

    [Header("Store Info")]
    public Ducks[] ducks;
    [SerializeField]
    public int CurrSpriteIndex = 0;
    public Animator Anim;
    public TMPro.TMP_Text StoreUpgradeText;
    public UnityEngine.UI.Image NextUpgradeImage;
    private bool StoreActive = true;
    [SerializeField]
    GameObject StoreScreen;
    [SerializeField]
    GameObject lockicon;
    [SerializeField]
    GameObject upgradeicon;
    [SerializeField]
    GameObject upgradeAvailable;
    [SerializeField]
    Button UpgradeButton;


    [Header("Win Screen Info")]
    [SerializeField]
    GameObject WinScreen;
    [SerializeField]
    int winIndex;

    [Header("Options Info")]
    [SerializeField]
    VideoClip Space;
    [SerializeField]
    VideoClip Coins;
    [SerializeField]
    VideoClip Parkour;
    [SerializeField]
    VideoPlayer backgroundVideo;
    [SerializeField]
    UnityEngine.UI.Slider VolSlider;
    [SerializeField]
    GameObject OptionScreen;


    [Header("Game Screen Info")]
    [SerializeField]
    TMPro.TMP_Text ScoreText;
    [SerializeField]
    TMPro.TMP_Text HighScoreText;
    [SerializeField]
    AudioSource BGAudio;
    [SerializeField]
    GameObject GameScreen;
    [SerializeField]
    GameObject LoginScreen;


    [Header("Leaderboard UI Info")]
    [SerializeField] private TMP_Text top1NameText;
    [SerializeField] private TMP_Text top1ScoreText;
    [SerializeField] private TMP_Text top2NameText;
    [SerializeField] private TMP_Text top2ScoreText;
    [SerializeField] private TMP_Text top3NameText;
    [SerializeField] private TMP_Text top3ScoreText;
    [Header("Leaderboard Update")]
    [SerializeField] private float refreshRate = 3f;
    bool showWinScreen = false;

    void Start()
    { 
        gameManager = GetComponent<GameManager>();

        winIndex = ducks.Length - 1;
    }

    //called on login, sets game from player saved info
    public void LoadGame(int score, int highScore)
    {
        if (highScore < score) {  highScore = score; }
        UpdateScoreText();
        UpdateHighScoreText();
        CurrSpriteIndex--;
        Upgrade();
    }


    public void OpenCloseStore(bool toggle)
    {
        if (StoreActive)
        {
            StoreScreen.SetActive(toggle);
        }
    }
    public void OpenWinScreen()
    {
        WinScreen.SetActive(true);
    }
    public void OpenGameScreen()
    {
        GameScreen.SetActive(true);
        LoginScreen.SetActive(false);
        LoadGame(gameManager.score, gameManager.highScore);
    }
    public void OpenSettings(bool toggle)
    {
        OptionScreen.SetActive(toggle);
    }

    public void UpdateSettings(string Option)
    {
        if (Option == "Space")
        {
            backgroundVideo.clip = Space;
        }
        else if (Option == "Coin")
        {
            backgroundVideo.clip = Coins;
        }
        else if (Option == "MC")
        {
            backgroundVideo.clip = Parkour;
        }
        else
        {
            BGAudio.volume = VolSlider.value;
        }

    }


    public void UpdateTop3(LeaderboardEntry[] list)
    {
        if (list.Length > 0)
        {
            top1NameText.text = list[0].username;
            top1ScoreText.text = list[0].high_score.ToString();
        }
        else
        {
            top1NameText.text = "---";
            top1ScoreText.text = "---";
        }

        if (list.Length > 1)
        {
            top2NameText.text = list[1].username;
            top2ScoreText.text = list[1].high_score.ToString();
        }
        else
        {
            top2NameText.text = "---";
            top2ScoreText.text = "---";
        }

        if (list.Length > 2)
        {
            top3NameText.text = list[2].username;
            top3ScoreText.text = list[2].high_score.ToString();
        }
        else
        {
            top3NameText.text = "---";
            top3ScoreText.text = "---";
        }
    }
    public void UpdateScoreText()
    {
        ScoreText.text = gameManager.score.ToString() + " Points";
    }
    public void UpdateHighScoreText()
    {
        HighScoreText.text = "All Time High Score: " + gameManager.highScore.ToString();
    }

    //called ever player clicks
    public void CheckCanUpgrade()
    {
        if (showWinScreen) return;
        else if (gameManager.score >= ducks[CurrSpriteIndex + 1].scoreNeeded)
        {
            lockicon.SetActive(false);
            upgradeicon.SetActive(true);
            //StartCoroutine(UpgradeAvailablePopup());
            UpgradeButton.interactable = true;
        }
    }

    //called on button click
    public void Upgrade()
    {
        CurrSpriteIndex++;
        showWinScreen = false;
        
        if (CurrSpriteIndex == winIndex || gameManager.score >= ducks[winIndex].scoreNeeded)
        {
            showWinScreen = true;
            StoreActive = false;
            StoreScreen.SetActive(false);
            OpenWinScreen();
        }
        else
        {
            Debug.Log("state: " + ducks[CurrSpriteIndex].Duckanim);
            Anim.Play(ducks[CurrSpriteIndex].Duckanim);
            lockicon.SetActive(true);
            upgradeicon.SetActive(false);
            UpgradeButton.interactable = false;
            NextUpgradeImage.sprite = ducks[CurrSpriteIndex + 1].staticSprite;
            gameManager.scoreIncreaseAmt = ducks[CurrSpriteIndex].scoreIncrease;
            StoreUpgradeText.text = "<color=\"red\">" + ducks[CurrSpriteIndex+1].scoreNeeded + "</color> points needed to upgrade";
            gameManager.SendSprite();
        }

    }
    IEnumerator UpgradeAvailablePopup()
    {
        upgradeAvailable.SetActive(true);
        yield return new WaitForSeconds(3f);
        upgradeAvailable.SetActive(false);
    }
}
