using System;
using System.Linq;
using MagicUI.Core;
using MagicUI.Elements;

namespace GodhomeEloCounter
{
    public static class ModUI
    {
        public static void SpawnBossUI(string sceneName, int tier) {
            GodhomeEloCounter.Instance.ClearUI();

            LocalData data = GodhomeEloCounter.Instance._localData;
            ModSettings settings = GodhomeEloCounter.Instance.modSettings;
            Boss boss = data.FindOrCreateBoss(sceneName, tier);
            string textUI = "";

            if (!settings.hideBossName) textUI += $"{BossMappings.GetDisplayFromScene(boss.sceneName)}\n";
            textUI += $"Elo: {boss.RoundedElo()} ({boss.RoundedElo() - boss.RoundedLastElo()})\n";
            textUI += $"Peak: {boss.RoundedPeakElo()}\n";
            if (!settings.hideWinstreak) textUI += $"Streak: {boss.streak} / Best: {boss.bestWinStreak}\n";
            if (!settings.hideWinsLosses) textUI += $"Wins: {boss.wins} / Losses: {boss.losses}\n";
            if (!settings.hideTimeSpent) textUI += $"Time: {FormatTimeSpan(boss.timeSpent)}\n";
            if (!settings.hideMatchHistory) textUI += $"{boss.matchHistory}\n";

            LayoutRoot layout = new(true);

            new TextObject(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 20,
                Font = UI.TrajanBold,
                Text = textUI,
                Padding = new(20)
            };

            GodhomeEloCounter.Instance.layouts.Add(layout);
        }

        public static void SpawnAllTierBossUI(string statueName) {
            GodhomeEloCounter.Instance.ClearUI();

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
                Font = UI.TrajanBold,
                Text = textUI,
                Padding = new(20)
            };

            GodhomeEloCounter.Instance.layouts.Add(layout);
        }

        public static void SpawnGlobalStatsUI() {
            GodhomeEloCounter.Instance.ClearUI();

            LocalData data = GodhomeEloCounter.Instance._localData;
            string textUI = "";
            textUI += $"Global Stats\n\n";

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

            textUI += "Attuned\n";
            textUI += $"ELO: {averageAttunedELO}\n";
            textUI += $"Wins: {totalAttunedWins} / Losses: {totalAttunedLosses}\n";
            textUI += $"Time: {FormatTimeSpan(totalAttunedTime)}\n\n";

            textUI += "Ascended\n";
            textUI += $"ELO: {averageAscendedELO}\n";
            textUI += $"Wins: {totalAscendedWins} / Losses: {totalAscendedLosses}\n";
            textUI += $"Time: {FormatTimeSpan(totalAscendedTime)}\n\n";

            textUI += "Radiant\n";
            textUI += $"ELO: {averageRadiantELO}\n";
            textUI += $"Wins: {totalRadiantWins} / Losses: {totalRadiantLosses}\n";
            textUI += $"Time: {FormatTimeSpan(totalRadiantTime)}\n\n";

            textUI += "Grand Total\n";
            textUI += $"ELO: {globalELO}\n";
            textUI += $"Wins: {totalAttunedWins + totalAscendedWins + totalRadiantWins} / Losses: {totalAttunedLosses + totalAscendedLosses + totalRadiantLosses}\n";
            textUI += $"Time: {FormatTimeSpan(totalAttunedTime + totalAscendedTime + totalRadiantTime)}\n";

            LayoutRoot layout = new(true);

            new TextObject(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 20,
                Font = UI.TrajanBold,
                Text = textUI,
                Padding = new(20)
            };

            GodhomeEloCounter.Instance.layouts.Add(layout);
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (GodhomeEloCounter.Instance.modSettings.naturalTime)
            {
                if (timeSpan.TotalHours >= 1) return $"{timeSpan.TotalHours:F1} Hours";
                if (timeSpan.TotalMinutes >= 1) return $"{timeSpan.TotalMinutes:F1} Minutes";
                return $"{timeSpan.TotalSeconds:F1} Seconds";
            }

            else
            {
                int totalHours = (int)timeSpan.TotalHours;
                int minutes = timeSpan.Minutes;
                int seconds = timeSpan.Seconds;

                // For times less than an hour, only show minutes and seconds
                if (totalHours == 0)
                {
                    return $"{minutes:D2}:{seconds:D2}";
                }
                
                // For times less than 100 hours, show hours:minutes:seconds
                if (totalHours < 100)
                {
                    return $"{totalHours:D2}:{minutes:D2}:{seconds:D2}";
                }
                
                // For very large times, show total hours without padding
                return $"{totalHours}:{minutes:D2}:{seconds:D2}";
            }
        }
    }
}
