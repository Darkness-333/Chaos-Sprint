using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Grid : MonoBehaviour {
    public int width;
    public int height;
    public float cellSize = 1f; 
    public LayerMask unwalkableMask;
    public LayerMask enemyMask;
    public Node[,] grid;
    
    public float pathUpdateInterval=.1f;

    private void OnDrawGizmos() {
        if(grid != null) {
            Vector3 worldBottomLeft = transform.position - Vector3.right * width * cellSize / 2 - Vector3.up * height * cellSize / 2;

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * cellSize + cellSize / 2) + Vector3.up * (y * cellSize + cellSize / 2);
                    bool walkable = !Physics2D.OverlapCircle(worldPoint, cellSize, unwalkableMask|enemyMask);
                    if (walkable) {
                        Gizmos.color = Color.blue;
                    }
                    else {
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawSphere(worldPoint, .05f);
                }
            }
        }
    }

    void Awake() {
        CreateGrid();
    }

    void CreateGrid() {
        if (grid == null) {
            grid = new Node[width, height];
        }

        Vector3 worldBottomLeft = transform.position - Vector3.right * width * cellSize / 2 - Vector3.up * height * cellSize / 2;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * cellSize + cellSize / 2) + Vector3.up * (y * cellSize + cellSize / 2);
                bool walkable = !Physics2D.OverlapCircle(worldPoint, cellSize, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        if (grid == null) {
            Debug.LogError("Grid is not initialized.");
            return null;
        }
        // Преобразование мировых координат в проценты относительно ширины и высоты сетки
        float percentX = (worldPosition.x + width * cellSize / 2f) / (width * cellSize);
        float percentY = (worldPosition.y + height * cellSize / 2f) / (height * cellSize);
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Получение индесов элемента в сетке на основе процентов
        int x = Mathf.RoundToInt((width - 1) * percentX);
        int y = Mathf.RoundToInt((height - 1) * percentY);
        y = Mathf.RoundToInt(Mathf.Clamp(height * percentY, 0, height - 1));


        if (x < 0 || x >= width || y < 0 || y >= height) {
            Debug.LogError($"Calculated grid position ({x}, {y}) is out of bounds.");
            return null;
        }


        Node node = grid[x, y];

        if (node.walkable) {
            return node;
        }

        // Поиск ближайшего проходимого узла, если текущий непроходим
        Node closestWalkableNode = null;
        int searchRadius = 1;
        //print("startnode" + node.worldPosition);
        while (closestWalkableNode == null) {
            for (int i = -searchRadius; i <= searchRadius; i++) {
                for (int j = -searchRadius; j <= searchRadius; j++) {
                    int checkX = x + i;
                    int checkY = y + j;

                    if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height) {
                        Node checkNode = grid[checkX, checkY];
                        //print("checknode" + checkNode.worldPosition);

                        if (checkNode.walkable) {
                            closestWalkableNode = checkNode;
                            return closestWalkableNode;
                        }
                    }
                }
            }
            searchRadius++;

            // Остановить поиск, если радиус превысил размер сетки (предотвращение бесконечного цикла)
            if (searchRadius > Mathf.Max(width, height)) {
                Debug.LogError("No walkable nodes found within grid bounds.");
                return null;
            }
        }

        return closestWalkableNode;
    }

    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();

        // Проход по всем соседним ячейкам 
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public List<Node> CheckWalkableForEnemy(Node startNode) {
        List<Node> nearEnemy = new List<Node>();
        Collider2D[] colliders =Physics2D.OverlapCircleAll(startNode.worldPosition, cellSize, unwalkableMask | enemyMask);
        startNode.walkable = colliders.Length<=1;
        nearEnemy.Add(startNode);
        foreach(Node neighbour in GetNeighbours(startNode)) {
            colliders = Physics2D.OverlapCircleAll(neighbour.worldPosition, cellSize, unwalkableMask | enemyMask);
            neighbour.walkable = colliders.Length <= 1;
            nearEnemy.Add(neighbour);
        }
        return nearEnemy;
        
    }
}

public class Node {
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public Node parent;

    public int fCost { get { return gCost + hCost; } }

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}
