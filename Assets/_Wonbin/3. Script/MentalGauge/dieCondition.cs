using UnityEngine;

namespace Wonbin
{
    public class dieCondition : _Ghost
    {
        private float distanceThreshold = 1f;  // ��� �Ÿ� �Ӱ谪
        private Animator dieAnimator;  // ���� �� �ִϸ��̼��� ���� Animator

        // Start���� Animator �ʱ�ȭ
        private void Start()
        {
            dieAnimator = GetComponent<Animator>(); // ��Ʈ�� Animator ������Ʈ Ž��
            distanceThreshold = 1;  // _Ghost Ŭ������ HunttingTargetDistance ���
        }

        // ��ǥ���� �Ÿ� ������ Ȯ���ϰ� ��� ���θ� ��ȯ
        public bool CheckDieCondition(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("Target�� �������� �ʾҽ��ϴ�.");
                return false;
            }

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < distanceThreshold)
            {
                DieTrigger(); // ������ �����ϸ� �ִϸ��̼� ����
                return true;
            }
            return false;
        }

        // ���� �� �ִϸ��̼��� ���
        private void DieTrigger()
        {
            if (dieAnimator != null)
            {
                dieAnimator.SetTrigger("Die");  // "Die" Ʈ���ŷ� �ִϸ��̼� ����
            }
           
        }
    }
}