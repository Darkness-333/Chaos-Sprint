using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
    Grid grid;

    void Awake() {
        grid = GetComponent<Grid>();
        if (grid == null) {
            Debug.LogError("Grid component not found in the scene.");
        }
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos) {
        if (grid == null) {
            Debug.LogError("Grid is not initialized.");
            return null;
        }

        // Находим начальный и целевой узлы по их мировым координатам
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode == null || targetNode == null) {
            Debug.LogError("Start or Target Node is null.");
            return null;
        }

        // Список открытых узлов (которые нужно проверить)
        List<Node> openSet = new List<Node>();
        // Множество закрытых узлов (которые уже проверены)
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        // Основной цикл поиска пути
        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            // Поиск узла с наименьшей стоимостью fCost в открытом списке
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }

            // Перемещение текущего узла из открытого списка в закрытый
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Если текущий узел является целевым, восстанавливаем и возвращаем путь
            if (currentNode == targetNode) {
                return RetracePath(startNode, targetNode);
            }

            // Проверяем каждого соседа текущего узла
            foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                // Пропускаем непроходимых соседей или уже проверенных

                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }

                // Вычисляем новую стоимость движения до соседа
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                // Если новый путь к соседу короче или сосед не в открытом списке, обновляем его параметры
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    // Если сосед не в открытом списке, добавляем его туда
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }



    List<Node> RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        // Проходим от целевого узла к начальному через родительские узлы
        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    public Vector3[] ConvertPathToVector3(List<Node> path) {
        if (path == null) {
            Debug.LogError("Path is null.");
            return null;
        }
        Vector3[] waypoints = new Vector3[path.Count];
        // Заполняем массив позициями из узлов пути
        for (int i = 0; i < path.Count; i++) {
            waypoints[i] = path[i].worldPosition;
        }
        return waypoints;
    }

    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return dstX * dstX + dstY * dstY;

        //if (dstX > dstY)
        //    return 14 * dstY + 10 * (dstX - dstY);
        //return 14 * dstX + 10 * (dstY - dstX);
    }
}
