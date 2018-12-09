using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace asp_hbl917070.Controllers {
    public class SignWebController : Controller {
        // GET: SignWeb
        public ActionResult Index() {
            return View();
        }



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




        public String t3() {

            DateTime time_start = DateTime.Now;//計時開始 取得目前時間

            String ss = func_get_base64("藪貓");
            //
            //
            //
            DateTime time_end = DateTime.Now;//計時結束 取得目前時間            
            string result2 = ((TimeSpan)(time_end - time_start)).TotalMilliseconds.ToString();//後面的時間減前面的時間後 轉型成TimeSpan即可印出時間差
            System.Console.WriteLine("+++++++++++++++++++++++++++++++++++" + result2 + " 毫秒");


            return result2;
        }


        /// <summary>
        /// 入口。會顯示使用者ＩＰ的簽名檔
        /// 網址：localhost:50471/img/SignWeb
        /// </summary>
        /// <returns></returns>
        public ActionResult SignWeb() {




            String s_顏色 = func_隨機顏色();
            String t1 = "";
            String t2 = "";
            String t3 = "";
            String s_img = "";
            func_隨機圖片(ref t1, ref t2, ref t3, ref s_img);





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
    {s_img}
	<text transform='matrix(1 0 0 1 535 114)' font-family='Microsoft JhengHei' font-size='12px'>hbl917070(深海異音)</text>
	<text transform='matrix(1 0 0 1 148 37.8271)' fill='{s_顏色}' font-family='Microsoft JhengHei' font-size='25px'>{t1}</text>
	<text transform='matrix(1 0 0 1 148 69.9877)' fill='{s_顏色}' font-family='Microsoft JhengHei' font-size='25px'>{t2}</text>
	<text transform='matrix(1 0 0 1 148 103.7304)' fill='{s_顏色}' font-family='Microsoft JhengHei' font-size='25px'>{t3}</text>
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
                } else if (h.Contains("+arch")) {
                    return "Arch Linux";
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
                } else if (h.Contains("1234")) {
                    return "1234瀏覽器";
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
        private void func_隨機圖片(ref String t1, ref String t2, ref String t3, ref String img) {


            String s_作業系統 = func_取得作業系統();
            String s_手機或電腦 = (func_是否用手機瀏覽()) ? "手機" : "電腦";
            String s_瀏覽器 = func_取得瀏覽器類型();


            String ip = "1.1.1.1";

            try {
                String s_ip = func_取得IP();
                if (s_ip.Length >= 8)
                    ip = func_取得IP();
            } catch { }


            List<svg_data> ar = new List<svg_data>();

            //博士
            ar.Add(new svg_data {
                t1 = "我很聰明，所以我知道你正在用",
                t2 = $"{s_作業系統} {s_手機或電腦}的{s_瀏覽器}瀏覽器",
                t3 = $"也知道你的IP是{ip}",
                img = "<image width='160' height='125' xlink:href='" + func_get_base64("博士") + "'></image>"
            });

            //助手
            ar.Add(new svg_data {
                t1 = "先別管我嘴裡的是什麼，我知道你正在用",
                t2 = $"{s_作業系統} {s_手機或電腦}的{s_瀏覽器}瀏覽器",
                t3 = $"也知道你的IP是{ip}",
                img = "<image width='145' height='125' xlink:href='" + func_get_base64("助手") + "'></image>"
            });

            //長頸鹿
            ar.Add(new svg_data {
                t1 = "你就是犯人吧，我知道你正在用",
                t2 = $"{s_作業系統} {s_手機或電腦}的{s_瀏覽器}瀏覽器",
                t3 = $"當然也知道你的IP是{ip}",
                img = "<image width='134' height='125' xlink:href='" + func_get_base64("長頸鹿") + "'></image>"
            });

            //長頸鹿
            ar.Add(new svg_data {
                t1 = "",
                t2 = $"你是山羊吧",
                img = "<image width='134' height='125' xlink:href='" + func_get_base64("長頸鹿") + "'></image>"
            });

            //藪貓
            ar.Add(new svg_data {
                t1 = "去圖書館也沒用的朋友竟然在用",
                t2 = $"{s_作業系統} {s_手機或電腦}的{s_瀏覽器}瀏覽器",
                t3 = $"而且IP竟然是{ip}",
                img = "<image width='145' height='125' xlink:href='" + func_get_base64("藪貓") + "'></image>"
            });

            //土龍
            ar.Add(new svg_data {
                t1 = "可疑的傢伙，你正在用",
                t2 = $"{s_作業系統} {s_手機或電腦}的{s_瀏覽器}瀏覽器",
                t3 = $"而且你的IP是{ip}",
                img = "<image width='129' height='125' xlink:href='" + func_get_base64("土龍") + "'></image>"
            });

            //食蟻獸
            ar.Add(new svg_data {
                t1 = "走開，你這個正在用",
                t2 = $"{s_作業系統} {s_手機或電腦}的{s_瀏覽器}瀏覽器",
                t3 = $"的傢伙，別逼我說出你的IP是{ip}",
                img = "<image width='158' height='125' xlink:href='" + func_get_base64("食蟻獸") + "'></image>"
            });

            //沙漠貓
            ar.Add(new svg_data {
                t1 = "好像很有趣，你正在用的是",
                t2 = $"{s_作業系統} {s_手機或電腦}的{s_瀏覽器}瀏覽器",
                t3 = $"沒意外的話IP是{ip}",
                img = "<image width='144' height='125' xlink:href='" + func_get_base64("沙漠貓") + "'></image>"
            });

            //浣熊
            ar.Add(new svg_data {
                t1 = "這是公園的大危機，你居然用的是",
                t2 = $"{s_作業系統} {s_手機或電腦}的{s_瀏覽器}瀏覽器",
                t3 = $"而且IP還是{ip}",
                img = "<image width='159' height='125' xlink:href='" + func_get_base64("浣熊") + "'></image>"
            });

            //海豚
            ar.Add(new svg_data {
                t1 = $"來自 {func_取得地區(ip)} 的朋友你好",
                t2 = $"我是來自大西洋的海豚",
                t3 = $"",
                img = "<image width='152' height='125' xlink:href='" + func_get_base64("海豚") + "'></image>"
            });

            //耳廓狐
            ar.Add(new svg_data {
                t1 = $"連藪貓都不想帶你去圖書館",
                t2 = $"這是真的嗎",
                t3 = $"",
                img = "<image width='169' height='125' xlink:href='" + func_get_base64("耳廓狐") + "'></image>"
            });

            //銀狐
            ar.Add(new svg_data {
                t1 = $"什麼跳起來插下去",
                t2 = $"你到底在說些什麼東西",
                t3 = $"",
                img = "<image width='145' height='125' xlink:href='" + func_get_base64("銀狐") + "'></image>"
            });
           
            //皇帝企鵝
            ar.Add(new svg_data {
                t1 = $"",
                t2 = $"聽說現在流行變成屍體的偶像",
                t3 = $"",
                img = "<image width='125' height='125' xlink:href='" + func_get_base64("皇帝企鵝") + "'></image>"
            });

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int x = rnd.Next(1, ar.Count);
            //	<image width='143' height='125' xlink:href=''></image>

            t1 = ar[x].t1;
            t2 = ar[x].t2;
            t3 = ar[x].t3;
            img = ar[x].img;


        }


        private class svg_data {
            public String t1 = "";
            public String t2 = "";
            public String t3 = "";
            public String img = "";
        }

        /// <summary>
        /// Stream 轉 byte[]
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private byte[] StreamToBytes(Stream stream) {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 設置當前流的位置為流的開始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
        /// 取得圖片的base64
        /// </summary>
        /// <param name="imgName"></param>
        /// <returns></returns>
        private String func_get_base64(String imgName) {
            string filepath = Server.MapPath($"~/Image/SignWeb/{imgName}.png");//要下載的檔案位置       
            string filename = System.IO.Path.GetFileName(filepath);  //取得檔案名稱       
            Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read); //讀成串流     

            Byte[] bytes = StreamToBytes(iStream);
            String file = Convert.ToBase64String(bytes);
            return "data:image/png;base64," + file;
        }





        class C_地區暫存 {
            public DateTime d_時間;
            public String s_區域;
        }


        Dictionary<String, C_地區暫存> ar_地區暫存 = new Dictionary<String, C_地區暫存>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private String func_取得地區(String ip) {

            if (ip == "1.1.1.1") {
                return "台灣";
            }
      

            //如果1天內有暫存，就直接用暫存的資料
            if (ar_地區暫存.ContainsKey(ip)) {
                if (ar_地區暫存[ip].d_時間.AddDays(1) > DateTime.Now) {
                    return ar_地區暫存[ip].s_區域;
                }
            }

            String url = "http://freeapi.ipip.net/" + ip;

            String html = "";

            //從網路抓資料
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 20 * 1000;
                request.Method = "GET";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream())) {
                        html = reader.ReadToEnd();
                        response.Close();
                    }
                }
            } catch {
                return "台灣";
            }


            try {

                JArray json = JArray.Parse(html);
                String s = "";
                if (json.Count >= 2) {
                    if (json[2].ToString() != "") {//縣市
                        s = json[2].ToString();
                    } else if (json[1].ToString() != "") { //省
                        s = json[1].ToString();
                    } else if (json[0].ToString() != "") {//國家
                        s = json[0].ToString();
                    }
                }

                //存到暫存
                s = func_轉繁體(s);
                if (ar_地區暫存.ContainsKey(ip) == false) {
                    ar_地區暫存.Add(ip, new C_地區暫存());
                }
                ar_地區暫存[ip] = new C_地區暫存 { d_時間 = DateTime.Now, s_區域 = s };

                return s;

            } catch { }




            return "台灣";



        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private String func_轉繁體(String s) {
            return s.Replace("国", "國").Replace("亚", "亞").Replace("湾", "灣")
                      .Replace("台", "臺").Replace("东", "東").Replace("县", "縣")
                      .Replace("莲", "蓮").Replace("连", "連").Replace("区", "區")
                      .Replace("顿", "頓").Replace("欧", "歐").Replace("门", "門")
                      .Replace("厦", "廈").Replace("广", "廣").Replace("网", "網")
                      .Replace("机", "機").Replace("页", "頁").Replace("兰", "蘭")
                      .Replace("妈", "媽").Replace("马", "馬").Replace("来", "來")
                      .Replace("叶", "葉").Replace("业", "業").Replace("园", "園")
                      .Replace("云", "雲").Replace("义", "義").Replace("长", "長")
                      .Replace("爱", "愛").Replace("岛", "島").Replace("关", "關")
                      .Replace("泻", "潟").Replace("冈", "岡").Replace("贺", "賀")
                      .Replace("库", "庫").Replace("华", "華").Replace("苏", "蘇")
                      .Replace("庆", "慶").Replace("辖", "轄").Replace("陕", "陜")
                      .Replace("贵", "貴").Replace("龙", "龍").Replace("辽", "遼")
                      .Replace("宁", "寧").Replace("维", "維").Replace("尔", "爾")
                      .Replace("壮", "壯").Replace("肃", "肅").Replace("联", "聯")
                      .Replace("伦", "倫").Replace("罗", "羅").Replace("达", "達")
                      .Replace("刚", "剛").Replace("萨", "薩").Replace("纳", "納")
                      .Replace("腊", "臘").Replace("韩", "韓").Replace("劳", "勞")
                      .Replace("宾", "賓").Replace("叙", "敘").Replace("乌", "烏")
                      .Replace("莱", "萊").Replace("缅", "緬").Replace("麦", "麥")
                      .Replace("济", "濟").Replace("买", "買").Replace("卢", "盧")
                      .Replace("宾", "賓").Replace("绍", "紹").Replace("颠", "顛")
                      .Replace("纽", "紐").Replace("约", "約").Replace("矶", "磯")
                      .Replace("圣", "聖").Replace("扬", "揚").Replace("迈", "邁")
                      .Replace("凤", "鳳").Replace("图", "圖").Replace("温", "溫")
                      .Replace("会", "會").Replace("绘", "繪").Replace("画", "畫")
                      .Replace("峡", "峽").Replace("电", "電").Replace("闪", "閃")
                      .Replace("闭", "閉").Replace("发", "發").Replace("楼", "樓")
                      .Replace("几", "幾").Replace("阳", "陽").Replace("开", "開")
                      .Replace("点", "點").Replace("费", "費").Replace("离", "離")
                      .Replace("币", "幣").Replace("贴", "貼").Replace("顾", "顧")
                      .Replace("选", "選").Replace("乐", "樂").Replace("观", "觀")
                      .Replace("场", "場").Replace("态", "態").Replace("戏", "戲")
                      .Replace("时", "時").Replace("宝", "寶").Replace("间", "間")
                      .Replace("盖", "蓋").Replace("经", "經").Replace("单", "單")
                      .Replace("闲", "閒").Replace("远", "遠").Replace("种", "種")
                      .Replace("学", "學").Replace("补", "補").Replace("尽", "盡")
                      .Replace("帮", "幫").Replace("觉", "覺").Replace("计", "計")
                      .Replace("现", "現").Replace("张", "張").Replace("双", "雙")
                      .Replace("实", "實").Replace("尘", "塵").Replace("还", "還")
                      .Replace("团", "團").Replace("员", "員").Replace("梦", "夢")
                      .Replace("医", "醫").Replace("占", "佔").Replace("优", "優")
                      .Replace("质", "質").Replace("体", "體").Replace("驾", "駕")
                      .Replace("为", "為").Replace("仅", "僅").Replace("书", "書")
                      .Replace("价", "價").Replace("适", "適").Replace("帮", "幫")
                      .Replace("顶", "頂").Replace("华", "華").Replace("记", "記")
                      .Replace("两", "兩").Replace("读", "讀").Replace("设", "設")
                      .Replace("毕", "畢").Replace("资", "資").Replace("奋", "奮")
                      .Replace("万", "萬").Replace("当", "當").Replace("报", "報")
                      .Replace("过", "過").Replace("头", "頭").Replace("兴", "興")
                      .Replace("统", "統").Replace("测", "測").Replace("垦", "墾")
                      .Replace("营", "營").Replace("对", "對").Replace("个", "個")
                      .Replace("认", "認").Replace("说", "說").Replace("坜", "壢")
                      .Replace("该", "該").Replace("让", "讓").Replace("卖", "賣")
                      .Replace("这", "這").Replace("愿", "願").Replace("无", "無")
                      .Replace("摊", "攤").Replace("则", "則").Replace("数", "數")
                      .Replace("样", "樣").Replace("块", "塊").Replace("猪", "豬")
                      .Replace("边", "邊").Replace("灵", "靈").Replace("调", "調")
                      .Replace("没", "沒").Replace("讲", "講").Replace("难", "難")
                      .Replace("萝", "蘿").Replace("战", "戰").Replace("线", "線")
                      .Replace("么", "麼").Replace("从", "從").Replace("声", "聲")
                      .Replace("听", "聽").Replace("变", "變").Replace("铁", "鐵");
        }

    }
}