using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    protected GameUIController gameUIController;

    public virtual void Init(GameUIController _gameUIController)
    {
        gameUIController = _gameUIController;
    }

    public abstract void Setup();

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false );
    }
}
