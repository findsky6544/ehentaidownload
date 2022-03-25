using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace EHentaiDwonload
{
    public partial class Main : Form
    {
        private Dictionary<string, string> config = new Dictionary<string, string>();//设置
        private CookieCollection cookies = new CookieCollection();
        private WebProxy webProxy;//网络代理

        private static HttpClient client;//用于发送网页请求

        private AutoSizeFormClass asc = new AutoSizeFormClass();//控件大小自适应

        private Thread addSearchListThread;//添加搜索列表线程
        private bool isAddSearchListPause = false;//添加搜索列表线程是否暂停
        private ManualResetEvent addSearchListEvent;//信号量，用于阻塞

        private Thread downloadThread;//下载线程
        private bool isDownloadPause = false;
        private ManualResetEvent downloadEvent;

        private Thread timerThread;//计时器线程
        private bool isTimerPause = false;

        private AutoResetEvent downloadImageEvent;//下载图片信号量，用于限制一次只下载一张图

        private int downloadingIndex = -1;//当前下载下标
        private int allPage;//总下载页数
        private int allCompletePage;//总完成页数
        private int allRemainPage;//总剩余页数
        private int completeManga = 0;//总完成漫画数

        private string domain = ".e-hentai.org";
        private string path;



        private long programStart;// 记录线程开始时间

        private long pauseStart;// 记录线程暂停开始时间  

        private long pauseCount = 0;// 线程暂停的总时间  
        private long totalTime = 0;//显示总时间

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            //控件大小自适应
            if (!asc.isInit)
            {
                asc.controllInitializeSize(this);
            }

            //读取配置
            readConfig();

            path = config["downloadPath"];

            WebRequestHandler webRequesthandler = new WebRequestHandler();
            //应用cookie
            cookies.Add(new Cookie("sk", "mcenn8w6eeod8of5x697k24xhu5o") { Domain = domain });//默认提取中文标题，应该是这个，不知道是否长时间有效
            cookies.Add(new Cookie("nw", "1") { Domain = domain });//取消警告
            if (!string.IsNullOrEmpty(userIdTextBox.Text))
            {
                cookies.Add(new Cookie("ipb_member_id", userIdTextBox.Text) { Domain = domain });//个人设置，筛选掉不要的分类
            }
            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.Add(cookies);
            webRequesthandler.CookieContainer = cookieContainer;

            if (!string.IsNullOrEmpty(config["serverip"]) && !string.IsNullOrEmpty(config["port"]))
            {
                webProxy = new WebProxy(config["serverip"], int.Parse(config["port"]));
                webRequesthandler.Proxy = webProxy;
            }

            //建立httpclient
            client = new HttpClient(webRequesthandler);

            //进度条文本配置
            oneMangaProgressBarLabel.Parent = oneMangaProgressBar;
            oneMangaProgressBarLabel.Location = new Point(oneMangaProgressBar.Size.Width / 2 - oneMangaProgressBarLabel.Size.Width / 2, oneMangaProgressBar.Size.Height / 2 - oneMangaProgressBarLabel.Size.Height / 2);

            allMangasProgressBarLabel.Parent = allMangasProgressBar;
            allMangasProgressBarLabel.Location = new Point(allMangasProgressBar.Size.Width / 2 - allMangasProgressBarLabel.Size.Width / 2, allMangasProgressBar.Size.Height / 2 - allMangasProgressBarLabel.Size.Height / 2);

            CheckForIllegalCrossThreadCalls = false;//干掉检测 不再检测跨线程
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            if (!asc.isInit)
            {
                asc.controllInitializeSize(this);
            }
            asc.controlAutoSize(this);
        }

        //从文件读取配置
        private void readConfig()
        {
            config.Add("serverip", "");
            config.Add("port", "");
            config.Add("userid", "");
            config.Add("downloadPath", "");

            if (File.Exists("config.txt"))
            {
                using (StreamReader sr = new StreamReader("config.txt"))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] configItem = line.Split('=');
                        if (config.ContainsKey(configItem[0]))
                        {
                            config[configItem[0]] = configItem[1];
                        }
                        else
                        {
                            config.Add(configItem[0], configItem[1]);
                        }
                    }
                }
                ServerIpTextBox.Text = config["serverip"];
                portTextBox.Text = config["port"];
                userIdTextBox.Text = config["userid"];
                downloadPathTextBox.Text = config["downloadPath"];
            }
            else
            {
                //如果没有文件就写入新的
                saveConfig(config);
            }
        }

        //保存配置
        private void saveConfig(Dictionary<string, string> config)
        {
            using (StreamWriter sw = new StreamWriter("config.txt"))
            {
                foreach (KeyValuePair<string, string> kv in config)
                {
                    sw.WriteLine(kv.Key + "=" + kv.Value);
                }
            }
            if (string.IsNullOrEmpty(userIdTextBox.Text))
            {
                cookies.Add(new Cookie("ipb_member_id", userIdTextBox.Text) { Domain = domain });//个人设置，筛选掉不要的分类
            }
            if(!string.IsNullOrEmpty(config["serverip"]) && !string.IsNullOrEmpty(config["port"]))
            {
                webProxy = new WebProxy(config["serverip"], int.Parse(config["port"]));
            }
            path = config["downloadPath"];
        }

        private void addUrlButton_Click(object sender, EventArgs e)
        {
            //修改界面控件状态
            addUrlButton.Enabled = false;
            addUrlButton.Text = "添加中";

            pauseAddUrlButton.Enabled = true;

            stopAddUrlButton.Enabled = true;

            urlTextBox.Enabled = false;

            //开启添加线程
            addSearchListThread = new Thread(new ThreadStart(addSearchListClickAsync));
            addSearchListEvent = new ManualResetEvent(true);
            addSearchListThread.Start();
        }

        private async void addSearchListClickAsync()
        {
            string url = urlTextBox.Text;

            //url转为中文
            url = HttpUtility.UrlDecode(url);
            urlTextBox.Text = url;

            IHtmlDocument doc;
            string title;

            if (url.Contains("f_search") || url.Contains("tag"))
            {
                outputTextBoxAddText("检测到搜索列表:\r\n");

                int currentPage = 1;

                if (url.Contains("f_search"))
                {
                    //输出搜索内容
                    outputTextBoxAddText(url.Substring(url.IndexOf("f_search=") + 9) + "\r\n");
                    //获得搜索当前页
                    if (url.Contains("page="))
                    {
                        currentPage = int.Parse(url.Substring(url.IndexOf("page=") + 5, url.IndexOf("page=") + url.Substring(url.IndexOf("page=")).IndexOf("&"))) + 1;
                    }
                }
                else if (url.Contains("tag/"))
                {
                    //输出搜索内容
                    outputTextBoxAddText(url.Substring(url.IndexOf("tag/") + 4, url.IndexOf("tag/") + 4 + url.Substring(url.IndexOf("tag/") + 4).IndexOf("/")) + "\r\n");
                    //获得搜索当前页
                    currentPage = int.Parse(url.Substring(url.LastIndexOf("/") + 1)) + 1;
                }
                do
                {
                    //暂停前修改界面控件状态
                    if (isAddSearchListPause)
                    {
                        pauseAddUrlButton.Enabled = true;
                        pauseAddUrlButton.Text = "恢复添加";

                        outputTextBoxAddText("暂停中\r\n");
                    }
                    //暂停阻塞
                    addSearchListEvent.WaitOne();
                    //停止前修改界面控件状态
                    if (addSearchListThread.ThreadState == ThreadState.AbortRequested || addSearchListThread.ThreadState == ThreadState.Aborted)
                    {
                        //释放暂停信号量控制
                        addSearchListEvent.Dispose();

                        urlTextBox.Enabled = true;

                        addUrlButton.Enabled = true;
                        addUrlButton.Text = "添加";

                        pauseAddUrlButton.Enabled = false;
                        pauseAddUrlButton.Text = "暂停添加";

                        stopAddUrlButton.Text = "停止添加";

                        outputTextBoxAddText("停止\r\n");
                        return;
                    }

                    //获得当前页面doc
                    doc = await getDocAsync(url);
                    if (doc == null)
                    {
                        return;
                    }

                    //获得页码元素
                    IHtmlCollection<IElement> pages = doc.QuerySelectorAll(".ptt td");

                    //获得最后一页的值
                    IElement totalPageElement = pages[pages.Length - 2];
                    string totalPageStr = totalPageElement.QuerySelector("a").InnerHtml;
                    int totalPage = int.Parse(totalPageStr);
                    outputTextBoxAddText("add page" + currentPage + "/" + totalPage + " start...\r\n");

                    //添加漫画
                    IHtmlCollection<IElement> mangas = doc.QuerySelectorAll(".gltc tr");
                    int addCount = 0;
                    int repeatCount = 0;
                    mangaListView.BeginUpdate();
                    for (int i = 1; i < mangas.Length; i++)
                    {
                        IElement manga = mangas[i];
                        string mangaUrl = manga.QuerySelector(".glname a").GetAttribute("href");

                        //用url判断是否已存在
                        if (!mangaListView.Items.ContainsKey(mangaUrl))
                        {
                            addCount++;

                            //获取并处理标题
                            title = manga.QuerySelector(".glname a .glink").InnerHtml;

                            title = title.Replace('/', '_');
                            title = title.Replace('\\', '_');
                            title = title.Replace(':', '_');
                            title = title.Replace('*', '_');
                            title = title.Replace('?', '_');
                            title = title.Replace('"', '_');
                            title = title.Replace('<', '_');
                            title = title.Replace('>', '_');
                            title = title.Replace('|', '_');

                            //listview添加一行
                            ListViewItem lvi = new ListViewItem();
                            lvi.Name = mangaUrl;//name=key，否则用不了ContainsKey
                            lvi.Text = mangaUrl;
                            lvi.SubItems.Add(title);

                            string mangaPages = manga.QuerySelectorAll(".glhide div").Last().InnerHtml.Split(' ')[0];

                            int fileCount = 0;
                            if (Directory.Exists(path + title))
                            {
                                fileCount = Directory.GetFiles(path + title).Count();
                            }

                            if (int.Parse(mangaPages) == fileCount)
                            {
                                continue;
                            }

                            allPage += int.Parse(mangaPages);
                            lvi.SubItems.Add(mangaPages);

                            lvi.SubItems.Add(fileCount.ToString());

                            lvi.SubItems.Add((int.Parse(mangaPages) - fileCount).ToString());
                            allRemainPage += int.Parse(mangaPages) - fileCount;

                            mangaListView.Items.Add(lvi);
                        }
                        else
                        {
                            repeatCount++;
                        }
                    }
                    mangaListView.EndUpdate();

                    //滚动条跳转到底部
                    mangaListView.EnsureVisible(mangaListView.Items.Count - 1);

                    outputTextBoxAddText("complete。添加" + addCount + "条,重复" + repeatCount + "条\r\n");

                    //下一页
                    IElement urlElement = pages.Last().QuerySelector("a");
                    //如果没有下一页了就退出
                    if (urlElement == null)
                    {
                        break;
                    }

                    //获取下一页url
                    url = urlElement.GetAttribute("href");

                    //每一页添加间隔3秒
                    //Random random = new Random();
                    int random = 3;
                    outputTextBoxAddText("等待" + random + "秒...\r\n");
                    Thread.Sleep(random * 1000);

                    currentPage++;
                } while (true);
                outputTextBoxAddText("添加完成\r\n");
            }
            else
            {
                doc = await getDocAsync(url);
                if (doc == null)
                {
                    return;
                }
                //获取标题
                title = doc.QuerySelector("#gj").InnerHtml;
                title = title.Replace('/', '_');
                title = title.Replace('\\', '_');
                title = title.Replace(':', '_');
                title = title.Replace('*', '_');
                title = title.Replace('?', '_');
                title = title.Replace('"', '_');
                title = title.Replace('<', '_');
                title = title.Replace('>', '_');
                title = title.Replace('|', '_');

                int mangaIndex = 0;

                if (!mangaListView.Items.ContainsKey(url))
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Name = url;
                    lvi.Text = url;
                    lvi.SubItems.Add(title);

                    string mangaPages = doc.QuerySelectorAll(".gdt2")[5].InnerHtml.Split(' ')[0];
                    allPage += int.Parse(mangaPages);
                    lvi.SubItems.Add(mangaPages);

                    lvi.SubItems.Add("0");

                    allRemainPage += int.Parse(mangaPages);
                    lvi.SubItems.Add(mangaPages);

                    mangaListView.Items.Add(lvi);
                    mangaListView.EnsureVisible(mangaListView.Items.Count - 1);
                }
                else
                {
                    outputTextBoxAddText("该漫画已存在于列表中\r\n");

                    mangaListView.Items[mangaIndex].Selected = true;
                    mangaListView.EnsureVisible(mangaIndex);
                }
            }
            //完成后修改界面控件状态
            addUrlButton.Text = "添加";
            addUrlButton.Enabled = true;

            urlTextBox.Text = "";
            urlTextBox.Enabled = true;

            if (mangaListView.Items.Count > 0)
            {
                exportMangaButton.Enabled = true;
                downloadButton.Enabled = true;
            }

            //修改进度条状态
            allMangasProgressBar.Value = completeManga;
            allMangasProgressBar.Maximum = mangaListView.Items.Count;
            setProgressBarLabelText(allMangasProgressBarLabel, "已完成" + completeManga + "本 剩余" + mangaListView.Items.Count + "本 " + allCompletePage + "/" + allPage + "页");
        }

        private void pauseAddUrlButton_Click(object sender, EventArgs e)
        {
            isAddSearchListPause = !isAddSearchListPause;
            if (isAddSearchListPause)
            {
                pauseAddUrlButton.Text = "正在暂停";
                pauseAddUrlButton.Enabled = false;
                addSearchListEvent.Reset();
            }
            else
            {
                addSearchListEvent.Set();
                outputTextBoxAddText("恢复添加\r\n");
                pauseAddUrlButton.Text = "暂停添加";
            }
        }

        private void stopAddUrlButton_Click(object sender, EventArgs e)
        {
            stopAddUrlButton.Text = "正在停止";
            stopAddUrlButton.Enabled = false;
            pauseAddUrlButton.Enabled = false;
            try
            {
                //先通知释放，再给信号，否则可能多跑一轮
                addSearchListThread.Abort();
                addSearchListEvent.Set();
            }
            catch (Exception ex)
            {
                outputTextBoxAddText(ex.ToString() + "\r\n");
            }
        }

        private void ServerIpTextBox_TextChanged(object sender, EventArgs e)
        {
            config["serverip"] = ServerIpTextBox.Text;
        }

        private void portTextBox_TextChanged(object sender, EventArgs e)
        {
            config["port"] = portTextBox.Text;
        }

        private void userIdTextBox_TextChanged(object sender, EventArgs e)
        {
            config["userid"] = userIdTextBox.Text;
        }

        private void downloadPathTextBox_TextChanged(object sender, EventArgs e)
        {
            config["downloadPath"] = downloadPathTextBox.Text;
        }

        private void applyConfigButton_Click(object sender, EventArgs e)
        {
            saveConfig(config);
        }

        public void outputTextBoxAddText(string text)
        {
            outputTextBox.Text += text;
            outputTextBox.Select(outputTextBox.TextLength, 0);
            outputTextBox.ScrollToCaret();
        }

        public async Task<IHtmlDocument> getDocAsync(string url)
        {
            IHtmlDocument doc = null;
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var getResponsestring = await response.Content.ReadAsStringAsync();
                    HtmlParser parser = new HtmlParser();
                    doc = await parser.ParseDocumentAsync(getResponsestring);
                }
                else
                {
                    outputTextBoxAddText(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                outputTextBoxAddText(ex.ToString() + "\r\n");
            }
            return doc;
        }

        private void mangaListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            deleteMangaButton.Enabled = true;
        }

        private void deleteMangaButton_Click(object sender, EventArgs e)
        {
            int index = mangaListView.SelectedItems[0].Index;
            if (index == downloadingIndex)
            {
                MessageBox.Show("该漫画正在下载，不能删除", "", MessageBoxButtons.OK);
            }
            else
            {
                allPage -= string.IsNullOrEmpty(mangaListView.Items[index].SubItems[2].Text) ? 0 : int.Parse(mangaListView.Items[index].SubItems[2].Text);
                allCompletePage -= string.IsNullOrEmpty(mangaListView.Items[index].SubItems[3].Text) ? 0 : int.Parse(mangaListView.Items[index].SubItems[3].Text);
                allRemainPage -= string.IsNullOrEmpty(mangaListView.Items[index].SubItems[4].Text) ? 0 : int.Parse(mangaListView.Items[index].SubItems[4].Text);

                mangaListView.Items.RemoveAt(index);

                if (index < mangaListView.Items.Count)
                {
                    mangaListView.Items[index].Selected = true;
                }
            }

            if (mangaListView.Items.Count == 0)
            {
                deleteMangaButton.Enabled = false;
                exportMangaButton.Enabled = false;
                downloadButton.Enabled = false;
            }

            setProgressBarLabelText(allMangasProgressBarLabel, "已完成" + completeManga + "本 剩余" + mangaListView.Items.Count + "本 " + allCompletePage + "/" + allPage + "页");
        }

        private void exportMangaButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    foreach (ListViewItem lvi in mangaListView.Items)
                    {
                        sw.WriteLine(lvi.Text + "|" + lvi.SubItems[1].Text + "|" + lvi.SubItems[2].Text);
                    }
                }
                outputTextBoxAddText("导出成功\r\n");
            }
        }

        private void ImportMangaButton_Click(object sender, EventArgs e)
        {
            ImportMangaButton.Enabled = false;
            ImportMangaButton.Text = "导入中";

            addUrlButton.Enabled = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Thread importMangaThread = new Thread(new ThreadStart(importMangaSync));
                importMangaThread.Start();
            }
        }

        private void importMangaSync()
        {
            using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
            {
                string line;

                int importCount = 0;
                int repeatCount = 0;
                int completeCount = 0;

                List<ListViewItem> lvis = new List<ListViewItem>();

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    string[] str = line.Split('|');

                    if (!mangaListView.Items.ContainsKey(str[0]))
                    {
                        ListViewItem lvi = new ListViewItem();

                        lvi.Name = str[0];
                        lvi.Text = str[0];
                        lvi.SubItems.Add(str[1]);
                        lvi.SubItems.Add(str[2]);

                        int fileCount = 0;
                        if (Directory.Exists(path + str[1]))
                        {
                            fileCount = Directory.GetFiles(path + str[1]).Count();
                        }

                        int allPageCount = 0;
                        if (!string.IsNullOrEmpty(str[2]))
                        {
                            allPageCount = int.Parse(str[2]);
                        }

                        if (allPageCount == fileCount)
                        {
                            completeCount++;
                            continue;
                        }
                        importCount++;
                        lvi.SubItems.Add(fileCount.ToString());
                        lvi.SubItems.Add((allPageCount - fileCount).ToString());

                        allPage += allPageCount;

                        //allCompletePage += fileCount;

                        allRemainPage += allPageCount - fileCount;

                        lvis.Add(lvi);

                        setProgressBarLabelText(allMangasProgressBarLabel, "已完成" + completeManga + "本 剩余" + importCount + "本 " + allCompletePage + "/" + allPage + "页");
                    }
                    else
                    {
                        repeatCount++;
                    }
                    //mangaListView.EnsureVisible(mangaListView.Items.Count - 1);
                }
                mangaListView.Items.AddRange(lvis.ToArray());
                outputTextBoxAddText("导入成功,共导入" + importCount + "条,重复" + repeatCount + "条,已完成"+completeCount+"条\r\n");

                ImportMangaButton.Enabled = true;
                ImportMangaButton.Text = "导入";

                addUrlButton.Enabled = true;

                if (mangaListView.Items.Count > 0)
                {
                    exportMangaButton.Enabled = true;
                    downloadButton.Enabled = true;
                }
            }
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("请先在设置页填写图片下载路径", "", MessageBoxButtons.OK);
                return;
            }


            downloadButton.Enabled = false;
            downloadButton.Text = "下载中";

            pauseDownloadButton.Enabled = true;

            stopDownloadButton.Enabled = true;

            downloadThread = new Thread(new ThreadStart(downloadClickAsync));
            downloadEvent = new ManualResetEvent(true);
            downloadThread.Start();

            timerThread = new Thread(new ThreadStart(timerRun));
            timerThread.Start();

            downloadImageEvent = new AutoResetEvent(true);
        }

        private async void downloadClickAsync()
        {
            IHtmlDocument doc;

            allMangasProgressBar.Maximum = mangaListView.Items.Count;

            for (int i = 0; i < mangaListView.Items.Count; i++)
            {

                mangaListView.Items[i].Selected = true;
                string url = mangaListView.Items[i].Text;

                bool hasError = false;
                url.Replace("?nw=always", "");

                doc = await getDocAsync(url);
                if (doc == null)
                {
                    // TODO Auto-generated catch block
                    outputTextBoxAddText("无法连接EHentai\r\n");
                    downloadButton.Text = "提取";
                    downloadButton.Enabled = true;
                    pauseDownloadButton.Enabled = false;
                    stopDownloadButton.Enabled = false;
                    timerThread.Abort();

                    return;
                }
                //获取并处理标题
                string title = doc.QuerySelector("#gj").InnerHtml;

                if (title == "")
                {
                    title = doc.Title;
                }
                title = title.Replace('/', '_');
                title = title.Replace('\\', '_');
                title = title.Replace(':', '_');
                title = title.Replace('*', '_');
                title = title.Replace('?', '_');
                title = title.Replace('"', '_');
                title = title.Replace('<', '_');
                title = title.Replace('>', '_');
                title = title.Replace('|', '_');
                string mkdirName = path + title;
                outputTextBoxAddText(mkdirName + " is downloading...\r\n");

                //创建文件夹
                if (!Directory.Exists(mkdirName))
                {
                    Directory.CreateDirectory(mkdirName);
                }
                //获取总页数

                string totalPageStr = mangaListView.Items[i].SubItems[2].Text;
                int totalPage = int.Parse(totalPageStr);

                oneMangaProgressBar.Maximum = totalPage;
                for (int j = 0; j < totalPage; j++)
                {
                    //暂停功能
                    if (isDownloadPause)
                    {
                        pauseDownloadButton.Enabled = true;
                        pauseDownloadButton.Text = "恢复下载";

                        outputTextBoxAddText("已暂停\r\n");
                    }
                    downloadEvent.WaitOne();
                    if (downloadThread.ThreadState == ThreadState.AbortRequested || downloadThread.ThreadState == ThreadState.Aborted)
                    {
                        downloadEvent.Dispose();

                        downloadButton.Enabled = true;
                        downloadButton.Text = "下载";

                        pauseDownloadButton.Enabled = false;
                        pauseDownloadButton.Text = "暂停下载";

                        stopDownloadButton.Text = "停止下载";

                        outputTextBoxAddText("停止下载\r\n");
                        return;
                    }

                    int retryTime = 0;
                    //处理当前页前面的0
                    string pageStr = pageToString(j + 1, totalPageStr.Length);


                    int currentPage = int.Parse(pageStr);

                    oneMangaProgressBar.Value = currentPage;

                    //outputTextBoxAddText(pageStr+"/"+totalPageStr);

                    if (retryTime > 0)
                    {
                        outputTextBoxAddText("重试第" + retryTime + "次");
                    }

                    string imageFilePath = mkdirName + "\\" + pageStr + ".jpg";

                    //判断该图片是否已存在
                    if (!File.Exists(imageFilePath))
                    {
                        downloadImageEvent.WaitOne();
                        setProgressBarLabelText(oneMangaProgressBarLabel, "正在提取" + currentPage + "/" + totalPage + "\r\n");

                        //每本漫画保留一行输出就行，仅修改最后的当前页/总页数
                        outputTextBox.Text = outputTextBox.Text.Substring(0, outputTextBox.Text.LastIndexOf(".") + 1);

                        string pageUrl = url;
                        //翻页
                        pageUrl += "?p=" + (currentPage - 1) / 40;

                        if (doc.Location.Href != pageUrl)
                        {

                            doc = await getDocAsync(pageUrl);
                        }

                        try
                        {
                            view(doc, mkdirName, pageStr);

                            mangaListView.Items[i].SubItems[3].Text = (int.Parse(mangaListView.Items[i].SubItems[3].Text) + 1).ToString();
                            allCompletePage++;
                            mangaListView.Items[i].SubItems[4].Text = (int.Parse(mangaListView.Items[i].SubItems[4].Text) - 1).ToString();
                            allRemainPage--;
                            
                            setProgressBarLabelText(allMangasProgressBarLabel, "已完成" + completeManga + "本 剩余" + mangaListView.Items.Count + "本 " + allCompletePage + "/" + allPage + "页");
                        }
                        catch (Exception e)
                        {
                            outputTextBoxAddText(e.ToString());
                            //出错的话重试3次
                            if (retryTime == 4)
                            {
                                hasError = true;
                            }
                            else
                            {
                                retryTime++;
                                j--;
                            }
                        }
                    }
                }

                string text = outputTextBox.Text;
                text = text.Substring(0, text.LastIndexOf("."));
                outputTextBox.Text = text;
                //完成后移除该漫画
                if (!hasError)
                {
                    //把最后的当前页/总页数修改为complete
                    outputTextBoxAddText("complete!\r\n");

                    mangaListView.Items.RemoveAt(i);
                    i--;
                    completeManga++;

                }
                else
                {
                    outputTextBoxAddText("完成，但有一些图片下载失败\r\n");
                }

                allMangasProgressBar.Value = completeManga;
                setProgressBarLabelText(allMangasProgressBarLabel, "已完成" + completeManga + "本 剩余" + mangaListView.Items.Count + "本 "+ allCompletePage + "/" + allRemainPage+"页");
            }
            //完成输出
            outputTextBoxAddText("---------------------------------------------------------------\r\n");
            outputTextBoxAddText("所有漫画下载完成\r\n");
            downloadButton.Text = "提取";
            downloadButton.Enabled = true;
            pauseDownloadButton.Enabled = false;
            stopDownloadButton.Enabled = false;
            setProgressBarLabelText(allMangasProgressBarLabel, "提取完成");

            timerThread.Abort();
        }

        public async void view(IDocument doc, string mkdirName, string pageStr)
        {

            //获取图片url
            IHtmlCollection<IElement> images = doc.QuerySelectorAll(".gdtm div a");
            string imageUrl = images[(int.Parse(pageStr) - 1) % 40].GetAttribute("href");
            IHtmlDocument imagePageDoc = await getDocAsync(imageUrl);

            IHtmlCollection<IElement> images2 = imagePageDoc.QuerySelectorAll("#i3 a img");
            foreach (IElement image in images2)
            {
                string imageSrc = image.GetAttribute("src");
                string imageName = pageStr + ".jpg";
                string imagePath = "";

                //下载图片
                try
                {
                    WebRequest imgRequest = WebRequest.Create(imageSrc);
                    HttpWebResponse res = (HttpWebResponse)imgRequest.GetResponse();

                    imagePath = mkdirName + "\\" + imageName;

                    System.Drawing.Image downImage = System.Drawing.Image.FromStream(res.GetResponseStream());
                    downImage.Save(imagePath);

                    //等待3秒
                    int random = 3;
                    outputTextBoxAddText("等待" + random + "秒\r\n");
                    Thread.Sleep(random * 1000);
                }
                catch (Exception e)
                {
                    //outputTextBoxAddText(e.ToString());
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);//如果出现错误则删除该图片
                        throw;
                    }
                }
                finally{
                    downloadImageEvent.Set();
                }
            }
        }

        private string pageToString(int page, int length)
        {
            string pageStr = "0000000000" + page;
            return pageStr.Substring(pageStr.Length - length, length);
        }
        private void timerRun()
        {
            programStart = DateTime.Now.Ticks;
            pauseStart = programStart;

            while (timerThread.ThreadState != ThreadState.Aborted && timerThread.ThreadState != ThreadState.AbortRequested)
            {
                if (!isTimerPause)
                {
                    totalTime = (DateTime.Now.Ticks - programStart - pauseCount) / 10000;

                    remainTimeLabel.Text = "已下载时间 " + format(totalTime) + " 预计剩余" + getRemainTime(totalTime);
                }

                    Thread.Sleep(1000);  // 1秒更新一次显示
            }
        }

        // 将毫秒数格式化  
        public string format(long totalTime)
        {
            int day, hour, minute, second;

            totalTime = totalTime / 1000;

            second = (int)(totalTime % 60);
            totalTime = totalTime / 60;

            minute = (int)(totalTime % 60);
            totalTime = totalTime / 60;

            hour = (int)totalTime % 24;

            day = (int)totalTime / 24;

            return day + "天" + hour + "时" + minute + "分" + second + "秒";
        }

        public string getRemainTime(long totalTime)
        {
            if (allCompletePage != 0)
            {
                totalTime = totalTime / allCompletePage * allRemainPage;
                return format(totalTime);
            }
            else
            {
                return "--天--时--分--秒";
            }
        }

        private void setProgressBarLabelText(Label label, string text)
        {
            label.Text = text;
            try
            {
                label.Location = new Point(label.Parent.Size.Width / 2 - label.Size.Width / 2, label.Parent.Size.Height / 2 - label.Size.Height / 2);
            }
            catch (Exception ex)
            {

            }
        }

        private void pauseDownloadButton_Click(object sender, EventArgs e)
        {
            isDownloadPause = !isDownloadPause;
            if (isDownloadPause)
            {
                pauseDownloadButton.Text = "正在暂停";
                pauseDownloadButton.Enabled = false;
                downloadEvent.Reset();

                pauseStart = DateTime.Now.Ticks;
                isTimerPause = true;
            }
            else
            {
                downloadEvent.Set();
                outputTextBoxAddText("恢复下载\r\n");
                pauseDownloadButton.Text = "暂停下载";

                pauseCount += (DateTime.Now.Ticks - pauseStart);
                isTimerPause = false;
            }
        }

        private void stopDownloadButton_Click(object sender, EventArgs e)
        {
            stopDownloadButton.Text = "正在停止";
            stopDownloadButton.Enabled = false;
            pauseDownloadButton.Enabled = false;
            downloadThread.Abort();
            timerThread.Abort();
            try
            {
                downloadEvent.Set();
            }
            catch (Exception ex)
            {
                outputTextBoxAddText(ex.ToString() + "\r\n");
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            addSearchListThread.Abort();
            downloadThread.Abort();
            timerThread.Abort();
        }
    }
}
