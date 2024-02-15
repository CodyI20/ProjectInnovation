using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Photon.Menu {
  public class PhotonMenuUIController<T> : MonoBehaviour, IPhotonMenuUIController where T : IPhotonMenuConnectArgs, new() {
    [SerializeField] private PhotonMenuConfig _config;
    [SerializeField] private PhotonMenuConnectionBehaviour _connection;
    [SerializeField] private PhotonMenuUIScreen[] _screens;
    private Dictionary<Type, PhotonMenuUIScreen> _screenLookup;
    private IPhotonMenuPopup _popupHandler;

    public virtual T CreateConnectArgs => new T();

    protected virtual void Awake()
    {
      var connectionArgs = CreateConnectArgs;
      _screenLookup = new Dictionary<Type, PhotonMenuUIScreen>();

      foreach (var screen in _screens)
      {
        screen.Config = _config;
        screen.Connection = _connection;
        screen.ConnectionArgs = connectionArgs;
        screen.Controller = this;

        var t = screen.GetType();
        while (true)
        {
          _screenLookup.Add(t, screen);
          if (t.BaseType == null || typeof(PhotonMenuUIScreen).IsAssignableFrom(t) == false || t.BaseType == typeof(PhotonMenuUIScreen))
          {
            break;
          }

          t = t.BaseType;
        }

        if (typeof(IPhotonMenuPopup).IsAssignableFrom(t))
        {
          _popupHandler = (IPhotonMenuPopup)screen;
        }
      }
    }

    protected virtual void Start()
    {
      if (_screens != null && _screens.Length > 0)
      {
        // First screen is displayed by default
        _screens[0].Show();
      }
    }

    public virtual void Show<S>() where S : PhotonMenuUIScreen { 
      if (_screenLookup.TryGetValue(typeof(S), out var result))
      {
        result.Show();
      }
      else
      {
        Debug.LogError($"Show() - Screen type '{typeof(S).Name}' not found");
      }
    }

    public virtual S Get<S>() where S : PhotonMenuUIScreen
    {
      if (_screenLookup.TryGetValue(typeof(S), out var result))
      {
        return result as S;
      }
      else
      {
        Debug.LogError($"Show() - Screen type '{typeof(S).Name}' not found");
        return null;
      }
    }

    public void Popup(string msg, string header = default)
    {
      if (_popupHandler == null)
      {
        Debug.LogError("Popup() - no popup handler found");
      }
      else
      {
        _popupHandler.OpenPopup(msg, header);
      }
    }
    
    public Task PopupAsync(string msg, string header = default)
    {
      if (_popupHandler == null)
      {
        Debug.LogError("Popup() - no popup handler found");
        return Task.CompletedTask;
      }
      else
      {
        return _popupHandler.OpenPopupAsync(msg, header);
      }
    }
  }
}
