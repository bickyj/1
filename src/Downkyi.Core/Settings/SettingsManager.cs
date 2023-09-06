﻿using Downkyi.Core.Settings.Models;
using Newtonsoft.Json;

#if DEBUG
#else
using Downkyi.Core.Utils.Encryptor;
#endif

namespace Downkyi.Core.Settings;

public partial class SettingsManager
{
    private static SettingsManager instance;

    // 内存中保存一份配置
    private AppSettings appSettings;

#if DEBUG
    // 设置的配置文件
    private readonly string settingsName = Storage.StorageManager.GetSettings() + "_debug.json";
#else
        // 设置的配置文件
        private readonly string settingsName = Storage.StorageManager.GetSettings();

        // 密钥
        private readonly string password = "YO1J$4#p";
#endif

    /// <summary>
    /// 获取SettingsManager实例
    /// </summary>
    /// <returns></returns>
    public static SettingsManager GetInstance()
    {
        instance ??= new SettingsManager();
        return instance;
    }

    /// <summary>
    /// 隐藏Settings()方法，必须使用单例模式
    /// </summary>
    private SettingsManager()
    {
        appSettings = GetSettings();
    }

    /// <summary>
    /// 获取AppSettingsModel
    /// </summary>
    /// <returns></returns>
    private AppSettings GetSettings()
    {
        if (appSettings != null) { return appSettings; }

        try
        {
            string jsonWordTemplate = string.Empty;
            using (var stream = new FileStream(settingsName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonWordTemplate = reader.ReadToEnd();
            }

#if DEBUG
#else
                // 解密字符串
                jsonWordTemplate = Encryptor.DecryptString(jsonWordTemplate, password);
#endif

            return JsonConvert.DeserializeObject<AppSettings>(jsonWordTemplate);
        }
        catch (Exception e)
        {
            Log.Log.Logger.Error(e);
            return new AppSettings();
        }
    }

    /// <summary>
    /// 设置AppSettingsModel
    /// </summary>
    /// <returns></returns>
    private bool SetSettings()
    {
        try
        {
            string json = JsonConvert.SerializeObject(appSettings);

#if DEBUG
#else
                // 加密字符串
                json = Encryptor.EncryptString(json, password);
#endif

            File.WriteAllText(settingsName, json);
            return true;
        }
        catch (Exception e)
        {
            Log.Log.Logger.Error(e);
            return false;
        }
    }

}