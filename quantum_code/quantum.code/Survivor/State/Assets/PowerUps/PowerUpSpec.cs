using System;
using Photon.Deterministic;

namespace Quantum
{
  public unsafe abstract partial class PowerUpSpec : AssetObject
  {
    public abstract void PerformPowerUpEffect(Frame f, EntityRef target);
  }
}
