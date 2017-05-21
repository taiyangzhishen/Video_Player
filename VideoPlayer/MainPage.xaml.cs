using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace VideoPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ViewModel.VideoViewModel videoViewModel;
        public MainPage()
        {
            this.InitializeComponent();
            videoViewModel = new ViewModel.VideoViewModel();
            this.DataContext = videoViewModel;
           
        }

        int pages = 1;
        bool isAtTop = false;
        bool isAtButtom = false;
        //判断滚动条是否在最底部
        public void IsVerticalScrollBarAtButtom()
        {
                double dVer = scrollviewer.VerticalOffset;
                double dViewport = scrollviewer.ViewportHeight;
                double dExtent = scrollviewer.ExtentHeight;
                if (dVer != 0)
                {
                    if (dVer + dViewport == dExtent)
                    {
                        isAtButtom = true;
                    }
                    else
                    {
                        isAtButtom = false;
                    }
                }
                else
                {
                    isAtButtom = false;
                }
        }

        //判断滚动条是否在最顶部
        public void IsVerticalScrollBarAtTop()
        {
                double dVer = scrollviewer.VerticalOffset;
                if (dVer == 0)
                {
                    isAtTop = true;
                }
                else
                {
                    isAtTop = false;
                }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await FirstStep();
        }

        //JSON反序列化
        private async Task FirstStep()
        {
            string json = await HttpRequest.HttpRequest.MainPageRequest(41, pages);
            Model.Rootobject root;
            if (!System.String.IsNullOrWhiteSpace(json))
            {
                try
                {
                    root = JsonConvert.DeserializeObject<Model.Rootobject>(json);
                    videoViewModel.Contentlist = root.showapi_res_body.pagebean.contentlist;
                    listview.ItemsSource = videoViewModel.Contentlist;
                }
                catch
                {
                    ContentDialog errorDialog = new ContentDialog()
                    {
                        Title = "返回json失败",
                        Content = "请重试",
                        PrimaryButtonText = "确定"
                    };
                    ContentDialogResult result = await errorDialog.ShowAsync();
                }
            }
        }

        //下拉刷新、底部加载更多
        private async void scrollviewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            IsVerticalScrollBarAtButtom();
            IsVerticalScrollBarAtTop();
            if (isAtButtom == true)
            {
                isAtButtom = false;
                scrollviewer.ScrollToVerticalOffset(20);
                pages++;
                progressbar.Visibility = Visibility.Visible;
                await FirstStep();
                progressbar.Visibility = Visibility.Collapsed;
            }
            if (isAtTop == true)
            {
                isAtTop = false;
                if(pages > 1)
                {
                    pages--;
                }
                pages = 1;
                progressring.Visibility = Visibility.Visible;
                await FirstStep();
                progressring.Visibility = Visibility.Collapsed;
            }
        }

        //下载视频
        private async void down_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int i = listview.SelectedIndex;
                string strURL = videoViewModel.Contentlist[i].video_uri;
                string time = videoViewModel.Contentlist[i].create_time;
                ContentDialog dDialog = new ContentDialog()
                {
                    Title = "下载"+i,
                    Content = strURL + "开始下载了",
                    PrimaryButtonText = "确定"
                };
                ContentDialogResult dresult = await dDialog.ShowAsync();
               // string n = time + ".mp4";
               // Frame root = Window.Current.Content as Frame;
               // root.Navigate(typeof(pages.DownLoad), n);

                await Load(strURL, time);

                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "下载",
                    Content = strURL+"下载成功！",
                    PrimaryButtonText = "确定"
                };
                ContentDialogResult result = await errorDialog.ShowAsync();
            }
            catch
            {
                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "下载",
                    Content = "下载失败",
                    PrimaryButtonText = "确定"
                };
                ContentDialogResult result = await errorDialog.ShowAsync();
            }
            
        }

        //下载视频到本地
        public async Task<StorageFile> Load(string url, string time)
        {
            try
            {
                var httpClient = new HttpClient();
                var buffer = await httpClient.GetBufferAsync(new Uri(url));
                if (buffer != null && buffer.Length > 0u)
                {
                    var file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(time+".mp4", CreationCollisionOption.ReplaceExisting);
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await stream.WriteAsync(buffer);
                        await stream.FlushAsync();
                    }
                    return file;
                }
            }
            catch
            {
                ContentDialog errorDialog = new ContentDialog()
                {
                    Title = "下载",
                    Content = "下载失败",
                    PrimaryButtonText = "确定"
                };
                ContentDialogResult result = await errorDialog.ShowAsync();
            }
            return null;
        }

        //播放历史导航
        private void History_Click(object sender, RoutedEventArgs e)
        {
            Frame root = Window.Current.Content as Frame;
            root.Navigate(typeof(pages.History));
        }

        //下载记录导航
        private void Down_Click(object sender, RoutedEventArgs e)
        {
            Frame root = Window.Current.Content as Frame;
            root.Navigate(typeof(pages.DownLoad));
        }
        

    }
}
