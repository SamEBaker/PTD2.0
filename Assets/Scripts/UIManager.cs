using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class UIManager : MonoBehaviour
{
    //STORE
    public Ducks[] ducks;
    [SerializeField]
    public int CurrSpriteIndex = 0;
    public Animator SpriteUI;
    public TMPro.TMP_Text StoreUpgradeText;
    public UnityEngine.UI.Image NextUpgradeImage;
    private bool StoreActive = true;
    [SerializeField]
    GameObject StoreScreen;
    [SerializeField]
    GameObject lockicon;
    GameObject upgradeAvailable;
    [SerializeField]
    Button UpgradeButton;

    // WIN SCREEN
    [SerializeField]
    GameObject WinScreen;

    //GAME SCREEN
    [SerializeField]
    TMPro.TMP_Text ScoreText;
    int score;
    [SerializeField]
    GameObject GameScreen;

    //LEADERBOARD
    [SerializeField] private TMP_Text top1NameText;
    [SerializeField] private TMP_Text top1ScoreText;
    [SerializeField] private TMP_Text top2NameText;
    [SerializeField] private TMP_Text top2ScoreText;
    [SerializeField] private TMP_Text top3NameText;
    [SerializeField] private TMP_Text top3ScoreText;

    [SerializeField] private float refreshRate = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        SpriteUI.SetInteger(0, CurrSpriteIndex);
        GameManager gamemanager = GetComponent<GameManager>();
    }
    // Update is called once per frame
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
    }


    public void UpdateTop3(LeaderboardEntry[] list)
    {
        if (list.Length > 0)
        {
            top1NameText.text = list[0].username;
            top1ScoreText.text = list[0].score.ToString();
        }
        else
        {
            top1NameText.text = "---";
            top1ScoreText.text = "---";
        }

        if (list.Length > 1)
        {
            top2NameText.text = list[1].username;
            top2ScoreText.text = list[1].score.ToString();
        }
        else
        {
            top2NameText.text = "---";
            top2ScoreText.text = "---";
        }

        if (list.Length > 2)
        {
            top3NameText.text = list[2].username;
            top3ScoreText.text = list[2].score.ToString();
        }
        else
        {
            top3NameText.text = "---";
            top3ScoreText.text = "---";
        }
    }


    //called ever player click
    public void CheckCanUpgrade()
    {
        if (score == ducks[CurrSpriteIndex].scoreNeeded)
        {
            lockicon.SetActive(false);
            StartCoroutine(UpgradeAvailablePopup());
            UpgradeButton.interactable = true;
        }
    }

    //called on button click
    public void Upgrade()
    {
        if (CurrSpriteIndex < 7)
        {
            CurrSpriteIndex++;
            SpriteUI.SetInteger(0, CurrSpriteIndex);
            lockicon.SetActive(false);
            NextUpgradeImage.sprite = ducks[CurrSpriteIndex + 1].staticSprite;
            UpgradeButton.interactable = false;
        }
        // win condition - disables store and tells player they reached max upgrade
        else if (CurrSpriteIndex == 6)
        {
            CurrSpriteIndex++;
            SpriteUI.SetInteger(0, CurrSpriteIndex);
            StoreActive = false;
            StoreScreen.SetActive(false);
            OpenWinScreen();
        }
    }
    IEnumerator UpgradeAvailablePopup()
    {
        upgradeAvailable.SetActive(true);
        yield return new WaitForSeconds(2f);
        upgradeAvailable.SetActive(false);
    }
}
