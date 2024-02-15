using System.Collections;
using UnityEngine;

namespace Photon.Menu {
  public class PhotonMenuUIScreen : MonoBehaviour {
    public static PhotonMenuUIScreen ActiveScreen;

    public bool IsModal;

    protected Animator _animator;
    protected static readonly int HideAnimHash = Animator.StringToHash("Hide");
    protected static readonly int ShowAnimHash = Animator.StringToHash("Show");
    private Coroutine _hideCoroutine;

    public IPhotonMenuConfig Config { get; set; }
    public IPhotonMenuConnection Connection { get; set; }
    public IPhotonMenuConnectArgs ConnectionArgs { get; set; }
    public IPhotonMenuUIController Controller { get; set; }

    protected virtual void Awake()
    {
      TryGetComponent(out _animator);
    }

    public virtual void Hide()
    {
      if (_animator)
      {
        if (_hideCoroutine != null)
        {
          StopCoroutine(_hideCoroutine);
        }

        _hideCoroutine = StartCoroutine(HideAnimCoroutine());
        return;
      }

      gameObject.SetActive(false);
    }

    public virtual void BackToMenu()
    {
      Controller.Show<PhotonMenuMainUI>();
    }

    public virtual void Show()
    {
      if (_hideCoroutine != null)
      {
        StopCoroutine(_hideCoroutine);
      }

      if (!IsModal && ActiveScreen != this && ActiveScreen)
      {
        ActiveScreen.Hide();
      }

      gameObject.SetActive(true);

      if (!IsModal)
        ActiveScreen = this;
    }

    private IEnumerator HideAnimCoroutine()
    {
      _animator.Play(HideAnimHash);
      yield return null;
      while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
      {
        yield return null;
      }

      gameObject.SetActive(false);
    }
  }
}
