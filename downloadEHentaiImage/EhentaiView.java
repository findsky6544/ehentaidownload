package downloadEHentaiImage;

import java.awt.Robot;
import java.io.ByteArrayOutputStream;
import java.io.DataInputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;

import javax.swing.SwingWorker;

import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;
import org.jsoup.select.Elements;

public class EhentaiView extends SwingWorker<Void,Integer> {
	 String path = "G:\\EhentaiDownload\\";
	 List<String> urlList = new ArrayList<String>();
	
	public void view(Document doc,String url,String mkdirName,String pageStr) throws Exception {
		try {
			//暂停功能
			if(ShowFrame.isPause) {
				synchronized(this) {
    	    		ShowFrame.pauseButton.setText("恢复下载");
					wait();
				}
			}

			//获取图片url
			Elements images = doc.select(".gdtm div a");
			String imageUrl = images.get((Integer.parseInt(pageStr)-1)%40).attr("href");
			Document imageDoc = Jsoup.connect(imageUrl).get();
			
 			Elements images2 = imageDoc.select("#i3 a img");
 			 for (Element image : images2) {
 				String imageSrc = image.attr("src");
 				String imageName = pageStr+".jpg";

 				//下载图片
 				URL imageDownloadUrl = new URL(imageSrc);
 		        DataInputStream dataInputStream = new DataInputStream(imageDownloadUrl.openStream());

 		        File file = new File(mkdirName+"\\"+imageName);
 		        FileOutputStream fileOutputStream = new FileOutputStream(file);
 		        ByteArrayOutputStream output = new ByteArrayOutputStream();
 		        try {

 		            byte[] buffer = new byte[1024];
 		            int length;

 		            while ((length = dataInputStream.read(buffer)) > 0) {
 		                output.write(buffer, 0, length);
 		            }
 		            fileOutputStream.write(output.toByteArray());
 		            dataInputStream.close();
 		            fileOutputStream.close();
 		        } catch (Exception e) {
 		            e.printStackTrace();
 		    		output.close();
 		    		fileOutputStream.close();
 		    		dataInputStream.close();
 		    		file.delete();//如果出现错误则删除该图片
 		    		throw e;
 		        }
 			 }

 			 //等待1-3秒
 			int random = (int)(Math.random()*3)+1;
 			Robot r=new Robot(); 
 			r.delay(random*1000); 
		} catch (Exception e) {
			e.printStackTrace();
			throw e;
		}
	}
	
	private  String pageToString(int page,int length) {
		String pageStr = "0000000000"+page;
		return pageStr.substring(pageStr.length() - length, pageStr.length());
	}

	@Override
	protected Void doInBackground() throws Exception {
		Document doc = null;
		for(int i = 0;i < urlList.size();i++) {
			ShowFrame.list.setSelectedIndex(i);
			String url = urlList.get(i);
			
			boolean hasError = false;
			String mkdirName = null;
			String title = null;
			try {
				//有些页面会有警告，处理一下
				doc = Jsoup.connect(url).get();

    			if(doc.html().contains("Never Warn Me Again")) {
    				url = url + "?nw=always";
					doc = Jsoup.connect(url).get();
    			}
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
				ShowFrame.textAreaAddText("无法连接EHentai\n");
				return null;
			}
			//获取并处理标题
			title = doc.select("#gj").html();
			
			if(title.equals("")) {
				title = doc.title();
			}
			title = title.replace('/', '_');
			title = title.replace('\\', '_');
			title = title.replace(':', '_');
			title = title.replace('*', '_');
			title = title.replace('?', '_');
			title = title.replace('"', '_');
			title = title.replace('<', '_');
			title = title.replace('>', '_');
			title = title.replace('|', '_');
			mkdirName = path+title;
			ShowFrame.textAreaAddText(mkdirName+" is downloading...\n");
			
			//创建文件夹
		 	File file = new File(mkdirName);
		 	if(!file.exists()) {
		 		file.mkdirs();
		 	}
				//获取总页数
				Elements tempElements1 = doc.select("#gdd .gdt1");
				Elements tempElements2 = doc.select("#gdd .gdt2");
				
				String totalPageStr = "";
				
				for(int j = 0;j < tempElements1.size();j++) {
					if(tempElements1.get(j).html().equals("Length:")) {
						totalPageStr = tempElements2.get(j).html().split(" ")[0];
						break;
					}
				}
				
				for(int j = 0;j < Integer.parseInt(totalPageStr);j++) {
					int retryTime = 0;
					//处理当前页前面的0
					String pageStr = pageToString(j+1,totalPageStr.length());
					

					int page = Integer.parseInt(pageStr);
					
					//每本漫画保留一行输出就行，仅修改最后的当前页/总页数
				  	String text = ShowFrame.textArea.getText();
				  	text = text.substring(0,text.lastIndexOf(".")+1);
				  	ShowFrame.textArea.setText(text);
				  	
				  	ShowFrame.textAreaAddText(pageStr+"/"+totalPageStr);
				  	
				  	if(retryTime > 0) {
				  		ShowFrame.textAreaAddText("重试第"+retryTime+"次");
				  	}
				  	
					File imageFile = new File(mkdirName+"\\"+pageStr+".jpg");
					
					//判断该图片是否已存在
					if(!imageFile.exists()) {
						String pageUrl = url;
						//翻页
						if(url.contains("?")) {
							pageUrl += "&";
						}
						else {
							pageUrl += "?";
						}
						pageUrl += "p=" + (page-1)/40;
						
						if(!doc.location().equals(pageUrl)) {
							doc = Jsoup.connect(pageUrl).get();
						}

						try {
							view(doc,url,mkdirName,pageStr);
						}
						catch(Exception e) {
							//出错的话重试3次
							if(retryTime == 4) {
								hasError = true;
							}
							else {
								retryTime++;
								j--;
							}
						}
				}
			}

			  	String text = ShowFrame.textArea.getText();
			  	text = text.substring(0,text.lastIndexOf("."));
			  	ShowFrame.textArea.setText(text);
			//完成后移除该漫画
			if(!hasError) {
				//把最后的当前页/总页数修改为complete
				ShowFrame.textAreaAddText("complete!\n");
				
				ShowFrame.listModel.remove(i);
				urlList.remove(i);
				i--;
			}
			else {
				ShowFrame.textAreaAddText("complete,but some image is failed\n");
			}
		}
		//完成输出
		ShowFrame.textAreaAddText("---------------------------------------------------------------\n");
		ShowFrame.textAreaAddText("all manga download finished!\n");
		ShowFrame.downloadButton.setText("提取");
		ShowFrame.downloadButton.setEnabled(true);
	
		return null;
	}
}
