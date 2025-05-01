using Zenject;
using DI.Signals;
using UnityEngine;


namespace DI.Installers
{
    public class Installer: MonoInstaller
    {

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.Bind<ScoreService>().AsSingle();

            Debug.Log(Container.Settings.Signals.RequireStrictUnsubscribe);

            Container.DeclareSignal<GameStartedSignal>();
            Container.DeclareSignal<GameOverSignal>();
            Container.DeclareSignal<ScoreChangedSignal>();
        }

    }
}