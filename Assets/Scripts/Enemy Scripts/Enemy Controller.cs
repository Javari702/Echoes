using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
   Patrolling,
   Chasing,
   Attacking
}

public class EnemyController : MonoBehaviour
{
   [SerializeField] Transform[] _patrolPoints;
   [SerializeField] Transform playerLocation;
   [SerializeField] float _pointWaitTime;
   [SerializeField] float stopDistance;
   [SerializeField] float detectionRange;
   [SerializeField] float veiwAngle;
   [SerializeField] float stopChaseTime; 
   [SerializeField] float attackRange;
   [SerializeField] Collider attackHitbox1;
   [SerializeField] Collider attackHitbox2;

   private NavMeshAgent _agent;
   private Animator _animator;
   private EnemyState _state = EnemyState.Patrolling;
   private int _currentPointIndex;
   private bool _isWaiting;
   private float _timeSinceStopChase;

   void Awake()
   {
      _agent = GetComponent<NavMeshAgent>();
      _animator = GetComponent<Animator>();   
   }

    void Start()
   {
      FollowPoints();
   }

    void Update()
   {
      float distanceToPlayer = Vector3.Distance(playerLocation.position, transform.position);

      switch (_state)
      {
         case EnemyState.Patrolling:
            Patrol();

            if (distanceToPlayer <= detectionRange && CanSeePlayer())
            {
               _state = EnemyState.Chasing;
            }

            break;

         case EnemyState.Chasing:
            ChasePlayer();

            if (distanceToPlayer <= attackRange)
            {
               _state = EnemyState.Attacking;
            }

            if (!CanSeePlayer())
            {
               _timeSinceStopChase += Time.deltaTime;

               if (_timeSinceStopChase >= stopChaseTime) 
               {
                  _timeSinceStopChase = 0f;
                  _state = EnemyState.Patrolling;
                  GoToClosestPatrolPoint();
               }
            }
            else
            {
               _timeSinceStopChase = 0f;
            }

            break;

         case EnemyState.Attacking:
            Attack();

            if (distanceToPlayer > attackRange)
            {
               attackHitbox1.enabled = false;
               attackHitbox2.enabled = false;
               _animator.SetBool("inRange", false);
               _state = EnemyState.Chasing;
               _agent.isStopped = false;
            }

            break;
      }

      UpdateAnimation();  
   }

   private void ChasePlayer()
   {
      _animator.SetBool("inPursuit", true);
      _agent.SetDestination(playerLocation.position);
   }

   private void Patrol()
   {
      _animator.SetBool("inPursuit", false); 

      if (_isWaiting) return;
      
      if (!_agent.pathPending && _agent.remainingDistance <= stopDistance)
      {
         StartCoroutine(WaitAtPoint());
      }
   }

   private void Attack()
   {
      attackHitbox1.enabled = true;
      attackHitbox2.enabled = true;
      _agent.isStopped = true;
      _animator.SetBool("inRange", true);

      Vector3 direction = (playerLocation.position - transform.position).normalized;
      direction.y = 0f;

      if (direction != Vector3.zero)
      {
         transform.rotation = Quaternion.LookRotation(direction);
      }

      OnAttackEnd();
   }

   private void OnAttackEnd()
   {
      _agent.isStopped = false;
   }

   private IEnumerator WaitAtPoint()
   {
      _isWaiting = true;
      _agent.isStopped = true;

      yield return new WaitForSeconds(_pointWaitTime);

      _agent.isStopped = false; 
      FollowPoints();
      _isWaiting = false;
   }

   private void GoToClosestPatrolPoint()
   {
      if (_patrolPoints.Length == 0) return;

      int closestPoint = 0;
      float closestDistance = float.MaxValue;

      for (int i = 0; i < _patrolPoints.Length; i++)
      {
         float distance = Vector3.Distance(transform.position, _patrolPoints[i].position);

         if (distance < closestDistance)
         {
            closestDistance = distance;
            closestPoint = i;
         }
      }

      _currentPointIndex = closestPoint;
      _agent.SetDestination(_patrolPoints[_currentPointIndex].position);

   }

   private void FollowPoints()
   {
      if (_patrolPoints.Length == 0) return;

      _agent.SetDestination(_patrolPoints[_currentPointIndex].position);
      _currentPointIndex = (_currentPointIndex + 1) % _patrolPoints.Length;
   }

   private void UpdateAnimation()
   {
      Vector3 localVelocity = transform.InverseTransformDirection(_agent.velocity);

      float horizontal = localVelocity.x / _agent.speed; 
      float forward   = localVelocity.z / _agent.speed; 


      _animator.SetFloat("XVelocity", horizontal, 0.1f, Time.deltaTime);
      _animator.SetFloat("ZVelocity", forward, 0.1f, Time.deltaTime);
   }

   private bool CanSeePlayer()
   {
      return IsFacingPlayer() && HasClearPathToPlayer();
   }
   private bool IsFacingPlayer()
   {
      Vector3 directionToPlayer = (playerLocation.position - transform.position).normalized;
      float angle = Vector3.Angle(transform.forward, directionToPlayer);
      return angle <= veiwAngle / 2f;
   }

   private bool HasClearPathToPlayer()
   {
      Vector3 directionToPlayer = playerLocation.position - transform.position;

      if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, directionToPlayer.magnitude))
      {
         return hit.transform == playerLocation;
      }

      return true;
   }
}
