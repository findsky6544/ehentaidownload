package downloadEHentaiImage;

import javax.swing.DefaultListModel;
import javax.swing.JList;
import javax.swing.JScrollBar;
import javax.swing.SwingWorker;

import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;
import org.jsoup.select.Elements;

public class AddSearchList extends SwingWorker<Void,Integer> {
	 Document doc = null;
	EhentaiView ehv;
	int startPage = 1;
	
	@Override
	protected Void doInBackground() throws Exception {
		String nextpageurl = null;
		int page = startPage;
		do {
			//暂停功能
			if(ShowFrame.isPause) {
				synchronized(this) {
					wait();
				}
			}

			Elements pages = doc.select(".ptt td");
			Element totalPageEle = pages.get(pages.size()-2);
			String totalPageStr = totalPageEle.selectFirst("a").html();
			int totalPage = Integer.parseInt(totalPageStr);
			
			ShowFrame.textAreaAddText("add page"+page+"/"+totalPage+" start...");

			ShowFrame.progressBar.setValue(page);
			ShowFrame.progressBar.setMaximum(totalPage);
			ShowFrame.progressBar.setString("正在提取第:"+page+"页,共"+totalPage+"页");
			
			Elements mangas = doc.select(".glname");
			//添加漫画
			int count = 0;
			for(Element manga : mangas) {
				//获取并处理标题
				String url = manga.selectFirst("a").attr("href");
				String title = manga.selectFirst("a .glink").html();
				
				title = title.replace('/', '_');
				title = title.replace('\\', '_');
				title = title.replace(':', '_');
				title = title.replace('*', '_');
				title = title.replace('?', '_');
				title = title.replace('"', '_');
				title = title.replace('<', '_');
				title = title.replace('>', '_');
				title = title.replace('|', '_');
				if(!ShowFrame.urlList.contains(url)) {
					count++;
					ShowFrame.listModel.addElement(title+url);
					ShowFrame.urlList.add(url);
				}
				//滚动条跳转到底部
				JScrollBar sBar = ShowFrame.scrollPanel2.getVerticalScrollBar();
				sBar.setValue(sBar.getMaximum());
			}
			//刷新界面
			ShowFrame.scrollPanel2.remove(ShowFrame.list);
			DefaultListModel<String> listModel = new DefaultListModel<String>();
			ShowFrame.list = new JList<String>(listModel);
			for(int i = 0;i < ShowFrame.listModel.size();i++) {
				listModel.addElement(ShowFrame.listModel.get(i));
			}
			
			ShowFrame.listModel = listModel;
		       //setupList();
			ShowFrame.scrollPanel2.setViewportView(ShowFrame.list);
			//ShowFrame.list.repaint();
			ShowFrame.textAreaAddText("complete ");

			ShowFrame.textAreaAddText("添加"+count+"条 ");
			
			//下一页
			Element nextpage = pages.last();
			
			Element urlElement = nextpage.selectFirst("a");
			//如果没有下一页了就退出
			if(urlElement == null) {
				break;
			}

			//获取下一页url
			nextpageurl = urlElement.attr("href");
			
			doc = Jsoup.connect(nextpageurl).cookies(ShowFrame.cookie).get();
			page++;
			
			//每一页添加间隔5-10秒
			int random = (int)(Math.random()*5)+5;
			ShowFrame.textAreaAddText("等待"+random+"秒...\n");
			try {
				Thread.sleep(random*1000); 
			}
			catch(InterruptedException ex) {
				return null;
			}
		}while(true);
		ShowFrame.textAreaAddText("add all page complete\n");
		ShowFrame.addButton.setText("添加");
    	ShowFrame.addButton.setEnabled(true);
		ShowFrame.urlText.setText("");
		ShowFrame.urlText.setEnabled(true);
		ShowFrame.progressBar.setString("添加完成");
		return null;
	}
}
