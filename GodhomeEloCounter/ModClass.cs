using MagicUI.Core;
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
        private List<string> whitelistedScenes = new List<string>() { "GG_Workshop", "GG_Atrium" };

        private int tier = 0;

        private DateTime _startTime;
        private DateTime _endTime;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;

            ModHooks.BeforeSceneLoadHook += OnSceneLoad;
            ModHooks.TakeHealthHook += OnDamageTaken;
            On.BossChallengeUI.LoadBoss_int += OnBossLevelSelect;
        }

        private void OnBossLevelSelect(On.BossChallengeUI.orig_LoadBoss_int orig, BossChallengeUI self, int level)
        {
            tier = level;
            orig(self, level);
        }

        public bool ToggleButtonInsideMenu => true;

        private List<LayoutRoot> layouts = [];

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

            RefreshUI(sceneName, tier);
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
            _localData.UpdateBoss(currentScene, tier, has_won, timeSpan);

            Log("Finished fight against " + currentScene);

            RefreshUI(currentScene, tier);

            currentScene = sceneName;
        }

        private string OnSceneLoad(string name)
        {
            Log($"Loading new scene = {name}");

            if (name == "GG_Workshop" && isPlayerFighting) { OnBossExit(name); }
            if (currentScene == "GG_Workshop" && !whitelistedScenes.Contains(name)) { OnBossEnter(name); }

            if (name == "GG_Workshop") { currentScene = name; }

            if (name == "GG_Atrium" || !name.StartsWith("GG_")) {
                ClearUI();
            }

            return name;
        }

        private void RefreshUI(string sceneName, int tier)
        {
            ClearUI();
            LayoutRoot layout_ui = new(true);
            ModUI.SpawnBossUI(layout_ui, _localData.FindOrCreateBoss(sceneName, tier));
            layouts.Add(layout_ui);
        }

        private void ClearUI()
        {
            foreach (var layout in layouts)
            {
                layout.Destroy();
            }
            layouts.Clear();
        }
    }
}
