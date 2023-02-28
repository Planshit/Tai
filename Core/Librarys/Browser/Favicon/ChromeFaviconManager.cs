using Core.Enums;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Librarys.Browser.Favicon
{
    public class ChromeFaviconManager : IFaviconManager
    {
        private readonly string _chromeFaviconsPath;
        private readonly string _faviconsTempPath;
        private readonly string _taiFaviconsPath;
        public ChromeFaviconManager(BrowserType browserType = BrowserType.Chrome)
        {
            var localAppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _chromeFaviconsPath =
                browserType == BrowserType.Chrome
                ? Path.Combine(localAppDataDir, "Google", "Chrome", "User Data", "Default", "Favicons")
                : Path.Combine(localAppDataDir, "Microsoft", "Edge", "User Data", "Default", "Favicons");
            _faviconsTempPath = $"{_chromeFaviconsPath}_tai.temp";
            _taiFaviconsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebFavicons");
            if (!File.Exists(_chromeFaviconsPath))
            {
                throw new FileNotFoundException("favicons文件路径没有找到");
            }
            if (!Directory.Exists(_taiFaviconsPath))
            {
                Directory.CreateDirectory(_taiFaviconsPath);
            }
            Debug.WriteLine(_chromeFaviconsPath);
        }
        public async Task<object> GetIconDataAsync(string url)
        {
            if (string.IsNullOrEmpty(url)) { throw new ArgumentNullException("url"); }

            url = UrlHelper.ChromeURLEncode(url);
            var result = await Task.Run(() =>
              {
                  byte[] icon = { };
                  try
                  {
                      File.Copy(_chromeFaviconsPath, _faviconsTempPath, true);

                      using (var con = new SQLiteConnection($"Data Source={_faviconsTempPath}"))
                      {
                          con.Open();
                          string iconId = string.Empty;
                          using (var cmd = new SQLiteCommand($"select * from icon_mapping where page_url like'{url}%' LIMIT 1", con))
                          {
                              using (SQLiteDataReader dr = cmd.ExecuteReader())
                              {
                                  if (dr.Read())
                                  {
                                      iconId = dr["icon_id"].ToString();
                                  }
                              }
                              cmd.CommandText = "select * from ";
                          }
                          if (iconId != string.Empty)
                          {
                              using (var cmd = new SQLiteCommand($"select * from favicon_bitmaps where icon_id={iconId} order by width desc LIMIT 1", con))
                              {
                                  using (SQLiteDataReader dr = cmd.ExecuteReader())
                                  {
                                      if (dr.Read())
                                      {
                                          icon = (System.Byte[])dr["image_data"];
                                      }
                                  }
                              }
                          }
                      }

                      File.Delete(_faviconsTempPath);
                  }
                  catch (Exception ex)
                  {
                      Logger.Error(ex.ToString());
                  }
                  return icon;
              });

            return result.Length == 0 ? null : result;
        }

        public async Task<string> SaveToLocalIconAsync(object data)
        {
            if (data == null) { throw new ArgumentNullException("data"); }
            return await Task.Run(() =>
              {
                  var icon = data as byte[];
                  string iconPath = string.Empty;
                  using (var md5 = MD5.Create())
                  {
                      var hash = md5.ComputeHash(icon);
                      var md5Str = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                      iconPath = Path.Combine(_taiFaviconsPath, $"{md5Str}.png");
                      if (!File.Exists(iconPath))
                      {
                          File.WriteAllBytes(iconPath, icon);
                      }
                      iconPath = Path.Combine("WebFavicons", $"{md5Str}.png");
                  }
                  return iconPath;
              });

        }
    }
}
