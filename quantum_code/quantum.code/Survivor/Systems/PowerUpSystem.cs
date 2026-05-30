using Photon.Deterministic;

namespace Quantum
{
  public unsafe class PowerUpSystem : SystemSignalsOnly, ISignalOnPlayerChoosePowerUp, ISignalOnTeamLevelUp,
    ISignalOnSetMoreChooseTime
  {
    public void OnPlayerChoosePowerUp(Frame f, int player, int index)
    {
      f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var team);
      EntityRef character = team->Characters[player];

      if (f.Unsafe.TryGetPointer<PowerUpSelection>(character, out var selection))
      {
        Log.Debug(" Player: " + player + " Choose index: " + index);
        if (index < 3)
        {
          PowerUpSpec powerUpSpec = f.Assets.PowerUpSpec(selection->Options[index]);
          powerUpSpec.PerformPowerUpEffect(f, character);
        }
        f.Remove<PowerUpSelection>(character);
      }
      else
      {
        Log.Debug("Player can't choose!");
      }
    }

    public void OnSetMoreChooseTime(Frame f)
    {
      f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var progression);
      progression->TimeToSelect += 10;
    }

    public void OnTeamLevelUp(Frame f, TeamProgression* progression)
    {
      Log.Debug("Team LeveuUp!");
      progression->Level++;
      progression->XP = 0;
      progression->State = GameState.Selecting;
      progression->TimeToSelect = 10;
      f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var team);

      for (int playerID = 0; playerID < Constants.MaxPlayers; playerID++)
      {
        var character = team->Characters[playerID];
        if (f.Exists(character))
        {
          Log.Warn("Filling in powerup selection options.");
          var roster = f.Assets.PowerUpRoster(f.RuntimeConfig.PowerUps);
          var selection = roster.FillOptions(f, character);
          f.Events.OnLevelUp(character, selection);
        }
      }


    }
  }
}