using MagicUI.Core;
using Modding;
using System;
using System.Collections.Generic;
using UnityEngine;
using Satchel.BetterMenus;

namespace GodhomeEloCounter
{
    public class GodhomeEloCounter : Mod, ILocalSettings<LocalData>, ICustomMenuMod
    {
        internal static GodhomeEloCounter Instance;

        public GodhomeEloCounter() : base("Godhome Elo Counter") {}

        public override string GetVersion() => "0.1";

        private LocalData _localData = new LocalData();
        public void OnLoadLocal(LocalData data) => _localData = data;
        public LocalData OnSaveLocal() => _localData;

        private bool isPlayerFighting;
        private bool isPlayerDead;
        private string currentScene;
        private string lastBossScene;
        private readonly List<string> whitelistedScenes = ["GG_Workshop", "GG_Atrium"];

        private int tier = 0;

        private DateTime _startTime;
        private DateTime _endTime;

        private Menu MenuRef;

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modtoggledelegates) 
        {
            MenuRef ??= new Menu(
                        name: "Godhome Elo Counter",
                        elements:
                        [
                            new HorizontalOption(
                                name: "Base ELO",
                                description: "How much ELO you should start with",
                                values: ["800", "900", "1000", "1100", "1200", "1300", "1400", "1500", "1600"],
                                applySetting: index =>
                                {
                                    // GlobalSettings.health = index switch
                                    // {
                                    //     0 => 800,
                                    //     1 => 900,
                                    //     2 => 1000,
                                    //     3 => 1100,
                                    //     4 => 1200,
                                    //     5 => 1300,
                                    //     6 => 1400,
                                    //     7 => 1500,
                                    //     8 => 1600,
                                    //     _ => 1000
                                    // };
                                    // return 1000;
                                },
                                loadSetting: () => 
                                {
                                    // GlobalSettings.health switch
                                    // {
                                    //     800 => 0,
                                    //     900 => 1,
                                    //     1000 => 2,
                                    //     1100 => 3,
                                    //     1200 => 4,
                                    //     1300 => 5,
                                    //     1400 => 6,
                                    //     1500 => 7,
                                    //     1600 => 8,
                                    //     _ => 2
                                    // }
                                    return 2;
                                }
                            ),
                            new MenuButton(
                                name: "Reset ELO",
                                description: "Reset your ELO for all bosses", 
                                submitAction: (_) => {
                                    MenuRef.ShowDialog(Blueprints.CreateDialogMenu(
                                        title: "Reset ELO ?",
                                        subTitle: "Your ELO will be lost forever",
                                        Options: ["Yes", "No"],
                                        OnButtonPress: (selection) =>
                                        {
                                            switch (selection)
                                            {
                                                case "Yes": 
                                                    _localData.bosses.ForEach(boss => {
                                                        boss.elo = 1000.0;
                                                        boss.lastElo = 1000.0;
                                                        boss.peakElo = 1000.0;
                                                    });
                                                    Utils.GoToMenuScreen(MenuRef.menuScreen);
                                                    ClearUI();
                                                    break;
                                                case "No":
                                                    Utils.GoToMenuScreen(MenuRef.menuScreen);
                                                    break;
                                            }
                                        }
                                    ));
                                }
                            ),
                            new MenuButton(
                                name: "Reset Last Boss ELO",
                                description: "Reset your ELO for the last boss and difficulty", 
                                submitAction: (_) => {
                                    MenuRef.ShowDialog(Blueprints.CreateDialogMenu(
                                        title: "Reset Last Boss ELO ?",
                                        subTitle: "Your ELO will be lost forever",
                                        Options: ["Yes", "No"],
                                        OnButtonPress: (selection) =>
                                        {
                                            switch (selection)
                                            {
                                                case "Yes": 
                                                    Boss boss = _localData.bosses
                                                        .Find(b => b.tier == tier && b.sceneName == lastBossScene);
                                                    boss.elo = 1000.0;
                                                    boss.lastElo = 1000.0;
                                                    boss.peakElo = 1000.0;

                                                    Utils.GoToMenuScreen(MenuRef.menuScreen);
                                                    ClearUI();
                                                    break;
                                                case "No":
                                                    Utils.GoToMenuScreen(MenuRef.menuScreen);
                                                    break;
                                            }
                                        }
                                    ));
                                }
                            ),
                        ]
            );

            return MenuRef.GetMenuScreen(modListMenu);
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;

            ModHooks.BeforeSceneLoadHook += OnSceneLoad;
            ModHooks.TakeHealthHook += OnDamageTaken;
            On.BossChallengeUI.LoadBoss_int += OnBossLevelSelect;
            On.BossChallengeUI.Setup += OnBossLevelMenu;
            On.BossSummaryBoard.Show += OnBossSummaryBoardShow;
            On.BossSummaryBoard.Hide += OnBossSummaryBoardHide;
        }

        private void OnBossSummaryBoardShow(On.BossSummaryBoard.orig_Show orig, BossSummaryBoard self)
        {
            ClearUI();

            LayoutRoot layout = new(true);
            ModUI.SpawnGlobalStatsUI(layout, _localData);
            layouts.Add(layout);

            orig(self);
        }

        private void OnBossSummaryBoardHide(On.BossSummaryBoard.orig_Hide orig, BossSummaryBoard self)
        {
            ClearUI();
            orig(self);
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
            lastBossScene = currentScene;

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

            if (name.StartsWith("GG_Atrium") || !name.StartsWith("GG_")) {
                ClearUI();
                currentScene = name;
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

        public void Unload()
        {
            ModHooks.BeforeSceneLoadHook -= OnSceneLoad;
            ModHooks.TakeHealthHook -= OnDamageTaken;
            On.BossChallengeUI.LoadBoss_int -= OnBossLevelSelect;
            On.BossChallengeUI.Setup -= OnBossLevelMenu;
            On.BossSummaryBoard.Show -= OnBossSummaryBoardShow;
            On.BossSummaryBoard.Hide -= OnBossSummaryBoardHide;

            ClearUI();
        }
    }
}

// TODO: Global Toggle
// TODO: Unload Mod
// TODO: Show/Hide Keybind
// TODO: Reset ELO
// TODO: UI Customization
// TODO: Recent Games
// TODO: Destroy UI when quitting

