using System;
using System.Linq;
using MagicUI.Core;
using MagicUI.Elements;

namespace GodhomeEloCounter
{
    public static class ModUI
    {
        public static void SpawnBossUI(LayoutRoot layout, Boss boss) {
            string sceneName = BossMappings.GetDisplayFromScene(boss.sceneName);
            string eloString = $"Elo: {boss.RoundedElo()} ({boss.RoundedElo() - boss.RoundedLastElo()})";
            string peakEloString = $"Peak: {boss.RoundedPeakElo()}";
            string streakString = $"Streak: {boss.streak} / Best: {boss.bestWinStreak}";
            string winLossString = $"Wins: {boss.wins} / Losses: {boss.losses}";
            string timeString = $"Time: {boss.timeSpent:hh\\:mm\\:ss}";

            string textUI = $"{sceneName}\n{eloString}\n{peakEloString}\n{streakString}\n{winLossString}\n{timeString}";

            new TextObject(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 20,
                Font = UI.TrajanBold,
                Text = textUI,
                Padding = new(20)
            };
        }

        public static void SpawnAllTierBossUI(LayoutRoot layout, LocalData data, string statueName) {
            string sceneName = BossMappings.GetSceneFromStatue(statueName);
            
            string textUI = "";
            textUI += $"{BossMappings.GetDisplayFromScene(sceneName)}\n\n";

            Boss attuned = data.FindOrCreateBoss(sceneName, 0);
            textUI += "Attuned\n";
            textUI +=  $"Elo: {attuned.RoundedElo()} ({attuned.RoundedElo() - attuned.RoundedLastElo()})\n";
            textUI +=  $"Peak: {attuned.RoundedPeakElo()}\n";
            textUI +=  $"Streak: {attuned.streak} / Best: {attuned.bestWinStreak}\n";
            textUI +=  $"Wins: {attuned.wins} / Losses: {attuned.losses}\n";
            textUI +=  $"Time: {attuned.timeSpent:hh\\:mm\\:ss}\n\n";

            Boss ascended = data.FindOrCreateBoss(sceneName, 1);
            textUI += "Ascended\n";
            textUI +=  $"Elo: {ascended.RoundedElo()} ({ascended.RoundedElo() - ascended.RoundedLastElo()})\n";
            textUI +=  $"Peak: {ascended.RoundedPeakElo()}\n";
            textUI +=  $"Streak: {ascended.streak} / Best: {ascended.bestWinStreak}\n";
            textUI +=  $"Wins: {ascended.wins} / Losses: {ascended.losses}\n";
            textUI +=  $"Time: {ascended.timeSpent:hh\\:mm\\:ss}\n\n";

            Boss radiant = data.FindOrCreateBoss(sceneName, 2);
            textUI += "Radiant\n";
            textUI +=  $"Elo: {radiant.RoundedElo()} ({radiant.RoundedElo() - radiant.RoundedLastElo()})\n";
            textUI +=  $"Peak: {radiant.RoundedPeakElo()}\n";
            textUI +=  $"Streak: {radiant.streak} / Best: {radiant.bestWinStreak}\n";
            textUI +=  $"Wins: {radiant.wins} / Losses: {radiant.losses}\n";
            textUI +=  $"Time: {radiant.timeSpent:hh\\:mm\\:ss}\n\n";

            new TextObject(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 20,
                Font = UI.TrajanBold,
                Text = textUI,
                Padding = new(20)
            };
        }

        public static void SpawnGlobalStatsUI(LayoutRoot layout, LocalData data) {
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
            textUI += $"Time: {totalAttunedTime:hh\\:mm\\:ss}\n\n";

            textUI += "Ascended\n";
            textUI += $"ELO: {averageAscendedELO}\n";
            textUI += $"Wins: {totalAscendedWins} / Losses: {totalAscendedLosses}\n";
            textUI += $"Time: {totalAscendedTime:hh\\:mm\\:ss}\n\n";

            textUI += "Radiant\n";
            textUI += $"ELO: {averageRadiantELO}\n";
            textUI += $"Wins: {totalRadiantWins} / Losses: {totalRadiantLosses}\n";
            textUI += $"Time: {totalRadiantTime:hh\\:mm\\:ss}\n\n";

            textUI += "Grand Total\n";
            textUI += $"ELO: {globalELO}\n";
            textUI += $"Wins: {totalAttunedWins + totalAscendedWins + totalRadiantWins} / Losses: {totalAttunedLosses + totalAscendedLosses + totalRadiantLosses}\n";
            textUI += $"Time: {totalAttunedTime + totalAscendedTime + totalRadiantTime:hh\\:mm\\:ss}\n";

            new TextObject(layout)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 20,
                Font = UI.TrajanBold,
                Text = textUI,
                Padding = new(20)
            };
        }
    }
}
