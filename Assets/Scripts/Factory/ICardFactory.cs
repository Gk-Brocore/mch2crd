using Game.Card;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Factory
{
    public interface ICardFactory
    {
        Task<ICard> CreateAsync(Transform parent, string name);

    }
}