using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour, IDataPersistance
{
    public bool isFadingIn { get; private set; }
    public bool isFadingOut { get; private set; }

    [SerializeField] private Camera mainCamera;
    private SceneField currentActiveScene;
    [SerializeField] private SerializedDictionary<SceneField, List<SceneField>> adjacentScene;

    [SerializeField] private Image fadeInOutImage;
    [Range(0.1f, 3.0f), SerializeField] private float fadeOutTime;
    [Range(0.1f, 3.0f), SerializeField] private float fadeInTime;

    [SerializeField] private Color currentColor;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TMP_Text loadingBarText;
    [SerializeField] private List<string> loadingText;

    private Dictionary<string, AsyncOperation> currentAsyncOperationDictionary = new Dictionary<string, AsyncOperation>();
    private DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt;
    private SceneConnectorInteraction.Direction direction;
    private float elapsedTime;
    private float currentProgress;
    private bool useLoadingBar;


    private void Awake()
    {
        currentActiveScene = new SceneField(SceneManager.GetActiveScene().name);
        currentColor.a = 0.0f;
        fadeInOutImage.color = currentColor;
        // Debug.Log("SceneManager.GetSceneByName(\"SampleScene\").name: " + SceneManager.GetSceneByName("SampleScene").name); // SampleScene
        // Debug.Log("SceneManager.GetSceneByName(\"SampleScene2\").name: " + SceneManager.GetSceneByName("SampleScene2").name); // null
        // Debug.Log("SceneManager.GetSceneByName(\"NonExistingScene\").name: " + SceneManager.GetSceneByName("NonExistingScene").name); // null
        // Debug.Log("SceneManager.GetAllScenes().Contains(SceneManager.GetSceneByName(\"NonExistingScene\")): " + SceneManager.GetAllScenes().Contains(SceneManager.GetSceneByName("NonExistingScene"))); // false
        // SceneManager.LoadSceneAsync("SampleScene2", LoadSceneMode.Additive);
        // Debug.Log("SceneManager.GetSceneByName(\"SampleScene2\"): " + SceneManager.GetSceneByName("SampleScene2").name);
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        loadingBar.value = Mathf.MoveTowards(loadingBar.value, currentProgress, 3 * Time.unscaledDeltaTime);
        
        if (isFadingOut)
        {
            elapsedTime += Time.unscaledDeltaTime;

            if (fadeInOutImage.color.a < 1.0f)
            {
                currentColor.a = Mathf.Clamp(elapsedTime / fadeOutTime, 0.0f, 1.0f);
                fadeInOutImage.color = currentColor;
            }
            else
            {
                loadingBarText.text = "Game Tips | " + RandomFunction.GetRandom<List<string>, string>(loadingText);
                isFadingOut = false;
                elapsedTime = 0.0f;
            }
        }
        else if (isFadingIn)
        {
            elapsedTime += Time.unscaledDeltaTime;

            if (fadeInOutImage.color.a > 0.0f)
            {
                currentColor.a = Mathf.Clamp(1.0f - elapsedTime / fadeInTime, 0.0f, 1.0f);
                fadeInOutImage.color = currentColor;
            }
            else
            {
                isFadingIn = false;
                Manager.Instance.gameManager.ResumeGame();
                // Manager.Instance.gameManager.player.playerInput.currentActionMap.Enable();
            }
        }
    }

    // called when active scene changed
    public async void FadeIn()
    {
        if (useLoadingBar)
        {
            while (!loadingBar.gameObject.activeSelf)
            {
                await Task.Delay(10);
            }
        }

        loadingBar.gameObject.SetActive(false);
        fadeInOutImage.color = currentColor;
        isFadingIn = true;
    }

    // called on scene transition
    public void FadeOut()
    {
        Manager.Instance.gameManager.PauseGame();
        // Manager.Instance.gameManager.player.playerInput.currentActionMap.Disable();
        elapsedTime = 0.0f;
        fadeInOutImage.color = currentColor;
        isFadingOut = true;
    }

    // called when player arrives specific position for scene transition
    public void SceneTransition(SceneField targetScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        if (useFadeInOut)
        {
            FadeOut();
        }

        // if scene is not in SceneManager, it means that the scene is neither loaded nor currently loading
        if (!IsLoadingScene(targetScene.SceneName))
        {
            Debug.Log("Scene was not loaded. Start Loading the target scene: " + targetScene.SceneName);
            currentAsyncOperationDictionary.Add(targetScene.SceneName, SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive));
        }

        if (!SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is not done loading.");
            Manager.Instance.gameManager.PauseGame();

            this.useLoadingBar = useLoadingBar;
            if (useLoadingBar)
            {
                LoadingBar(targetScene.SceneName);
            }
        }

        this.doorToSpawnAt = doorToSpawnAt;
        this.direction = SceneConnectorInteraction.Direction.None;
        currentActiveScene = targetScene;
    }

    public void SceneTransition(SceneField targetScene, SceneConnectorInteraction.Direction direction, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        if (useFadeInOut)
        {
            FadeOut();
        }

        // if scene is not in SceneManager, it means that the scene is neither loaded nor currently loading
        if (!IsLoadingScene(targetScene.SceneName))
        {
            Debug.Log("Scene was not loaded. Start Loading the target scene: " + targetScene.SceneName);
            currentAsyncOperationDictionary.Add(targetScene.SceneName, SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive));
        }

        if (!SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is not done loading.");
            Manager.Instance.gameManager.PauseGame();

            this.useLoadingBar = useLoadingBar;
            if (useLoadingBar)
            {
                LoadingBar(targetScene.SceneName);
            }
        }

        this.doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None;
        this.direction = direction;
        currentActiveScene = targetScene;
    }

    public void SceneTransition(SceneField targetScene, bool useFadeInOut = false, bool useLoadingBar = false)
    {
        if (useFadeInOut)
        {
            FadeOut();
        }

        // if scene is not in SceneManager, it means that the scene is neither loaded nor currently loading
        if (!IsLoadingScene(targetScene.SceneName))
        {
            Debug.Log("Scene was not loaded. Start Loading the target scene: " + targetScene.SceneName);
            currentAsyncOperationDictionary.Add(targetScene.SceneName, SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive));
        }

        if (!SceneManager.GetSceneByName(targetScene).isLoaded)
        {
            Debug.Log($"Target scene \"{targetScene.SceneName}\" is not done loading.");
            Manager.Instance.gameManager.PauseGame();

            this.useLoadingBar = useLoadingBar;
            if (useLoadingBar)
            {
                LoadingBar(targetScene.SceneName);
            }
        }

        this.doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None;
        this.direction = SceneConnectorInteraction.Direction.None;
        currentActiveScene = targetScene;
    }

    // called when scene transition is complete
    // load the scenes that are adjacent to current scene and unload that are not
    public void LoadAndUnloadScenes()
    {
        UnloadScene();
        LoadScene();
    }

    // below function loads the scene that is adjacent to target scene
    private void LoadScene()
    {
        foreach (SceneField sceneField in adjacentScene[currentActiveScene])
        {
            if (!IsLoadingScene(sceneField.SceneName))
            {
                Debug.Log($"Load scene: {sceneField.SceneName}");
                AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneField, LoadSceneMode.Additive);
                currentAsyncOperationDictionary.Add(sceneField.SceneName, loadingScene);
                loadingScene.allowSceneActivation = false;
            }
        }
    }

    // below function unloads the scenes that seems to be unnecessary
    // wait until fading out is done
    private async void UnloadScene()
    {
        while (isFadingOut)
        {
            await Task.Delay(10);
        }

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene currentScene = SceneManager.GetSceneAt(i);

            if (!IsAdjacentScene(currentScene.name) && !currentActiveScene.SceneName.Equals(currentScene.name))
            {
                Debug.Log($"Unload scene: {currentScene.name}");
                SceneManager.UnloadSceneAsync(currentScene);
            }
        }
    }

    private bool IsAdjacentScene(string sceneName)
    {
        foreach(SceneField sceneField in adjacentScene[currentActiveScene])
        {
            if (sceneName.Equals(sceneField.SceneName))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsLoadingScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Equals(sceneName))
            {
                return true;
            }
        }

        return false;
    }

    private void FindDoor(DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt)
    {
        Debug.Log("doorToSpawnAt: " + doorToSpawnAt);
        if (doorToSpawnAt.Equals(DoorTriggerInteraction.DoorToSpawnAt.None)) return;
        
        DoorTriggerInteraction[] doorTriggerInteractions = FindObjectsOfType<DoorTriggerInteraction>();

        foreach (DoorTriggerInteraction doorTriggerInteraction in doorTriggerInteractions)
        {
            if (doorTriggerInteraction.currentDoorIndex.Equals(doorToSpawnAt))
            {
                Debug.Log("Door found");
                Collider2D doorCollider = doorTriggerInteraction.gameObject.GetComponent<Collider2D>();
                Vector2 groundPosition = new Vector2(doorCollider.bounds.center.x, doorCollider.bounds.min.y);
                Manager.Instance.gameManager.player.transform.position = groundPosition + Manager.Instance.gameManager.player.entityCollider.bounds.extents.y * Vector2.up;
                break;
            }
        }
        Debug.Log("Position Set");
    }

    private void ChangePosition(SceneConnectorInteraction.Direction direction)
    {
        if (direction.Equals(SceneConnectorInteraction.Direction.None)) return;

        Vector2 targetPosition = Manager.Instance.gameManager.player.transform.position;

        switch (direction)
        {
            case SceneConnectorInteraction.Direction.Up:
                targetPosition += Manager.Instance.gameManager.player.entityCollider.bounds.size.y * Vector2.up; break;
            case SceneConnectorInteraction.Direction.Down:
                targetPosition += Manager.Instance.gameManager.player.entityCollider.bounds.size.y * Vector2.down; break;
            case SceneConnectorInteraction.Direction.Left:
                targetPosition += Manager.Instance.gameManager.player.entityCollider.bounds.size.x * Vector2.left; break;
            case SceneConnectorInteraction.Direction.Right:
                targetPosition += Manager.Instance.gameManager.player.entityCollider.bounds.size.x * Vector2.right; break;
            default: break;
        }

        Manager.Instance.gameManager.player.transform.position = targetPosition;
    }

    private void OnActiveSceneChanged(Scene current, Scene next)
    {
        Debug.Log($"Active scene changed from \"{current.name}\" to \"{next.name}\"");
        FadeIn();
        LoadAndUnloadScenes();
        FindDoor(doorToSpawnAt);
        // ChangePosition(direction);
        mainCamera.transform.position = Manager.Instance.gameManager.player.transform.position + Manager.Instance.gameManager.player.transform.right * 5.0f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Loading Scene \"{scene.name}\" is done.");

        currentAsyncOperationDictionary.Remove(scene.name);

        if (!SceneManager.GetActiveScene().name.Equals(currentActiveScene.SceneName))
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentActiveScene.SceneName));
        }
    }

    public async void LoadingBar(string loadingScene)
    {
        AsyncOperation asyncOperation;

        if (!currentAsyncOperationDictionary.TryGetValue(loadingScene, out asyncOperation))
        {
            Debug.Log($"Can't find async operation of {loadingScene}. Inactivate loading bar.");
            return;
        }

        currentProgress = 0.0f;

        while (currentColor.a < 1.0f)
        {
            await Task.Delay(10);
        }

        loadingBar.gameObject.SetActive(true);

        do
        {
            currentProgress = asyncOperation.progress;
            await Task.Delay(10);
        } while (asyncOperation.progress < 1.0f);
    }

    public void LoadData(GameData data)
    {
        
    }

    public void SaveData(GameData data)
    {
        if (currentActiveScene.SceneName != "MainMenu")
        {
            data.currentScene = currentActiveScene.SceneName;
            // data system saves the game before loading the game
        }
    }
}
