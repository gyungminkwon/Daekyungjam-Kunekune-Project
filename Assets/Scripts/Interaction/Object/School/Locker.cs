using UnityEngine;

public class Locker : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isMyLocker = false;

    [SerializeField] private TextData notMyLocker;
    [SerializeField] private TextData myLocker;
    [SerializeField] private GameObject passwordPanel;
    private PasswordPanel pp;
    [SerializeField] private GameObject shovel;

    void Awake()
    {
        if (shovel) 
        {
            if (isMyLocker)
            {
                shovel.transform.position = transform.position;
            }
            shovel.SetActive(false);
        }
        
        passwordPanel?.SetActive(false);
        pp = passwordPanel?.GetComponent<PasswordPanel>();
    }

    void Start()
    {
        pp.OnCorrectPassword += OpenLocker;
    }

    public void OnInteractPressed()
    {
        if (!ProgressManager.Instance.GetFlag(ProgressFlag.BrokenPot)) return;
        
        if (isMyLocker)
        {
            TextManager.Instance?.PlayText(myLocker);
            passwordPanel?.SetActive(true);
        }
        else
        {
            TextManager.Instance?.PlayText(notMyLocker);
        }
    }

    public void OpenLocker()
    {
        if (shovel) shovel.SetActive(true);
        passwordPanel?.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
    }

    public void OnInteractHeld() {}
    public void OnInteractReleased() {}

    public string GetInteractPrompt()
    {
        return "사물함 (F)";
    }
}
