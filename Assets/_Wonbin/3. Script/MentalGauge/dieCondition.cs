using UnityEngine;

namespace Wonbin
{
    public class dieCondition : MonoBehaviour
    {
        private float distanceThreshold = 1;  // 사망 거리 임계값
        private Animator playerAnimator;       // 플레이어 Animator
        private Transform ghostTransform;      // Ghost의 Transform
        private Transform playerTransform;     // 플레이어 Transform

        // Start에서 Animator 및 고스트 오브젝트 초기화
        private void Start()
        {
            // 태그로 고스트 오브젝트를 찾고 Animator 할당
            GameObject player = GameObject.FindWithTag("Player");
            GameObject ghostObject = GameObject.FindWithTag("Ghost");

            playerTransform = player.transform;

            playerAnimator = gameObject.GetComponent<Animator>();

            if (ghostObject != null)
            {
                ghostTransform = ghostObject.transform;
                Debug.Log($"Ghost object found: {ghostObject.name}");
                Debug.Log($"Ghost transform position: {ghostTransform.position}");
            }
            else
            {
                Debug.LogError("Ghost 태그를 가진 오브젝트를 찾을 수 없습니다.");
            }
        }

        // Update에서 거리 조건을 주기적으로 확인
        private void Update()
        {
            if (ghostTransform != null)
            {
                CheckDieCondition(playerTransform);  // 거리 조건 확인
            }
        }

        // 목표와의 거리 조건을 확인하고 사망 여부를 반환
        public bool CheckDieCondition(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("Target이 설정되지 않았습니다.");
                return false;
            }

            // 목표와의 거리 계산
            float distance = Vector3.Distance(ghostTransform.position, target.position);


            if (distance < distanceThreshold)
            {
                Debug.Log("Die condition met. Triggering die animation.");
                DieTrigger(); // 조건을 만족하면 애니메이션 실행
                return true;
            }

            return false;
        }

        // 죽을 때 애니메이션을 재생
        private void DieTrigger()
        {
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Die");  // "Die" 트리거로 애니메이션 실행
            }
           
        }
    }
}