using UnityEngine;
using UnityEngine.UI;
using System;

public class RectBar : MonoBehaviour
{
    public Action OnComplete;
    private Image progressBar;
    [SerializeField] private float fillSpeed = 0.5f;
    private float targetProgress = 1f;
    private float currentProgress = 0f;
    private bool hasCompleted = false;  // 완료 여부를 체크하는 변수 추가

    void Start()
    {
        progressBar = GetComponent<Image>();
        progressBar.fillAmount = 0f;
    }

    void Update()
    {
        if (currentProgress < targetProgress)
        {
            currentProgress += fillSpeed * Time.deltaTime;
            progressBar.fillAmount = currentProgress;
        }
        else if (!hasCompleted)  // 아직 완료되지 않았을 때만 실행
        {
            hasCompleted = true;  // 완료 표시
            OnComplete?.Invoke();  // 한 번만 호출됨
        }
    }
}