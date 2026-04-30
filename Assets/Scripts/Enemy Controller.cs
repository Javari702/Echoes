using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
   [SerializeField] Transform[] _patrolPoints;
   [SerializeField] float _pointWaitTime;
   [SerializeField] float stopDistance;

   private NavMeshAgent _agent;
   private Animator _animator;
   private int _currentPointIndex;
   private bool _isWaiting;

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
      Patrol();
      UpdateAnimation();  
   }

   private void Patrol()
   {
      if (_isWaiting) return;
      
      if (!_agent.pathPending && _agent.remainingDistance <= stopDistance)
      {
         StartCoroutine(WaitAtPoint());
      }
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

   private void FollowPoints()
   {
      if (_patrolPoints.Length == 0) return;

      _agent.SetDestination(_patrolPoints[_currentPointIndex].position);
      _currentPointIndex = (_currentPointIndex + 1) % _patrolPoints.Length;
   }

   private void UpdateAnimation()
   {
      Vector3 localVelocity = transform.InverseTransformDirection(_agent.velocity);

      float horizontal = localVelocity.x / _agent.speed; // -1 to 1 (left/right)
      float forward   = localVelocity.z / _agent.speed; // -1 to 1 (forward/back)


      _animator.SetFloat("XVelocity", horizontal, 0.1f, Time.deltaTime);
      _animator.SetFloat("ZVelocity", forward, 0.1f, Time.deltaTime);
   }
}
