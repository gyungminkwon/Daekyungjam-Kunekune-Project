using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : MonoBehaviour
{
    [SerializeField] private string chaseSceneName;
    private bool isPlayerIn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Update()
    {
        if (ProgressManager.Instance.GetFlag(ProgressFlag.HasBusTicket) && GameManager.Instance.currentState != GameState.Interact && isPlayerIn)
        {
            SceneManager.LoadScene(chaseSceneName);        
        } 
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerIn = true;
        }
        
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerIn = false;
        }
    }
}
