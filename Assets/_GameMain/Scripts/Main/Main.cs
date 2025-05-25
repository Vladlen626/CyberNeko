using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class Main : IInitializable, IDisposable
{
    private readonly EnemyController _enemyController;
    private readonly NpcController _npcController;
    private readonly PlayerSpawner _playerSpawner;
    private readonly FoodSpawner _foodSpawner;
    private readonly PointsManager _pointsManager;
    private readonly UIManager _uiManager;
    private readonly DoorConnectionManager _doorConnectionManager;

    private CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public Main(
        EnemyController enemyController,
        NpcController npcController,
        PlayerSpawner playerSpawner,
        FoodSpawner foodSpawner,
        PointsManager pointsManager,
        UIManager uiManager,
        DoorConnectionManager doorConnectionManager)
    {
        _enemyController = enemyController;
        _npcController = npcController;
        _playerSpawner = playerSpawner;
        _foodSpawner = foodSpawner;
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
        _uiManager.Menu.OnRestart -= HandlerOnRestart;
    }

    // _____________ Flow _____________

    private async UniTaskVoid StartGame()
    {
        await InitializeAll();
        await StartRoundAsync();
    }

    private async UniTask InitializeAll()
    {
        _uiManager.Initialize();
        _uiManager.Menu.OnRestart += HandlerOnRestart;

        _pointsManager.Initialize();
        _enemyController.Initialize();
        _foodSpawner.Initialize();
        _npcController.Initialize();
        
        _playerSpawner.Initialize();

        await UniTask.Yield();
    }

    private async UniTask StartRoundAsync()
    {
        _npcController.SpawnNpc();
        _foodSpawner.ResetAllFood();
        await _enemyController.SpawnEnemies();
        
        _playerSpawner.RespawnPlayer();
        SubscribePlayerGrabbed(_playerSpawner.GetPlayerController());

        await UniTask.WaitForSeconds(2f, true);
        _uiManager.HideBlackScreen();
        await UniTask.WaitForSeconds(0.15f, true);
    }

    private async UniTask GameOverAsync()
    {
        await UniTask.WaitForSeconds(1f, true);
        AudioManager.inst.PlaySound(SoundNames.GameOver);
        _uiManager.Menu.GameOver(_pointsManager.CurrentPoints.Value);
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
    
    // _____________ Flow-end _____________
    
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

    private void QuitGame()
    {
        Application.Quit();
    }
}