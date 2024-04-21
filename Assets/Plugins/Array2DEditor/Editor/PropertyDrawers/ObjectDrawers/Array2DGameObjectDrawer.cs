using UnityEngine;
using UnityEditor;

namespace Array2DEditor
{
    [CustomPropertyDrawer(typeof(Array2DGameObject))]
    public class Array2DGameObjectDrawer : Array2DObjectDrawer<GameObject> { }
}