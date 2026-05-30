using Photon.Deterministic;
using Quantum.Task;

namespace Quantum
{
  public unsafe class SpawnCharacterSystem : SystemSignalsOnly
  {
    public override void OnInit(Frame f)
    {
      var roster = f.Assets.CharacterRoster(f.RuntimeConfig.Roster);
      int playerCount = f.RuntimeConfig.TestOnePlayer ? 1 : Constants.MaxPlayers;
      for (int i = 0; i < playerCount; i++)
      {
        roster.SpawnPlayerCharacter(f, i);
      }
    }

  }
}