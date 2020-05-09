using Jint;
using Newtonsoft.Json;
using System;
using System.Timers;
using System.Net;

namespace JintAddons
{
    public class JintAddons
    {
        private static Action<string,string> CacheStore = null;
        private static Func<string, string> CacheRetrieve = null;

        public static void Inject(Engine engine, bool debugMode = false, bool forceSafeMode = false)
        {
            new Plugins.Injector().Inject(engine);
            new Extensions.Injector().Inject(engine);
            debug = debugMode;
            safeMode = forceSafeMode;
        }
        public static void SetCatchingStrategy(Action<string,string> store, Func<string, string> retrieve)
        {
            CacheStore = store;
            CacheRetrieve = retrieve;
        }

        public static void LoadHostedScript(Engine engine,string url,bool runAsJintScript = true)
        {
            CacheData data = new CacheData() {
            Data = "",
            Size = 0
            };
            var urlUri = new Uri(url);
            var cacheKey = $"{url.Length}-{urlUri.Host}-{urlUri.Segments[urlUri.Segments.Length - 1]}";
            if(CacheRetrieve != null)
            {
                if (!string.IsNullOrEmpty(CacheRetrieve.Invoke(cacheKey)))
                {
                    data = JsonConvert.DeserializeObject<CacheData>(CacheRetrieve.Invoke(cacheKey));
                }
            }
            var remoteFileSize = Utils.CatchingHelper.GetRemoteFileSize(url);
            if (data.Size != remoteFileSize && remoteFileSize != -1 )
            {
                var content = new WebClient().DownloadString(url);
                data = new CacheData()
                {
                    Data = content,
                    Size = remoteFileSize

                };
                if(CacheStore != null)
                {
                    CacheStore.Invoke(cacheKey, JsonConvert.SerializeObject(data));
                }
            }

            if (!string.IsNullOrEmpty(data.Data))
            {
                if (runAsJintScript)
                {
                    RunJintScript(engine, data.Data);
                }
                else
                {
                    engine.Execute(data.Data);
                }
               
               
            }

        }

        public static void RunJintScript(Engine eng,string content)
        {
            var normalizedScript = Utils.ScriptHelper.NormalizeScript(content);
             eng.Execute(normalizedScript);
        }

          public static void ListenVariableChanges(Engine eng,string variableName, Action<object> callback,int watchInterval = 350)
        {
           
                var timer = new Timer();
                object catchedObj = null;
                timer.Elapsed += delegate
                {
                    var engObject = eng.GetValue(variableName).ToObject();
                    if (Utils.ObjectUtils.HasDiffs(engObject, catchedObj))
                    {
                        catchedObj = engObject;
                        callback.Invoke(engObject);
                    }                };
                timer.Interval = watchInterval;
                timer.Start();
        }
        public static bool debug = false;
        public static bool safeMode = false;


         class CacheData
        {
           public long Size { get; set; }
           public string Data { get; set; }
             
        }

    }

}
