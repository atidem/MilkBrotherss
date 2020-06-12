using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class HelloRequesterAciklama : RunAbleThread
{
    public int count = 0;
    public Vector3 Point{ get; set; }
    public string info="";                          // gameScore ve gameState yollamak için string
    public string message="";                       // pythıon'dan bize gelen string    (r, l, u, d gelebilir. bunun anlamı r gelirse right yani player sağa gitsin gibi)
    protected override void Run()
    {
        using (RequestSocket client = new RequestSocket())              // NetMQ.Socket sınıfından bir client oluşturuyoruz bu bizim iletişimimizi sağlayacağımız client
        {
            client.Connect("tcp://localhost:5555");                     // client'ı localhosta bağlıyoruz yani servera bağlıyoruz gibi iletişim amaçlı tabi
    
            for (int i = 0; i < 100000 && Running; i++)                 // RunAbleThread'taki bool Running = true ise (ki true olması için Start() çağırılması gerekli) Oyunun zaten başında çağırılıyor sıkıntı yok
            {
                Debug.Log("Sending position");          
                if(i % 10 == 0)
                    client.SendFrame(info);                                 // client'a skor ve oyunun çalışıp çalışmadığı gönderiliyor. sanırım hesaplama ve eniyisini kontrol etmek amaçlı yani eğitimle ilgili
                    
                bool gotMessage = false;                                // bu bizim client ile iletişimde olduğumuzu tutacak
                
                while (Running)                                         //                      
                {
                    if (count == 0)                                     // 
                    {
                        gotMessage = client.TryReceiveFrameString(out message);             // client'tan message geldiyse (player için yön cevabı) gotMEssage = true yap
                        count++;            
                        if (gotMessage)                                                     // gotMessage = true ise While'dan çık
                        {
                            break;
                        }    
                    }
                    else
                    {
                        count++;
                    }      

                    if (count == 100)
                    {
                        count = 0;
                    }                       
                }
            }
        }
        NetMQConfig.Cleanup();
    }
}