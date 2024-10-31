using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace GodhomeEloCounter
{
    public class GodhomeEloCounter : Mod
    {
        internal static GodhomeEloCounter Instance;

        public GodhomeEloCounter() : base("Godhome Elo Counter") {}

        public override string GetVersion() => "v0.1";

        private bool isPlayerFighting;
        private bool isPlayerDead;
        private string currentScene;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;

            ModHooks.BeforeSceneLoadHook += OnSceneLoad;
            ModHooks.TakeHealthHook += OnDamageTaken;
        }

        private int OnDamageTaken(int damage)
        {
            if (damage > PlayerData.instance.health)
            {
                isPlayerDead = true;
            }

            return damage;
        }

        private void OnBossEnter(string sceneName)
        {
            isPlayerFighting = true;
            isPlayerDead = false;

            currentScene = sceneName;
            Log($"Started fight against {currentScene}");
        }

        private void OnBossExit(string sceneName) 
        {
            if (isPlayerDead) { Log("Player Lost"); }
            else { Log("Player Won"); }

            isPlayerFighting = false;
            isPlayerDead = false;

            currentScene = sceneName;
        }

        private string OnSceneLoad(string name)
        {
            Log($"Loading new scene = {name}");

            if (name == "GG_Workshop" && isPlayerFighting) { OnBossExit(name); }
            if (currentScene == "GG_Workshop") { OnBossEnter(name); }

            return name;
        }

    }
}
