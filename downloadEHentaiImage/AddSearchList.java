package downloadEHentaiImage;


import java.awt.Robot;

import javax.swing.JScrollBar;
import javax.swing.SwingWorker;

import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;
import org.jsoup.select.Elements;

public class AddSearchList extends SwingWorker<Void,Integer> {
	 Document doc = null;
	EhentaiView ehv;
	
	@Override
	protected Void doInBackground() throws Exception {
		ShowFrame.urlText.setText("");
		String nextpageurl = null;
		int page = 1;
		do {
			ShowFrame.textAreaAddText("add page"+page+" start...");
			
			Elements mangas = doc.select(".glname");
			//添加漫画
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
				ShowFrame.listModel.addElement(title+url);
				ehv.urlList.add(url);
				//滚动条跳转到底部
				JScrollBar sBar = ShowFrame.scrollPanel2.getVerticalScrollBar();
				sBar.setValue(sBar.getMaximum());
			}
			//刷新界面
			ShowFrame.list.repaint();
			ShowFrame.textAreaAddText("complete\n");
			
			//下一页
			Elements pages = doc.select(".ptt td");
			Element nextpage = pages.last();
			
			Element urlElement = nextpage.selectFirst("a");
			//如果没有下一页了就退出
			if(urlElement == null) {
				break;
			}

			//获取下一页url
			nextpageurl = urlElement.attr("href");
			doc = Jsoup.connect(nextpageurl).get();
			page++;
			
			//每一页添加间隔1-3秒
			int random = (int)(Math.random()*3)+1;
			Robot r=new Robot(); 
			r.delay(random*1000); 
		}while(true);
		ShowFrame.textAreaAddText("add all page complete\n");
		return null;
	}
}
