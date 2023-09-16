using Cysharp.Threading.Tasks;
using Monetization.Ads.Providers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Zenject;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameUI gameUI;

    private List<NetworkPlayer> players = new();

    private NetworkPlayer lockalPlayer;
    private int localScore;

    private List<LeaderbordItem> leaderbords = new();
    private IGameSettingsManager gameSettingsManager;
    private IPlayerInventory playerInventory;
    private ISaveManager saveManager;
    private AudioManager audioManager;
    private IIronSourceProvider ironSourceProvider;

    public List<NetworkPlayer> Players => players;

    [Inject]
    private void Construct(IGameSettingsManager gameSettingsManager,
        IPlayerInventory playerInventory, ISaveManager saveManager,
        AudioManager audioManager, IIronSourceProvider ironSourceProvider)
    {
        this.gameSettingsManager = gameSettingsManager;
        this.playerInventory = playerInventory;
        this.saveManager = saveManager;
        this.audioManager = audioManager;
        this.ironSourceProvider = ironSourceProvider;

    }

    private void Start()
    {
        gameUI.UpdateGameTime(FormatingTimeToString(gameSettingsManager.Map.PlayTime * 60));

        audioManager.PlayCrossFadeMusic("Game");

        if (PhotonNetwork.InRoom)
        {
            StartGameAsync(this.GetCancellationTokenOnDestroy()).Forget();
            return;
        }
        StartCountingAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask StartGameAsync(CancellationToken token)
    {
        await UniTask.WaitUntil(()=> players.Count == PhotonNetwork.CurrentRoom.PlayerCount, cancellationToken: token);
        
        if(token.IsCancellationRequested)
            return;

        StartCountingAsync(token).Forget();
    }

    public void AddPlayer(NetworkPlayer player)
    {
        players.Add(player);

        player.SetInGame(false);

        if(player.IsLockal)
            lockalPlayer = player;

        var leaderbordItem =  gameUI.CreateLeaderbordItem(player.name);
        leaderbords.Add(leaderbordItem);
    }

    private void Update()
    {
        if (lockalPlayer != null && lockalPlayer.IsDrift)
        {
            lockalPlayer.SetScore(gameSettingsManager.Map.ScoreInTick);
            players.ForEach(p =>
            {
                if (p != null)
                    leaderbords.FirstOrDefault(x => x.Name == p.name)?.SetScore(p.Score);
            });
        }
    }

    private async UniTask GameTimeCountingAsync(float gameTime, CancellationToken token)
    {
        gameTime *= 60;

        while(!token.IsCancellationRequested && gameTime > 0)
        {
            gameTime -= Time.deltaTime;

            gameUI.UpdateGameTime(FormatingTimeToString(gameTime));

            await UniTask.Yield();
        }

        if (token.IsCancellationRequested)
            return;

        gameTime = 0;
        gameUI.UpdateGameTime(FormatingTimeToString(gameTime));

        EndGame();
    }

    private void EndGame()
    {
        players.ForEach(p =>
        {
            p.SetInGame(false);
        });

        gameUI.SetFinalScore(lockalPlayer.Score);
        gameUI.SetScoreDoubledClick(() => WatchAdAsync().Forget());
        gameUI.SetActiveFinalScore(true);
        gameUI.SetActimeDoubleScoreButton(true);
        SetMoneyToPlayer();
        saveManager.SavePlayerData();
    }

    private void SetMoneyToPlayer()
    {
        playerInventory.SetMoney(lockalPlayer.Score / gameSettingsManager.Map.ScoreInTick);
    }

    private async UniTask WatchAdAsync()
    {
        var result = await ironSourceProvider.ShowRewardedVideoAsync();
        if (result)
            DoublePlayerScore();
    }

    private void DoublePlayerScore()
    {
        SetMoneyToPlayer();
        gameUI.SetActimeDoubleScoreButton(false);
        gameUI.SetFinalScore(lockalPlayer.Score * 2);
    }

    private async UniTask StartCountingAsync(CancellationToken token)
    {
        var startCount = 3;

        gameUI.SetActiveStartCount(true);
        gameUI.UpdateStartCount(startCount);

        while (!token.IsCancellationRequested && startCount > 0)
        {
            await UniTask.Delay(1000);
            startCount--;
            gameUI.UpdateStartCount(startCount);
        }

        if (token.IsCancellationRequested)
            return;

        players.ForEach(p =>
        {
            p.SetInGame(true);
        });

        gameUI.SetActiveStartCount(false);
        GameTimeCountingAsync(gameSettingsManager.Map.PlayTime, token).Forget();
    }

    private string FormatingTimeToString(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        return (string.Format("{0:00}:{1:00}", minutes, seconds));
    }
}
