using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Layout
{
    /// <summary>
    /// Interface for a collection of layout values. 
    /// This can be used for any type of layout, such as a grid, a list, etc. 
    /// It provides methods to get and set values, as well as to convert the collection to and from a list.
    /// </summary>
    /// <typeparam name="T">Data to Store</typeparam>
    public interface ILayoutCollection<T>
    {
        Vector2Int GetSize();
        T GetValue(int _x, int _y);
        void SetValue(int _x, int _y, T _value);
        void FromList(List<T> _list);

        List<T> ToList();
    }
}