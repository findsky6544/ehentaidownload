package downloadEHentaiImage;

public class Timer extends Thread {
	
	public EhentaiView ehv;
   
    // 记录程序开始时间  
    public long programStart = System.currentTimeMillis();  
   
    // 程序一开始就是暂停的  
    public long pauseStart = programStart;  
    
    // 程序暂停的总时间  
    public long pauseCount = 0;  
   
    public boolean stopped = false;  
    
    public long totalTime = 0;
   
    public Timer() {  
        setDaemon(true);  
    }  
   
    @Override  
    public void run() {  
        while (true) {  
            if (!stopped) {  
            	totalTime = System.currentTimeMillis() - programStart - pauseCount;
            	
            	ShowFrame.remainTimeLabel.setText("已下载时间 "+ format(totalTime) +" 预计剩余" +getRemainTime(ehv.completeManga,ehv.currentPage,ehv.totalPage));
            }  
   
            try {  
                sleep(1000);  // 1毫秒更新一次显示
            } catch (InterruptedException e) {  
            	e.printStackTrace();  
                System.exit(1);  
            }  
        }  
    }  
   
    // 将毫秒数格式化  
    public String format(long totalTime) {  
        int day,hour, minute, second;  
   
        totalTime = totalTime / 1000;  
   
        second = (int) (totalTime % 60);  
        totalTime = totalTime / 60;  
   
        minute = (int) (totalTime % 60);  
        totalTime = totalTime / 60;  
   
        hour = (int)totalTime%24;  
        
        day = (int)totalTime/24;
   
        return String.format("%d天%d时%d分%d秒",day, hour, minute, second);  
    }  
	
	public String getRemainTime(int completeNum,int page,int totalPage) {
		if(completeNum == 0){
			if(page == 0) {
				return "--天--时--分--秒";
			}
			else {
				double totalTimeD = 1.0 * totalTime;
				double complaeteRate = 1.0 * page/totalPage;
				totalTimeD = totalTimeD/complaeteRate;
				totalTimeD = totalTimeD * (ShowFrame.urlList.size()+1-complaeteRate);
				long totalTime = (long)totalTimeD;
				return format(totalTime);
			}
		}
		else {
			double totalTimeD = 1.0 * totalTime;
			double complaeteRate = completeNum + 1.0 * page/totalPage;
			totalTimeD = totalTimeD/complaeteRate;
			totalTimeD = totalTimeD * (ShowFrame.urlList.size()+1-complaeteRate);
			long totalTime = (long)totalTimeD;
			return format(totalTime);
		}
	}
}
