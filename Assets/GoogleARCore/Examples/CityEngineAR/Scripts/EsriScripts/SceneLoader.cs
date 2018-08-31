using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public void Load(string currentSceneName)
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        StartCoroutine(MoveAfterLoad(scene));
    }

    private IEnumerator MoveAfterLoad(Scene scene)
    {
        while (scene.isLoaded == false)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Moving Scene " + transform.position.x);
        Debug.Log("Moving Scene " + transform.position.y);
        Debug.Log("Moving Scene " + transform.position.z);

        var rootGameObjects = scene.GetRootGameObjects();
        foreach (var rootGameObject in rootGameObjects)
            rootGameObject.transform.position += transform.position;

    }
}