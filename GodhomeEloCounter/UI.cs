using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicUI.Core;
using MagicUI.Elements;

namespace GodhomeEloCounter
{
    public class UI
    {
        private readonly List<LayoutRoot> layouts = [];

        public void Clear() 
        {
            foreach (LayoutRoot layout in layouts) layout.Destroy();
            layouts.Clear();
        }

        public void DrawBossStats() 
        {
            Clear();

            LocalData data = GodhomeEloCounter.Instance._localData;
            Config settings = GodhomeEloCounter.Instance.config;
            string bossName = GodhomeEloCounter.Instance.bossFight.Boss;
            int tier = GodhomeEloCounter.Instance.bossFight.Tier;

            Boss boss = data.FindOrCreateBoss(bossName, tier);

            StringBuilder textUI = new();

            // Display Boss Name
            if (!settings.hideBossName) textUI.Append($"{BossMappings.GetDisplayFromScene(boss.sceneName)}\n");

            // Display ELO
            textUI.Append($"Elo: {boss.RoundedElo()} ({boss.RoundedElo() - boss.RoundedLastElo()})\n");
            textUI.Append($"Peak: {boss.RoundedPeakElo()}\n");

            // Display Winstreak
            if (!settings.hideWinstreak) textUI.Append($"Streak: {boss.streak} / Best: {boss.bestWinStreak}\n");

            // Display Wins/Losses
            if (!settings.hideWinsLosses) textUI.Append($"Wins: {boss.wins} / Losses: {boss.losses}\n");

            // Display Time Spent
            if (!settings.hideTimeSpent) textUI.Append($"Time: {FormatTimeSpan(boss.timeSpent)}\n");

            // Display Combo
            int comboDamage = GodhomeEloCounter.Instance.bossFight.ComboDamage;
            int prevComboDamage = GodhomeEloCounter.Instance.bossFight.PrevComboDamage;
            if (!settings.hideCombo) textUI.Append($"Combo: {comboDamage} ({prevComboDamage})\n");

            // Display Match History
            if (!settings.hideMatchHistory) textUI.Append($"{boss.matchHistory}\n");

            LayoutRoot layout = new(true);

            new TextObject(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 20,
                Font = MagicUI.Core.UI.TrajanBold,
                Text = textUI.ToString(),
                Padding = new(20)
            };

            layouts.Add(layout);
        }

        public void DrawBossTiersStats(string statueName) 
        {
            Clear();

            LocalData data = GodhomeEloCounter.Instance._localData;
            string sceneName = BossMappings.GetSceneFromStatue(statueName);

            // If the scene name is null, it means that the statue doesn't have an arena scene linked, which means we cannot track data. Absent mappings will cause the game to open a bugged UI.
            if (sceneName == null) return;
            
            string textUI = "";
            textUI += $"{BossMappings.GetDisplayFromScene(sceneName)}\n\n";

            Boss attuned = data.FindOrCreateBoss(sceneName, 0);
            textUI += "Attuned\n";
            textUI +=  $"Elo: {attuned.RoundedElo()} ({attuned.RoundedElo() - attuned.RoundedLastElo()})\n";
            textUI +=  $"Peak: {attuned.RoundedPeakElo()}\n";
            textUI +=  $"Streak: {attuned.streak} / Best: {attuned.bestWinStreak}\n";
            textUI +=  $"Wins: {attuned.wins} / Losses: {attuned.losses}\n";
            textUI += $"Time: {FormatTimeSpan(attuned.timeSpent)}\n";
            if (attuned.matchHistory.Length > 0) textUI += $"{attuned.matchHistory}\n";

            Boss ascended = data.FindOrCreateBoss(sceneName, 1);
            textUI += "\nAscended\n";
            textUI +=  $"Elo: {ascended.RoundedElo()} ({ascended.RoundedElo() - ascended.RoundedLastElo()})\n";
            textUI +=  $"Peak: {ascended.RoundedPeakElo()}\n";
            textUI +=  $"Streak: {ascended.streak} / Best: {ascended.bestWinStreak}\n";
            textUI +=  $"Wins: {ascended.wins} / Losses: {ascended.losses}\n";
            textUI += $"Time: {FormatTimeSpan(ascended.timeSpent)}\n";
            if (ascended.matchHistory.Length > 0) textUI += $"{ascended.matchHistory}\n";

            Boss radiant = data.FindOrCreateBoss(sceneName, 2);
            textUI += "\nRadiant\n";
            textUI +=  $"Elo: {radiant.RoundedElo()} ({radiant.RoundedElo() - radiant.RoundedLastElo()})\n";
            textUI +=  $"Peak: {radiant.RoundedPeakElo()}\n";
            textUI +=  $"Streak: {radiant.streak} / Best: {radiant.bestWinStreak}\n";
            textUI +=  $"Wins: {radiant.wins} / Losses: {radiant.losses}\n";
            textUI += $"Time: {FormatTimeSpan(radiant.timeSpent)}\n";
            if (radiant.matchHistory.Length > 0) textUI += $"{radiant.matchHistory}\n";

            LayoutRoot layout = new(true);

            new TextObject(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 20,
                Font = MagicUI.Core.UI.TrajanBold,
                Text = textUI,
                Padding = new(20)
            };

            layouts.Add(layout);
        }

