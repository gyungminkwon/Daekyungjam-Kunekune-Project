using UnityEngine;

public class KunekuneProp : MonoBehaviour
{
    [Header("변신 애니메이션 설정")]
    [Tooltip("broomstick")]
    public string transformAnimName = "default_transfrom";
    
    [Tooltip("재생 길이")]
    public float transformDuration = 1.0f;
}