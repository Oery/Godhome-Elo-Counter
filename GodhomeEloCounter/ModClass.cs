using Modding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GodhomeEloCounter
{
    public class GodhomeEloCounter : Mod, ILocalSettings<LocalData>
    {
        internal static GodhomeEloCounter Instance;

        public GodhomeEloCounter() : base("Godhome Elo Counter") {}

        public override string GetVersion() => "v0.1";

        private LocalData _localData = new LocalData();
        public void OnLoadLocal(LocalData data) => _localData = data;
        public LocalData OnSaveLocal() => _localData;

        private bool isPlayerFighting;
        private bool isPlayerDead;
        private string currentScene;

        private DateTime _startTime;
        private DateTime _endTime;

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
            Log($"Entered Boss = {sceneName}");
            isPlayerFighting = true;
            isPlayerDead = false;

            currentScene = sceneName;
            _startTime = DateTime.Now;
            Log($"Started fight against {currentScene}");
        }

        private void OnBossExit(string sceneName) 
        {
            bool has_won;

            if (isPlayerDead) { has_won = false; }
            else { has_won = true; }

            isPlayerFighting = false;
            isPlayerDead = false;

            _endTime = DateTime.Now;

            TimeSpan timeSpan = _endTime - _startTime;
            _localData.UpdateBoss(currentScene, has_won, timeSpan);

            currentScene = sceneName;

            Log("Finished fight against " + currentScene);
        }

        private string OnSceneLoad(string name)
        {
            Log($"Loading new scene = {name}");

            if (name == "GG_Workshop" && isPlayerFighting) { OnBossExit(name); }
            if (currentScene == "GG_Workshop" && name != currentScene) { OnBossEnter(name); }

            if (name == "GG_Workshop") { currentScene = name; }

            return name;
        }
    }
}
