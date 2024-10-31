using MagicUI.Core;
using MagicUI.Elements;

namespace GodhomeEloCounter
{
    public static class ModUI
    {
        public static void SpawnBossUI(LayoutRoot layout, Boss boss) {
            string sceneName = BossMappings.GetDisplayFromScene(boss.sceneName);
            string eloString = $"Elo: {boss.elo} ({boss.bestWinStreak})";
            string streakString = $"Streak: {boss.streak} / Best: {boss.bestWinStreak}";
            string winLossString = $"Wins: {boss.wins} / Losses: {boss.losses}";
            string timeString = $"Time: {boss.timeSpent:hh\\:mm\\:ss}";

            string textUI = $"{sceneName}\n{eloString}\n{streakString}\n{winLossString}\n{timeString}";

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

        public static void SpawnAllTierBossUI(LayoutRoot layout, LocalData data, string name) {
            string textUI = "";

            textUI += $"{BossMappings.GetDisplayFromStatue(name)}\n\n";

            Boss attuned = data.FindOrCreateBoss(name, 0);
            textUI += "Attuned\n";
            textUI +=  $"Elo: {attuned.elo} ({attuned.bestWinStreak})\n";
            textUI +=  $"Streak: {attuned.streak} / Best: {attuned.bestWinStreak}\n";
            textUI +=  $"Wins: {attuned.wins} / Losses: {attuned.losses}\n";
            textUI +=  $"Time: {attuned.timeSpent:hh\\:mm\\:ss}\n\n";

            Boss ascended = data.FindOrCreateBoss(name, 1);
            textUI += "Ascended\n";
            textUI +=  $"Elo: {ascended.elo} ({ascended.bestWinStreak})\n";
            textUI +=  $"Streak: {ascended.streak} / Best: {ascended.bestWinStreak}\n";
            textUI +=  $"Wins: {ascended.wins} / Losses: {ascended.losses}\n";
            textUI +=  $"Time: {ascended.timeSpent:hh\\:mm\\:ss}\n\n";

            Boss radiant = data.FindOrCreateBoss(name, 2);
            textUI += "Radiant\n";
            textUI +=  $"Elo: {radiant.elo} ({radiant.bestWinStreak})\n";
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
    }
}
