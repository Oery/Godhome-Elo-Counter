using MagicUI.Core;
using MagicUI.Elements;

namespace GodhomeEloCounter
{
    public static class ModUI
    {
        public static void SpawnBossUI(LayoutRoot layout, Boss boss) {
            string sceneName = BossMappings.GetDisplayFromScene(boss.sceneName);
            string eloString = $"Elo: {boss.RoundedElo()} ({boss.RoundedElo() - boss.RoundedLastElo()})";
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

        public static void SpawnAllTierBossUI(LayoutRoot layout, LocalData data, string statueName) {
            string sceneName = BossMappings.GetSceneFromStatue(statueName);
            
            string textUI = "";
            textUI += $"{BossMappings.GetDisplayFromScene(sceneName)}\n\n";

            Boss attuned = data.FindOrCreateBoss(sceneName, 0);
            textUI += "Attuned\n";
            textUI +=  $"Elo: {attuned.RoundedElo()} ({attuned.RoundedElo() - attuned.RoundedLastElo()})\n";
            textUI +=  $"Streak: {attuned.streak} / Best: {attuned.bestWinStreak}\n";
            textUI +=  $"Wins: {attuned.wins} / Losses: {attuned.losses}\n";
            textUI +=  $"Time: {attuned.timeSpent:hh\\:mm\\:ss}\n\n";

            Boss ascended = data.FindOrCreateBoss(sceneName, 1);
            textUI += "Ascended\n";
            textUI +=  $"Elo: {ascended.RoundedElo()} ({ascended.RoundedElo() - ascended.RoundedLastElo()})\n";
            textUI +=  $"Streak: {ascended.streak} / Best: {ascended.bestWinStreak}\n";
            textUI +=  $"Wins: {ascended.wins} / Losses: {ascended.losses}\n";
            textUI +=  $"Time: {ascended.timeSpent:hh\\:mm\\:ss}\n\n";

            Boss radiant = data.FindOrCreateBoss(sceneName, 2);
            textUI += "Radiant\n";
            textUI +=  $"Elo: {radiant.RoundedElo()} ({radiant.RoundedElo() - radiant.RoundedLastElo()})\n";
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
