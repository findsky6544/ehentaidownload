package downloadEHentaiImage;

public class Main {

	public static void main(String[] args) {
		String proxyHost = "127.0.0.1";
		String proxyPort = "10800";

		System.setProperty("http.proxyHost", proxyHost);
		System.setProperty("http.proxyPort", proxyPort);

		// 对https也开启代理
		System.setProperty("https.proxyHost", proxyHost);
		System.setProperty("https.proxyPort", proxyPort);
		System.setProperty("https.protocols", "TLSv1,TLSv1.1,TLSv1.2,SSLv3");
		// TODO Auto-generated method stub
		new ShowFrame();
	}
}
