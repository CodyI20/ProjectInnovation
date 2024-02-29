using UnityEngine;

namespace FusionUtils {
  public class RotateAvatar : MonoBehaviour {
    void Update() {
      transform.Rotate(Vector3.up, 60 * Time.deltaTime);
    }
  }
}