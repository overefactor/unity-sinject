using System.Linq;
using Overefactor.DI.Runtime.Behaviours;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Overefactor.DI.Editor.Editor.Behaviours
{
    internal class EditorMenuActions
    {
        [MenuItem("GameObject/DI/Root Injector", false)]
        public static void CreateRootInjector()
        {
            var g = new GameObject("Root Injector", typeof(SRootInjector));
            g.transform.hideFlags = HideFlags.HideInInspector;
            
            var scene = SceneManager.GetActiveScene();
            if (Object.FindObjectsOfType<SSceneInject>().All(i => i.gameObject.scene != scene))
                g.AddComponent<SSceneInject>();
            
            Selection.activeGameObject = g;
            Undo.RegisterCreatedObjectUndo(g, "Create Root Injector");
        }

        [MenuItem("GameObject/DI/Root Injector", true)]
        public static bool ValidateCreateRootInjector() => Object.FindObjectOfType<SRootInjector>() == null;

        [MenuItem("GameObject/DI/Scene Inject", false)]
        public static void CreateSceneInject() => CreateSceneInjectInScene(SceneManager.GetActiveScene());

        [MenuItem("GameObject/DI/Scene Inject", true)]
        public static bool ValidateCreateSceneInject() => SceneManager.loadedSceneCount == 1;

        
        [MenuItem("GameObject/DI/Scene Inject (In Active Scene)", false)]
        public static void CreateSceneInjectInActiveScene() => CreateSceneInjectInScene(SceneManager.GetActiveScene());

        [MenuItem("GameObject/DI/Scene Inject (In Active Scene)", true)]
        public static bool ValidateCreateSceneInjectInActiveScene() => SceneManager.loadedSceneCount > 1;
        
        
        
        private static void CreateSceneInjectInScene(Scene scene)
        {
            if (Object.FindObjectsOfType<SSceneInject>().Any(s => s.gameObject.scene == scene)) return;
            
            var g = new GameObject("Scene Inject", typeof(SSceneInject));
            g.transform.hideFlags = HideFlags.HideInInspector;
            SceneManager.MoveGameObjectToScene(g, scene);
            
            Selection.activeGameObject = g;
            Undo.RegisterCreatedObjectUndo(g, "Create Scene Inject");
        }

        [MenuItem("GameObject/DI/Scene Inject (In Each Scene)", true)]
        public static bool ValidateCreateSceneInjectInEachScene() => SceneManager.loadedSceneCount > 1;


        [MenuItem("GameObject/DI/Scene Inject (In Each Scene)", false)]
        public static void CreateSceneInjectInEachScene()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                
                CreateSceneInjectInScene(scene);
            }
        }
        
    }
}