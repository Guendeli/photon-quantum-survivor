using Photon.Deterministic;
using System;

namespace Quantum {
  partial class RuntimeConfig
  {
    public AssetRefCharacterRoster Roster;
    public bool TestOnePlayer;
    public AssetRefPowerUpRoster PowerUps;
    public bool UseSnapMovementation;
    public int CollectibleBaseCount;

    partial void SerializeUserData(BitStream stream)
    {
      stream.Serialize(ref Roster.Id);
      stream.Serialize(ref TestOnePlayer);
      stream.Serialize(ref PowerUps);
      stream.Serialize(ref UseSnapMovementation);
      stream.Serialize(ref CollectibleBaseCount);
    }
  }
}