using Quantum.Task;

namespace Quantum
{
  public unsafe class CharacterMovementSystem : SystemThreadedFilter<CharacterMovementSystem.Filter>
  {
    public struct Filter
    {
      public EntityRef Entity;
      public Transform2D* Transform;
      public InputContainer* InputContainer;
      public CharacterController* CC;
    }

    // dangerous, runs in parallel (do NOT query from here)
    public override void Update(FrameThreadSafe f, ref Filter filter)
    {
      if (f.TryGetPointerSingleton<TeamProgression>(out var progression))
      {
        if (progression->State != GameState.Playing) return;
      }
      filter.CC->Update(f, ref filter);
    }
  }
}