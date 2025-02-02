using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarPathfinder : MonoBehaviour
{
    private struct Node
    {
        public NodeType nodeType;
        public Vector3Int cellgrid;
        public Vector3 worldgrid;

        public Node(NodeType nodeType, Vector3Int cellgrid, Vector3 worldgrid)
        {
            this.nodeType = nodeType;
            this.cellgrid = cellgrid;
            this.worldgrid = worldgrid;
        }
    }

    private enum NodeType { Normal, Edge }

    [SerializeField] private float pathReloadTime;
    [SerializeField] private Tilemap tilemap;
    private Enemy enemy;
    List<Node> navMeshList;
    Vector3 offset;
    
    // Start is called before the first frame update
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();

        foreach (Vector3Int cellgrid in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.GetTile(cellgrid + Vector3Int.up) && tilemap.GetTile(cellgrid))
            {
                Node node = new Node();

                node.cellgrid = cellgrid + Vector3Int.up;
                node.worldgrid = tilemap.CellToWorld(cellgrid + Vector3Int.up) + offset;

                if (tilemap.GetTile(cellgrid + Vector3Int.right) && tilemap.GetTile(cellgrid + Vector3Int.left))
                {
                    node.nodeType = NodeType.Normal;
                }
                else
                {
                    node.nodeType = NodeType.Edge;
                }

                navMeshList.Add(node);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        navMeshList = new List<Node>();

        foreach (Vector3Int cellgrid in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.GetTile(cellgrid + Vector3Int.up) && tilemap.GetTile(cellgrid))
            {
                Node node = new Node();

                node.cellgrid = cellgrid + Vector3Int.up;
                node.worldgrid = tilemap.CellToWorld(cellgrid + Vector3Int.up) + offset;

                if (tilemap.GetTile(cellgrid + Vector3Int.right) && tilemap.GetTile(cellgrid + Vector3Int.left))
                {
                    node.nodeType = NodeType.Normal;
                }
                else
                {
                    node.nodeType = NodeType.Edge;
                }

                navMeshList.Add(node);
            }
        }
        
        offset = new Vector3(tilemap.cellSize.x / 2.0f, tilemap.cellSize.y / 2.0f);

        foreach (Node node in navMeshList)
        {
            if (node.nodeType == NodeType.Normal)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = new Color(255, 100, 0);
            }
            Handles.Label(node.worldgrid, ((Vector2Int)node.cellgrid).ToString());
            Gizmos.DrawSphere(node.worldgrid, 0.3f);
        }
    }
}
