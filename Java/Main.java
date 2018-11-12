import com.google.gson.Gson;
import com.profesorfalken.jpowershell.PowerShell;
import net.lightbody.bmp.BrowserMobProxy;
import net.lightbody.bmp.BrowserMobProxyServer;
import net.lightbody.bmp.core.har.Har;
import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntity;
import org.apache.http.entity.mime.content.FileBody;
import org.apache.http.impl.client.DefaultHttpClient;

import org.openqa.selenium.Proxy;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.chrome.ChromeOptions;
import org.openqa.selenium.remote.DesiredCapabilities;

import java.io.File;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;


import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URL;
import java.text.Normalizer;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class Main {

    //private static HttpURLConnection con;

    public static void main(String[] args) throws MalformedURLException,
            ProtocolException, IOException, InterruptedException {

//        BrowserMobProxy proxy = new BrowserMobProxyServer();
//        proxy.start(0);
//        // get the JVM-assigned port and get to work!
//        int port = proxy.getPort();
//
//        // start the proxy
//        BrowserMobProxy proxy = new BrowserMobProxyServer();
//        proxy.start(0);

        String pageName  = "NewCheck";
        String fileName = pageName + ".har";
        String filePath = "C:\\Temp\\results\\har-files\\" + fileName;
        int proxyPort = 8081;


        String command = "C:\\tools\\experiment.ps1";
        PowerShell powerShell = PowerShell.openSession();
        System.out.println(powerShell.executeCommand(command).getCommandOutput());

        try {
            System.out.println("start sleep");
            Thread.sleep(60000);
            System.out.println("end sleep");
        } catch (InterruptedException e) {
            e.printStackTrace();
        }


        //====================================================
        ChromeOptions options = new ChromeOptions();
        Proxy proxy = new Proxy();

        proxyPort = getPort("http://localhost:9595/proxy");
        proxy.setHttpProxy("localhost:8081");
        options.setExperimentalOptions("proxy", proxy);
        Request.put("http://localhost:9595/proxy");
        //options.setExperimentalOptions("proxy", proxy);
        DesiredCapabilities capabilities = DesiredCapabilities.chrome();
        capabilities.setCapability("proxy", proxy);
        //capabilities.setCapability("path", "");


        System.setProperty("webdriver.chrome.driver", "c:\\tools\\chromedriver.exe");
        //ChromeDriver driver = new ChromeDriver(options);
        ChromeDriver driver = new ChromeDriver(capabilities);




        String initPage = "http://localhost:9595/proxy/" + proxyPort +"/har/?captureContent=true&captureHeaders=true&captureCookies=true&initialPageRef=" + pageName;

        URL url = new URL(initPage);
        HttpURLConnection httpCon = (HttpURLConnection) url.openConnection();
        httpCon.setDoOutput(true);
        httpCon.setRequestMethod("PUT");
        OutputStreamWriter out = new OutputStreamWriter(
                httpCon.getOutputStream());
        out.write("Resource content");
        out.close();
        httpCon.getInputStream();






        driver.get("http://yahoo.com");
        driver.quit();








        String getHarUrl = "http://localhost:9595/proxy/"+proxyPort+"/har";
        String requestBody = Request.getBody(getHarUrl);
        PrintWriter writer = new PrintWriter(filePath, "UTF-8");
        writer.println(requestBody);
        writer.close();



        pumpToHarStorage(filePath);








        powerShell.close();







    }

    public static void pumpToHarStorage(String filePath) throws InterruptedException {


        try {

            String mongoHost = "http://192.168.0.25:5000";


            //Post to mongo server which stores Har file
            HttpClient client = new DefaultHttpClient();
            HttpPost post = new HttpPost(mongoHost + "/results/upload");

            //prepare post body: file=har
            MultipartEntity entity = new MultipartEntity(HttpMultipartMode.BROWSER_COMPATIBLE);
            entity.addPart("file", new FileBody(new File(filePath)));
            post.setEntity(entity);

            //send post request to mongo server
            client.execute(post);

        } catch (IOException ex) {
            System.out.println("Error: ");
            System.out.println(ex.getMessage());
        }


    }

    public static int getPort(String inputUrl) throws IOException {
        String portJson = Request.get(inputUrl);
        //System.out.println(portJson);

        Gson g = new Gson();
        HashMap qwerty = g.fromJson(portJson, HashMap.class);

        List proxyList = (List) qwerty.get("proxyList");
        Map<String, Object> o = (Map<String, Object>) proxyList.get(0);
        //System.out.println(o.get("port").toString());
        Double d = (Double)o.get("port");
        int returnedPort = d.intValue();
        return returnedPort;
    }

}
