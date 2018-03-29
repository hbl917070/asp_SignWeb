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

namespace asp_時間圖片.Controllers {
    public class ImgController : Controller {

        // GET: Img
        /*public ActionResult Index()        {
            return View();
        }*/


        /// <summary>
        /// 入口。顯示使用者送過來基本資料
        /// </summary>
        public String t2() {

            String sum = "";

            var hbc = Request.Browser;
            sum += "IP：" + func_取得IP() + "<hr/>";
            sum += "是否為手機：" + func_是否用手機瀏覽() + "<hr/>";
            sum += hbc.Browser.ToString() + "<hr/>"; //取得瀏覽器名稱
            sum += hbc.Version.ToString() + "<hr/>"; //取得瀏覽器版本號
            sum += hbc.Platform.ToString() + "<hr/>"; // 取得作業系統名稱

            sum += Uri.UnescapeDataString(Request.Headers.ToString());

            return sum;
        }


        /// <summary>
        /// 入口，測試回傳一張圖片
        /// </summary>
        public ActionResult t1() {

            string filepath = Server.MapPath("~/Image/wry.jpg");//要下載的檔案位置       
            string filename = System.IO.Path.GetFileName(filepath);  //取得檔案名稱       
            Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read); //讀成串流     
            return File(iStream, "image/jpeg", filename);  //回傳出檔案
        }



        /// <summary>
        /// 入口。會顯示使用者ＩＰ的簽名檔
        /// 網址：localhost:50471/img/SignWeb
        /// </summary>
        /// <returns></returns>
        public ActionResult SignWeb() {


            String s1 = "";

            //例如： 『Windows』『電腦的』『chrome』 瀏覽器
            s1 += func_取得作業系統();
            s1 += (func_是否用手機瀏覽()) ? "手機的" : "電腦的";
            s1 += func_取得瀏覽器類型();
            s1 += "瀏覽器";

            String s2 = "但是你似乎不想讓我知道IP";



            String s_顏色 = func_隨機顏色();
            String t1 = "";
            String t2 = "";
            String s_圖片 = func_隨機圖片(ref t1, ref t2);


            try {
                String s_ip = func_取得IP();
                if (s_ip.Length >= 8)
                    s2 = t2 + func_取得IP();
            } catch { }


            String sum = $@"<?xml version='1.0' encoding='utf-8'?>
<!-- Generator: Adobe Illustrator 20.1.0, SVG Export Plug-In . SVG Version: 6.00 Build 0)  -->
<svg version='1.2' baseProfile='tiny' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px'
	 y='0px' viewBox='0 0 660 125' overflow='scroll' xml:space='preserve'>

<g id='圖層_2'>
	<rect fill='rgba(0,0,0,0.8)' width='660' height='125'>
        <!-- <animate attributeName='fill' values='rgba(167,128,99,0.8);rgba(167,128,99,0.5);rgba(167,128,99,0.8);' dur='30s' repeatCount='indefinite' /> -->
    </rect>
</g>


<g id='圖層_1'>
    {s_圖片}
	<text transform='matrix(1 0 0 1 535 114)' font-family='Microsoft JhengHei' font-size='12px'>hbl917070(深海異音)</text>
	<text transform='matrix(1 0 0 1 148 37.8271)' fill='{s_顏色}' font-family='Microsoft JhengHei' font-size='25px'>{t1}</text>
	<text transform='matrix(1 0 0 1 148 69.9877)' fill='{s_顏色}' font-family='Microsoft JhengHei' font-size='25px'>{s1}</text>
	<text transform='matrix(1 0 0 1 148 103.7304)' fill='{s_顏色}' font-family='Microsoft JhengHei' font-size='25px'>{s2}</text>
</g>

<g id='圖層_3'>
	<rect fill='{s_顏色}' width='660' height='5'></rect>
	<rect x='-60' y='60' transform='matrix(6.123234e-17 -1 1 6.123234e-17 -60.1152 65.1152)' fill='{s_顏色}' width='125' height='5'></rect>
	<rect y='120' fill='{s_顏色}' width='660' height='5'></rect>
	<rect x='595' y='60' transform='matrix(6.123234e-17 -1 1 6.123234e-17 594.8848 720.1152)' fill='{s_顏色}' width='125' height='5'></rect>	
</g>

</svg>";



            byte[] b = Encoding.UTF8.GetBytes(sum);

