using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("User Input")]
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField passwordField;

    private string savedToken;
    private string baseUrl = "http://3.130.41.241/api"; //elastic ip
    [Header("References")]
    public UnityEvent StartGame;
    public UIManager UIM;

    [Header("Game Stats")]
    public int score = 0;
    public int highScore = 0;
    public int scoreIncreaseAmt = 1;
    bool gameStarted;
    [SerializeField] private TMP_Text DebugText;
    public float syncRate = 1.5f;


    void Start()
    {

    }
    public void LoopStart()
    {
        StartCoroutine(LeaderboardLoop(3f));
    }

    void OnApplicationQuit()
    {
        SendScore();
        SendSprite();
        Debug.Log("Quit Saved");
    }

    // ---------------- LOGIN ----------------

    public void Login()
    {
        Debug.Log("LOGIN BUTTON HIT");
        if (!string.IsNullOrWhiteSpace(nameField.text) &&
            !string.IsNullOrWhiteSpace(passwordField.text))
        { 
            StartCoroutine(LoginRoutine(
                nameField.text,
                passwordField.text,
                false
            ));
        }
        else
        {
            DebugText.text = "Enter Login Info";
        }

    }
    public void LoginNewGame()
    {
        if (!string.IsNullOrWhiteSpace(nameField.text) &&
            !string.IsNullOrWhiteSpace(passwordField.text))
        {
            StartCoroutine(LoginRoutine(
                nameField.text,
                passwordField.text,
                true
            ));
        }
        else
        {
            DebugText.text = "Enter Login Info";
        }
    }

    IEnumerator LoginRoutine(string username, string password, bool newgame)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/login", form))
        {
            yield return www.SendWebRequest();
            Debug.Log("RAW: " + www.downloadHandler.text);

            if (www.result == UnityWebRequest.Result.Success)
            {
                AuthResponse response =
                    JsonUtility.FromJson<AuthResponse>(www.downloadHandler.text);

                savedToken = response.token;
                
                //load player info
                if (!newgame)
                {
                    score = response.user.score;
                    highScore = response.user.high_score;
                    UIM.CurrSpriteIndex = response.user.sprite;
                    scoreIncreaseAmt = UIM.ducks[UIM.CurrSpriteIndex].scoreIncrease;
                }
                if (response.newUser) { DebugText.text = "Account Created!"; }
                DebugText.text = "Login Successful!";
                //UIM.LoadGame(response.user.sprite, score, highScore);
                gameStarted = true;
                StartGame?.Invoke();
            }
            else
            {
                Debug.LogError(www.downloadHandler.text);
                DebugText.text = "Error: " + www.downloadHandler.text;
            }
        }
    }


    // ---------------- SCORE ----------------

    public void AddScore()
    {
        if (gameStarted)
        {
            score += scoreIncreaseAmt;
            SendScore();
        }
    }
    public void SendScore()
    {
        if (string.IsNullOrEmpty(savedToken))
        {
            Debug.LogError("No token!");
            return;
        }
        StartCoroutine(SaveScoreRoutine(score));
    }

    IEnumerator SaveScoreRoutine(int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("score", score);
        form.AddField("high_score", highScore);

        using (UnityWebRequest www =
               UnityWebRequest.Post($"{baseUrl}/update-score", form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + savedToken);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Current Score saved" + score);
                Debug.Log("Current High Score saved" + highScore);
            }
            else
            {
                Debug.LogError(www.downloadHandler.text);
            }
        }
    }

    // ---------------- SPRITE ----------------

    public void SendSprite()
    {
        if (string.IsNullOrEmpty(savedToken))
        {
            Debug.LogError("No token!");
            return;
        }
        StartCoroutine(SaveSpriteRoutine(UIM.CurrSpriteIndex));
    }

    IEnumerator SaveSpriteRoutine(int sprite)
    {
        WWWForm form = new WWWForm();
        form.AddField("sprite", sprite);

        using (UnityWebRequest www =
               UnityWebRequest.Post($"{baseUrl}/update-sprite", form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + savedToken);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Current Sprite Upgrade saved" + sprite);
            }
            else
            {
                Debug.LogError(www.downloadHandler.text);
            }
        }
    }
    // ---------------- LEADERBOARD ----------------

    IEnumerator LeaderboardLoop(float refreshRate)
    {
        while (true)
        {
            yield return new WaitForSeconds(refreshRate);
            yield return FetchLeaderboard();
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator FetchLeaderboard()
    {
        // Debug.Log("Leaderboard JSON: " + json);
        using (UnityWebRequest www =
               UnityWebRequest.Get($"{baseUrl}/leaderboard"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {

                string json = www.downloadHandler.text;
                Debug.Log("Leaderboard JSON: " + json);
                LeaderboardWrapper data =
                    JsonUtility.FromJson<LeaderboardWrapper>(json);

                UIM.UpdateTop3(data.items);
            }
            else
            {
                Debug.LogError(www.downloadHandler.text);
            }
        }
    }

}

// ---------------- DATA MODELS ----------------

[System.Serializable]
public class AuthResponse
{
    public string token;
    public PlayerData user;
    public bool newUser;
}

[System.Serializable]
public class PlayerData
{
    public string username;
    public int score;
    public int high_score;
    public int sprite;
}

[System.Serializable]
public class LeaderboardEntry
{
    public string username;
    public int high_score;
}

[System.Serializable]
public class LeaderboardWrapper
{
    public LeaderboardEntry[] items;
}
