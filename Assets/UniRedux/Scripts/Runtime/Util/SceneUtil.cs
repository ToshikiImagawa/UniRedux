using System;
using System.Collections.Generic;
using System.Linq;
using UniRedux.Provider;
using UnityEngine.SceneManagement;

namespace UniRedux
{
    public static class SceneUtil
    {
        public static IEnumerable<Scene> AllScenes
        {
            get
            {
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    yield return SceneManager.GetSceneAt(i);
                }
            }
        }

        public static IEnumerable<Scene> AllLoadedScenes
        {
            get { return AllScenes.Where(scene => scene.isLoaded); }
        }

        public static IEnumerable<Scene> FindAllLoadedScenes(string[] parentSceneNames) =>
            AllLoadedScenes.Where(scene => parentSceneNames.Contains(scene.name));

        public static IEnumerable<SceneContainerBundle> GetSceneContainerBundles(this IEnumerable<Scene> scenes)
        {
            return from parentScene in scenes
                select parentScene.GetRootGameObjects()
                    .FirstOrDefault(obj => obj.GetComponent<SceneContainerBundle>() != null)
                into sceneContainerBundle
                where sceneContainerBundle != null
                select sceneContainerBundle.GetComponent<SceneContainerBundle>();
        }

        public static IEnumerable<IUniReduxComponent> GetUniReduxComponents(this Scene self)
        {
            return self.GetRootGameObjects()?.SelectMany(
                    obj => obj.GetComponentsInChildren<IUniReduxComponent>(true))
                .ToArray() ?? Array.Empty<IUniReduxComponent>();
        }
    }
}