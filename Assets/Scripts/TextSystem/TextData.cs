using UnityEngine;

public enum TextType
{
    Monologue,      // 주인공 혼잣말 (클릭 진행, 이동 불가, 타이핑)
    Interaction,    // 사물 상호작용 (클릭 진행, 이동 불가, 타이핑, 일러스트)
    ScreenFade,     // 화면 페이드 인/아웃 (자동 진행, 검은 배경, 페이드)
    SystemGuide     // 시스템 안내 (이동 가능, 즉시 출력, 페이드 인/아웃)
}

[CreateAssetMenu(fileName = "New Text Data", menuName = "UI/Text Data")]
public class TextData : ScriptableObject
{
    public TextType type;
    
    [TextArea(3, 5)]
    public string[] lines;  // 출력할 텍스트

    [Header("Optional Settings")]
    public Sprite objectIcon;   // 사물 상호작용 시 띄울 일러스트 (null 가능)
    public float displayDuration = 2f; // ScreenFade, SystemGuide에서 띄워둘 시간
}
