using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Media.Imaging;

namespace asp_hbl917070.Controllers {
    public class ImgReplyController : Controller {


        // GET: ImgReply
        public String Index() {
            return "<h1>你在期待什麼啦</h1>";
        }



        static DateTime dt_t01_上次執行時間 = DateTime.Now;
        static byte[] byte_t01_img;




        public ActionResult t01() {
            /*
            string filepath2 = Server.MapPath("~/Image/hbl917070.png");//要下載的檔案位置       
            string filename2 = System.IO.Path.GetFileName(filepath2);  //取得檔案名稱       
            Stream iStream2 = new FileStream(filepath2, FileMode.Open, FileAccess.Read, FileShare.Read); //讀成串流     
            return File(iStream2, "image/png", filename2);  //回傳出檔案

            */


            try {



                String url = "https://forum.gamer.com.tw/C.php?bsn=60076&snA=5037743&page=81000";

                //避免過度重複請求
                if (dt_t01_上次執行時間.AddSeconds(3) > DateTime.Now) {
                    if (byte_t01_img != null && byte_t01_img.Length != 0) {
                        return File(byte_t01_img, "image/png");
                    }
                }

                //取得最後一個回文的帳號
                HtmlAgilityPack.HtmlDocument doc = new HtmlWeb().Load(url);
                var nodeHeaders = doc.DocumentNode.SelectNodes("//a[@class='userid']");
                var item = nodeHeaders[nodeHeaders.Count - 1];
                String user = item.GetAttributeValue("href", "");

                //取得勇照網址
                user = user.Substring(user.LastIndexOf("/") + 1).ToLower();
                String url_user_img = $"https://avatar2.bahamut.com.tw/avataruserpic/{ user.Substring(0, 1) }/{ user.Substring(1, 1) }/{ user }/{ user }.png";

                //下載圖片
                MyWebClient MWC = new MyWebClient();
                byte_t01_img = MWC.DownloadData(url_user_img);

                //更新最後請求時間
                dt_t01_上次執行時間 = DateTime.Now;

                return File(byte_t01_img, "image/png");


            } catch (Exception) {


                string filepath = Server.MapPath("~/Image/hbl917070.png");//要下載的檔案位置       
                string filename = System.IO.Path.GetFileName(filepath);  //取得檔案名稱       
                Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read); //讀成串流     
                return File(iStream, "image/png", filename);  //回傳出檔案

            }



        }




    }





    public class MyWebClient : WebClient {
        protected override WebRequest GetWebRequest(Uri uri) {
            WebRequest WR = base.GetWebRequest(uri);
            WR.Timeout = 30 * 1000;
            return WR;
        }
    }

}