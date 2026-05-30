using Photon.Deterministic;
using Quantum.Task;

namespace Quantum
{
  public unsafe class MonsterInputSystem : SystemThreadedFilter<MonsterInputSystem.Filter>
  {
    public struct Filter
    {
      public EntityRef Entity;
      public Transform2D* Transform;
      public Monster* Monster;
      public InputContainer* InputContainer;
    }

    public override void Update(FrameThreadSafe f, ref Filter filter)
    {
      filter.Monster->UpdateInput(f, ref filter);
    }
  }
}
