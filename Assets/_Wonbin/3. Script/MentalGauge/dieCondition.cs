using UnityEngine;

namespace Wonbin
{
    public class dieCondition : _Ghost
    {
        private float distanceThreshold = 1f;  // 사망 거리 임계값
        private Animator dieAnimator;  // 죽을 때 애니메이션을 위한 Animator

        // Start에서 Animator 초기화
        private void Start()
        {
            dieAnimator = GetComponent<Animator>(); // 고스트의 Animator 컴포넌트 탐색
            distanceThreshold = 1;  // _Ghost 클래스의 HunttingTargetDistance 사용
        }

        // 목표와의 거리 조건을 확인하고 사망 여부를 반환
        public bool CheckDieCondition(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("Target이 설정되지 않았습니다.");
                return false;
            }

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < distanceThreshold)
            {
                DieTrigger(); // 조건을 만족하면 애니메이션 실행
                return true;
            }
            return false;
        }

        // 죽을 때 애니메이션을 재생
        private void DieTrigger()
        {
            if (dieAnimator != null)
            {
                dieAnimator.SetTrigger("Die");  // "Die" 트리거로 애니메이션 실행
            }
           
        }
    }
}