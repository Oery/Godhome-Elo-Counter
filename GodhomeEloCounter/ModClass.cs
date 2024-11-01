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
        private readonly List<string> whitelistedScenes = ["GG_Workshop", "GG_Atrium"];

        private int tier = 0;

        private DateTime _startTime;
        private DateTime _endTime;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;

            ModHooks.BeforeSceneLoadHook += OnSceneLoad;
            ModHooks.TakeHealthHook += OnDamageTaken;
            On.BossChallengeUI.LoadBoss_int += OnBossLevelSelect;
            On.BossChallengeUI.Setup += OnBossLevelMenu;
        }

        private void OnBossLevelMenu(On.BossChallengeUI.orig_Setup orig, BossChallengeUI self, BossStatue bossStatue, string bossNameSheet, string bossNameKey, string descriptionSheet, string descriptionKey)
        {
            ClearUI();
            LayoutRoot layout = new(true);
            ModUI.SpawnAllTierBossUI(layout, _localData, bossNameKey);
            layouts.Add(layout);

            orig(self, bossStatue, bossNameSheet, bossNameKey, descriptionSheet, descriptionKey);
        }

        private void OnBossLevelSelect(On.BossChallengeUI.orig_LoadBoss_int orig, BossChallengeUI self, int level)
        {
            tier = level;
            orig(self, level);
        }

        public bool ToggleButtonInsideMenu => true;

        private readonly List<LayoutRoot> layouts = [];

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

            if (currentScene.EndsWith("_V")) { currentScene = currentScene.Substring(0, currentScene.Length - 2); }

            _startTime = DateTime.Now;
            RefreshUI(currentScene, tier);
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

            RefreshUI(currentScene, tier);

            currentScene = sceneName;
        }

        private string OnSceneLoad(string name)
        {
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
