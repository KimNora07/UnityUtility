using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader 
{
    /// <summary>
    /// 비동기 씬 전환
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="sceneName">이동할 씬</param>
    /// <param name="onComplete">씬 이동이 완료되었을 때의 이벤트</param>
    public static void LoadSceneAsync(MonoBehaviour runner, string sceneName, Action onComplete = null)
    {
        runner.StartCoroutine(LoadSceneAsync(sceneName, onComplete));
    }
    
    private static IEnumerator LoadSceneAsync(string sceneName, Action onComplete = null)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        if (operation != null)
        {
            operation.allowSceneActivation = false;

            // 로딩 90% 도달 후 대기
            while (operation.progress < 0.9f)
            {
                yield return null;
            }

            // 로딩 완료 처리 
            yield return new WaitForSeconds(1f); 
            operation.allowSceneActivation = true;
        }

        onComplete?.Invoke();
    }
}
