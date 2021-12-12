using Core.Event;
using Core.Librarys;
using Core.Models.Config;
using Core.Servicers.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Instances
{
    public class AppConfig : IAppConfig
    {
        private string fileName;
        private ConfigModel config;

        public event AppConfigEventHandler ConfigChanged;
        private ConfigModel oldConfig;

        public AppConfig()
        {
            fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "AppConfig.json");
        }
        public void Load()
        {
            string configText = string.Empty;
            try
            {
                if (File.Exists(fileName))
                {
                    configText = File.ReadAllText(fileName);
                    config = JsonConvert.DeserializeObject<ConfigModel>(configText);
                }
                else
                {
                    //  创建基础配置
                    CreateDefaultConfig();

                    Save();
                }

                CopyToOldConfig();
            }
            catch (JsonSerializationException ex)
            {
                HandleLoadFail(ex.Message, configText);
            }
            catch (JsonReaderException ex)
            {
                HandleLoadFail(ex.Message, configText);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private void CopyToOldConfig()
        {
            string configStr = JsonConvert.SerializeObject(config);
            oldConfig = JsonConvert.DeserializeObject<ConfigModel>(configStr);
        }

        private void HandleLoadFail(string err, string configText)
        {
            //  创建基础配置
            CreateDefaultConfig();

            Save();

            Logger.Error(err + "\r\nConfig content: " + configText);
        }
        private void CreateDefaultConfig()
        {
            config = new ConfigModel();
            config.Links = new List<Models.Config.Link.LinkModel>();

            config.General = new GeneralModel();
            config.General.IsStartatboot = false;
        }

        public ConfigModel GetConfig()
        {
            return config;
        }


        public void Save()
        {
            try
            {
                string dir = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(fileName, JsonConvert.SerializeObject(config));

                ConfigChanged?.Invoke(oldConfig, config);

                CopyToOldConfig();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }
    }
}
