using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DMaterial : Array2D<Material>
    {
        [SerializeField]
        CellRowMaterial[] cells = new CellRowMaterial[Consts.defaultGridSize];

        protected override CellRow<Material> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }
}
