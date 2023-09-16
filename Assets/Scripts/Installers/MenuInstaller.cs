using UnityEngine;
using Zenject;

public class MenuInstaller : MonoInstaller
{
    [SerializeField] private UiController uiController;
    [SerializeField] private ObjectPoolController poolController;
    [SerializeField] private AudioManager audioManager;
 
    public override void InstallBindings()
    {
        Container
            .Bind<ObjectPoolController>()
            .FromInstance(poolController)
            .AsSingle();

        Container
            .Bind<UiController>()
            .FromInstance(uiController)
            .AsSingle();

        Container
            .Bind<AudioManager>()
            .FromInstance(audioManager)
            .AsSingle();
    }
}