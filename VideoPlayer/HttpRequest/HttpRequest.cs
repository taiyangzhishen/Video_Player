using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace VideoPlayer.HttpRequest
{
    public class HttpRequest
    {
        private const string video_api = "http://route.showapi.com/255-1?type={0}&page={1}&showapi_appid={2}&showapi_sign={3}";
        public static async Task<string> MainPageRequest(int type, int page)
        {
            HttpClient httpclient = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();
            string result = null;
            string api = video_api.Replace("{0}", type.ToString());
            api = api.Replace("{1}", page.ToString());
            api = api.Replace("{2}", "38536");
            api = api.Replace("{3}", "149237eaa6c4453b815024f95a35bcc1");
            try
            {
                response = await httpclient.GetAsync(api);
                result = await response.Content.ReadAsStringAsync();
            }
            catch
            {
                ContentDialog noWifiDialog = new ContentDialog()
                {
                    Title = "网络请求失败",
                    Content = "请检查网络连接",
                    PrimaryButtonText = "确定"
                };
                ContentDialogResult results = await noWifiDialog.ShowAsync();
            }
            return result;
        }
    }
}
