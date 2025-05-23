using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class Main : IInitializable, IDisposable
{
    private readonly EnemyController _enemyController;
    private readonly PlayerSpawner _playerSpawner;
    private readonly FoodManager _foodManager;
    private readonly PointsManager _pointsManager;
    private readonly UIManager _uiManager;
    private readonly DoorConnectionManager _doorConnectionManager;

    private CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public Main(
        EnemyController enemyController,
        PlayerSpawner playerSpawner,
        FoodManager foodManager,
        PointsManager pointsManager,
        UIManager uiManager,
        DoorConnectionManager doorConnectionManager)
    {
        _enemyController = enemyController;
        _playerSpawner = playerSpawner;
        _foodManager = foodManager;
        _pointsManager = pointsManager;
        _uiManager = uiManager;
        _doorConnectionManager = doorConnectionManager;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Initialize()
    {
        StartGame().Forget();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Dispose()
    {
        _disposables?.Dispose();
        UnsubscribePlayerGrabbed(_playerSpawner.GetPlayerController());
        _uiManager.GetMenu().OnRestart -= HandlerOnRestart;
    }

    private async UniTaskVoid StartGame()
    {
        await InitializeAll();
        await StartRoundAsync();
    }

    private async UniTask InitializeAll()
    {
        _uiManager.Initialize();
        _uiManager.GetMenu().OnRestart += HandlerOnRestart;

        _pointsManager.Initialize();
        _enemyController.Initialize();
        _foodManager.Initialize();

        _playerSpawner.Initialize();

        await UniTask.Yield();
    }

    private async UniTask StartRoundAsync()
    {
        _foodManager.RespawnFood();
        _playerSpawner.RespawnPlayer();
        SubscribePlayerGrabbed(_playerSpawner.GetPlayerController());

        await _enemyController.SpawnEnemies();

        await UniTask.WaitForSeconds(1f, true);
        _uiManager.HideBlackScreen();
        await UniTask.WaitForSeconds(0.15f, true);
    }

    private void SubscribePlayerGrabbed(PlayerController _player)
    {
        UnsubscribePlayerGrabbed(_player);
        if (_player != null)
            _player.OnGrabbed += HandlerOnGameOver;
    }

    private void UnsubscribePlayerGrabbed(PlayerController _player)
    {
        if (_player != null)
            _player.OnGrabbed -= HandlerOnGameOver;
    }

    private void HandlerOnGameOver()
    {
        GameOverAsync().Forget();
    }

    private void HandlerOnRestart()
    {
        Restart().Forget();
    }

    private async UniTask GameOverAsync()
    {
        await UniTask.WaitForSeconds(1f, true);
        AudioManager.inst.PlaySound(SoundNames.GameOver);
        _uiManager.GetMenu().GameOver(_pointsManager.CurrentPoints.Value);
    }

    private async UniTask Restart()
    {
        _uiManager.ShowBlackScreen();
        _pointsManager.ResetAllPoints();
        await UniTask.WaitForSeconds(0.5f, true);
        await StartRoundAsync();
    }

    private async UniTask Final()
    {
        await UniTask.WaitForSeconds(2f, true);
        AudioManager.inst.PlaySound(SoundNames.Win);
        _uiManager.Win();
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}