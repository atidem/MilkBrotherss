using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;


/// <summary>
///     Example of requester who only sends Hello. Very nice guy.
///     You can copy this class and modify Run() to suits your needs.
///     To use this class, you just instantiate, call Start() when yQou want to start and Stop() when you want to stop.
/// </summary>
public class HelloRequester : RunAbleThread
{
    public int count = 0;
    public Vector3 Point{ get; set; }
    public string info="";
    public string message="";
    /// <summary>
    ///     Request Hello message to server and receive message back. Do it 10 times.
    ///     Stop requesting when Running=false.
    /// </summary>

    public void fonk()
    {
        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5557");
               Debug.Log("Sending position");

                client.SendFrame(info);

                bool gotMessage = false;

                while (Running)
                {
                    if (count == 0)
                    {
                        gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
                    if (gotMessage)
                    {
                        Debug.Log("get move");
                        break;
                    }
                    }
                    else
                        count++;                  
                    if (count == 100)
                        count = 0;
                }     
        }
        
        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }

    protected override void Run()
    {
        // playerMovement player = new playerMovement();
        // ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
       /* using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5555");
    
            for (int i = 0; i < 100000 && Running; i++)
            {
               Debug.Log("Sending position : " + i);

                client.SendFrame(info);

                // ReceiveFrameString() blocks the thread until you receive the string, but TryReceiveFrameString()
                // do not block the thread, you can try commenting one and see what the other does, try to reason why
                // unity freezes when you use ReceiveFrameString() and play and stop the scene without running the server
                // string message = client.ReceiveFrameString();
                // Debug.Log("Received: " + message);
                // string message = null;
                bool gotMessage = false;

                while (Running)
                {
                    if (count == 0)
                    {
                        gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful

                        count++;
                        if (gotMessage)
                        {
                            break;
                        }    
                    }
                    else
                        count++;                  
                    if (count == 100)
                        count = 0;
                }     
            }
        }
        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
*/
    }
}