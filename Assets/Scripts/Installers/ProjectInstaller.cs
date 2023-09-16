using Monetization.Ads.Providers;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private CarsExamples carsExamples;

    public override void InstallBindings()
    {
        Container
            .Bind<CarsExamples>()
            .FromInstance(carsExamples)
            .AsSingle();

        Container
            .BindInterfacesTo<PlayerSettings>()
            .AsSingle();

        Container
            .BindInterfacesTo<PlayerInventory>()
            .AsSingle();

        Container
            .BindInterfacesTo<GameSettingsManager>()
            .AsSingle();

        Container
            .BindInterfacesTo<SaveManager>()
            .AsSingle();

        Container
            .BindInterfacesTo<IronSourceProvider>()
            .AsSingle();
    }
}