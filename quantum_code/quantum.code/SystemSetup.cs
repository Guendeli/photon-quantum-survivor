using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum {
  public static class SystemSetup {
    public static SystemBase[] CreateSystems(RuntimeConfig gameConfig, SimulationConfig simulationConfig) {
      return new SystemBase[] {
        
        new SystemGroup("gameplay", new SystemBase[]
        {
          // pre-defined core systems
          new Core.CullingSystem2D(), 
          //new Core.CullingSystem3D(),
        
          new ProjectileQueryInjectSystem(),
          new ProgressionQueryInjectSystem(),
        
          new Core.PhysicsSystem2D(),
          //new Core.PhysicsSystem3D(),

          Core.DebugCommand.CreateSystem(),

          //new Core.NavigationSystem(),
          new Core.EntityPrototypeSystem(),
          new Core.PlayerConnectedSystem(),

          // user systems go here 
          new SpawnCharacterSystem(),
          new ChoosePowerUpCommandSystem(),
          new PowerUpSystem(),
          new MoreTimeCommandSystem(),
        
          // these are thread UNSAFE (parallel)
          new MonsterInputSystem(),
        
          // these are thread safe
          new WavesManagerSystem(),
          new PlayerInputSystem(),
          new AbiltiySystem(),
          new ProjectileQueryUpdateSystem(),
          new ProgressionQueryUpdateSystem(),
          new MonsterSystem(),
          new CollectibleSystem(),
          new WaveSystem(),
          new ProgressionSystem(),
          new MagneticLockSystem(),

        
          // these are thread UNSAFE (parallel)
          new CharacterMovementSystem(),
          
          
        }),
        
        
      };
    }
  }
}
