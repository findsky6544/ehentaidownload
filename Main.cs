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

        private bool isDownloading = false;



        private long programStart;// 记录线程开始时间

        private long pauseStart;// 记录线程暂停开始时间  

        private long pauseCount = 0;// 线程暂停的总时间  
        private long totalTime = 0;//显示总时间

        private long onePageStart;
        private long onePageEnd;

        private float downloadSpeed = 0;

        private int minWaitTime = 7;

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

            minWaitTime = int.Parse(config["minWaitTime"]);

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
            setProgressBarLabelText(oneMangaProgressBarLabel, oneMangaProgressBarLabel.Text);
            setProgressBarLabelText(allMangasProgressBarLabel, allMangasProgressBarLabel.Text);
        }

        //从文件读取配置
        private void readConfig()
        {
            config.Add("serverip", "");
            config.Add("port", "");
            config.Add("userid", "");
            config.Add("downloadPath", "");
            config.Add("minWaitTime", "7");

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
                minWaitTimeTextBox.Text = string.IsNullOrEmpty(config["minWaitTime"])?"7": config["minWaitTime"];
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
            minWaitTime = int.Parse(config["minWaitTime"]);
        }

        private void addUrlButton_Click(object sender, EventArgs e)
        {
            //添加期间禁用输入框和添加按钮，开启暂停、停止按钮
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

                        //启用url输入框、添加按钮
                        urlTextBox.Enabled = true;

                        addUrlButton.Enabled = true;
                        addUrlButton.Text = "添加";

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

                            //获取该漫画总页数
                            string mangaPages = manga.QuerySelectorAll(".glhide div").Last().InnerHtml.Split(' ')[0];

                            //获取已下载文件数
                            int fileCount = 0;
                            if (Directory.Exists(path + title))
                            {
                                fileCount = Directory.GetFiles(path + title).Count();
                            }

                            //如果已下载文件数=总页数，说明该漫画已下载完成，不需添加
                            if (int.Parse(mangaPages) == fileCount)
                            {
                                continue;
                            }
                            lvi.SubItems.Add(mangaPages);
                            lvi.SubItems.Add(fileCount.ToString());
                            lvi.SubItems.Add((int.Parse(mangaPages) - fileCount).ToString());

                            mangaListView.Items.Add(lvi);

                            //计算全局总页数，全局剩余页数
                            allPage += int.Parse(mangaPages);
                            allRemainPage += int.Parse(mangaPages) - fileCount;
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
            //添加单部漫画
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
                if (!mangaListView.Items.ContainsKey(url))
                {
                    //添加一行
                    ListViewItem lvi = new ListViewItem();
                    lvi.Name = url;
                    lvi.Text = url;
                    lvi.SubItems.Add(title);

                    string mangaPages = doc.QuerySelectorAll(".gdt2")[5].InnerHtml.Split(' ')[0];

                    //获取已下载文件数
                    int fileCount = 0;
                    if (Directory.Exists(path + title))
                    {
                        fileCount = Directory.GetFiles(path + title).Count();
                    }

                    //如果已下载文件数=总页数，说明该漫画已下载完成，不需添加
                    if (int.Parse(mangaPages) != fileCount)
                    {
                        lvi.SubItems.Add(mangaPages);
                        lvi.SubItems.Add(fileCount.ToString());
                        lvi.SubItems.Add((int.Parse(mangaPages) - fileCount).ToString());

                        mangaListView.Items.Add(lvi);

                        //滚动条跳转到底部
                        mangaListView.EnsureVisible(mangaListView.Items.Count - 1);

                        //计算全局总页数，全局剩余页数
                        allPage += int.Parse(mangaPages);
                        allRemainPage += int.Parse(mangaPages);
                    }
                }
                else
                {
                    outputTextBoxAddText("该漫画已存在于列表中\r\n");

                    int mangaIndex = mangaListView.Items.IndexOfKey(url);
                    mangaListView.Items[mangaIndex].Selected = true;
                    mangaListView.EnsureVisible(mangaIndex);
                }
            }
            //完成后修改界面控件状态
            addUrlButton.Text = "添加";
            addUrlButton.Enabled = true;

            urlTextBox.Text = "";
            urlTextBox.Enabled = true;

            if (mangaListView.Items.Count > 0 && !isDownloading)
            {
                exportMangaButton.Enabled = true;
                downloadButton.Enabled = true;
            }

            //修改进度条状态
            allMangasProgressBar.Value = allCompletePage;
            allMangasProgressBar.Maximum = allPage;
            setProgressBarLabelText(allMangasProgressBarLabel, "已完成" + completeManga + "本"+ allCompletePage + "页 剩余" + mangaListView.Items.Count + "本"+ allRemainPage + "页");
        }

        //暂停添加按钮
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

        //停止添加按钮
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
                outputTextBoxAddText(ex.Message + "\r\n");
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

        //获得界面dom
        public async Task<IHtmlDocument> getDocAsync(string url)
        {
            IHtmlDocument doc = null;
            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    outputTextBoxAddText(response.ReasonPhrase + "\r\n");
                }
                var getResponsestring = await response.Content.ReadAsStringAsync();
                HtmlParser parser = new HtmlParser();
                doc = await parser.ParseDocumentAsync(getResponsestring);
            }
            catch (Exception ex)
            {
                outputTextBoxAddText(ex.Message + "\r\n");
            }
            return doc;
        }

        private void mangaListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            deleteMangaButton.Enabled = true;
        }

        //删除漫画按钮
        private void deleteMangaButton_Click(object sender, EventArgs e)
        {
            int index = mangaListView.SelectedItems[0].Index;
            if (index == downloadingIndex)
            {
                MessageBox.Show("该漫画正在下载，不能删除", "", MessageBoxButtons.OK);
            }
            else
            {
                //计算全局总页数、总已完成页数、总剩余页数
                allPage -= string.IsNullOrEmpty(mangaListView.Items[index].SubItems[2].Text) ? 0 : int.Parse(mangaListView.Items[index].SubItems[2].Text);
                allCompletePage -= string.IsNullOrEmpty(mangaListView.Items[index].SubItems[3].Text) ? 0 : int.Parse(mangaListView.Items[index].SubItems[3].Text);
                allRemainPage -= string.IsNullOrEmpty(mangaListView.Items[index].SubItems[4].Text) ? 0 : int.Parse(mangaListView.Items[index].SubItems[4].Text);

                mangaListView.Items.RemoveAt(index);

                if(index < downloadingIndex)
                {
                    downloadingIndex--;
                }

                //移除之后自动选择下一项
                if (index < mangaListView.Items.Count)
                {
                    mangaListView.Items[index].Selected = true;
                }
            }

            //如果没有数据了就把删除、导出、下载按钮禁用
            if (mangaListView.Items.Count == 0)
            {
                deleteMangaButton.Enabled = false;
                exportMangaButton.Enabled = false;
                downloadButton.Enabled = false;
            }

            setProgressBarLabelText(allMangasProgressBarLabel, "已完成" + completeManga + "本" + allCompletePage + "页 剩余" + mangaListView.Items.Count + "本" + allRemainPage + "页");
        }

        //导出按钮
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

        //导入按钮
        private void ImportMangaButton_Click(object sender, EventArgs e)
        {
            ImportMangaButton.Enabled = false;
            ImportMangaButton.Text = "导入中";

            addUrlButton.Enabled = false;

            //选择文件后开启导入线程
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

                //先读到数组中再一次添加，否则导入大量数据的时候会很慢
                List<ListViewItem> lvis = new List<ListViewItem>();

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    string[] str = line.Split('|');

                    if (!mangaListView.Items.ContainsKey(str[0]))
                    {
                        //读取已下载文件数
                        int fileCount = 0;
                        if (Directory.Exists(path + str[1]))
                        {
                            fileCount = Directory.GetFiles(path + str[1]).Count();
                        }

                        //总页数=已下载文件数时跳过
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

                        //计算全局总页数、全局剩余页数
                        allPage += allPageCount;
                        allRemainPage += allPageCount - fileCount;

                        //新建一行
                        ListViewItem lvi = new ListViewItem();

                        lvi.Name = str[0];//name=key，否则用不了containsKey
                        lvi.Text = str[0];
                        lvi.SubItems.Add(str[1]);
                        lvi.SubItems.Add(str[2]);
                        lvi.SubItems.Add(fileCount.ToString());
                        lvi.SubItems.Add((allPageCount - fileCount).ToString());

                        lvis.Add(lvi);

                        setProgressBarLabelText(allMangasProgressBarLabel, "已完成" + completeManga + "本" + allCompletePage + "页 剩余" + lvis.Count + "本" + allRemainPage + "页");
                    }
                    else
                    {
                        repeatCount++;
                    }
                    //mangaListView.EnsureVisible(mangaListView.Items.Count - 1);
                }
                //排序
                lvis.Sort(delegate (ListViewItem x, ListViewItem y)
                {
                    if (x.SubItems[1].Text == null && y.SubItems[1].Text == null) return 0;
                    else if (x.SubItems[1].Text == null) return -1;
                    else if (y.SubItems[1].Text == null) return 1;
                    else return x.SubItems[1].Text.CompareTo(y.SubItems[1].Text);
                });
                //添加到界面组件
                mangaListView.Items.AddRange(lvis.ToArray());

                outputTextBoxAddText("导入成功,共导入" + importCount + "条,重复" + repeatCount + "条,已完成"+completeCount+"条\r\n");

                //恢复界面组件状态
                ImportMangaButton.Enabled = true;
                ImportMangaButton.Text = "导入";

                addUrlButton.Enabled = true;

                if (mangaListView.Items.Count > 0 && !isDownloading)
                {
                    exportMangaButton.Enabled = true;
                    downloadButton.Enabled = true;
                }
            }
        }

        //下载按钮
        private void downloadButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("请先在设置页填写图片下载路径", "", MessageBoxButtons.OK);
                return;
            }

            //禁用界面组件
            downloadButton.Enabled = false;
            downloadButton.Text = "下载中";

            pauseDownloadButton.Enabled = true;

            stopDownloadButton.Enabled = true;

            //新建下载线程
            downloadThread = new Thread(new ThreadStart(downloadClickAsync));
            downloadEvent = new ManualResetEvent(true);
            downloadThread.Start();

            //新建计时器线程，用于每秒计算剩余时间
            timerThread = new Thread(new ThreadStart(timerRun));
            timerThread.Start();

            //下载图片信号量，下完一张图片才能下载下一张
            downloadImageEvent = new AutoResetEvent(true);
        }
        private async void downloadClickAsync()
        {
            isDownloading = true;
            IHtmlDocument doc;
            allPage = allRemainPage;
            allCompletePage = 0;
            completeManga = 0;

            allMangasProgressBar.Maximum = allPage;

            for (downloadingIndex = 0; downloadingIndex < mangaListView.Items.Count; downloadingIndex++)
            {
                mangaListView.Items[downloadingIndex].Selected = true;

                bool hasError = false;
                //获取并处理标题
                string title = mangaListView.Items[downloadingIndex].SubItems[1].Text;
                /*string title = doc.QuerySelector("#gj").InnerHtml;

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
                title = title.Replace('|', '_');*/
                string mkdirName = path + title;

                //创建文件夹
                if (!Directory.Exists(mkdirName))
                {
                    Directory.CreateDirectory(mkdirName);
                }
                outputTextBoxAddText(mkdirName + " is downloading...\r\n");
                //获取总页数

                string totalPageStr = mangaListView.Items[downloadingIndex].SubItems[2].Text;
                int totalPage = string.IsNullOrEmpty(totalPageStr)?0:int.Parse(totalPageStr);

                oneMangaProgressBar.Maximum = totalPage;

                int retryTime = 0;
                for (int j = 0; j < totalPage; j++)
                {
                    onePageStart = DateTime.Now.Ticks;
                    //暂停功能
                    if (isDownloadPause)
                    {
                        isDownloading = false;
                        pauseDownloadButton.Enabled = true;
                        pauseDownloadButton.Text = "恢复下载";

                        outputTextBoxAddText("已暂停\r\n");
                    }
                    downloadEvent.WaitOne();
                    //停止功能
                    if (downloadThread.ThreadState == ThreadState.AbortRequested || downloadThread.ThreadState == ThreadState.Aborted)
                    {
                        isDownloading = false;
                        downloadEvent.Dispose();

                        downloadButton.Enabled = true;
                        downloadButton.Text = "下载";

                        pauseDownloadButton.Text = "暂停下载";

                        stopDownloadButton.Text = "停止下载";

                        outputTextBoxAddText("停止下载\r\n");
                        return;
                    }
                    //处理当前页前面的0
                    string pageStr = pageToString(j + 1, totalPageStr.Length);


                    int currentPage = int.Parse(pageStr);

                    oneMangaProgressBar.Value = currentPage;

                    //outputTextBoxAddText(pageStr+"/"+totalPageStr);

                    string retryStr = "";
                    if (retryTime > 0)
                    {
                        retryStr = " 重试第" + retryTime + "次";
                    }

                    string imageFilePath = mkdirName + "\\" + pageStr;
                    string newImageFilePath = imageFilePath;

                    //判断该图片是否已存在
                    if (!File.Exists(imageFilePath))
                    {
                        downloadImageEvent.WaitOne();
                        setProgressBarLabelText(oneMangaProgressBarLabel, "正在提取" + currentPage + "/" + totalPage + retryStr);

                        //每本漫画保留一行输出就行
                        outputTextBox.Text = outputTextBox.Text.Substring(0, outputTextBox.Text.LastIndexOf(".") + 1);
                        outputTextBox.Select(outputTextBox.TextLength, 0);
                        outputTextBox.ScrollToCaret();

                        string pageUrl = mangaListView.Items[downloadingIndex].Text.Replace("?nw=always", "");
                        //翻页
                        pageUrl += "?p=" + (currentPage - 1) / 40;

                        doc = await getDocAsync(pageUrl);
                        if (doc == null)
                        {
                            // TODO Auto-generated catch block
                            outputTextBoxAddText("无法连接EHentai\r\n");
                            isDownloading = false;
                            downloadButton.Text = "提取";
                            downloadButton.Enabled = true;
                            pauseDownloadButton.Enabled = false;
                            stopDownloadButton.Enabled = false;
                            timerThread.Abort();

                            return;
                        }

                        try
                        {
                            //获取图片url
                            IHtmlCollection<IElement> images = doc.QuerySelectorAll(".gdtm div a");
                            if(images.Length > 0)
                            {
                                string imageUrl = images[(int.Parse(pageStr) - 1) % 40].GetAttribute("href");
                                IHtmlDocument imagePageDoc = await getDocAsync(imageUrl);

                                //下载图片
                                view(imagePageDoc, mkdirName, pageStr);

                                //计算完成页数、剩余页数
                                mangaListView.Items[downloadingIndex].SubItems[3].Text = (int.Parse(mangaListView.Items[downloadingIndex].SubItems[3].Text) + 1).ToString();
                                allCompletePage++;
                                mangaListView.Items[downloadingIndex].SubItems[4].Text = (int.Parse(mangaListView.Items[downloadingIndex].SubItems[4].Text) - 1).ToString();
                                allRemainPage--;

                                downloadSpeed = (float)totalTime / 1000 / (allCompletePage == 0 ? 1 : allCompletePage);

                                //修改进度条
                                setProgressBarLabelText(allMangasProgressBarLabel, "已完成" + completeManga + "本" + allCompletePage + "页 剩余" + mangaListView.Items.Count + "本" + allRemainPage + "页");
                            }
                            else
                            {
                                //等待3秒
                                /*if (!isDownloadPause && downloadThread.ThreadState != ThreadState.AbortRequested && downloadThread.ThreadState != ThreadState.Aborted)
                                {
                                    int random = 6;
                                    while (random > 0)
                                    {
                                        outputTextBox.Text = outputTextBox.Text.Substring(0, outputTextBox.Text.LastIndexOf(".") + 1);
                                        outputTextBoxAddText("等待" + random + "秒\r\n");
                                        random--;
                                        Thread.Sleep(1000);
                                    }
                                }*/
                                outputTextBoxAddText(doc.Body.InnerHtml + "\r\n");
                                if (doc.Body.InnerHtml.Contains("banned"))
                                {
                                    timerThread.Abort();
                                    return;
                                }
                                hasError = true;
                                downloadImageEvent.Set();
                                break;
                            }

                            //完成下载后重试次数置零
                            retryTime = 0;
                        }
                        catch (Exception e)
                        {
                            outputTextBoxAddText(e.Message+"\r\n");
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
                outputTextBox.Select(outputTextBox.TextLength, 0);
                outputTextBox.ScrollToCaret();
                //完成后移除该漫画
                if (!hasError)
                {
                    outputTextBoxAddText("complete!\r\n");

                    mangaListView.Items.RemoveAt(downloadingIndex);
                    downloadingIndex--;
                    completeManga++;

                }
                else
                {
                    outputTextBoxAddText("完成，但有一些图片下载失败\r\n");
                }

                allMangasProgressBar.Value = allCompletePage;
                setProgressBarLabelText(allMangasProgressBarLabel, "已完成" + completeManga + "本" + allCompletePage + "页 剩余" + mangaListView.Items.Count + "本" + allRemainPage + "页");
            }
            //完成输出
            outputTextBoxAddText("---------------------------------------------------------------\r\n");
            outputTextBoxAddText("所有漫画下载完成\r\n");
            isDownloading = false;
            downloadButton.Text = "提取";
            downloadButton.Enabled = true;
            pauseDownloadButton.Enabled = false;
            stopDownloadButton.Enabled = false;
            setProgressBarLabelText(allMangasProgressBarLabel, "提取完成");

            timerThread.Abort();
        }

        public void view(IDocument doc, string mkdirName, string pageStr)
        {
            //其实应该只能搜到一张图片，所以用循环也无所谓
            IHtmlCollection<IElement> images2 = doc.QuerySelectorAll("#i3 a img");
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
                    if (!isDownloadPause && downloadThread.ThreadState != ThreadState.AbortRequested && downloadThread.ThreadState != ThreadState.Aborted)
                    {

                        onePageEnd = DateTime.Now.Ticks;

                        int onePageDuring = (int)((onePageEnd - onePageStart) / 10000000);
                        int random = 3;
                        if (onePageDuring < minWaitTime)
                        {
                            random = minWaitTime - onePageDuring;
                        }
                        while (random > 0)
                        {
                            outputTextBox.Text = outputTextBox.Text.Substring(0, outputTextBox.Text.LastIndexOf(".")+1);
                            outputTextBoxAddText("等待" + random + "秒\r\n");
                            random--;
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception e)
                {

                    onePageEnd = DateTime.Now.Ticks;

                    long onePageDuring = onePageEnd - onePageStart;
                    pauseCount += onePageDuring;

                    //outputTextBoxAddText(e.ToString());
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);//如果出现错误则删除该图片
                    }
                    throw;
                }
                finally{
                    downloadImageEvent.Set();
                }
            }
        }

        //页码前面补零，使能够正常按名称排序
        private string pageToString(int page, int length)
        {
            string pageStr = "0000000000" + page;
            return pageStr.Substring(pageStr.Length - length, length);
        }

        //计时器线程
        private void timerRun()
        {
            programStart = DateTime.Now.Ticks;
            pauseStart = programStart;
            pauseCount = 0;

            while (timerThread.ThreadState != ThreadState.Aborted && timerThread.ThreadState != ThreadState.AbortRequested)
            {
                if (!isTimerPause)
                {
                    totalTime = (DateTime.Now.Ticks - programStart - pauseCount) / 10000;

                    remainTimeLabel.Text = "下载速度 "+ Math.Round(downloadSpeed, 3) +"秒/页 已下载时间 " + format(totalTime) + " 预计剩余" + getRemainTime();
                }

                    Thread.Sleep(1000);  // 1秒更新一次显示
            }
        }

        // 将毫秒数格式化  
        public string format(long totalTime)
        {
            float totalTimef = (float)totalTime;
            int day, hour, minute, second;

            totalTimef = totalTimef / 1000;

            second = (int)(totalTimef % 60);
            totalTimef = totalTimef / 60;

            minute = (int)(totalTimef % 60);
            totalTimef = totalTimef / 60;

            hour = (int)totalTimef % 24;

            day = (int)totalTimef / 24;

            return day + "天" + hour + "时" + minute + "分" + second + "秒";
        }

        //计算剩余时间
        public string getRemainTime()
        {
            if (allCompletePage != 0)
            {
                long totalTime = (long)(downloadSpeed*1000 * allRemainPage);
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
                outputTextBoxAddText(ex.Message + "\r\n");
            }
        }

        //暂停按钮
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

        //停止按钮
        private void stopDownloadButton_Click(object sender, EventArgs e)
        {
            stopDownloadButton.Text = "正在停止";
            stopDownloadButton.Enabled = false;

            pauseDownloadButton.Enabled = false;
            try
            {
                //先释放再给信号，否则可能多跑一轮
                downloadThread.Abort();
                downloadEvent.Set();

                timerThread.Abort();
            }
            catch (Exception ex)
            {
                outputTextBoxAddText(ex.Message + "\r\n");
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(addSearchListThread != null)
            {
                addSearchListThread.Abort();
            }
            if(downloadThread != null)
            {
                downloadThread.Abort();
            }
            if(timerThread != null)
            {
                timerThread.Abort();
            }
        }

        private void minWaitTimeTextBox_GotFocus(object sender, EventArgs e)
        {
            minWaitTimeTextBox.Tag = minWaitTimeTextBox.Text;
        }

        private void minWaitTimeTextBox_LostFocus(object sender, EventArgs e)
        {
            try
            {
                config["minWaitTime"] = minWaitTimeTextBox.Text;
            }catch(Exception ex)
            {
                minWaitTimeTextBox.Text = minWaitTimeTextBox.Tag.ToString();
            }
        }
    }
}
