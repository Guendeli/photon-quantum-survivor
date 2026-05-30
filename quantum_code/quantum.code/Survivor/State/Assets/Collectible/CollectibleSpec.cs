using System;
using Photon.Deterministic;

namespace Quantum
{
  public abstract unsafe partial class CollectibleSpec : AssetObject
  {
    public abstract void PerformCollectibleEffect(Frame f, EntityRef entity, EntityRef target);
  }
}
