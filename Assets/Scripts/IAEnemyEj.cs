using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAEnemyEj : MonoBehaviour
{
    enum State
    {
        Patrolling,
        Chasing,
        Searching,
        Waiting,
        Attacking
    }

    State currentState;
    UnityEngine.AI.NavMeshAgent enemyAgent;
    Transform playerTransform;
    [SerializeField] Transform patrolAreaCenter;
    [SerializeField] Vector2 patrolAreaSize;

    public Transform[] areas;
    [SerializeField] int patrolArea = 0;
    [SerializeField] float visionRange = 10;
    [SerializeField] float visionAngle = 90;
    Vector3 lastTargetPosition;
    [SerializeField] float searchTimer;
    [SerializeField] float searchWaitTime = 15;
    [SerializeField] float searchRadius = 30;

    [SerializeField] float patrolWaitTime = 2;
    [SerializeField] float waitTimer;

    [SerializeField] float attackRange = 1;

    void Awake() 
    {
        enemyAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    void Start()
    {
        enemyAgent.destination = areas[patrolArea].position;
        currentState = State.Patrolling;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
            break;
            case State.Chasing:
                Chase();
            break;
            case State.Searching:
                Search();
            break;
            case State.Waiting:
                Wait();
            break;
            case State.Attacking:
                Attack();
            break;
        }
    }

    void Patrol()
    {
        if(OnRange() == true)
        {
            currentState = State.Chasing;
        }
        if(enemyAgent.remainingDistance < 0.5f)
        {
            //SetRandomPoint();
            currentState = State.Waiting;
            SetPoint();
        }
    }

    void Chase()
    {
        enemyAgent.destination = playerTransform.position;
        if(OnRange() == false)
        {
            //currentState = State.Patrolling;
            currentState = State.Searching;
        }
        if(OnRangeAttack() == true)
        {
            if(enemyAgent.remainingDistance > 0.5f)
            {//currentState = State.Patrolling;
            currentState = State.Attacking;
            }
        }
    }

    void Search()
    {
        if(OnRange() == true)
        {
            searchTimer = 0;
            currentState = State.Chasing;
        }
        searchTimer += Time.deltaTime;
        if(searchTimer < searchWaitTime)
        {
            if(enemyAgent.remainingDistance < 0.5f)
            {
                Debug.Log("Buscando punto aleatorio");
                Vector3 randomSearchPoint = lastTargetPosition + Random.insideUnitSphere * searchRadius;
                randomSearchPoint.y = lastTargetPosition.y;
                enemyAgent.destination = randomSearchPoint; 
            }
            
        }
        else
        {
            currentState = State.Patrolling;
        }
    }

    void Wait()
    {
        if(enemyAgent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;
            if(waitTimer >= patrolWaitTime)
            {
                waitTimer = 0;
                currentState = State.Patrolling;
                
            }
        }

        if(OnRange() == true)
        {
            currentState = State.Chasing;
        }
    }

    void Attack()
    {
        Debug.Log("Atacado!!");
        currentState = State.Chasing;
    }
    void SetPoint()
    {
        patrolArea ++;
        if(patrolArea > 4)
        {
            patrolArea = 0;
        }
        enemyAgent.destination = areas[patrolArea].position;
    }
    /*void SetRandomPoint()
    {
        float randomX = Random.Range(-patrolAreaSize.x / 2, patrolAreaSize.x / 2);
        float randomZ = Random.Range(-patrolAreaSize.y / 2, patrolAreaSize.y / 2);
        Vector3 randomPoint = new Vector3(randomX, 0f, randomZ) + patrolAreaCenter.position;

        enemyAgent.destination = randomPoint;
    }*/

    bool OnRange()
    {
        /*if(Vector3.Distance(transform.position, playerTransform.position) <= visionRange)
        {
            return true;
        }
        return false;*/
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if(distanceToPlayer <= visionRange && angleToPlayer < visionAngle * 0.5f)
        {
            if(playerTransform.position == lastTargetPosition)
            {
                lastTargetPosition = playerTransform.position;
                return true;
            }
            //return true;
            RaycastHit hit;
            if(Physics.Raycast(transform.position, directionToPlayer, out hit,distanceToPlayer))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    bool OnRangeAttack()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if(distanceToPlayer <= attackRange && angleToPlayer < visionAngle * 0.5f)
        {
            if(playerTransform.position == lastTargetPosition)
            {
                lastTargetPosition = playerTransform.position;
                return true;
            }
            //return true;
            RaycastHit hit;
            if(Physics.Raycast(transform.position, directionToPlayer, out hit,distanceToPlayer))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(patrolAreaCenter.position, new Vector3(patrolAreaSize.x, 0, patrolAreaSize.y));
        

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.green;
        Vector3  fovLine1 = Quaternion.AngleAxis(visionAngle * 0.5f, transform.up) * transform.forward * visionRange;
        Vector3  fovLine2 = Quaternion.AngleAxis(-visionAngle * 0.5f, transform.up) * transform.forward * visionRange;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }
}
