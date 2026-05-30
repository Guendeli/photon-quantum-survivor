using System;
using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum
{

  public unsafe partial class WaveConfig : AssetObject
  {
    public FP StartTime;
    public FP TTL;
    public int SpawnCount;
    public AssetRefEntityPrototype Prototype;

    [HideInInspector]
    public AssetRefWaveConfig ConfigRef;

    public virtual RuntimeWave* CreateWave(Frame f)
    {
      {
        var e = f.Create();
        f.Add<RuntimeWave>(e, out var runtimeWave);
        runtimeWave->Config = ConfigRef;
        runtimeWave->EndTime = StartTime + TTL;
        runtimeWave->CurrentSpawnCount = 0;
        return runtimeWave;
      }
    }

    public virtual void Spawn(Frame f, EntityRef waveEntity,  RuntimeWave* runtimeWave)
    {
      f.Unsafe.TryGetPointerSingleton<WavesManager>(out var wavesManager);

      if (wavesManager->Spawn(f, Prototype, out var waveableEntity, out var waveable, out var position))
      {
        runtimeWave->CurrentSpawnCount++;
        waveable->Wave = waveEntity;
      }

      if (f.Unsafe.TryGetPointer<Monster>(waveableEntity, out var monster))
      {
        monster->SetMonsterTarget(f);
      }
    }
  }
}