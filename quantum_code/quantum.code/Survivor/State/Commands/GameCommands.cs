using Photon.Deterministic;
using System;

namespace Quantum
{
  public class ChoosePowerUpCommand : DeterministicCommand
  {
    public Int32 ChooseIndex;

    public override void Serialize(BitStream stream)
    {
      stream.Serialize(ref ChooseIndex);
    }
  }

  public class MoreTimeCommand : DeterministicCommand
  {

    public override void Serialize(BitStream stream)
    {
    }
  }

}
