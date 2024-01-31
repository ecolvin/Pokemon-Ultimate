using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;

    List<ISaveable> saveablesInScene;

    public bool IsLoaded{ get; private set;}

    void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {

            LoadScene();
            GameController.Instance.SetCurrentScene(this);

            foreach(SceneDetails scene in connectedScenes)
            {
                scene.LoadScene();
            }

            SceneDetails prevScene = GameController.Instance.PrevScene;
            if(prevScene != null)
            {
                List<SceneDetails> prevConnectedScenes = prevScene.connectedScenes;
                foreach(SceneDetails scene in prevConnectedScenes)
                {
                    if(!connectedScenes.Contains(scene) && scene != this)
                    {
                        scene.UnloadScene();
                    }
                }
                if(!connectedScenes.Contains(prevScene) && prevScene != this)
                {
                    prevScene.UnloadScene();
                }
            }
        }
    }

    List<ISaveable> GetSaveablesInScene()
    {
        return FindObjectsOfType<MonoBehaviour>().Where(x => x.gameObject.scene == SceneManager.GetSceneByName(gameObject.name)).OfType<ISaveable>().ToList();
    }

    public void LoadScene()
    {
        if(!IsLoaded)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

            op.completed += (AsyncOperation op) =>
            {
                saveablesInScene = GetSaveablesInScene();
                SaveManager.Instance.LoadDataForList(saveablesInScene);
            };
        }
    }
        
    public void UnloadScene()
    {
        if(IsLoaded)
        {
            SaveManager.Instance.SaveDataForList(saveablesInScene);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
}
