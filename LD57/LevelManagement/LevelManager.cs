using qASIC.Console;
using Stride.Core.Serialization;

namespace LD57.LevelManagement
{
    [LogColor(138, 12, 242)]
    [LogPrefix("Level Manager")]
    public class LevelManager : SyncScript
    {
        public string defaultLevelTag;
        public List<LevelData> levels = new List<LevelData>();

        private string tag;
        private List<LoadedScene> loadedScenes = new List<LoadedScene>();

        public string CurrentLevelTag =>
            tag;

        public override void Start()
        {
            Services.AddService(this);

            LoadLevel(defaultLevelTag);
        }

        public override void Update()
        {
            if (Input.IsKeyPressed(Keys.D1))
                LoadLevel("mm");

            if (Input.IsKeyPressed(Keys.D2))
                LoadLevel("l0");
        }

        public void LoadLevel(string tag)
        {
            var data = levels.Where(x => x.tag == tag)
                .FirstOrDefault();

            if (data == null)
            {
                qDebug.LogError($"Couldn't load level '{tag}', level data doesn't exist!");
                return;
            }

            this.tag = tag;

            foreach (var item in new List<LoadedScene>(loadedScenes))
            {
                if (!data.utilityScenes.Any(x => x.Url == item.url.Url))
                {
                    Entity.Scene.Children.Remove(item.scene);
                    Content.Unload(item.scene);
                    loadedScenes.Remove(item);
                }
            }

            foreach (var item in data.scenes)
                LoadScene(item);

            foreach (var item in data.utilityScenes)
                LoadScene(item);

            qDebug.Log($"Loaded level '{tag}'.");
        }

        private void LoadScene(UrlReference<Scene> url)
        {
            if (!loadedScenes.Any(x => x.url.Url == url.Url))
            {
                var scene = Content.Load(url);
                Entity.Scene.Children.Add(scene);
                loadedScenes.Add(new LoadedScene()
                {
                    scene = scene,
                    url = url,
                });
            }
        }

        [DataContract]
        public class LevelData
        {
            public string tag;
            public List<UrlReference<Scene>> scenes = new List<UrlReference<Scene>>();
            public List<UrlReference<Scene>> utilityScenes = new List<UrlReference<Scene>>();
        }

        public struct LoadedScene
        {
            public Scene scene;
            public UrlReference<Scene> url;
        }
    }
}