            return File(b, "image/svg+xml");
        }




        /// <summary>
        /// 取得IP
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        private String func_取得IP() {

            string ip;
            string trueIP = string.Empty;
            //先取得是否有經過代理伺服器
            ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(ip)) {
                //將取得的 IP 字串存入陣列
                string[] ipRange = ip.Split(',');

                //比對陣列中的每個 IP
                for (int i = 0; i < ipRange.Length; i++) {
                    //剔除內部 IP 及不合法的 IP 後，取出第一個合法 IP
                    if (ipRange[i].Trim().Substring(0, 3) != "10." &&
                       ipRange[i].Trim().Substring(0, 7) != "192.168" &&
                        ipRange[i].Trim().Substring(0, 7) != "172.16." &&
                       CheckIP(ipRange[i].Trim())) {
                        trueIP = ipRange[i].Trim();
                        break;
                    }
                }

            } else {
                //沒經過代理伺服器，直接使用 ServerVariables["REMOTE_ADDR"]
                //並經過 CheckIP( ) 的驗證
                trueIP = CheckIP(Request.ServerVariables["REMOTE_ADDR"]) ?
                     Request.ServerVariables["REMOTE_ADDR"] : "";

            }
            return trueIP;

        }



        /// <summary>
        /// 檢查 IP 是否合法
        /// </summary>
        /// <param name="strPattern">需檢測的 IP</param>
        /// <returns>true:合法 false:不合法</returns>
        private bool CheckIP(string strPattern) {
            // 繼承自：System.Text.RegularExpressions
            // regular: ^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$
            Regex regex = new Regex("^\\d{1,3}[\\.]\\d{1,3}[\\.]\\d{1,3}[\\.]\\d{1,3}$");
            Match m = regex.Match(strPattern);
            return m.Success;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private String func_取得作業系統() {

            //判斷Windows作業系統與版本
            try {
                String s_win = Uri.UnescapeDataString(Request.Headers.ToString());
                if (s_win.Contains("Windows+NT+6.4") || s_win.Contains("Windows+NT+10")) {
                    return "Win10 ";
                } else if (s_win.Contains("Windows+NT+6.3")) {
                    return "Win8.1 ";
                } else if (s_win.Contains("Windows+NT+6.2")) {
                    return "Win8 ";
                } else if (s_win.Contains("Windows+NT+6.1")) {
                    return "Win7 ";
                } else if (s_win.Contains("Windows+NT+6.0")) {
                    return "Win Vista ";
                } else if (s_win.Contains("Windows+NT+5.1")) {
                    return "Win XP ";
                }
            } catch { }


            //從Headers判斷作業系統
            try {
                String h = Uri.UnescapeDataString(Request.Headers.ToString()).ToLower();

                if (h.Contains("android")) {
                    return "Android";
                } else if (h.Contains("+centos")) {
                    return "Centos";
                } else if (h.Contains("+debian")) {
                    return "Debian";
                } else if (h.Contains("+fedora")) {
                    return "Fedora";
                } else if (h.Contains("mint")) {
                    return "Linux Mint";
                } else if (h.Contains("+xubuntu")) {
                    return "Xubuntu";
                } else if (h.Contains("+lubuntu")) {
                    return "Lubuntu";
                } else if (h.Contains("+kubuntu")) {
                    return "Kubuntu";
                } else if (h.Contains("+manjaro")) {
                    return "Manjaro Linux";
                } else if (h.Contains("+ubuntu")) {
                    return "Ubuntu";
                } else if (h.Contains("playstation+4")) {
                    return "PS4";
                } else if (h.Contains("playstation+3")) {
                    return "PS3";
                }

            } catch { }

            //嘗試直接取得作業系統，誤判率極高
            try {
                String s_作業系統名稱 = Request.Browser.Platform.ToString().ToLower();

                if (s_作業系統名稱.Contains("mac")) {
                    return "Mac";
                } else if (s_作業系統名稱.ToUpper().Contains("linux")) {
                    return "Linux";
                } else if (s_作業系統名稱.ToUpper().Contains("android")) {
                    return "Android";
                } else if (s_作業系統名稱.ToUpper().Contains("iphone")) {
                    return "iPhone";
                } else if (s_作業系統名稱.ToUpper().Contains("unknown")) {
                    return "";
                } else if (s_作業系統名稱.ToUpper().Contains("win")) {
                    return "Windows";
                }

                //避免linux被誤判城unix
                try {
                    String h = Uri.UnescapeDataString(Request.Headers.ToString()).ToLower();
                    if (s_作業系統名稱.ToLower() == "unix" && h.Contains("linux")) {
                        return "Linux";
                    }
                } catch { }

                return s_作業系統名稱;

            } catch { }


            return "";


        }




        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool func_是否用手機瀏覽() {

            try {
                string u = Request.ServerVariables["HTTP_USER_AGENT"];
                Regex b = new Regex(@"android.+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4)))) {
                    return true;
                }
            } catch { }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private String func_取得瀏覽器類型() {

            try {
                String h = Uri.UnescapeDataString(Request.Headers.ToString());

                if (h.Contains("+OPR")) {
                    return "Opera";
                } else if (h.Contains("+Edge")) {
                    return "Edge";
                } else if (h.Contains("+Line")) {
                    return "Line內建";
                } else if (h.Contains("+XiaoMi")) {
                    return "小米內建";
                } else if (h.Contains("QQBrowser") || h.Contains("TencentTraveler")) {
                    return "QQ";
                } else if (h.Contains("MetaSr") || h.Contains("SogouMobileBrowser")) {
                    return "搜狗";
                } else if (h.Contains("360SE")) {
                    return "360";
                } else if (h.Contains("Maxthon")) {
                    return "遨遊";
                } else if (h.Contains("UCBrowser") || h.Contains("UCWEB")) {
                    return "UC";
                } else if (h.Contains("AppleWebKit") && h.Contains("iPhone")) {
                    return "Apple";
                } else if (h.Contains("Puffin")) {
                    return "Puffin";
                } else if (h.Contains("Vivaldi")) {
                    return "Vivaldi";
                } else if (h.Contains("lynx")) {//Linux終端機
                    return "Lynx";
                } else if (h.Contains("playstation+")) {//ps4
                    return "PS內建";
                }
            } catch { }


            try {
                String b = Request.Browser.Browser.ToString(); //取得瀏覽器名稱

                if (b == "InternetExplorer") {
                    return "IE";
                } else if (b.Contains("Unknown")) {//
                    return "";
                }

                return b;
            } catch { }


            return "";
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private String func_隨機顏色() {

            return "#00A6F0";//藍色

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int xx = rnd.Next(1, 7);

            if (xx == 1) {
                return "#00A6F0";//藍色
            }
            if (xx == 2) {
                return "#F05000";//橘色
            }
            if (xx == 3) {
                return "#1D8E43";//綠色
            }
            if (xx == 4) {
                return "#B23E9B";//紫色
            }
            if (xx == 5) {
                return "#F6C500";//黃色
            }
            if (xx == 6) {
                return "#A6A6A6";//灰色
            }
            return "";
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private String func_隨機圖片(ref String t1, ref String t2) {

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int xx = rnd.Next(1, 9);
            //	<image width='143' height='125' xlink:href=''></image>

            if (xx == 1) {//博士
                t1 = "我很聰明，所以我知道你正在用";
                t2 = "也知道你的IP是";
                return "<image width='160' height='125' xlink:href='data:image/png;base64,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'></image>";
            }
            if (xx == 2) {//助手
                t1 = "先別管我嘴裡的是什麼，我知道你正在用";
                t2 = "也知道你的IP是";
                return "<image width='145' height='125' xlink:href='data:image/png;base64,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'></image>";
            }
            if (xx == 3) {//長頸鹿
                t1 = "你就是犯人吧，我知道你正在用";
                t2 = "而且也知道你的IP是";
                return "<image width='134' height='125' xlink:href='data:image/png;base64,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'></image>";
            }
            if (xx == 4) {//藪貓
                t1 = "去圖書館也沒用的朋友竟然在用";
                t2 = "而且IP竟然是";
                return "<image width='145' height='125' xlink:href='data:image/png;base64,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'></image>";
            }
            if (xx == 5) {//土龍
                t1 = "可疑的傢伙，你正在用";
                t2 = "而且你的IP是";
                return "<image width='129' height='125' xlink:href='data:image/png;base64,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'></image>";
            }
            if (xx == 6) {//食蟻獸
                t1 = "走開，你這個正在用";
                t2 = "的傢伙，別逼我說出你的IP是";
                return "<image width='158' height='125' xlink:href='data:image/png;base64,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'></image>";
            }
            if (xx == 7) {//沙漠貓
                t1 = "好像很有趣，你正在用的是";
                t2 = "沒意外的話IP是";
                return "<image width='144' height='125' xlink:href='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAJAAAAB9CAMAAAB6QhrWAAADAFBMVEUAAACZg2jWv6fnxKeunYvku4iCYDidgV6rhVa0k3NVbySTck+tpZl1ZjGslX7Iomp5PQ7lwJW2kWefhWvNtaDWsoK9oo3p3dToxZbIml/Zx7fGoHPAtqnjtXvZxLHitn5fTBqqflBYQifEr52YZT+FdWFJLw2MQg2ZZTXbr3PhvpBjVTesknu+jVWunInd1cns6eDc0sfr1rDs06rjzLTs1K3s17Pb0MXky7Ps0qfkzbfu6+Lt3bHt2LTq6ubs0aXqx5HiybHp17Ps0KLoxo/ry5nowojryZP///7t1q7rypbv6eDpxIze1McsFwTu263sz5/p6urs0rnn4NfoyZT95cHi29LnwYPn5+Pfxqzo0KTl6Og7KBrz7eTf183/6brw1r3/6cXqzZzy1aaxhlo1FgHv6+fr49r/47TJqIZZTSYoEwP//PXx7+vZzr9FKRH5+vnt5t79367v49T54Lu6lnT09PH42ak8IAn33bHp1ayzknKfc0z/7cvlyq4EAQDz3rvDooDovn/s7+/y27b51aDErJnSs5a5mn2+lWqqg2CqgFTv3cT12cDv1bLKtqTBnXnGnW/t27nevp/xzprYu5m2jF3sz7Hv0qT20ZirjG7y9vfm2spbOx358uzXx7WVdVjl4t743sXj0L3Pr43JpnpQMxUzHw/y5tjiyKP2y49rUTD+26jRo3K8kWIeCQLt387+1JfXrnrk1V3atITcyk6MVyn16t3/78Do2b7nyZuul4R3WTrjwZTauo62noygf126h1WCYkGYZjf15rjQr4OyjmfQk0PJurD+2aGNbk3Vx8DQwbjYqYmjhWu1h2iqd0ljQyf+8d2+oorJl2NjUkJPPSj++O2mjXorCwD65s7m08Xy0Z67pJR/b2PcrZHUoYB7Yk91VCS5q6G9XQe/s6tEHAHTzsvqzKnw4WXqu6CXe2T41bSqnZaqVgz/9tLdqGfbnk6Vh33CsErIZg3wxITEiDyDciuaQwPjtZmahjyqlzqwcS7jsnSwnkfZx2Io0WaWAAAAL3RSTlMAEv7+J/whQnxbfmhJT3HH/qmWsZy3mNnhtrWqeefe13XhysXAlOfh2cvAr9jmu+XXoXgAACLSSURBVHjarJhpTCNlGMdhPfC+r6gxxuuLxoWpLdY6i5mq2dogjXUabdLWoUBbLdbSYem0VFutHLtYoFgWmwWqArayYLMhXEIXOQ0qurgK6xlNxE1Wg/hhP2ni885MW8qM6/krQxloeH/9P8/7vjPN+w/kn51/9o1E3f1ijPcdtavVhTtx6oqKdm8nfk3e/8RFN17V2rrraGSeEyjLHgg/4zXuKxTi1Gp1aZciePxfQvk33mqLJKQ4nZqez1pk1CAgW4O9UAyDQeG06Iz/c0IX3dUnp/0qYoIIMuz4GGjkwDTpqkWFFAqFwaBWa4u4kP4foYtqNXRYhun1Yb+qrCybTNZqfrph777q6uqqHTpsU4GRAYQ4XJf8W4uzL2IpKCi4cLZ8nAwSeGh6qD8nFoJ1MY3P328KvKt902jUqXvsz1Xv2bOnEPpbAemoFayPM9NHxi/P/lctU1BwXltbIGADAm3Hmxf3958k/WHyvQqfPxgM3j8P+MFlftxUNr3Q39+fattfG7AFAoFPG943GkBKoVaDEEKNdDIzTXvTP5Y5t+C8G64zW8dNHOPj8FW2ULvY3EFSI0Mv/oL4/eEHH/qqrq481V/b3NwG7X7Q7w8C0kSkL9D0acOHLvs+SIfDYtw28+NX/QOVi28+99zzLp2sAYf5shzuHx9XjjTX0q8tpdnaWvrkQGd7f18iSEJ2AE7gOB72k+RP07a22qO99jWoFmCwQD4Z1H+3hfIvvuDO7qn9C11MWZnHU7bDh3Wa7684vpXW+e23T9pTEpqkwwQuk+EyOAhAJiNkQZA62ed9t9herWaVLLqMj9FVIBj6AiE333zLHe6NgcFYtG0hkvCAkIo3QQ/OCK01qd9ObyGdpd/mKhZCvjfCoAA+OC4jWPxhQkaE0W/DFN0X/bpnrZAtnEFbWsoJ6d7MFwg988yjjz5amcuJE4++5Rg4NrUcDaQSehDa7gMq/Pdx8/Dpra0t0ImQJI7JMIiEl5GBGZ4IBXHcgxMInKJr1y32QhBSw6TnhSzX5wmFcgA7OIC33I7uwdmZ2oWIPlOxzILDQ8xbf986PXeggyJxAsNQnSAdBCoWTttsJOFv7CMJHJDR47aZL+2Fiipo64zQjWcQejQNJ/SWGyKKeW28EJrapnnENi/P+NujcykfiXlAAglBQDiCVQsGaxN02ON93o/O4C9UZKZ07x6FQgsqf74ssi5CKt1uB1uziF7CmEzMZKrPBgtRXyRxf5b5yQOLDCXDABUqVgYcQZCNXpqgmpoo1OXIkeqIGfc+UehUqy1GrqfPFwo9Kk4lW7PlqC0h0aQWAm2BhtbeDz989stneycmwmkhOhXxhaE6SIkrVFYIQXr7SH9i5qCfP/dQu2KHnttTpTCo2bl/CHpawDvvZEu2Laq3ICLU1jPR2qamhtbiqn32fUB1T49T+yXBC2F+GkzAhuA6B0LYLiQjI14ap1baKL6OKmztyZj2uT0KtY6rmNg6PeBww7QC3kEgK/anEycqHd1Tp9a/bu3VGez26j1PwNaoVqPFrUpbyoUEnYw8wCcjBHBC2Yiovhgd5IQwrMW+a92wxvuIr9O7djUcPfruu+srp2Kx2OxG5QZUauq7U+tHG95/M2lYs9v3VSkKc6l6XXcofD8ug3TQwREMgg6C4EHrEdUXJf3B5T5KxoE95rI3RlfTe73rRhGhQzqtC4jH4dvYY97op629X7a4DPa9e+1rPQb+oiGX6rFdu4j7MSArhCcSJDebuDC4lILBmQhJzcz40kLYhMu3uD6WXqfFNo7i4mezFLvGLFq47DVqnXC56XSiIgmoXnv89BEcZ4UgJj6gRMAGVwBgxqogRQQVjVJU0yzu541UKsJPzzSoi9BabUwW/KUQOisGHQOIgI6YUFXP5OlfVDjGC6XxhxrbAlIqzHYQ104A2RfzU5HBCIkKHMKTLVIZRp489Sy7eeiuzRPhWaSwDdBRq51IRNznJfA5HgpjLLxMyO+HTZQMBbw2P8k6EmxCSHO50ff8YJMPzuEi0xUnMByjAtG4EYy0N4kLsUYZqVK1oTBbK4HQnp6T4NOCbRfCg5EOksaJMHmyKdpIkqiD+G0NJ2NeX3AqRqmIZDzsmRhLqmTE/db9DerS3cb4rWJCnEuWIi2XDGck8Nl3cGlJ4yeyQohgRyDQQdEEAeVZWUn4/MiHQOC+aIwiZ2fpIJasduFYSxJe7qEjM4d0RiNMMhGQT9apqKio2KizwJ5cKCq0r2Vp6QHwSQvx0HB1Ee3wkdAgdGDZS1K4jI+Jss0GfbFB3B+acBpaVLKwTCXDPKbao3HRjQPIjQdATzot9LUit2LorPqLX07/TuNYrhCAk8FAbAW2/VDQl4gtR3w0wW1fZGQqsTnTnaBlxCHDGC7F2KWd0a98qdMJLvCFCYEP78TmtPMmpqrnnNNbkjAmEAKC1MH9U7GEj/bQZNvUftrHbaj+g7O2zehAIzQW1AsHG4SHqj2qtkALiZCTTho4AyHBfXD1F1unH4aCScWUMNrXMds9g/uCuC8xO5uAZwQZi6596ghQsAaFQiF4GUtwcqV07JK/JVTK2ygMCuEKNH36SJCQqlSYiFGIIGAN7D4WICmMJGeOtflgk8cI36en7A1uL3sJgoETVzOPyfv1qvgt2bZywQGP4t1QLDXoCIAOWlKGCQyEcuGEZAQW9D0/657t8NF+X1P3CuUnWlxjyfXVBneUgnBAiE9IitGp6PV5ouysGKyLBha4+HU6t/s8se/k0kh2hglTQqsxSXodA9FXfC2ruwZWfC1OhWLt6K5WR4wMZd+Gx6MKesjYnWcU4lvHaFEDTovWAnuaNldoz9jQ8XRHixuhwfy+xmPuqYa1tbXIsabVwqrCtd7eJwdm6bQQ5OtRyfWq8fpbziTE6sBjt9GIvtiznV39Er50zhsE9hd4VL6D31U6fnXZNxundq0qFM6x1Xu6p0JBVkiFhKTMdFuZqbn77DMLAfDEn0AjQdo7No0U2sMyqNCXEKkUo95Yd7w11bq52edNojW/Jzl47CTDCUmlSKh8UU/aNi45k1AuSIcF7jXTi+JL8aHpoGebUAj+u9AJlYRea/jcPXAqtJl4MjlW6DSsfgcro4pFCkhUzHDKZ3Pf8pclS2O0oD5CoE2EF9p335GfPNuEpAdDMFtE8Xxh7x10OAYTPgmRhOXVvj7QQWNSVkgilUgk1ub+zTa3+6IzCsGREWKveaGVdlsUrJACMHRMM1IEZwEz2pWcEGtwEJqI21u+Q0aUhHApCu2t3Y0kJkVIAL3K2t68GXVXis6zbMUEdSsq0hoUGeIJCaxrUgB1g4dwVSnU8bAEE0EKudhXYw73YIj0TLjU9me7baQqLSSX65mFxfHZSseAWFuLVAzSgQO+aVE8vE+VS6aSbgNLwidQY36JSgi0Lmzta5srDvcyyXjCLV88NthESj0qmqQZlUTC9E8/0nzOgNtReeEZhLJwNuBjRB2dFQpJ2dT5NypNwp/iYa4xgFwh6YSrhaa8DtgzoGnkzLLXJDVZ+zvbFqb10EEL1vb6Sodj45a/Etqd9QEhi6EQ4H0MLVKMF0IN5Je2QOPHcYlUHGh/CUN9OjAQoeQSiWlm0WdamButWJie1MtNzd8fPrDhdjgc3Rf9jYTS9YKCqZ1ofnFCcTwbkCdZFVdNwAqTlKaFMPgJosoKQYbyoK9xYJBm5B6yNkDXzzWbTdYyc5fSVLFxzvAJt9u9sV9kKdIVs+ysGAhlrof4ikkkmXq1KNRhwqUea/FIs+QIcS/GfZGBGAULU+RA/X7GVKbX60uU5pfnNo4PVYJQ5YLINeNYrhD4ZBICj7TQnqoWCZAejHCNhT14S4vMg/ES4qWTqHyRQRvtUf202D9eppd3KTVKTc3I6GXHmzfcoHSOyL10crdACAFPTkNhRqhK/ZRckgYKFPbjMFw2H3Ep5E5FFmHtltTQcrkc0jFrNHVDc6NDBzYqBza6D18nFLrVCTqCFsoIcZ8Kqqvism1CGFsRVa4A+p0E2ClJq/QoKgkIKc3Tk111qfZPPjk+NzC6/0S96bDw3vVCVDNOJZMQd+gUiuwkS6LheLjRhWFkyS2bBCEHSmqa22usw+2fjFRsXHbO95WTNTXXCITynz3EC5Xm9BBnxOvApJfnDgnwVmlDIbmm4FNi7Rw2pTpT9T/V33LbZScqTEpGpGbXsBFlhYC0lCVtVBgP8xUTSp0B3icrZKroHB/+aKGemrkwv3LWZFbKzcLd4+J40bYm4mR4XucDcmqTUtEBhT+igSUqvkpwbNOBPcx6oP2Rj8mPo1Tb7XkXnmMth8wuEdbsJm2OEJBtIr5iY0+V8G3Ajy2OHA3LmKxdCUGeEE+JvISpGG7uoL19ZOD2vLyrazR6fY3IxL8Sapb1ySakU/P1qh576qQeXFDocoSoCqDXl5ms08OffNKcULGR5Ap1ddUN1/eFn0w8H0JCl9RolMq66/KFNTtkLN65TKOtFT4G4QIamzgpz4VXyKrA8LAKq0xdzXOjc8PnaEoknHwW8AGhuQObbzz5/POP2a7MyyuAXURphiYScPeYoGScT6Ea+fQ09DFyvZxHn6sGA6fRM12do6MV01ZTGfcLFFIGlJDZOvXOZ41fHHzyMdup/Lz868xKpRJWImFErt1FAiML3JkZ4JZIq3M19dNQDT3D6IVKaR94r1bN3Gh/nYlRlsCG1cWgyLJCcKbsqjs8+MILL3zje+Xes05dhJoIhGquEArl92qLBEY6LaDTGYuKkpBQSYkk4Z00oeEFMnw8de2d/TUms16J6JpMmfWQCRcUwDrXjcQ++PCbFz5/xUcuQxOdh5rIekOekCtWjcKiZbYRV4ONgffKtH1vo+qUmRH0XFE45NbpiuFXrWYlh8b8yOgIrc/6ckLWI898s3fvj++8890XzcfOzit4WQk729UiQvk3rOqKhSsRj7a1tg4ikFNtA/tJBurAFoPFDBEgAXPdQueI1VwOKhpO6Ku5ekZSkoUVMnW6X/jMvvf1F144dqQerjyuq9NozJfniXHeF9ritNBOdL2L5aiHusbbKmcZE0QDD86nf9rM+ky2N39VwzCaDOYHjte3m/S5QubDc91PvwAZ/fzW598fAaGrD4sLIQpesRTzQtwji/HQ4kgd9KSmi/JWHouQKqUS3i2aNZoDKSRU0jWyUFc3XbFw+HC5RskJTXcO1U9CgZXbqDmyPPj05+98uLd6/fm2IRC6okZT3gVColz8lPPH3aWlfEI5TnFvO2NmumAYsvaEu4lkUFlQQA/Vp8qUQLm5plz/8tyB9iOHGWU5J1TxbUWFtSRHqM678vTTT7/12XN7f3j3pyHonfPreCFxo3ssP2ZKVgpHBktDpzWVYkw1SrPJW1kZY6xKDVupr+pHGLZjUtN/MGq3MW3UcRzAi4gPmVHjU+ILn974wlcaL7HeC+3Fnr2c0MCtW6xU2/VBSnGCN7vOrGtjsHZt6aRyFBA6Z7sODaWYQtF1MMcQYnGVh9EqpbKuy9zQEQ28IEKy+btrwXZq65cmhKMvPvn+/nf/a3paJe1q/XXaPWSgVXCkU6BOx4wT2tfyPNqJ86caQSS+ROy8ZLKRt/Juq+78bxCIbnmFm1oWlNfQ7kW74f0TI5+0aWmV9qBmYeQLeaeSJQWMLm01xDA07KdVo/Yjznq12z+aMhhoxnYh5ppJGVgRx39N1WX/dOkEVxGxv44KVsDMnjIUAYGo7XNWVMWB8vOVPvTeiYEDnim/Sv61XrPgaZUD5KVqrVHNgVQVRofNIA8edjqper2t1e/3T+qvXYi51XQncLiomMyHPYmETpfpk10h9u7Th56CW5+iIBB1wCZSxQUaKpiZ3OrR9IoPjEziv080LhyYisgNSqVWPQ7LslqplLjK9Xbcoi7nY87f93xxvv50fbvCFIvN2LQvAhwC7531QUG9urXrmlWiZt/xeUMZ7z4DXhQEU9vHVsSm4MT32Su/vtonlsk0mhPHrlpne8Weg89raW1omFQpmWqlYfr4nnr7UL/fm1Scist6ZUv8OQW/PJY2+uVKFYvutNz783eNmY3EQDh8Ik4015ncXXfxdgSULwGoSB7f8yV09I+Svjpoo7XXRkAk621onIrDL41naoKWD6vlSqiIth10Osv1DmPHKZk5m4sfKag2VqRVu2iVUjs//OM53ebypnlzeV32WVOdszX0AK+MUVaXALl29cCyvrmjnbeoLUr62tkGGUsCF0S8cKDPP5NhaBKXGPyTCowCkgcoiV7zQLzXfCqJUu0gGhobDqpoi8M699z62mB4Y21wQ/wd8c5iuesxHq8ilSoOetBlO1QFxXCivGW075MJQ4TWTol7ZduBsjQeo1ukpS1dFZMmPkbNXQTOlRWzeZU4unLRqkApaywdnBkbM5IVem/P7rOb4fDyYPh6wwrxtE9R7yrjTRgsj9xaFKR1+EGUBeVdHz8/P0lHIgatXSPOYbKvBfvpzFkXTsttVop/6BhLeWVgQLeT6CbOfUghVMeFdGx6/tfgjLG9xff61Ho4HB4cXBtYJd7xYUf8t/H80cDpu4uC6GrHBCsqbKjqDZ99tBOXpLR+TwNAciiY3uy79b8sePocRiuVtJrNlwhiyazTXSaa3nzrS6kJoUyx9IVY+s/28xSK/biqYz3LawMrxCs+jLq6Y0crfd9kcVBKRYKoBy5G+Yuo6pnPD9toHJeQWuWxxm2SGL5PPzjX5tF4PBmrN5kwrxD7jybEfQndqTqCOOrDMAQFUTptPdmCIthX505cHwTQdXYNoXzsg0efnJLz7thRFFQtsTAO16GW53I3/Ftn3G7Mjr8UiShhbMfPNi5o2HkdGLGfOWvs8JZPueT9+rnzsJE3N30/kIAdNBH/6fseH8pHEApO/pgURRCEv3g0rruxvLy8nmju3gdY6a7Dw4KystuKgZ6qlkgsFvXkB0nfc8D5G/TG53oXHVG+VB3BaXrIOHsAHoXQ9E2Oft1fT7UcCbr6W+cSDW811RKXG3RcYJM45QUQiOphffEBhBArDZtQUONl4oVFE4ZgpmtqdVkxDtx3pyQSScriUp/e1VKV+zhbxarYM59R4kqlEseVtLbz/TMjnsaFAc2sx57EqGvp8VYnLNXaGuKSGDRZ0gimwBAE7WhjCwIQVkdcFK+v61aI7p2Iic/HpLvqHy/x2JmIkUDwwJDbr9pzqKcKPM+x95BAStpcBgmOgykSiXTS2sBo/SfHRmYzxo6TGN/Z7j02sLOpBkC9nCauy2yuH/aiCFgo1gMg/ttNxGp86Xuiu7YKY0EYVc4rAcJhZCRJwthCRof/6p5DyZaTXD6GZ3PYijgRmHA8ZaG1Wtogd+lbMAyb+9CcIPZyoD7wyGRwAVw7g1IA2g6KPtNNdDcRe9+pAh4bzLmjBIhhwMOKmIDaHVQHXROjz773Lfu04Lff/u7qh4pyJI5G4gyTelltQlHqJzMs6RoAXZbpRtY3ErBDhMMbcG3MWbggyBu1+5vf6UEx7hgLeqg46HZGJJBAQBV1h+SC/qDb7bYF3Q6jza8fFcxEqsmcCN+KJGqrVyi8sF1kQed6devh5Q1zhgUdbkFzARIXX88iivFRNFcR+kBx0J2MAECcKOAORpWGgIWsGAr1z487Tp9WiobnaYkILwxpGZqkkt+Yf6tfamquqWn6TNa4Hh68kdkcDA9eP+OjWAtotgILh+0LyYGoEqC7GIFEJOJEUde8hYQ/SCaVskQDTEploP8YftlC3gzCVeo2hWZprvy3VwHUTMTF62vclhUeXDv7UQv6bwFPluS9u0RDUWEWJJJYXEELKSLZcCMUWgRjY8NjAZwkC0ESw4T+2G9zFPXTlW52Zkvmvk1uyxoMr/X97GUbKkz+MlfcXhx0H4AEHEhkcbm3QOwBQapyRh0rV/cHAIcXJmD8IenDOj79rJlb1ebGDfCwWdOdT/5XPVwUx28tDronIBQKRCKRQCDo6gcQwCBQkEiUcrR6TyavuStTICLzURFDSO9E+NKevTWQ/W8lzJk1DrS8OXtLSwHk5jgPlgJFAZRNV8idAkc2pIgZUnf4pK3T48YAA6LClizq4144dbiGaqGiAVjWsGfd0I14tzX/AsIU7ffzSoAs2yAm5GYAlIskFdJTHe7x6T/V7ig7QpZE5iKxDE1TPgTbXcNVRMTN4o0bgzfW+i4uKpAiILag0iDJFmioAMSEJqnymbRTMT08HyCFAMqLKBBs9SLY4ps1XEV74+YBXSbTt3p0TpoD5Sq5qSArFFQKJJAIsyXBjES4aDukQH0L35GmpDHXuCsgEoo4yJaIYRxtLXykrpkVvUq8tRqPL61cIb7CMORvDiqVFrSEHrHfWhoElG2QEIfVnQMJokF90j7dAZ8k+llR7nhuZkLDkF1BQUWcqLabIPYTRNPutxHI38MC3nYwBPO23scrDRICiAtZMfZyPoipcHRcUJej0nS6f7wfOuL+RW4lYJv0+tAeVlQLP83Nb9YtInzOktsnWAYfXtnAwNqeKPs/IIkwG1GluwKHBQUWNlCRbXLXnxdMVLs/nR52RLsq4SgrzZoZi+N9L4L2vFrDYt58ZeciygdPIQijKGoL5Dty5i5eaVA0OzIWJFQPZS05kIhxWL1Wq6mjLTZvVI8Jtq8Q2U67RA6rE0N8iz1fftmz6CuYFQvCEErhlFqlWyLnJ3fzSufhaKWwMg8kzAMJLSE7hbZbpR3Pttr3tM6EoqQwP5ZKR5sTM5n4GLd/QgpASIv3L8rNLbRpKIzjvTjdqJeiD6IiQxAvIPhSsFQQA5L2oS/mwZgHJQ9VKDgvSIbQUmxFe1tcNbPhbLppraDRFlKkIhNcFMaqzhat4IsWtw6kap3SSQsb+CXN6oo+xH8TCKEkP/7n/50cDnxcPpKPBGmgwayJ0DqdBnVeBBw4ZCB7NDOg0qg25OKhhNVgoCx8nvvOudL7L8oVqdoIwB0KkTI67RMz2EPTScCZrDBuWv5H4u06vRagLuQEi+CUy0xKoXagASHqTtgwiiKSkViwwbhSIjrgFHJov1wKdpTLjBlZTDVkUZiSHTYZieZLlUYjxoE9loQfeLRohaAAKULp1ICaDzUnToQX/DRLYBSfHI9EJivhdAaJUuEhJNxpJzMdD+xhI41hSmgXJQ8Qwee94UalVKrEGNqHYYlJ4NGk5R124GlCofgVpFqjyokjZ9TlN9J9jwmar0xyRDJ9xSk9d0UzIkKZ5+l4qhg1En8B2YhItAI4pUbJC9tGVKJk1twRtBM5cRxvOlQsIhmjBQT38AGUKWQLebeRgPhyBp4PPkxFjLFoPINy8Tscc8V700aohQ1Q6jqVGisBD5xeN4tRtPFmSK8VaNNFXJUTpdJIcUsROKU4h+dm69X6fNjP05iVgjh9ZkJUgvdHpQVy/HIfYQxSNmBYZGqKDUUqlUojBjw2jDZO3Ti7USuQScQXJaQkSJSa8KZDINIZqJZr5fmiy3/NSMPqPmV+mMAwmp90pSX/Ywz8weC3FAjDbPxgJA8KslaKvse8ODftACKNIRJaFgUkUg2UTKMKx9F8tVY2SwFJkgpwVMvVZJ+F8tFEMjwOBaXIqkoFogjezcSCBASe8L2+bjk1fdqhtfNuwxKLpNSAs01AKZBIqtdqdXNRFGar9apsVzXMszB8xGWOwnz/AAIiK8HSBFzYiMEJh+XUN8eFs50ae7hQC0iUJCVRf3hIHA/MoVlzvVx+vlUSxIVauVaTieZTHGSKslCUD4iWMinXvtYt1gM7SJdgt2Fo+NkKbUTdAryWVIC27BD/AOHAQ9q7Td3CwpbO7eXqyq6CIM6WgQiY6mZv3s/x52nC2g6kDKEKZaUMbOzp6QuO1++GLzgmtq3QGmtSJgKJnfpuhLcJbdJ1CXNrdfqt9R261QUyB0Tlaj3rShreevJ5T4y7zNO0vLmgIGGqVEIfz4wO9x488em9o/fRp06trW4dQhMIRyadKdcOJGzR6TaL8qPWbl+m69qRyf2oZqNvR87e3XPyMTBFsllolOEIyscTBCEXYQL4WJZNsDQsEwbfHxk63Lv749Fex+jH9TqN6hRJWQEBXwVvb1lEgvYHpvS6LnGDAq6Hzs5s/Guwf+/eQ49Gv9weGbl9/ar3g4EZvP8sNBoKhRiPxzPmcTMhJuzyesfdEc/EcO/hfSemZxw9d3+ekQtfa4oCMpC4Zsm8RCpCRfMygJxrtZp+eDU0dGb30Tf95170jYAMnhOX+t88OTZ6ZM/Jl6/6Z/pfTxUalaw5+2zMwzzde/rg4eM9t2aO9/RMvHTs0v+XRQBkak4DJNCoEn+Zl8tfYFNzcH+XbnahTUNhGNapw6Eo/sDwF0RFvBAdDHYhBr2RA9OLGIhnhBZvljJI1jNkoaAM1khW/wgogjRZslApUr3QG0EiZOhNWEqhltmqRCy1qzD1Qih4oeB3Wn/Ri0JfKKXn0J6H9/v65nDg9PUjIqgi/7y0lNUzg7Y9PlOIlezI3MsXeaQYhkEQMtOTY1fGBh8/uLbgI4FhGCVoKKLDK1Z6rq/TcEyeOptM5tb0tg/5cr94Tua+fqGDO/roxNZ+YgiYYVkpczG78DrTowNQOihxw3bxZdVq3bfFApHcuRnNtO1rJVdiYUzkRZHBspIPh7d0atHmh8lkEloXRP1otgwDxuPfPn2iQFQbwB0WY8wwslWdvTo2ePtqmLEHtYJmOWWzWCwTgaHCsjSgpTLj46dHCrzKsKJISTFDap7W8TXp3txZatDvCgIffUFGv2uP9vYj8L8tJbY4OjuWevaox7YfTxcD2XFQJv4GEdyeZ41oaghOTR+k8wbgyywFwkq+pINDHepg86dB7TZKtpXbc6BVstU7+1HkY0Npr6fcXUxkezIpPWNn9CsXeQY7jBRU3wzQeSqW84Z4nh++UEeiVOY4kQLh867ob+64rXO5XSt/J9NG8AjU/LZyxToAgl42iF6dUFmGSjJfjN57zXPRzKCmLSxLsgONhcpuTZbptMiosTDqcI4FT42Y7tU5enkOO5FUA+3uvGYPIdf/JGpSgza25rYiQ4BjKQ+1S8IiL770xORNU3cD62m8Jil0QpYs+kal8iGHHSylCkHo5Y8CD55wnMidcHvH+8aVu9b+/XlzrglAdLCPQ9DLkg5/bNoLgoruZpdGn76yNc9rGCi/lKggpDA/xFKpA1oEw3em54IJYrAYgByOs6b3rehc/2TWumYz2exd0bsXkkfAslRNDBGGEVSJ9717s8VlLQx90RDI6+xo4k09JkkWg2WiiEdgaTnQFUZmUWqZSAotIwXirTkoQhda+7B5cMXqfrAHgCQtHvcRIxiWNv+kEgl8uYENCCUB6feqs9nZxc+1+hElqmtazbeHIr5PGAej1HxJsw8fGab+8MMjYFBX2nb/0H6RQBIKghypJmCPhQmXnrz5vvSR5BuEFUBqrKqVA9Osfaw3RDnqB1GeQdHAbxgxX4ykHo/7+XEu5oA/sfNpaIAuiS7zEHYsTR5uMQ5AxnB6+tLU1KVJj/G4FhAKCxMKUQyEiIKhZIaBrDDkbWwM1FBp/rTJ8SCO4yLnK2ObuuRZv6VgwKq0U0nwIh53UcSbuTEC+nAhCOpGy6CKqTiQ3m0JKinrBY/wQwTZer4ykNftzGkTDiNO9xRS11f1dsWzFXENVQAgkGIW44nPyLtzboRq6pbL144ArOG7luNQnhaTzOnpQtTSNTsYL1TqUG9ACsPQrVQuzpybet9NU6/eTljVwD+BJhbjiWU9dYzinACLJgOf0hJPlylQOzBlSMEARW7dvODqmh5VVMgI1YmCQl+D7eOHvi7KNUEw0PwqBQDNxhfODI60dWZaz/tQM84dwBTohxwZWe782yuuQgwCvPQn6HEWCvI9Y2dOXN/x37W+A7aIdUnr1frdAAAAAElFTkSuQmCC'></image>";
            }
            if (xx == 8) {//浣熊
                t1 = "這是公園的大危機，你居然用的是";
                t2 = "而且IP還是";
                return "<image width='159' height='125' xlink:href='data:image/png;base64,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'></image>";
            }

            return "";
        }









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

            String html = @"<html>

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
        /// 網址：localhost:50471/img/kira?t=2018-1-1_0:0:0
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public ActionResult kira(String t) {


            if (t == null || t == "") {
                t = "2018-1-1";
            }

            String html = @"<html>

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





    }
}