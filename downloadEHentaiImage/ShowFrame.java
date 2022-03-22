package downloadEHentaiImage;

import java.awt.BorderLayout;
import java.awt.GridLayout;
import java.awt.Toolkit;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Scanner;

import javax.swing.DefaultListModel;
import javax.swing.JButton;
import javax.swing.JFileChooser;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JList;
import javax.swing.JPanel;
import javax.swing.JProgressBar;
import javax.swing.JScrollBar;
import javax.swing.JScrollPane;
import javax.swing.JTabbedPane;
import javax.swing.JTextArea;
import javax.swing.JTextField;
import javax.swing.SwingConstants;

import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.select.Elements;

public class ShowFrame {
	static JTabbedPane tabPanel = new JTabbedPane(JTabbedPane.NORTH);
	static JFrame frame = new JFrame("downloadEHentaiImage");
	
    static JPanel panel = new JPanel(); 
    static JPanel inputPanel = new JPanel();
    static JPanel showPanel = new JPanel();
    static JPanel controlPanel = new JPanel();
    static JPanel downloadPanel = new JPanel();
    static JPanel configPanel = new JPanel();
    
    static DefaultListModel<String> listModel = new DefaultListModel<String>();
    
    static JList<String> list = new JList<String>(listModel);
    
    static JTextArea textArea = new JTextArea();
    
    static JScrollPane scrollPanel2=new JScrollPane(list);
    static JScrollPane scrollPanel=new JScrollPane(textArea);
    
    static JTextField urlText = new JTextField(100);
    static JTextField hostText = new JTextField(100);
    static JTextField portText = new JTextField(100);
    static JTextField useridText = new JTextField(100);
    
    static JButton addButton = new JButton("添加");
    static JButton stopButton = new JButton("停止添加");
    static JButton deleteButton = new JButton("删除");
    static JButton importButton = new JButton("导入");
    static JButton exportButton = new JButton("导出");
    static JButton downloadButton = new JButton("提取");
    static JButton pauseButton = new JButton("暂停");
    
    static JLabel remainTimeLabel = new JLabel("已下载时间 --天--时--分--秒 预计剩余 --天--时--分--秒");
    

	public static JProgressBar progressBar = new JProgressBar();
	
    static boolean isPause = false;
    
    
    public EhentaiView ehv = new EhentaiView();
    public AddSearchList asl = new AddSearchList();
	public static List<String> urlList = new ArrayList<String>();
    
	public static Map<String,String> cookie = new HashMap<String,String>();
    
