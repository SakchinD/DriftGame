using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private AudioManager audioManager;

    public override void InstallBindings()
    {
        Container
            .Bind<AudioManager>()
            .FromInstance(audioManager)
            .AsSingle();
    }
}