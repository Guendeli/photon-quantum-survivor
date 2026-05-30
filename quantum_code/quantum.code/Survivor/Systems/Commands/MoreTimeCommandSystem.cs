using Photon.Deterministic;

namespace Quantum
{
  public unsafe class MoreTimeCommandSystem : SystemMainThread
  {
    public override void Update(Frame f)
    {
      for (int playerID = 0; playerID < Constants.MaxPlayers; playerID++)
      {
        MoreTimeCommand command = f.GetPlayerCommand(playerID) as MoreTimeCommand;
        if (command != null)
        {
          Log.Warn("Player ask for more time. ");
          f.Signals.OnSetMoreChooseTime();
        }
      }
    }
  }
}
