using Modding;
using Satchel.BetterMenus;

namespace GodhomeEloCounter
{
    public class ConfigMenu
    {
        private Menu MenuRef;
        private Menu UIMenuPageRef;

        public MenuScreen GetScreen(MenuScreen modListMenu, ModToggleDelegates? modtoggledelegates)
        {
            GodhomeEloCounter mod = GodhomeEloCounter.Instance;
            Config config = mod.config;

            UIMenuPageRef ??= new Menu
            (
                name: "UI Customization",
                elements: [
                    new HorizontalOption
                    (
                        name: "Hide Boss Name",
                        description: "Boss name will not be displayed",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            config.hideBossName = value == 0;
                            mod.UI.DrawBossStats();
                        },
                        loadSetting: () => config.hideBossName ? 0 : 1
                    ),
                    new HorizontalOption
                    (
                        name: "Hide Winstreak",
                        description: "Winstreak will not be displayed",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            config.hideWinstreak = value == 0;
                            mod.UI.DrawBossStats();
                        },
                        loadSetting: () => config.hideWinstreak ? 0 : 1
                    ),
                    new HorizontalOption
                    (
                        name: "Hide Wins/Losses",
                        description: "Wins/Losses will not be displayed",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            config.hideWinsLosses = value == 0;
                            mod.UI.DrawBossStats();
                        },
                        loadSetting: () => config.hideWinsLosses ? 0 : 1
                    ),
                    new HorizontalOption
                    (
                        name: "Hide Time Spent",
                        description: "Time Spent will not be displayed",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            config.hideTimeSpent = value == 0;
                            mod.UI.DrawBossStats();
                        },
                        loadSetting: () => config.hideTimeSpent ? 0 : 1
                    ),
                    new HorizontalOption
                    (
                        name: "Hide Combo",
                        description: "Combo will not be displayed",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            config.hideCombo = value == 0;
                            mod.UI.DrawBossStats();
                        },
                        loadSetting: () => config.hideCombo ? 0 : 1
                    ),
                    new HorizontalOption
                    (
                        name: "Hide Match History",
                        description: "Match History will not be displayed",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            config.hideMatchHistory = value == 0;
                            mod.UI.DrawBossStats();
                        },
                        loadSetting: () => config.hideMatchHistory ? 0 : 1
                    ),
                ]
            );

            MenuRef ??= new Menu
            (
                name: "Godhome Elo Counter",
                elements:
                [
                    Blueprints.CreateToggle
                    (
                        toggleDelegates: modtoggledelegates.Value,
                        name: "Elo Counter Enabled",
                        description: "Let you disable this mod entirely"
                    ),
                    new HorizontalOption
                    (
                        name: "Base ELO",
                        description: "How much ELO you should start with",
                        values: ["800", "900", "1000", "1100", "1200", "1300", "1400", "1500", "1600"],
                        applySetting: index =>
                        {
                            config.baseELO = index switch
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
                        loadSetting: () => config.baseELO switch
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
                    new HorizontalOption
                    (
                        name: "Natural Time",
                        description: "Ex: 12.3 Hours",
                        values: ["Yes", "No"],
                        applySetting: (value) => config.naturalTime = value == 0,
                        loadSetting: () => config.naturalTime ? 0 : 1
                    ),
                    new HorizontalOption
                    (
                        name: "Hide UI in combat",
                        description: "Stats are hidden when fighting bosses",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            config.hideUIinFights = value == 0;
                            if (config.hideUIinFights) mod.UI.Clear();
                            else mod.UI.DrawBossStats();
                        },
                        loadSetting: () => config.hideUIinFights ? 0 : 1
                    ),
                    new HorizontalOption
                    (
                        name: "Hide UI in Hall of Gods",
                        description: "Stats are hidden outside of fights",
                        values: ["Yes", "No"],
                        applySetting: (value) => {
                            config.hideUIinHoG = value == 0;
                            if (config.hideUIinHoG) mod.UI.Clear();
                        },
                        loadSetting: () => config.hideUIinHoG ? 0 : 1
                    ),
                    Blueprints.NavigateToMenu
                    (
                        name: "UI Customization",
                        description: "Customize the UI",
                        getScreen: () => UIMenuPageRef.GetMenuScreen(MenuRef.menuScreen)
                    ),
                    new MenuButton
                    (
                        name: "Reset ELO",
                        description: "Reset your ELO for all bosses",
                        submitAction: (_) =>
                        {
                            MenuRef.ShowDialog(Blueprints.CreateDialogMenu
                            (
                                title: "Reset ELO ?",
                                subTitle: "Your ELO will be lost forever",
                                Options: ["Yes", "No"],
                                OnButtonPress: (selection) =>
                                {
                                    switch (selection)
                                    {
                                        case "Yes":
                                            mod._localData.bosses.ForEach(boss => {
                                                boss.elo = config.baseELO;
                                                boss.lastElo = config.baseELO;
                                                boss.peakElo = config.baseELO;
                                            });
                                            Utils.GoToMenuScreen(MenuRef.menuScreen);
                                            mod.UI.Clear();
                                            break;
                                        case "No":
                                            Utils.GoToMenuScreen(MenuRef.menuScreen);
                                            break;
                                    }
                                }
                            ));
                        }
                    ),
                    new MenuButton
                    (
                        name: "Reset Last Boss ELO",
                        description: "Reset your ELO for the last boss and difficulty",
                        submitAction: (_) =>
                        {
                            MenuRef.ShowDialog(Blueprints.CreateDialogMenu
                            (
                                title: "Reset Last Boss ELO ?",
                                subTitle: "Your ELO will be lost forever",
                                Options: ["Yes", "No"],
                                OnButtonPress: (selection) =>
                                {
                                    switch (selection)
                                    {
                                        case "Yes":
                                            Boss boss = mod._localData.bosses
                                                .Find(b => b.tier == mod.bossFight.Tier && b.sceneName == mod.sceneManager.LastBossScene);
                                            boss.elo = config.baseELO;
                                            boss.lastElo = config.baseELO;
                                            boss.peakElo = config.baseELO;

                                            Utils.GoToMenuScreen(MenuRef.menuScreen);
                                            mod.UI.Clear();
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
    }
}
