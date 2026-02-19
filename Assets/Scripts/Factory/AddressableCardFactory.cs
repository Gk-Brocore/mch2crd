using Game.Addressable;
using Game.Card;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Factory
{
    public class AddressableCardFactory : ICardFactory
    {
        private readonly AssetReference cardPrefab;

        public AddressableCardFactory(AssetReference cardPrefab)
        {
            this.cardPrefab = cardPrefab;
        }

        public async Task<ICard> CreateAsync(Transform parent, string name)
        {
            GameObject card = await AddressableManager.Instance.InstantiateAsync(cardPrefab, Vector3.zero, Quaternion.identity, parent);
            card.name = name;
            return card.GetComponent<ICard>();
        }
    }
}