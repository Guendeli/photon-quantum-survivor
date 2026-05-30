using Photon.Deterministic;
namespace Quantum
{
  public unsafe class WaveSystem : SystemMainThreadFilter<WaveSystem.Filter>, ISignalOnComponentRemoved<Waveable>
  {
    public struct Filter
    {
      public EntityRef Entity;
      public RuntimeWave* Wave;
    }

    private static int _maxSpawnPerFrame = 5;

    public override void Update(Frame f, ref Filter filter)
    {
      if (f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var progression))
      {
        if (f.ComponentCount<PlayerLink>() == 0 || progression->State != GameState.Playing)
        {
          return;
        }
      }

      RuntimeWave* wave = filter.Wave;
      WaveConfig waveConfig = f.FindAsset<WaveConfig>(wave->Config.Id);

      if (progression->Timer >= wave->EndTime)
      {
        f.Destroy(filter.Entity);
        return;
      }


      var spawnCount = waveConfig.SpawnCount - wave->CurrentSpawnCount;
      spawnCount = FPMath.Clamp(spawnCount, 0, _maxSpawnPerFrame);

      for (int i = 0; i < spawnCount; i++)
      {
        waveConfig.Spawn(f, filter.Entity, wave);
      }
    }

    public void OnRemoved(Frame f, EntityRef entity, Waveable* waveble)
    {
      RuntimeWave* wave = default;
      if (!f.Exists(waveble->Wave))
      {
        //TODO
        //dropDefinition = f.FindAsset<DropDefinition>(waveble->_lootWithoutWave.Id);
      }
      else
      {
        wave = f.Unsafe.GetPointer<RuntimeWave>(waveble->Wave);
        wave->CurrentSpawnCount--;
      }
    }
  }
}