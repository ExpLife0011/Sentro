﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sentro.Utilities
{
    class Settings
    {
        public const string Tag = "Settings";
        private static Settings _settingsInstance;
        public dynamic Setting;

        public static Settings GetInstance()
        {
            return _settingsInstance ?? (_settingsInstance = new Settings());
        }

        private Settings()
        {            
            Task.Run(() =>
            {
                while (true)
                {
                    Reload();
                    Thread.Sleep(Convert.ToInt32(Setting.settingsReloadRate));
                }
            });
        }

        private void Reload()
        {
            try
            {
                var jsonSettings = File.ReadAllText("settings.json");
                Setting = JObject.Parse(jsonSettings);
                Setting = JsonConvert.DeserializeObject<dynamic>(jsonSettings);                
            }
            catch (Exception e)
            {
                ConsoleLogger.GetInstance().Log(Tag, LogLevel.Error, e.Message);
                ConsoleLogger.GetInstance().Log(Tag,LogLevel.Error,e.StackTrace);
            }
        }
    }    
}