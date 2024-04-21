using UnityEngine;
using UnityEditor;

namespace Array2DEditor
{
    [CustomPropertyDrawer(typeof(Array2DMaterial))]
    public class Array2DMaterialDrawer : Array2DObjectDrawer<Material> { }
}