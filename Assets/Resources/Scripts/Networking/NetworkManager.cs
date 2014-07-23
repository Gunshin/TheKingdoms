using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{

   // Use this for initialization
   void Start()
   {

   }

   // Update is called once per frame
   void Update()
   {

   }

   #region server

   const string typeName = "UniqueGameName";
   const string gameName = "ZOMGRoom";

   const int connectionCount = 4;

   const int portNumber = 25000;

   void StartServer()
   {
      Network.InitializeServer(connectionCount, portNumber, !Network.HavePublicAddress());
      MasterServer.RegisterHost(typeName, gameName);
   }

   void OnServerInitialized()
   {
      CreateGameObject();
   }

   void OnGUI()
   {
      if (!Network.isClient && !Network.isServer)
      {
         if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
            StartServer();

         if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
            RefreshHostList();

         if (hostList != null)
         {
            for (int i = 0; i < hostList.Length; i++)
            {
               if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
                  JoinServer(hostList[i]);
            }
         }
      }
   }

   #endregion server

   #region client

   HostData[] hostList;

   private void RefreshHostList()
   {
      MasterServer.RequestHostList(typeName);
   }

   void OnMasterServerEvent(MasterServerEvent msEvent)
   {
      if (msEvent == MasterServerEvent.HostListReceived)
         hostList = MasterServer.PollHostList();
   }

   private void JoinServer(HostData hostData)
   {
      Network.Connect(hostData);
   }

   void OnConnectedToServer()
   {
      CreateGameObject();
   }

   #endregion client

   void CreateGameObject()
   {
      GameObject gameObject = (GameObject)Resources.Load("Prefabs/ProceduralTerrain");
      Instantiate(gameObject, new Vector3(10, 0, 10), Quaternion.identity);
   }
}