	public ShowFrame() {
		try {
			readConfig();
		} catch (FileNotFoundException e2) {
			// TODO Auto-generated catch block
			e2.printStackTrace();
		}
		
		cookie.put("sk", "mcenn8w6eeod8of5x697k24xhu5o");//默认提取中文标题，应该是这个，不知道是否长时间有效
		cookie.put("nw", "1");//取消警告
		if(useridText.getText() != null && !useridText.getText().equals("")) {
			cookie.put("ipb_member_id", useridText.getText());//个人设置，筛选掉不要的分类
		}
		// 创建 JFrame 实例
	    // 得到显示器屏幕的宽高
	    int width = Toolkit.getDefaultToolkit().getScreenSize().width;
	    int height = Toolkit.getDefaultToolkit().getScreenSize().height;
	    // 定义窗体的宽高
	    int windowsWedth = 600;
	    int windowsHeight = 600;

	        // 设置窗体可见
	    frame.setVisible(true);
	        // 设置窗体位置和大小
	    frame.setSize(windowsWedth, windowsHeight);
	    frame.setBounds((width - windowsWedth) / 2,(height - windowsHeight) / 2, windowsWedth, windowsHeight);
	    frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
	    
	    // 添加面板
        panel.setLayout(new BorderLayout());
        inputPanel.setLayout(new BorderLayout());
        inputPanel.add(urlText,BorderLayout.CENTER);

        controlPanel.add(addButton);
        stopButton.setEnabled(false);
        controlPanel.add(stopButton);
        controlPanel.add(deleteButton);

        inputPanel.add(remainTimeLabel,BorderLayout.NORTH);
        remainTimeLabel.setHorizontalAlignment(SwingConstants.CENTER);
        inputPanel.add(controlPanel,BorderLayout.EAST);
        inputPanel.add(progressBar,BorderLayout.SOUTH);
		progressBar.setStringPainted(true);//设置进度条显示提示信息
        
        panel.add(inputPanel,BorderLayout.NORTH);
        
        showPanel.setLayout(new GridLayout(1,2,5,5));
        showPanel.add(scrollPanel2);
        showPanel.add(scrollPanel);
        
        panel.add(showPanel,BorderLayout.CENTER);

        downloadPanel.add(importButton);
        downloadPanel.add(exportButton);
        downloadPanel.add(downloadButton);
        downloadPanel.add(pauseButton);
        panel.add(downloadPanel,BorderLayout.SOUTH);
        
        tabPanel.add("下载",panel);
        
        
        tabPanel.add("设置",configPanel);
        JPanel configPanel2 = new JPanel();
        configPanel2.setLayout(new GridLayout(3,2));
        configPanel2.setSize(100,50);
        JLabel hostLabel = new JLabel("服务器地址：");
        hostLabel.setHorizontalAlignment(JLabel.RIGHT);
        configPanel2.add(hostLabel);
        configPanel2.add(hostText);
        hostText.setSize(frame.getSize().width/2-1000, hostText.getSize().height);
        JLabel portLabel = new JLabel("端口：");
        portLabel.setHorizontalAlignment(JLabel.RIGHT);
        configPanel2.add(portLabel);
        configPanel2.add(portText);
        portText.setSize(frame.getSize().width/2-1000, portText.getSize().height);
        JLabel useridLabel = new JLabel("用户ID：");
        useridLabel.setHorizontalAlignment(JLabel.RIGHT);
        configPanel2.add(useridLabel);
        configPanel2.add(useridText);
        useridText.setSize(frame.getSize().width/2-1000, useridText.getSize().height);
        JButton saveConfigButton = new JButton("应用");
        configPanel.add(configPanel2,BorderLayout.CENTER);
        configPanel.add(saveConfigButton,BorderLayout.NORTH);
        
	    frame.add(tabPanel);
        
	    //文本域自动换行
	    textArea.setLineWrap(true);
	    textArea.setWrapStyleWord(true);
	    
	    //按钮添加监听器
        addButton.addActionListener(new ActionListener() {
        	    public void actionPerformed(ActionEvent e) {
        	    	addButton.setEnabled(false);
        	    	stopButton.setEnabled(true);
        	    	addButton.setText("添加中");
        			String url = urlText.getText();
        			try {
						url = java.net.URLDecoder.decode(url, "UTF-8");
						urlText.setText(url);
					} catch (UnsupportedEncodingException e2) {
						// TODO Auto-generated catch block
						e2.printStackTrace();
					}
        			Document doc = null;
					try {
						
						doc = Jsoup.connect(url).cookies(cookie).get();
					} catch (IOException e1) {
						// TODO Auto-generated catch block
						e1.printStackTrace();
						ShowFrame.textAreaAddText(e1.toString()+"\n");
						return;
					}
					if(url.contains("f_search") || url.contains("tag")) {
						asl = new AddSearchList();
						ShowFrame.textAreaAddText("检测到搜索列表:");
						urlText.setEnabled(false);
						if(url.contains("f_search")) {
							ShowFrame.textAreaAddText(url.substring(url.indexOf("f_search=")+9));
							if(url.contains("page=")) {
								asl.startPage = Integer.parseInt(url.substring(url.indexOf("page=")+5,url.indexOf("page=")+url.substring(url.indexOf("page=")).indexOf("&")))+1;
							}
						}
						else if(url.contains("tag/")) {
							ShowFrame.textAreaAddText(url.substring(url.indexOf("tag/")+4,url.indexOf("tag/")+4+url.substring(url.indexOf("tag/")+4).indexOf("/")));
							asl.startPage = Integer.parseInt(url.substring(url.lastIndexOf("/")+1))+1;
						}
						ShowFrame.textAreaAddText("\n");
						asl.doc = doc;
						asl.ehv = ehv;
						asl.execute();
						return;
					}
					
        			String title = null;
        			Elements ele = doc.select("#gj");
    				title = ele.html();
    				
        			/*title = title.substring(0,title.indexOf(" Page"));*/
        			title = title.replace('/', '_');
        			title = title.replace('\\', '_');
        			title = title.replace(':', '_');
        			title = title.replace('*', '_');
        			title = title.replace('?', '_');
        			title = title.replace('"', '_');
        			title = title.replace('<', '_');
        			title = title.replace('>', '_');
        			title = title.replace('|', '_');
    				if(!urlList.contains(url)) {
            			listModel.addElement(title+url);
    					urlList.add(url);
            			JScrollBar sBar = scrollPanel2.getVerticalScrollBar();
            			sBar.setValue(sBar.getMaximum()+10);
    				}
    				else {
    					ShowFrame.textAreaAddText("该漫画已存在于列表中\n");
    					list.setSelectedIndex(urlList.indexOf(url));
            			JScrollBar sBar = scrollPanel2.getVerticalScrollBar();
    					sBar.setValue(sBar.getMaximum() * urlList.indexOf(url) / urlList.size());
    				}
        			ShowFrame.addButton.setText("添加");
        	    	ShowFrame.addButton.setEnabled(true);

        			ShowFrame.urlText.setText("");
        			ShowFrame.urlText.setEnabled(true);
        	    }
        });
        
        deleteButton.addActionListener(new ActionListener() {
        	    public void actionPerformed(ActionEvent e) {
        	    	urlList.remove(list.getSelectedIndex());
        	    	listModel.remove(list.getSelectedIndex());
        	    }
        });
        
        importButton.addActionListener(new ActionListener() {
			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub

				try {
		        	JFileChooser jfc=new JFileChooser();
		            if(jfc.showOpenDialog(null)==JFileChooser.APPROVE_OPTION){
		                File file=jfc.getSelectedFile();
		                FileInputStream fis = new FileInputStream(file.getAbsolutePath());
		                InputStreamReader isr = new InputStreamReader(fis,"utf-8");
		                BufferedReader br=new BufferedReader(isr);
		                String temp;
		                List<String> tempList = new ArrayList<String>();
		                while((temp=br.readLine())!=null){
							if(tempList.contains(temp)) {
								continue;
							}
		                	tempList.add(temp);
		                }
		                //导入的时候排个序
		        		Collections.sort(tempList);
		        		for(int i = 0;i < tempList.size();i++) {
		                	listModel.addElement(tempList.get(i));
		        			urlList.add(tempList.get(i).substring(tempList.get(i).indexOf("http")));
		        		}
		                br.close();
		                isr.close();
		                fis.close();
		                ShowFrame.textAreaAddText("导入成功！\n");
		            }
		            else
		            	ShowFrame.textAreaAddText("未选择文件\n");
				}
				catch(Exception ex) {
					ex.printStackTrace();
				}
			}
        });
        
