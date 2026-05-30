using System;
using Photon.Deterministic;

namespace Quantum
{
  public unsafe abstract partial class EffectSpec : AssetObject
  {
    public abstract void PerformEffect(Frame f, EntityRef source, EntityRef target);
  }
}
