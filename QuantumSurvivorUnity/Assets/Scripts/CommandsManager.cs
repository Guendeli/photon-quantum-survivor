using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandsManager : MonoBehaviour
{
  public static CommandsManager Instance;
  // Start is called before the first frame update
  void Start()
  {
    if (Instance == null)
    {
      Instance = this;
    }
  }

  public void SendChoosePowerUpCommand(int index)
  {
    ChoosePowerUpCommand c = new ChoosePowerUpCommand();
    c.ChooseIndex = index;
    QuantumGame game = QuantumRunner.Default.Game;
    int[] localPlayers = game.GetLocalPlayers();
    game.SendCommand(localPlayers[0], c);
  }

  public void SendMoreTimeCommand()
  {
    MoreTimeCommand c = new MoreTimeCommand();
    QuantumGame game = QuantumRunner.Default.Game;
    game.SendCommand(c);
  }
}