        public void DrawGlobalStats() 
        {
            Clear();

            LocalData data = GodhomeEloCounter.Instance._localData;
            StringBuilder textUI = new();
            textUI.Append($"Global Stats\n\n");

            int totalAttunedWins = data.bosses.Where(boss => boss.tier == 0).Sum(boss => boss.wins);
            int totalAscendedWins = data.bosses.Where(boss => boss.tier == 1).Sum(boss => boss.wins);
            int totalRadiantWins = data.bosses.Where(boss => boss.tier == 2).Sum(boss => boss.wins);

            int totalAttunedLosses = data.bosses.Where(boss => boss.tier == 0).Sum(boss => boss.losses);
            int totalAscendedLosses = data.bosses.Where(boss => boss.tier == 1).Sum(boss => boss.losses);
            int totalRadiantLosses = data.bosses.Where(boss => boss.tier == 2).Sum(boss => boss.losses);

            int averageAttunedELO = (int)Math.Round(data.bosses
                .Where(boss => boss.tier == 0 && boss.wins + boss.losses > 0)
                .Select(boss => boss.elo).Average());

            int averageAscendedELO = (int)Math.Round(data.bosses
                .Where(boss => boss.tier == 1 && boss.wins + boss.losses > 0)
                .Select(boss => boss.elo).Average());

            int averageRadiantELO = (int)Math.Round(data.bosses
                .Where(boss => boss.tier == 2 && boss.wins + boss.losses > 0)
                .Select(boss => boss.elo).Average());

            int globalELO = (int)Math.Round((averageAttunedELO + averageAscendedELO + averageRadiantELO) / 3.0, MidpointRounding.AwayFromZero);

            TimeSpan totalAttunedTime = data.bosses.Where(boss => boss.tier == 0)
                .Aggregate(TimeSpan.Zero, (sum, boss) => sum + boss.timeSpent);

            TimeSpan totalAscendedTime = data.bosses.Where(boss => boss.tier == 1)
                .Aggregate(TimeSpan.Zero, (sum, boss) => sum + boss.timeSpent);

            TimeSpan totalRadiantTime = data.bosses.Where(boss => boss.tier == 2)
                .Aggregate(TimeSpan.Zero, (sum, boss) => sum + boss.timeSpent);

            textUI.Append("Attuned\n");
            textUI.Append($"ELO: {averageAttunedELO}\n");
            textUI.Append($"Wins: {totalAttunedWins} / Losses: {totalAttunedLosses}\n");
            textUI.Append($"Time: {FormatTimeSpan(totalAttunedTime)}\n\n");

            textUI.Append("Ascended\n");
            textUI.Append($"ELO: {averageAscendedELO}\n");
            textUI.Append($"Wins: {totalAscendedWins} / Losses: {totalAscendedLosses}\n");
            textUI.Append($"Time: {FormatTimeSpan(totalAscendedTime)}\n\n");

            textUI.Append("Radiant\n");
            textUI.Append($"ELO: {averageRadiantELO}\n");
            textUI.Append($"Wins: {totalRadiantWins} / Losses: {totalRadiantLosses}\n");
            textUI.Append($"Time: {FormatTimeSpan(totalRadiantTime)}\n\n");

            textUI.Append("Grand Total\n");
            textUI.Append($"ELO: {globalELO}\n");
            textUI.Append($"Wins: {totalAttunedWins + totalAscendedWins + totalRadiantWins} / Losses: {totalAttunedLosses + totalAscendedLosses + totalRadiantLosses}\n");
            textUI.Append($"Time: {FormatTimeSpan(totalAttunedTime + totalAscendedTime + totalRadiantTime)}\n");

            LayoutRoot layout = new(true);

            new TextObject(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 20,
                Font = MagicUI.Core.UI.TrajanBold,
                Text = textUI.ToString(),
                Padding = new(20)
            };

            layouts.Add(layout);
        }

        public static string FormatTimeSpan(TimeSpan time)
        {
            if (GodhomeEloCounter.Instance.config.naturalTime)
            {
                if (time.TotalHours >= 1) return $"{time.TotalHours:F1} Hours";
                if (time.TotalMinutes >= 1) return $"{time.TotalMinutes:F1} Minutes";
                return $"{time.TotalSeconds:F1} Seconds";
            }

            int totalHours = (int)time.TotalHours;
            int minutes = time.Minutes;
            int seconds = time.Seconds;

            if (totalHours == 0) return $"{minutes:D2}:{seconds:D2}";
            if (totalHours < 100) return $"{totalHours:D2}:{minutes:D2}:{seconds:D2}";
            return $"{totalHours}:{minutes:D2}:{seconds:D2}";
        }
    }
}
