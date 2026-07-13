using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // true면 오른쪽(다음 장면으로), false면 왼쪽(이전 장면으로)
    [SerializeField] private bool isRightPortal = true; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 오브젝트의 태그가 "Player"인지 확인하거나, 
        // 2. 혹시 태그 설정을 깜빡했을 경우를 대비해 오브젝트 이름에 "Player"가 포함되어 있는지 모두 검사합니다.
        if (collision.CompareTag("Player") || collision.name.ToLower().Contains("player"))
        {
            // 현재 활성화된 장면의 빌드 번호(인덱스)를 가져옵니다.
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (isRightPortal)
            {
                // [오른쪽 포탈] 다음 장면으로 이동
                // 전체 등록된 장면 개수보다 작을 때만 이동 (마지막 장면 에러 방지)
                if (currentSceneIndex < SceneManager.sceneCountInBuildSettings - 1)
                {
                    SceneManager.LoadScene(currentSceneIndex + 1);
                }
                else
                {
                    Debug.LogWarning("마지막 장면입니다. 다음 장면이 없습니다!");
                }
            }
            else
            {
                // [왼쪽 포탈] 이전 장면으로 이동
                // 첫 번째 장면(0번)보다 클 때만 이동 (첫 장면 에러 방지)
                if (currentSceneIndex > 0)
                {
                    SceneManager.LoadScene(currentSceneIndex - 1);
                }
                else
                {
                    Debug.LogWarning("첫 번째 장면입니다. 이전 장면이 없습니다!");
                }
            }
        }
    }
}