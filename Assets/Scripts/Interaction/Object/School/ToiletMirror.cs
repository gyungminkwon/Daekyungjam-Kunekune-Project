using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToiletMirror : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite horrorSprite;
    [SerializeField] private Sprite originalSprite;
    [SerializeField] private Image image;
    [SerializeField] private float delayTime = 0.15f;
    
    public void OnInteractPressed()
    {
        if (image == null || horrorSprite == null || originalSprite == null) return;

        StartCoroutine(ChangeSprite());
    }

    private IEnumerator ChangeSprite()
    {
        image.sprite = originalSprite;
        image.gameObject.SetActive(true);

        float delay = Random.Range(1f, 2f);
        yield return new WaitForSeconds(delay);

        image.sprite = horrorSprite;
        yield return new WaitForSeconds(delayTime);
        image.sprite = originalSprite;

        yield return new WaitForSeconds(1f);
        image.gameObject.SetActive(false);
    }
    public void OnInteractHeld() {}
    public void OnInteractReleased() {}

    public string GetInteractPrompt()
    {
        return "세면대 (F)";
    }
}
