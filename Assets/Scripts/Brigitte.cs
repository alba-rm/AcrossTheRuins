using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//Poner 

public class Brigitte : MonoBehaviour
{
    //Hacer state maching
    public enum State
    {
        Patrolling,
        Chasing,
        Searching,
        Attacking
    }
    
    public State currentState;

    //Patrullar
    private NavMeshAgent agent;
    private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    
    [SerializeField] float visionRange = 10;
    [SerializeField] float visionAngle = 90;

    //Detectar jugador
    [SerializeField] private float detectionRange = 15;

    //Rango de ataque
    [SerializeField] private float attackRange = 1;

    [SerializeField] float searchTimer;
    [SerializeField] float searchWaitTime = 15;
    [SerializeField] float searchRadius = 30;

    Vector3 lastTargetPosition;



    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
    }
    //Decir estado inicial
    void Start()
    {
        SetRandomPoint();
        currentState = State.Patrolling;
    }
    
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
            case State.Attacking:
                Attack();
            break;
            case State.Searching:
                Search();
            break;
        }
        
    }

    void Patrol()
    {
        if(IsInRange() == true)
        {
            currentState = State.Chasing;
        }

        if(agent.remainingDistance < 0.5f)
        {
            SetRandomPoint();
        }
    }

    void Chase()
    {
        if(IsInRange() == false)
        {
            SetRandomPoint();
            currentState = State.Patrolling;
        }
        if(IsInRangeAttack() == true)
        {
            currentState = State.Attacking;
        }
        agent.destination = player.position;
    }

    void Attack()
    {
        Debug.Log("Atacando");
        currentState = State.Chasing;
    }

    void SetRandomPoint()
    {
        agent.destination = patrolPoints[Random.Range(0, patrolPoints.Length)].position;
    }

    /*bool IsInRange(float range)
    {
        if(Vector3.Distance(transform.position, player.position) < range)
        {
            return true;
        }

        else
        {
            return false;
        }
    }*/

    void Search()
    {
        if(IsInRange() == true)
        {
            searchTimer = 0;
            currentState = State.Chasing;
        }
        searchTimer += Time.deltaTime;
        if(searchTimer < searchWaitTime)
        {
            if(agent.remainingDistance < 0.5f)
            {
                Debug.Log("Buscando punto aleatorio");
                Vector3 randomSearchPoint = lastTargetPosition + Random.insideUnitSphere * searchRadius;
                randomSearchPoint.y = lastTargetPosition.y;
                agent.destination = randomSearchPoint; 
            }
            
        }
        else
        {
            currentState = State.Patrolling;
        }
    }

     bool IsInRange()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if(distanceToPlayer <= visionRange && angleToPlayer < visionAngle * 0.5f)
        {
            if(player.position == lastTargetPosition)
            {
                lastTargetPosition = player.position;
                return true;
            }

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

    bool IsInRangeAttack()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if(distanceToPlayer <= attackRange && angleToPlayer < visionAngle * 0.5f)
        {
            if(player.position == lastTargetPosition)
            {
                lastTargetPosition = player.position;
                return true;
            }

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
        foreach (Transform point in patrolPoints)
        {
            Gizmos.DrawWireSphere(point.position, 1f);
        }
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Vector3  fovLine1 = Quaternion.AngleAxis(visionAngle * 0.5f, transform.up) * transform.forward * visionRange;
        Vector3  fovLine2 = Quaternion.AngleAxis(-visionAngle * 0.5f, transform.up) * transform.forward * visionRange;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
