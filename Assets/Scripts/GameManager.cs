using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField passwordField;

    private string savedToken;
    private string baseUrl = "http://localhost:8000/api";

    public UnityEvent StartGame;
    public UIManager UIM;

    void Start()
    {
        StartCoroutine(LeaderboardLoop(3f));
    }

    // ---------------- LOGIN ----------------

    public void Login()
    {
        StartCoroutine(LoginRoutine(
            nameField.text,
            passwordField.text
        ));
    }

    IEnumerator LoginRoutine(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/login", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AuthResponse response =
                    JsonUtility.FromJson<AuthResponse>(www.downloadHandler.text);

                savedToken = response.token;

                Debug.Log("Login Successful!");
                StartGame?.Invoke();
            }
            else
            {
                Debug.LogError(www.downloadHandler.text);
            }
        }
    }

    // ---------------- SPRITE ----------------

    public void SendSprite(int spriteIndex)
    {
        if (string.IsNullOrEmpty(savedToken))
        {
            Debug.LogError("No token!");
            return;
        }

        StartCoroutine(SaveScoreRoutine(spriteIndex));
    }

    IEnumerator SaveScoreRoutine(int spriteIndex)
    {
        WWWForm form = new WWWForm();
        form.AddField("sprite", spriteIndex);

        using (UnityWebRequest www =
               UnityWebRequest.Post($"{baseUrl}/update-score", form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + savedToken);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Current Sprite saved");
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
            yield return FetchLeaderboard();
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator FetchLeaderboard()
    {

        using (UnityWebRequest www =
               UnityWebRequest.Get($"{baseUrl}/leaderboard"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;

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
}

[System.Serializable]
public class LeaderboardEntry
{
    public string username;
    public int score;
}

[System.Serializable]
public class LeaderboardWrapper
{
    public LeaderboardEntry[] items;
}