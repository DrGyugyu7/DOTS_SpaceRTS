using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;

using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UGSMgr : MonoBehaviour
{
    public static UGSMgr Instance;

    [SerializeField] TMP_InputField usernameIF, passwordIF, cloudSaveIF, scoreIF;
    [SerializeField] Button signupBtn, signinBtn, cloudSaveBtn, scoreSaveBtn, startGameBtn;
    private const string leaderBoardID = "LeaderBoard";
    // [SerializeField] private GameObject scoreItem;
    [SerializeField] private GameObject scoreItem;
    [SerializeField] private Transform leaderBoardContent;
    private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        await UnityServices.Instance.InitializeAsync();
        AuthenticationService.Instance.SignedIn += async () =>
        {
            await LoadAllScores();
            await GetRCAsync();
            startGameBtn.gameObject.SetActive(true);
        };
        RemoteConfigService.Instance.FetchCompleted += (response) =>
        {

        };
        signupBtn.onClick.AddListener(async () =>
        {
            await SignUpAsync(usernameIF.text, passwordIF.text);
        });
        signinBtn.onClick.AddListener(async () =>
        {
            await SignInAsync(usernameIF.text, passwordIF.text);
        });
        cloudSaveBtn.onClick.AddListener(async () =>
        {
            await CloudSaveAsync();
        });
        scoreSaveBtn.onClick.AddListener(async () =>
        {
            await ScoreSaveAsync();
        });
        //await LoadAllScores();
        startGameBtn.onClick.AddListener(async () =>
        {
            await SceneManager.LoadSceneAsync("RTS_Scene");
            this.gameObject.transform.localScale = Vector3.zero;
        });
        //await ServerAuthenticationService.Instance.SignInFromServerAsync();
    }
    private async Task LoadAllScores()
    {
        var response = await LeaderboardsService.Instance.GetScoresAsync(leaderBoardID);
        entries = response.Results;
        foreach (var entry in entries)
        {
            GameObject scoreItemInstancce = Instantiate(scoreItem, leaderBoardContent);
            scoreItemInstancce.GetComponent<TMP_Text>().text = $"  Username : {entry.PlayerName}\n  Score : {entry.Score}";
        }
    }

    private async Task ScoreSaveAsync()
    {
        await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderBoardID, int.Parse(scoreIF.text));
    }

    private async Task CloudSaveAsync()
    {
        var data = new Dictionary<string, object>
        {
            {"Score", Int32.Parse(cloudSaveIF.text)}
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        Debug.Log("ScoreSaved");
    }

    private async Task SignUpAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
        }
        catch (AuthenticationException e)
        {
            Debug.Log(e);
        }
        catch (RequestFailedException e)
        {
            Debug.Log(e);
        }
    }
    private async Task SignInAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
        }
        catch (AuthenticationException e)
        {
            Debug.Log(e);
        }
        catch (RequestFailedException e)
        {
            Debug.Log(e);
        }
    }
    private struct UserAttr { };
    private struct AppAttr { };
    private async Task GetRCAsync()
    {
        await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttr(), new AppAttr());
    }
}
