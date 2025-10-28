using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using Modding;

namespace GodhomeEloCounter
{
    public class GodhomeEloCounter : Mod, ILocalSettings<LocalData>, IGlobalSettings<Config>, ICustomMenuMod, ITogglableMod
    {
        internal static GodhomeEloCounter Instance;

        public GodhomeEloCounter() : base("Godhome Elo Counter") { }

        public override string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        // LocalSettings Interface
        public LocalData _localData = new();
        public void OnLoadLocal(LocalData data) => _localData = data;
        public LocalData OnSaveLocal() => _localData;

        // GlobalSettings Interface
        public Config config = new();
        public void OnLoadGlobal(Config data) => config = data;
        public Config OnSaveGlobal() => config;

        // TogglableMod Interface
        public bool ToggleButtonInsideMenu => true;

        // CustomMenuMod Interface
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modtoggledelegates)
        {
            return configMenu.GetScreen(modListMenu, modtoggledelegates);
        }

        // GodhomeEloCounter
        public readonly ConfigMenu configMenu = new();
        public readonly SceneManager sceneManager = new();
        public readonly UI UI = new();
        public BossFight bossFight;

        public int selectedTier = 0;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;

            ModHooks.BeforeSceneLoadHook += OnSceneLoad;
            ModHooks.TakeHealthHook += OnDamageTaken;
            On.BossChallengeUI.LoadBoss_int += OnBossLevelSelect;
            On.BossChallengeUI.Setup += OnBossLevelMenu;
            On.BossSummaryBoard.Show += OnBossSummaryBoardShow;
            On.BossSummaryBoard.Hide += OnBossSummaryBoardHide;
            // On.HealthManager.TakeDamage += OnTakeDamage;
            On.QuitToMenu.Start += OnQuitToMenuStart;
        }

        public void Unload()
        {
            UI.Clear();

            ModHooks.BeforeSceneLoadHook -= OnSceneLoad;
            ModHooks.TakeHealthHook -= OnDamageTaken;
            On.BossChallengeUI.LoadBoss_int -= OnBossLevelSelect;
            On.BossChallengeUI.Setup -= OnBossLevelMenu;
            On.BossSummaryBoard.Show -= OnBossSummaryBoardShow;
            On.BossSummaryBoard.Hide -= OnBossSummaryBoardHide;
            On.QuitToMenu.Start -= OnQuitToMenuStart;
        }

        private string OnSceneLoad(string name)
        {
            sceneManager.HandleSceneChange(name);
            return name;
        }

        // TODO: Replace This with an OnDeath event
        private int OnDamageTaken(int damage)
        {
            bossFight.IsPlayerDead = damage >= PlayerData.instance.health;
            return damage;
        }

        // private void OnTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        // {
        //     orig(self, hitInstance);
        //     bossFight.UpdateComboCounter(hitInstance);
        // }

        private IEnumerator OnQuitToMenuStart(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            UI.Clear();
            return orig(self);
        }

        private void OnBossSummaryBoardShow(On.BossSummaryBoard.orig_Show orig, BossSummaryBoard self)
        {
            if (!config.hideUIinHoG) UI.DrawGlobalStats();
            orig(self);
        }

        private void OnBossSummaryBoardHide(On.BossSummaryBoard.orig_Hide orig, BossSummaryBoard self)
        {
            UI.Clear();
            orig(self);
        }

        private void OnBossLevelMenu(On.BossChallengeUI.orig_Setup orig, BossChallengeUI self, BossStatue bossStatue, string bossNameSheet, string bossNameKey, string descriptionSheet, string descriptionKey)
        {
            if (!config.hideUIinHoG) UI.DrawBossTiersStats(bossNameKey);
            orig(self, bossStatue, bossNameSheet, bossNameKey, descriptionSheet, descriptionKey);
        }

        private void OnBossLevelSelect(On.BossChallengeUI.orig_LoadBoss_int orig, BossChallengeUI self, int level)
        {
            selectedTier = level;
            orig(self, level);
        }
    }
}
