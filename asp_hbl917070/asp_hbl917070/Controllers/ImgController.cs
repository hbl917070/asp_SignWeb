using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Windows.Media.Imaging;
using TheArtOfDev.HtmlRenderer.WPF;

namespace asp_hbl917070.Controllers {
    public class ImgController : Controller {

        // GET: Img
        /*public ActionResult Index()        {
            return View();
        }*/


        String html_640_360 = @"<html>

<head>
    <meta charset='UTF-8'>
    <style>
        html,
        body {
            margin: 0px;
            padding: 0px;
            text-align: center;
        }

        div {
            font-size: 30px;
            font-family: 'Microsoft JhengHei';
            color: #fff;
            text-align: center;
            position: fixed;
            width: 100%;
            z-index: 99;
            top: 300px;
            left: 0px;
        }
        #t1{
            color: #000;
            top: 301px;
            left: 1px;
        }
        #t2{
            color: #000;
            top: 299px;
            left: -1px;
        }
        #t3{
            color: #000;
            top: 301px;
            left: -1px;
        }
        #t4{
            color: #000;
            top: 299px;
            left: 1px;
        }
        #t5{
            color: #000;
            top: 299px;
            left: 0px;
        }
        #t6{
            color: #000;
            top: 301px;
            left: 0px;
        }
        #t7{
            color: #000;
            top: 300px;
            left: 1px;
        }
        #t8{
            color: #000;
            top: 300px;
            left: -1px;
        }
    </style>
</head>

<body>
   
    <div id='t1'>{txt}</div>
    <div id='t2'>{txt}</div>
    <div id='t3'>{txt}</div>
    <div id='t4'>{txt}</div>
    <div id='t5'>{txt}</div>
    <div id='t6'>{txt}</div>
    <div id='t7'>{txt}</div>
    <div id='t8'>{txt}</div>

    <div id='t'>{txt}</div>
    <img id='img' src='{url}'>
</body>

