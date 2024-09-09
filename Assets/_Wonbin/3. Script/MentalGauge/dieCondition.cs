using UnityEngine;

namespace Wonbin
{
    public class dieCondition : MonoBehaviour
    {
        private float distanceThreshold = 1;  // ��� �Ÿ� �Ӱ谪
        private Animator playerAnimator;       // �÷��̾� Animator
        private Transform ghostTransform;      // Ghost�� Transform
        private Transform playerTransform;     // �÷��̾� Transform

        // Start���� Animator �� ��Ʈ ������Ʈ �ʱ�ȭ
        private void Start()
        {
            // �±׷� ��Ʈ ������Ʈ�� ã�� Animator �Ҵ�
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
                Debug.LogError("Ghost �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }

        // Update���� �Ÿ� ������ �ֱ������� Ȯ��
        private void Update()
        {
            if (ghostTransform != null)
            {
                CheckDieCondition(playerTransform);  // �Ÿ� ���� Ȯ��
            }
        }

        // ��ǥ���� �Ÿ� ������ Ȯ���ϰ� ��� ���θ� ��ȯ
        public bool CheckDieCondition(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("Target�� �������� �ʾҽ��ϴ�.");
                return false;
            }

            // ��ǥ���� �Ÿ� ���
            float distance = Vector3.Distance(ghostTransform.position, target.position);


            if (distance < distanceThreshold)
            {
                Debug.Log("Die condition met. Triggering die animation.");
                DieTrigger(); // ������ �����ϸ� �ִϸ��̼� ����
                return true;
            }

            return false;
        }

        // ���� �� �ִϸ��̼��� ���
        private void DieTrigger()
        {
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Die");  // "Die" Ʈ���ŷ� �ִϸ��̼� ����
            }
           
        }
    }
}