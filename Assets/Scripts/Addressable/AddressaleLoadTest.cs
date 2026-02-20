using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Game.Addressable
{
    public class AddressaleLoadTest : MonoBehaviour
    {
        public AssetReference prefab;
        public AssetReferenceSprite sprite;
        public Transform parent;

        private GameObject instantiatedObject;

        private List<GameObject> instantiatedObjects = new List<GameObject>();   

        private Sprite loadedSprite;

        [ContextMenu("LoadPrefab")]
        public async void Loadit()
        {

            if (instantiatedObject != null)
            {
               instantiatedObjects.Add(instantiatedObject);
            }

            GameObject go = await AddressableManager.Instance.InstantiateAsync(prefab, Vector3.zero, Quaternion.identity, parent);
            if (go == null)
                return;
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            instantiatedObject = go;
        }

        [ContextMenu("ReleasePrefab")]
        public void Releaseit()
        {

            foreach (GameObject _go in instantiatedObjects)
            {
                AddressableManager.Instance.ReleaseInstance(prefab, _go);    
            }

            AddressableManager.Instance.ReleaseInstance(prefab, instantiatedObject);
        }
        [ContextMenu("ReleaseSprite")]
        public void ReleaseSprite()
        {
            AddressableManager.Instance.Unload(sprite);
        }

        [ContextMenu("ReleaseAll")]
        public void ReleaseAll()
        {
            AddressableManager.Instance.ReleaseAll();
        }

        [ContextMenu("LoadSprite")]
        public async void LoadSprite()
        {
            loadedSprite = await AddressableManager.Instance.LoadImageAsync<Sprite>(sprite);

            if (instantiatedObject != null)
                instantiatedObject.GetComponent<Image>().sprite = loadedSprite;
        }
    }
}