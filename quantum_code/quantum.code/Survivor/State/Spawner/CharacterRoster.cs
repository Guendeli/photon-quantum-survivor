using Photon.Deterministic;

namespace Quantum
{
  public unsafe partial class CharacterRoster : AssetObject
  {

    public AssetRefEntityPrototype[] PlayerPrototypes;

    public void SpawnPlayerCharacter(Frame f, PlayerRef player)
    {
      // random only if more than roster
      var playerPrototype = PlayerPrototypes[f.RNG->Next(0, PlayerPrototypes.Length)];
      if (PlayerPrototypes.Length > player)
      {
        playerPrototype = PlayerPrototypes[player];
      }


      var radians = f.RNG->Next(-FP.Pi, FP.Pi);
      var direction = FPVector2.Rotate(FPVector2.Right, radians);
      var character = f.Create(playerPrototype);

      if (f.Unsafe.TryGetPointer<Transform2D>(character, out var t) &&
          f.Unsafe.TryGetPointer<PlayerLink>(character, out var pl))
      {

        pl->Player = player;
        t->Position = default(FPVector2) + direction * (player + 1);
        f.Events.OnCharacterCreated(character, *pl);
      }

      if (f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var team))
      {
        team->Characters[player] = character;
      }
    }

  }
}