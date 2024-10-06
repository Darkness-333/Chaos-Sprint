using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class EnemyMovement : NetworkBehaviour {
    public float speed = 5f;
    public float pathUpdateInterval = 1f;
    public float nextWaypointDistance = 0.5f;

    public float changeDirectionTime = .1f;
    //public float avoidanceRadius = 1;
    private Transform player; 

    private Vector3[] path;
    private int targetIndex;
    private Pathfinding pathfinding;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private Vector2 direction;

    [HideInInspector] public Vector3 targetPosition;
    [HideInInspector] public bool targetChanged = false;

    public void SetPlayerTransform(Transform player) {
        this.player = player;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position + Vector3.up * .2f+new Vector3(direction.x, direction.y) * .5f, transform.position + new Vector3(direction.x, direction.y) * .5f + new Vector3(direction.x, direction.y));

        Gizmos.DrawWireCube(transform.position + Vector3.up * .2f, new Vector3(.82f, .42f));
    }

    void FixedUpdate() {
        if (!isServer) return;

        if (path != null && path.Length > 0) {
            FollowPath();
        }
    }

    private void Start() {

        GetComponent<Animator>().SetBool("isRunning", true);
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        pathfinding = FindObjectOfType<Pathfinding>();
        if (!isServer) return;  // Ensure only the server executes this
        if (pathfinding == null) {
            Debug.LogError("Pathfinding component not found in the scene.");
            return;
        }

        StartCoroutine(UpdatePath());

    }

    IEnumerator UpdatePath() {
        while (player == null) {
            yield return null;
        }

        while (true) {
            if(!targetChanged) {
                targetPosition=player.position;
            }
            List<Node> pathNodes = pathfinding.FindPath(transform.position, targetPosition);

            // Если путь найден, конвертируем его в массив Vector3 и сбрасываем индекс текущей точки пути
            if (pathNodes != null) {
                path = pathfinding.ConvertPathToVector3(pathNodes);
                if (path.Length > 0) {
                    targetIndex = 0;
                }
            }
            else {
                Debug.LogError("Path not found.");
            }

            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }


    void FollowPath() {
        if (path.Length == 0) return;

        Vector3 currentWaypoint = path[targetIndex];
        float distance = Vector3.Distance(transform.position, currentWaypoint);

        if (distance <= nextWaypointDistance) {
            targetIndex++;
            if (targetIndex >= path.Length) {
                path = new Vector3[0];
                return;
            }
            currentWaypoint = path[targetIndex];
        }

        direction = (currentWaypoint - transform.position).normalized;

        if (direction.x<0) { 
            RpcUpdateFlip(true);
        }
        else {
            RpcUpdateFlip(false);

        }

        //TryToSolve2();
        //TryToSolve3();

        TryToSolve4();

        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }


    [ClientRpc]
    private void RpcUpdateFlip(bool flip) {
        if (sprite) {
            sprite.flipX = flip;
        }
    }

    private void TryToSolve4() {
        LayerMask mask = LayerMask.GetMask("Obstacle");
        Collider2D collider = Physics2D.OverlapBox(transform.position + Vector3.up * .2f, new Vector2(.82f, .42f) , 0, mask);

        if (collider) {

            if (Random.value < .9) return;
            Vector2 toObstacleDir = collider.transform.position - transform.position;
            Vector2 newDirection = new Vector2(-toObstacleDir.x, toObstacleDir.y);

            newDirection=(newDirection-2*toObstacleDir).normalized;

            targetPosition = transform.position + new Vector3(newDirection.x, newDirection.y) * 3;
            StartCoroutine(ReturnTarget());
        }

    }

    private IEnumerator ReturnTarget() {
        targetChanged = true;
        yield return new WaitForSeconds(changeDirectionTime);
        targetChanged = false;

    }

    // попытка избегать препятствия при приближении
    //private void TryToSolve3() {
    //    //LayerMask mask = LayerMask.GetMask("EnemyLegs", "Obstacle");
    //    LayerMask mask = LayerMask.GetMask("Obstacle");
    //    Vector2 avoidanceDir = Vector2.zero;
    //    // Получаем всех ближайших врагов и препятствия
    //    Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position+Vector3.up*.2f, avoidanceRadius, mask);

    //    //Collider2D enemyLegs = transform.GetChild(0).GetComponent<Collider2D>();
    //    foreach (Collider2D collider in nearbyColliders) {
    //        Vector2 toCollider = transform.position - collider.transform.position;
    //        avoidanceDir += toCollider.normalized / toCollider.magnitude; // Чем ближе, тем сильнее избегаем
    //    }

    //    avoidanceDir = new Vector2(-avoidanceDir.x, avoidanceDir.y);
    //    // Применяем силу избегания к направлению движения
    //    Vector2 newDirection = (direction + avoidanceDir).normalized;
    //    //Vector2 newDirection=avoidanceDir.normalized;
    //    direction= newDirection;
    //}


    // изменение направления при столкновении
    //private void TryToSolve2() {
    //    //LayerMask mask = LayerMask.GetMask("EnemyLegs", "Obstacle");
    //    LayerMask mask = LayerMask.GetMask("Obstacle");
    //    if (Physics2D.Raycast(transform.position +Vector3.up*.2f+ new Vector3(direction.x, direction.y) * .45f, direction, .5f, mask)) {

    //        Vector2 addDir = Random.insideUnitCircle;
    //        direction=(direction + addDir).normalized;
    //    }

    //}

}
