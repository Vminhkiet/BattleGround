using UnityEngine;

public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;

	public Color color;

	[SerializeField]
	HexCell[] neighbors;
    void Awake()
    {
        if (neighbors == null || neighbors.Length != 6)
            neighbors = new HexCell[6];
    }

    public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}
}