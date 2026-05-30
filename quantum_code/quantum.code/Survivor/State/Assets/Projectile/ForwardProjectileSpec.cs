using System;
using Photon.Deterministic;

namespace Quantum
{

  [Serializable]
  public unsafe class ForwardProjectileSpec : ProjectileSpec
  {
    public override unsafe FPVector2 GetDirection(Frame f, EntityRef character, Transform2D* characterTransform, Ability* ability)
    {
      PlayerLink link = f.Get<PlayerLink>(character);
      if (f.RuntimeConfig.UseSnapMovementation)
      {
        return link.LastInputDirection;
      }
      else
        return FPVector2.Rotate(FPVector2.Right, characterTransform->Rotation);
    }
  }
}
