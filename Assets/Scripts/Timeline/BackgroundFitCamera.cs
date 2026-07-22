using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundFitCamera : MonoBehaviour
{
    void Start()
    {
        Camera cam = Camera.main;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float worldHeight = cam.orthographicSize * 2f;
        float worldWidth = worldHeight * cam.aspect;

        Vector2 spriteSize = sr.sprite.bounds.size;

        transform.localScale = new Vector3(
            worldWidth / spriteSize.x,
            worldHeight / spriteSize.y,
            1f);
    }

    void LateUpdate()
    {
        Camera cam = Camera.main;
        transform.localPosition = new Vector3(cam.transform.position.x, cam.transform.position.y, 0);
    }
}