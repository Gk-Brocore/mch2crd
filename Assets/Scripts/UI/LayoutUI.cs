
using Game.Addressable;
using Game.Core;
using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class LayoutUI : BaseUI
{
    public AssetReference button;
    public Transform holder;
    public override async void Setup()
    {
        var _data = GameDataManager.Settings;
        foreach(var _item in _data.Collection.List)
        {
            var _handle = AddressableManager.Instance.InstantiateAsync(button, Vector3.zero, Quaternion.identity, holder);
            await _handle;
            GameObject _button = _handle.Result;
            var _text = _button.GetComponentInChildren<TMP_Text>();
            if(_text != null)
            {
                _text.text = _item.value.Name;
            }
            var _btn = _button.GetComponent<Button>();
            if(_btn != null)
            {
                _btn.onClick.AddListener(() =>
                {
                    gameUIController.OnLayoutChange(_item.value);
                });
            }
        }
    }

  
}
