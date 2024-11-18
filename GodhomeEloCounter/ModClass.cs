using MagicUI.Core;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Satchel.BetterMenus;

namespace GodhomeEloCounter
{
    public class GodhomeEloCounter : Mod, ILocalSettings<LocalData>, IGlobalSettings<ModSettings>, ICustomMenuMod, ITogglableMod
    {
        internal static GodhomeEloCounter Instance;

        public GodhomeEloCounter() : base("Godhome Elo Counter") {}

        public override string GetVersion() => "1.1";

        private LocalData _localData = new();
        public void OnLoadLocal(LocalData data) => _localData = data;
        public LocalData OnSaveLocal() => _localData;

        private ModSettings modSettings = new();
        public void OnLoadGlobal(ModSettings data) => modSettings = data;
        public ModSettings OnSaveGlobal() => modSettings;

        private bool isPlayerFighting;
        private bool isPlayerDead;
        private string currentScene;
        private string lastBossScene;
        private readonly List<string> whitelistedScenes = ["GG_Workshop", "GG_Atrium"];

        private int tier = 0;

        private DateTime _startTime;
        private DateTime _endTime;

        private Menu MenuRef;
        private Menu UIMenuPageRef;

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modtoggledelegates) 
        {
            UIMenuPageRef ??= new Menu(
                name: "UI Customization",
                elements: [
                    new HorizontalOption(
                        name: "Hide Boss Name",
                        description: "Boss name will not be displayed",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            modSettings.hideBossName = value == 0;
                            RefreshUI(currentScene, tier);
                        },
                        loadSetting: () => modSettings.hideBossName ? 0 : 1
                    ),
                    new HorizontalOption(
                        name: "Hide Winstreak",
                        description: "Winstreak will not be displayed",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            modSettings.hideWinstreak = value == 0;
                            RefreshUI(currentScene, tier);
                        },
                        loadSetting: () => modSettings.hideWinstreak ? 0 : 1
                    ),
                    new HorizontalOption(
                        name: "Hide Wins/Losses",
                        description: "Wins/Losses will not be displayed",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            modSettings.hideWinsLosses = value == 0;
                            RefreshUI(currentScene, tier);
                        },
                        loadSetting: () => modSettings.hideWinsLosses ? 0 : 1
                    ),
                    new HorizontalOption(
                        name: "Hide Time Spent",
                        description: "Time Spent will not be displayed",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            modSettings.hideTimeSpent = value == 0;
                            RefreshUI(currentScene, tier);
                        },
                        loadSetting: () => modSettings.hideTimeSpent ? 0 : 1
                    ),
                    new HorizontalOption(
                        name: "Hide Match History",
                        description: "Match History will not be displayed",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            modSettings.hideMatchHistory = value == 0;
                            RefreshUI(currentScene, tier);
                        },
                        loadSetting: () => modSettings.hideMatchHistory ? 0 : 1
                    ),
                ]
            );

            MenuRef ??= new Menu(
                        name: "Godhome Elo Counter",
                        elements:
                        [
                            Blueprints.CreateToggle(
                                toggleDelegates: modtoggledelegates.Value,
                                name: "Elo Counter Enabled",  
                                description: "Let you disable this mod entirely"
                            ),
                            new HorizontalOption(
                                name: "Base ELO",
                                description: "How much ELO you should start with",
                                values: ["800", "900", "1000", "1100", "1200", "1300", "1400", "1500", "1600"],
                                applySetting: index =>
                                {
                                    modSettings.baseELO = index switch
                                    {
                                        0 => 800,
                                        1 => 900,
                                        2 => 1000,
                                        3 => 1100,
                                        4 => 1200,
                                        5 => 1300,
                                        6 => 1400,
                                        7 => 1500,
                                        8 => 1600,
                                        _ => 1000
                                    };
                                },
                                loadSetting: () => modSettings.baseELO switch
                                {
                                    800 => 0,
                                    900 => 1,
                                    1000 => 2,
                                    1100 => 3,
                                    1200 => 4,
                                    1300 => 5,
                                    1400 => 6,
                                    1500 => 7,
                                    1600 => 8,
                                    _ => 2
                                }
                            ),
                            new HorizontalOption(
                                name: "Hide UI in combat",
                                description: "Stats are hidden when fighting bosses",
                                values: ["Yes", "No"],
                                applySetting: (value) => {
                                    modSettings.hideUIinFights = value == 0;
                                    if (modSettings.hideUIinFights) ClearUI();
                                    else RefreshUI(currentScene, tier);
                                },
                                loadSetting: () => modSettings.hideUIinFights ? 0 : 1
                            ),
                            new HorizontalOption(
                                name: "Hide UI in Hall of Gods",
                                description: "Stats are hidden outside of fights",
                                values: ["Yes", "No"],
                                applySetting: (value) => {
                                    modSettings.hideUIinHoG = value == 0;
                                    if (modSettings.hideUIinHoG) ClearUI();
                                },
                                loadSetting: () => modSettings.hideUIinHoG ? 0 : 1
                            ),
                            Blueprints.NavigateToMenu(
                                name: "UI Customization",
                                description: "Customize the UI",
                                getScreen: () => UIMenuPageRef.GetMenuScreen(MenuRef.menuScreen)
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
                                                        boss.elo = modSettings.baseELO;
                                                        boss.lastElo = modSettings.baseELO;
                                                        boss.peakElo = modSettings.baseELO;
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
                                                    boss.elo = modSettings.baseELO;
                                                    boss.lastElo = modSettings.baseELO;
                                                    boss.peakElo = modSettings.baseELO;

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
            On.QuitToMenu.Start += OnQuitToMenuStart;
        }

        private IEnumerator OnQuitToMenuStart(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            ClearUI();
            return orig(self);
        }

        private void OnBossSummaryBoardShow(On.BossSummaryBoard.orig_Show orig, BossSummaryBoard self)
        {
            ClearUI();

            if (!modSettings.hideUIinHoG)
            {
                LayoutRoot layout = new(true);
                ModUI.SpawnGlobalStatsUI(layout, _localData);
                layouts.Add(layout);
            }

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

            if (!modSettings.hideUIinHoG)
            {
                LayoutRoot layout = new(true);
                ModUI.SpawnAllTierBossUI(layout, _localData, bossNameKey, modSettings.baseELO);
                layouts.Add(layout);
            }

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

            if (currentScene.EndsWith("_V") && currentScene != "GG_Mantis_Lords_V") { currentScene = currentScene.Substring(0, currentScene.Length - 2); }
            lastBossScene = currentScene;

            _startTime = DateTime.Now;

            if (modSettings.hideUIinFights) ClearUI();
            else RefreshUI(currentScene, tier);
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
            _localData.UpdateBoss(currentScene, tier, has_won, timeSpan, modSettings.baseELO);

            if (!modSettings.hideUIinHoG) RefreshUI(currentScene, tier);
            else ClearUI();

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
            ModUI.SpawnBossUI(layout_ui, _localData.FindOrCreateBoss(sceneName, tier, modSettings.baseELO), modSettings);
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
            On.QuitToMenu.Start -= OnQuitToMenuStart;

            ClearUI();
        }
    }
}