</html>";







        /// <summary>
        /// DIO圖片
        /// 網址：localhost:50471/img/wry?t=2018-1-1_0:0:0
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public ActionResult wry(String t) {

            if (t == null || t == "") {
                t = "2018-1-1";
            }


            String html = html_640_360;
            html = html.Replace("{url}", Server.MapPath("~/Image/wry.jpg"));


            try {

                //初始化時間
                t = t.Replace(" ", "-").Replace(":", "-").Replace("_", "-").Replace("/", "-").Replace("：", "-").Replace("\\", "-");
                t = t.Replace("--", "-").Replace("--", "-").Replace("--", "-");
                String[] tt = t.Split('-');
                int[] ar_d = { 2018, 1, 1, 0, 0, 0 };
                for (int i = 0; i < 6; i++) {
                    try {
                        ar_d[i] = Int32.Parse(tt[i]);
                    } catch { }
                }
                DateTime d = new DateTime(ar_d[0], ar_d[1], ar_d[2], ar_d[3], ar_d[4], ar_d[5]);

                double output = ((DateTime.Now - d).TotalSeconds);//與目前的時間相減，並換成『秒』

                if (output >= 0) {
                    html = html.Replace("{txt}", output.ToString("0") + "秒過去了");
                } else {
                    output = output * -1;
                    html = html.Replace("{txt}", "還有" + output.ToString("0") + "秒");
                }


            } catch (Exception) {

                html = html.Replace("{txt}", "wryyyyyy");
            }

            System.Windows.Media.Imaging.BitmapFrame image = HtmlRender.RenderToImage(html);

            byte[] imgBytes;
            var png = new JpegBitmapEncoder();
            png.Frames.Add(image);
            using (MemoryStream mem = new MemoryStream()) {
                png.Save(mem);
                imgBytes = mem.ToArray();  // and use the imgBytes array in your SQL operation
            }

            return File(imgBytes, "image/jpeg");

        }







        /// <summary>
        /// 吉良吉影圖片
        /// 網址：localhost:50471/img/Yoshikage?t=2018-1-1_0:0:0
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public ActionResult Yoshikage(String t) {


            if (t == null || t == "") {
                t = "2018-1-1";
            }

            String html = html_640_360;
            html = html.Replace("{url}", Server.MapPath("~/Image/kira.jpg"));


            try {

                //初始化時間
                t = t.Replace(" ", "-").Replace(":", "-").Replace("_", "-").Replace("/", "-").Replace("：", "-").Replace("\\", "-");
                t = t.Replace("--", "-").Replace("--", "-").Replace("--", "-");
                String[] tt = t.Split('-');
                int[] ar_d = { 2018, 1, 1, 0, 0, 0 };
                for (int i = 0; i < 6; i++) {
                    try {
                        ar_d[i] = Int32.Parse(tt[i]);
                    } catch { }
                }

                DateTime d = new DateTime(ar_d[0], ar_d[1], ar_d[2], ar_d[3], ar_d[4], ar_d[5]);


                //與目前的時間相減，並換成『秒』
                int int_天 = 0;
                int int_時 = 0;
                int int_分 = 0;
                int int_秒 = 0;

                if (DateTime.Now > d) {//已經過了

                    int_天 = Int32.Parse((DateTime.Now - d).ToString("dd"));
                    int_時 = ((DateTime.Now - d).Hours);
                    int_分 = ((DateTime.Now - d).Minutes);
                    int_秒 = ((DateTime.Now - d).Seconds);

                } else {//還沒到

                    int_天 = Int32.Parse((d - DateTime.Now).ToString("dd"));
                    int_時 = ((d - DateTime.Now).Hours);
                    int_分 = ((d - DateTime.Now).Minutes);
                    int_秒 = ((d - DateTime.Now).Seconds);
                }


                String output = "";

                if (int_天 != 0)
                    output += int_天 + "天";
                if (int_時 != 0)
                    output += int_時 + "小時";
                if (int_分 != 0)
                    output += int_分 + "分";
                if (int_秒 != 0)
                    output += int_秒 + "秒";


                if (DateTime.Now > d) {//已經過了
                    html = html.Replace("{txt}", "已經過了將近" + output + "了");
                } else {//還沒到
                    html = html.Replace("{txt}", "還有將近" + output);
                }


            } catch (Exception) {

                html = html.Replace("{txt}", "可惡的仗助");
            }



            //把html轉成圖片
            System.Windows.Media.Imaging.BitmapFrame image = HtmlRender.RenderToImage(html);

 
            //把圖片變成jpg形式
            byte[] imgBytes;
            var png = new JpegBitmapEncoder();
            png.Frames.Add(image);
            using (MemoryStream mem = new MemoryStream()) {
                png.Save(mem);
                imgBytes = mem.ToArray();  // and use the imgBytes array in your SQL operation
            }


            return File(imgBytes, "image/jpeg");





        }



        /// <summary>
        /// 喬瑟夫 圖片
        /// 網址：localhost:50471/img/kira?t=2018-1-1_0:0:0
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public ActionResult joseph(String t) {


            if (t == null || t == "") {
                t = "2018-1-1";
            }


            String html = html_640_360;
            html = html.Replace("{url}", Server.MapPath("~/Image/joseph.jpg"));


            try {

                //初始化時間
                t = t.Replace(" ", "-").Replace(":", "-").Replace("_", "-").Replace("/", "-").Replace("：", "-").Replace("\\", "-");
                t = t.Replace("--", "-").Replace("--", "-").Replace("--", "-");
                String[] tt = t.Split('-');
                int[] ar_d = { 2018, 1, 1, 0, 0, 0 };
                for (int i = 0; i < 6; i++) {
                    try {
                        ar_d[i] = Int32.Parse(tt[i]);
                    } catch { }
                }

                DateTime d = new DateTime(ar_d[0], ar_d[1], ar_d[2], ar_d[3], ar_d[4], ar_d[5]);


                //與目前的時間相減，並換成『秒』
                int int_天 = 0;
                int int_時 = 0;
                int int_分 = 0;
                int int_秒 = 0;

                if (DateTime.Now > d) {//已經過了

                    int_天 = Int32.Parse((DateTime.Now - d).ToString("dd"));
                    int_時 = ((DateTime.Now - d).Hours);
                    int_分 = ((DateTime.Now - d).Minutes);
                    int_秒 = ((DateTime.Now - d).Seconds);

                } else {//還沒到

                    int_天 = Int32.Parse((d - DateTime.Now).ToString("dd"));
                    int_時 = ((d - DateTime.Now).Hours);
                    int_分 = ((d - DateTime.Now).Minutes);
                    int_秒 = ((d - DateTime.Now).Seconds);
                }


                String output = "";

                if (int_天 != 0)
                    output += int_天 + "天";
                if (int_時 != 0)
                    output += int_時 + "小時";
                if (int_分 != 0)
                    output += int_分 + "分";
                if (int_秒 != 0)
                    output += int_秒 + "秒";


                if (DateTime.Now > d) {//已經過了
                    html = html.Replace("{txt}", "" + output + "過去了");
                } else {//還沒到
                    html = html.Replace("{txt}", "還有" + output);
                }


            } catch (Exception) {

                html = html.Replace("{txt}", "kono 喬瑟夫 da");
            }



            //把html轉成圖片
            System.Windows.Media.Imaging.BitmapFrame image = HtmlRender.RenderToImage(html);


            //把圖片變成jpg形式
            byte[] imgBytes;
            var png = new JpegBitmapEncoder();
            png.Frames.Add(image);
            using (MemoryStream mem = new MemoryStream()) {
                png.Save(mem);
                imgBytes = mem.ToArray();  // and use the imgBytes array in your SQL operation
            }


            return File(imgBytes, "image/jpeg");





        }



    }
}