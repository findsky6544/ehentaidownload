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
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.swing.DefaultListModel;
import javax.swing.JButton;
import javax.swing.JFileChooser;
import javax.swing.JFrame;
import javax.swing.JList;
import javax.swing.JPanel;
import javax.swing.JScrollBar;
import javax.swing.JScrollPane;
import javax.swing.JTabbedPane;
import javax.swing.JTextArea;
import javax.swing.JTextField;

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
    
    static JButton addButton = new JButton("添加");
    static JButton deleteButton = new JButton("删除");
    static JButton importButton = new JButton("导入");
    static JButton exportButton = new JButton("导出");
    static JButton downloadButton = new JButton("提取");
    static JButton pauseButton = new JButton("暂停");
    
    
    static boolean isPause = false;
    
    
    public EhentaiView ehv = new EhentaiView();
    public AddSearchList asl = new AddSearchList();
    
	public ShowFrame() {
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
        controlPanel.add(deleteButton);
        controlPanel.add(importButton);
        controlPanel.add(exportButton);
        
        inputPanel.add(controlPanel,BorderLayout.EAST);
        
        panel.add(inputPanel,BorderLayout.NORTH);
        
        showPanel.setLayout(new GridLayout(1,2,5,5));
        showPanel.add(scrollPanel2);
        showPanel.add(scrollPanel);
        
        panel.add(showPanel,BorderLayout.CENTER);

        downloadPanel.add(downloadButton);
        downloadPanel.add(pauseButton);
        panel.add(downloadPanel,BorderLayout.SOUTH);
        
        tabPanel.add("下载",panel);
        
        
        tabPanel.add("设置",configPanel);
        
        
	    frame.add(tabPanel);
        
	    textArea.setLineWrap(true);
	    textArea.setWrapStyleWord(true);
	    
        addButton.addActionListener(new ActionListener() {
        	    public void actionPerformed(ActionEvent e) {
        			String url = urlText.getText();
        			Document doc = null;
					try {
						Map<String,String> cookie = new HashMap<String,String>();
						cookie.put("sk", "mcenn8w6eeod8of5x697k24xhu5o");
						
						doc = Jsoup.connect(url).cookies(cookie).get();
					} catch (IOException e1) {
						// TODO Auto-generated catch block
						e1.printStackTrace();
						ShowFrame.textAreaAddText(e1.toString()+"\n");
						return;
					}
					if(url.contains("f_search")) {
						ShowFrame.textAreaAddText("检测到搜索列表\n");
						asl.doc = doc;
						asl.ehv = ehv;
						asl.execute();
						return;
					}
					
        			String title = null;
        			if(doc.html().contains("Never Warn Me Again")) {
        				url = url + "?nw=always";
						try {
							doc = Jsoup.connect(url).get();
						} catch (IOException e1) {
							// TODO Auto-generated catch block
							e1.printStackTrace();
						}
        			}
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
        			listModel.addElement(title+url);
        			urlText.setText("");
        			ehv.urlList.add(url);
        			JScrollBar sBar = scrollPanel2.getVerticalScrollBar();
        			sBar.setValue(sBar.getMaximum());
        	    }
        });
        
        deleteButton.addActionListener(new ActionListener() {
        	    public void actionPerformed(ActionEvent e) {
        	    	ehv.urlList.remove(list.getSelectedIndex());
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
		        		Collections.sort(tempList);
		        		for(int i = 0;i < tempList.size();i++) {
		                	listModel.addElement(tempList.get(i));
		        			ehv.urlList.add(tempList.get(i).substring(tempList.get(i).indexOf("http")));
		        		}
		                br.close();
		                isr.close();
		                fis.close();
		                ShowFrame.textAreaAddText("导入成功！\n");
		            }
		            else
		            	ShowFrame.textAreaAddText("No file is selected!\n");
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
		            	ShowFrame.textAreaAddText("No file is selected!\n");
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
    	    	synchronized(ehv) {
        	    	if(isPause) {
        	    		pauseButton.setText("暂停中");
        	    	}
        	    	else {
        	    		ehv.notify();
        	    		pauseButton.setText("暂停");
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
		sBar.setValue(sBar.getMaximum());
	}
}
