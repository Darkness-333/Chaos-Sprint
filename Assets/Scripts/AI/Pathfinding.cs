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

        // ������� ��������� � ������� ���� �� �� ������� �����������
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode == null || targetNode == null) {
            Debug.LogError("Start or Target Node is null.");
            return null;
        }

        // ������ �������� ����� (������� ����� ���������)
        List<Node> openSet = new List<Node>();
        // ��������� �������� ����� (������� ��� ���������)
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        // �������� ���� ������ ����
        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            // ����� ���� � ���������� ���������� fCost � �������� ������
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }

            // ����������� �������� ���� �� ��������� ������ � ��������
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // ���� ������� ���� �������� �������, ��������������� � ���������� ����
            if (currentNode == targetNode) {
                return RetracePath(startNode, targetNode);
            }

            // ��������� ������� ������ �������� ����
            foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                // ���������� ������������ ������� ��� ��� �����������

                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }

                // ��������� ����� ��������� �������� �� ������
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                // ���� ����� ���� � ������ ������ ��� ����� �� � �������� ������, ��������� ��� ���������
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    // ���� ����� �� � �������� ������, ��������� ��� ����
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

        // �������� �� �������� ���� � ���������� ����� ������������ ����
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
        // ��������� ������ ��������� �� ����� ����
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