        exportButton.addActionListener(new ActionListener() {
			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub

				try {
		        	JFileChooser jfc=new JFileChooser();
		            if(jfc.showOpenDialog(null)==JFileChooser.APPROVE_OPTION){
		                File file=jfc.getSelectedFile();
		                if(!file.exists()){
		                    file.createNewFile();
		                }
		                FileOutputStream fos = new FileOutputStream(file.getAbsolutePath());
		                OutputStreamWriter osw = new OutputStreamWriter(fos,"utf-8");
		                BufferedWriter bw=new BufferedWriter(osw);
		                for(int i = 0;i < listModel.getSize();i++) {
		                	String url = listModel.getElementAt(i).toString()+"\n";
		                	bw.write(url);
		                	bw.flush();
		                }
		                bw.close();
		                osw.close();
		                fos.close();
		            	ShowFrame.textAreaAddText("导出成功！\n");
		            }
		            else
		            	ShowFrame.textAreaAddText("未选择文件\n");
				}
				catch(Exception ex) {
					ex.printStackTrace();
				}
			}
        });
        
        downloadButton.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub
				try {
					progressBar.setValue(0);
					
					ehv = new EhentaiView();
					ehv.execute();
					downloadButton.setText("提取中");
					downloadButton.setEnabled(false);
				} catch (Exception e1) {
					// TODO Auto-generated catch block
					e1.printStackTrace();
				}
			}
        	
        });
        
        pauseButton.addActionListener(new ActionListener() {
    	    public synchronized void actionPerformed(ActionEvent e) {
    	    	isPause = !isPause;
    	    	if(isPause) {
    	    		pauseButton.setEnabled(false);
                    
    	    		pauseButton.setText("正在暂停……");
    	    		
    	    		ehv.timer.pauseStart = System.currentTimeMillis();  
    	    		ehv.timer.stopped = true;
    	    	}
    	    	else {
    	    		ehv.timer.pauseCount += (System.currentTimeMillis() - ehv.timer.pauseStart);  
    	    		ehv.timer.stopped = false;  
    	    		
    	    		pauseButton.setText("暂停");
	    	    	synchronized(ehv) {
        	    		ehv.notify();
        	    	}
	    	    	synchronized(asl) {
        	    		asl.notify();
	    	    	}
    	    	}
	    	}
    	});
        
        stopButton.addActionListener(new ActionListener() {
    	    public synchronized void actionPerformed(ActionEvent e) {
    	    	asl.cancel(true);
    	    	
    			addButton.setText("添加");
    	    	addButton.setEnabled(true);
    	    	stopButton.setEnabled(false);

    			ShowFrame.urlText.setEnabled(true);
    			urlText.setText("");
    			ShowFrame.progressBar.setString("停止添加");
    			ShowFrame.textAreaAddText("停止添加\n");
	    	}
    	});
        
        saveConfigButton.addActionListener(new ActionListener() {
    	    public synchronized void actionPerformed(ActionEvent e) {
    			File configFile = new File("config.txt");
    			Scanner input = null;
    			try {
    				input = new Scanner(configFile);
    				PrintWriter writer = new PrintWriter(configFile);
    				writer.println(hostText.getText());
    				writer.println(portText.getText());
    				writer.println(useridText.getText());
    				writer.close();

    				useConfig();
    			} catch (FileNotFoundException e1) {
    				// TODO Auto-generated catch block
    				e1.printStackTrace();
    			}
    			finally {
    				if(input != null) {
    					input.close();
    				}
    			}
    	    }
        });

	    // 设置界面可见
	    frame.setVisible(true);
	}
	
	public static void textAreaAddText(String text) {
		ShowFrame.textArea.append(text);
		JScrollBar sBar = scrollPanel.getVerticalScrollBar();
		sBar.setValue(sBar.getMaximum()+10);
	}
	
	public static void readConfig() throws FileNotFoundException {
		// 读取设置
		File configFile = new File("config.txt");
		if (configFile.exists()) {
			Scanner input = new Scanner(configFile);
			hostText.setText(input.nextLine());
			portText.setText(input.nextLine());
			useridText.setText(input.nextLine());
			input.close();
			useConfig();
		} else {
			// 若不存在设置txt则写入
			PrintWriter writer = new PrintWriter(configFile);
			writer.println("");
			writer.println("");
			writer.println("");
			writer.close();
		}
	}
	
	public static void useConfig() {
		if(hostText.getText() != null && !hostText.getText().equals("")) {
			System.setProperty("http.proxyHost", hostText.getText());
			System.setProperty("https.proxyHost", hostText.getText());
		}
		if(portText.getText() != null && !portText.getText().equals("")) {
			System.setProperty("http.proxyPort", portText.getText());
			System.setProperty("https.proxyPort", portText.getText());
		}
		System.setProperty("https.protocols", "TLSv1,TLSv1.1,TLSv1.2,SSLv3");
	}
}
