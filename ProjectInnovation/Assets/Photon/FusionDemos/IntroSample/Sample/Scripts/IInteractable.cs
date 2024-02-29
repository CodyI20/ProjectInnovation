using Fusion;

namespace FusionUtils {
  /// <summary>
  /// Interface to indicate an object that the player can interact in the world.
  /// </summary>
  public interface IInteractable {
    public void Interact(NetworkRunner runner, PlayerRef playerInteracting);
  }
}