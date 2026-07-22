using UnityEngine;
using System;
using System.Text;
using TMPro;
using System.Collections;

public class PasswordPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private string correctPassword = "0724";
    [SerializeField] private int maxLength = 4;
    
    private StringBuilder input = new();

    public event Action OnCorrectPassword;

    void Update()
    {
        displayText.text = input.ToString();
    }

    public void InputNumber(int num)
    {
        if (input.Length >= maxLength) return;

        input.Append(num);
        RefreshUI();

        if (input.Length >= 4) Enter();
    }

    public void Clear()
    {
        input.Clear();

        RefreshUI();
    }

    public void Enter()
    {
        if (input.ToString() == correctPassword)
        {
            OnCorrectPassword?.Invoke();
            Debug.Log("정답");
        }
        else
        {
            Debug.Log("오답");

            StartCoroutine(BlinkDisplay());
            RefreshUI();
        }
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    private void RefreshUI()
    {
        displayText.text = input.ToString();
    }

    private IEnumerator BlinkDisplay()
    {
        float timer = 0f;

        displayText.alpha = 1f;
        while (timer < 0.25f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        displayText.alpha = 0f;

        timer = 0f;
        while (timer < 0.25f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        displayText.alpha = 1f;

        yield return new WaitForSeconds(0.25f);

        Clear();
    }
}
