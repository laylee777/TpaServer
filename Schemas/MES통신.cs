using DevExpress.Diagram.Core.Shapes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using static DSEV.Schemas.MESClient;
using System.Windows.Forms;
using static DSEV.Schemas.MES통신;

namespace DSEV.Schemas
{
    public class MES통신
    {
        public event Global.BaseEvent 통신상태알림;
        //public event EventHandler<string> 검사진행요청;
        public MES통신() { }

        public String 로그영역 = "MES통신";
        public MESClient 통신장치;

        public enum 송신메세지아이디
        {
            REQ_PROCESS_START = 1,
            REQ_PROCESS_END = 3,
            REQ_LINK_TEST = 5,
        }
        public enum 수신메세지아이디
        {
            REP_PROCESS_START = 11,
            REP_PROCESS_END = 13,
            REP_LINK_TEST = 15,
        }


        public Boolean Init()
        {
            try
            {
                Debug.WriteLine("mes통신 시작");
                this.통신장치 = new MESClient();
                this.통신장치.Init();
                this.통신장치.자료수신 += 통신장치_자료수신;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public void 하부큐알결과신호전송(Boolean 결과)
        {
            Global.장치통신.하부큐알결과OK신호 = 결과;
            Global.장치통신.하부큐알결과NG신호 = !결과;
            Global.장치통신.하부큐알확인완료신호 = true;
        }

        private void 통신장치_자료수신(object sender, MESSAGE e)
        {
            try
            {
                Debug.WriteLine("자료수신");
                if (e.MSG_ID == 수신메세지아이디.REP_PROCESS_START.ToString()) //하부큐알관련
                {
                    //OK일 경우
                    if (e.RESULT == "0")
                    {
                        하부큐알결과신호전송(true);
                        Global.정보로그(로그영역, "MES통신", $"양품투입됨", false);
                        //검사진행요청.Invoke(this, null);
                        return;
                    }
                    하부큐알결과신호전송(false);
                    Global.오류로그(로그영역, "MES통신", $"불량품 투입됨", true);

                }
                else if (e.MSG_ID == 수신메세지아이디.REP_PROCESS_END.ToString())
                {
                    Global.정보로그(로그영역, "MES통신", $"착공완료응답 수신완료", false);
                    return;
                }
                else if (e.MSG_ID == 수신메세지아이디.REP_LINK_TEST.ToString())
                {
                    //PROGRAM에서 보낸 LINKTEST에 대한 응답.
                    this.통신상태알림?.Invoke();
                    Global.정보로그(로그영역, "MES통신", $"LINKTEST 수신완료", false);
                    return;
                }
                else if(e.MSG_ID == 송신메세지아이디.REQ_LINK_TEST.ToString())
                {
                    //MES에서 보낸 LINK테스트 확인.
                    Global.정보로그(로그영역, "MES통신", $"{e.SYSTEMID} 에서 송신한 LINKTEST 수신완료.", false);
                }
            }
            catch (Exception ex)
            {
                Global.오류로그(로그영역, "자료수신", $"수신 자료가 올바르지 않습니다. {ex.Message}", true);
            }
        }

        public void Close() => this.통신장치?.Close();
        public void Start() => this.통신장치?.Start();
        public void Stop() => this.통신장치?.Stop();

        public Boolean 자료송신(MESSAGE messge)
        {
            if (!this.통신장치.연결여부) return false;
            if (this.통신장치.Send(XmlMessageConverter.GenerateXmlMessage(messge))) return true;

            if(messge.MSG_ID == 송신메세지아이디.REQ_LINK_TEST.ToString())
            {
                //REQ TEST 실패시 MES사용유무 체크하여 다시 재연결 시도.
                Task.Run(() =>
                {
                    if (Global.환경설정.MES사용유무)
                    {
                        Global.mes통신.Init();
                        Global.mes통신.Start();
                    }
                });
            }

            Global.오류로그(로그영역, "자료전송", $"[{messge.MSG_ID}] 자료전송에 실패하였습니다.", true);
            //this.통신장치.Close();
            return false;
        }
    }



    public class MESClient
    {
        public event EventHandler<MESSAGE> 자료수신;
        public Int32 대기시간 { get; set; } = 20;
        public Boolean 동작여부 { get; set; } = false;
        public String 로그영역 { get; set; } = "MES통신";
        public virtual Boolean 연결여부 { get => this.Connected(); }
        public Boolean 핑퐁상태 { get; set; }
        public TcpClient 통신소켓 = null;
        public NetworkStream Stream { get => 통신소켓?.GetStream(); }

        //string xmlData = GenerateXmlMessage("REQ_PROCESS_START", "EQPID", "20240304093001553", "F00395AB231;F00395AB231");
        public void Init() { this.통신소켓 = new TcpClient() { ReceiveBufferSize = 4096, SendBufferSize = 4096, SendTimeout = 10000, ReceiveTimeout = 10000 }; }
        public void Start() { this.동작여부 = true; new Thread(Read) { Priority = ThreadPriority.AboveNormal }.Start(); }
        public void Close()
        {
            this.동작여부 = false;
            this.Disconnect();
        }
        public void Stop() => this.동작여부 = false;

        private Int32 PollingPeriod = 1000;
        private DateTime PollingTime = DateTime.Today;
        private Boolean PollingState = false;

        public Boolean Connected()
        {
            if (this.통신소켓 == null) return false;
            if ((DateTime.Now - PollingTime).TotalMilliseconds < PollingPeriod) return PollingState;
            try { PollingState = this.통신소켓.Client.Poll(1000, SelectMode.SelectWrite); }
            catch { PollingState = false; }
            PollingTime = DateTime.Now;
            return PollingState;
        }
        public void Disconnect()
        {
            if (this.연결여부)
            {
                this.통신소켓?.Client?.Shutdown(SocketShutdown.Both);
                this.통신소켓?.Close();
            }
            this.통신소켓?.Dispose();
            this.통신소켓 = null;
        }

        private Int32 통신연결간격 = 1;
        private DateTime 통신연결시간 = DateTime.Now;
        public Boolean Connect()
        {
            //if (Global.환경설정.동작구분 == 동작구분.LocalTest) return false;
            try
            {
                if ((DateTime.Now - 통신연결시간).TotalSeconds < 통신연결간격) return false;
                this.Disconnect();
                this.Init();
                this.통신연결시간 = DateTime.Now;
                String address = Global.환경설정.동작구분 == 동작구분.LocalTest ? "localhost" : Global.환경설정.MES주소;
                //String address = "192.168.10.2";
                this.통신소켓?.Connect(address, 6003);
                return 연결여부;
            }
            catch (Exception ex)
            {
                //Debug.WriteLine($"[{Global.환경설정.서버주소}] 연결할 수 없습니다. {ex.Message}", 로그영역);
                Global.경고로그(로그영역, "MES연결", $"[MES] 연결할 수 없습니다. {ex.Message}", true);
            }
            return false;
        }

        public List<Byte> ReceiveBuffer = new List<Byte>();
        public void Read()
        {
            while (this.동작여부)
            {
                Thread.Sleep(대기시간);
                if (!this.동작여부) break;

                if (!this.연결여부)
                {
                    if (this.Connect()) { }
                    continue;
                }

                try
                {
                    통신핑퐁전송();
                    if(this.통신소켓 == null)
                    {
                        this.동작여부 = false;
                        return;
                    }

                    if (this.통신소켓.Available < 1) continue;

                    Debug.WriteLine("자료수신2");
                    Byte[] buffer = new Byte[4096];
                    Int32 read = this.Stream.Read(buffer, 0, buffer.Length);
                    Debug.WriteLine("자료수신3");
                    string messege = Encoding.UTF8.GetString(buffer.ToArray());
                    Debug.WriteLine($"자료수신4 : {messege}");

                    if (messege == "") continue;
                    this.자료수신?.Invoke(this, XmlMessageConverter.DeserializeXmlMessage(messege));

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message, "Read Error");
                }
            }
        }
        public Boolean Send(String data)
        {
            if (!this.연결여부) return false;
            try
            {
                Byte[] bytes = Encoding.UTF8.GetBytes(data);
                this.Stream.Write(bytes, 0, bytes.Length);
                this.Stream.Flush();
                return true;
            }
            catch (Exception ex)
            {
                Global.오류로그(로그영역, "Send", ex.Message, true);
                this.Disconnect();
                return false;
            }
        }

        public void 통신핑퐁전송()
        {
            if ((DateTime.Now - 통신연결시간).TotalSeconds < 통신연결간격) return;
            통신연결시간 = DateTime.Now;

            this.핑퐁상태 = !this.핑퐁상태;

            MESSAGE message = new MESSAGE();
            message.SetMessage(송신메세지아이디.REQ_LINK_TEST.ToString(), "IVM01", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff"), String.Empty, String.Empty, String.Empty, String.Empty);
            Global.mes통신.자료송신(message);
        }
    }


    public class MESSAGE
    {
        public string MSG_ID { get; set; }
        public string SYSTEMID { get; set; }
        public string DATE_TIME { get; set; }
        public string BARCODE_ID { get; set; }

        public string RESULT { get; set; }
        public string RESULT_MSG { get; set; }
        public string KEY { get; set; }

        public MESSAGE SetMessage(String msgid, String systemid, String datetime, String barcodeid, String result, String resultmsg, String key)
        {
            this.MSG_ID = msgid.ToString();
            this.SYSTEMID = systemid;
            this.DATE_TIME = datetime;
            this.BARCODE_ID = barcodeid;
            //this.RESULT = result;
            //this.RESULT_MSG = resultmsg;
            this.KEY = key;

            return this;
        }
    }

    public class XmlMessageConverter
    {
        // XML 메시지를 생성하는 메서드 (원본 코드와 동일)
        public static string GenerateXmlMessage(string msgId, string systemId, string dateTime, string barcodeId, string result = "", string resultMsg = "", string key = "")
        {
            var message = new MESSAGE
            {
                MSG_ID = msgId,
                SYSTEMID = systemId,
                DATE_TIME = dateTime,
                BARCODE_ID = barcodeId,
                RESULT = result,
                RESULT_MSG = resultMsg,
                KEY = key
            };

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer serializer = new XmlSerializer(typeof(MESSAGE));
            using (StringWriter writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, message, ns);
                return writer.ToString();
            }
        }

        public static string GenerateXmlMessage(MESSAGE message)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer serializer = new XmlSerializer(typeof(MESSAGE));
            using (StringWriter writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, message, ns);
                return writer.ToString();
            }
        }

        // XML 문자열을 MESSAGE 클래스로 역직렬화하는 메서드
        public static MESSAGE DeserializeXmlMessage(string xmlMessage)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MESSAGE));
            using (StringReader reader = new StringReader(xmlMessage))
            {

                Debug.WriteLine("Deserialize시작");

                var message = (MESSAGE)serializer.Deserialize(reader);
                Debug.WriteLine("Deserialize끝");
                return message;
            }
        }
    }

    // UTF8 인코딩을 지원하는 StringWriter 클래스 정의
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

}
