using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using RestSharp;
using System.Runtime.Serialization;
using ServiceStack.Text;
using System.Threading.Tasks;


namespace Phabrik.Core
{
    public delegate void SolSysList_callback(List<SolSysObj> theResult);
    public delegate void PlanetList_callback(List<PlanetObj> theResult);
    public delegate void Player_callback(PlayerObj theResult);
    public delegate void string_callback(string theResult);
	public delegate void bool_callback(bool theResult);
	public delegate void null_callback();
    public delegate void SolSysObj_callback(SolSysObj theResult);
    public delegate void TerrainObj_callback(TerrainObj theResult);
    public delegate void SectorObj_callback(SectorObj theResult);
    public delegate void StructureObj_callback(StructureObj theResult);

    public class PhabrikServer
    {
        private static RestClient apiClient;
        private static string prodServerPath = "http://phabrik-server-01.appspot.com";
        private static string localServerPath = "http://192.168.0.22:8080";

		private static string apiPathExt = "/api/v1/";
        private static string imagePathExt = "/Images/";
        private static string serverPath;
        private static PlayerObj _currentUser = null;
		public static string LastError {get; set;}
		private static bool useProdServer = false;
        
        public static void InitServer(bool_callback callback)
        {
			if (useProdServer)
			{
				System.Console.WriteLine("Using Production Server");
				serverPath = prodServerPath;
			}
			else {
				System.Console.WriteLine("Using Local Server");
				serverPath = localServerPath;
			}
            apiClient = new RestClient(serverPath + apiPathExt);
            apiClient.CookieContainer = new CookieContainer();

			RestRequest request = new RestRequest("status", Method.GET);
			apiClient.ExecuteAsync(request, (response) =>
			{
				if (response.StatusCode == HttpStatusCode.OK)
					callback(true);
				else
					callback(false);
			});
        }

        public static string BaseImageUrl
        {
            get
            {
                return serverPath + imagePathExt;
            }
        }

        

        public static PlayerObj CurrentUser
        {
            get { return _currentUser; }
        }

       

        public static void Logout()
        {
            string fullURL = "user/logout";

            RestRequest request = new RestRequest(fullURL, Method.POST);

            apiClient.Execute(request);
			LastError = null;
            _currentUser = null;
        }


        public static void FetchKnownSystems(SolSysList_callback callback)
        {
            string fullURL = "solsys";
            RestRequest request = new RestRequest(fullURL, Method.GET);
            request.AddParameter("known", true);

            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    List < SolSysObj> newObj = response.Content.FromJson<List<SolSysObj>>();
                    callback(newObj);
                }
                else
                {
                    callback(null);
                }
            });
        }

        public static void FetchSolSys(int xLoc, int yLoc, int zLoc, SolSysObj_callback callback)
        {
            string fullURL = "solsys";
            RestRequest request = new RestRequest(fullURL, Method.GET);
            request.AddParameter("xloc", xLoc);
            request.AddParameter("yloc", yLoc);
            request.AddParameter("zloc", zLoc);

            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    SolSysObj newObj = response.Content.FromJson<SolSysObj>();
                    callback(newObj);
                } else
                {
                    callback(null);
                }
            });
        }

        public static void FetchSolSysById(long solSysId, SolSysObj_callback callback)
        {
            string fullURL = "solsys";
            RestRequest request = new RestRequest(fullURL, Method.GET);
            request.AddParameter("solsysid", solSysId);
            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    SolSysObj newObj = response.Content.FromJson<SolSysObj>();
                    callback(newObj);
                }
                else
                {
                    callback(null);
                }
            });
        }

        public static void FetchTerrain(long planetId, TerrainObj_callback callback)
        {
            string fullURL = "terrain";
            RestRequest request = new RestRequest(fullURL, Method.GET);
            request.AddParameter("planetid", planetId);


            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK && response.Content != "null")
                {
                    TerrainObj newObj = response.Content.FromJson<TerrainObj>();
                    callback(newObj);
                }
                else if (response.StatusCode != 0)
                {
                    callback(null);
                }
            });
        }

        public static void FetchSolSysStatus(long solSysId, TerrainObj_callback callback)
        {
            string fullURL = "solsys";
            RestRequest request = new RestRequest(fullURL, Method.GET);
            request.AddParameter("solsysid", solSysId);
            request.AddParameter("status", solSysId);


            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TerrainObj newObj = response.Content.FromJson<TerrainObj>();
                    callback(newObj);
                }
                else if (response.StatusCode != 0)
                {
                    callback(null);
                }
            });
        }

        public static void ProbePlanet(long planetId, TerrainObj_callback callback)
        {
            string fullURL = "probeplanet";
            RestRequest request = new RestRequest(fullURL, Method.POST);
            request.AddParameter("planetid", planetId);


            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TerrainObj newObj = response.Content.FromJson<TerrainObj>();
                    callback(newObj);
                }
                else
                {
                    callback(null);
                }
            });
        }

        public static void SaveTerrainPaint(TerrainObj theTerrain, bool_callback callback)
        {
            string fullURL = "terrain";
            RestRequest request = new RestRequest(fullURL, Method.PUT);
            

            List<SectorPaintObj> sectorList = new List<SectorPaintObj>();

            for (int x = 0; x < theTerrain.width; x++)
            {
                for (int y = 0; y < theTerrain.height; y++)
                {
                    var curSect = theTerrain._sectorArray[x][y];
                    if (curSect.dirty)
                    {
                        sectorList.Add(new SectorPaintObj(curSect));
                        curSect.dirty = false;
                    }
                }
            }
            string sectorStr = sectorList.ToJson<List<SectorPaintObj>>();
            request.AddParameter("sectormap", sectorStr);
            request.AddParameter("paint", true);

            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    callback(true);
                }
                else
                {
                    callback(false);
                }
            });
        }




        public static void FetchSector(long sectorId, SectorObj_callback callback)
        {
            string fullURL = "sector";
            RestRequest request = new RestRequest(fullURL, Method.GET);
            request.AddParameter("sectorid", sectorId);


            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK && response.Content != "null")
                {
                    SectorObj newObj = response.Content.FromJson<SectorObj>();
                    callback(newObj);
                }
                else if (response.StatusCode != 0)
                {
                    callback(null);
                }
            });
        }

        public static void FetchStructure(long structureId, StructureObj_callback callback)
        {
            string fullURL = "structure";
            RestRequest request = new RestRequest(fullURL, Method.GET);
            request.AddParameter("structureid", structureId);


            apiClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StructureObj newObj = response.Content.FromJson<StructureObj>();
                    callback(newObj);
                }
                else
                {
                    callback(null);
                }
            });
        }




        public static void Login(string username, string pwd, bool create, Player_callback callback)
        {
            string fullURL = "signin";

            RestRequest request = new RestRequest(fullURL, Method.POST);
            request.AddParameter("N", username);
            request.AddParameter("pwd", pwd);
			if (create)
			{
				request.AddParameter("create", "on");
			}

            apiClient.ExecuteAsync(request, (response) =>
                {
					LastError = null;
				PlayerObj newUser = response.Content.FromJson<PlayerObj>();
                    if (newUser != null)
                    {
                        _currentUser = newUser;
                        callback(newUser);
                    }
                    else
                        callback(null);
                });
        }

  

        public static void UploadImage(Stream photoStream, string fileName, string_callback callback)
		{
			string uploadURL = GetImageUploadURL();
			int pathSplit = uploadURL.IndexOf("/", 10);
			string appPath = uploadURL.Substring(0, pathSplit);
			string requestPath = uploadURL.Substring(pathSplit);
			RestClient onetimeClient = new RestClient(appPath);
			//onetimeClient.CookieContainer = apiClient.CookieContainer;
			var request = new RestRequest(requestPath, Method.POST);
			request.AddHeader("Accept", "*/*");
			request.AddFile("file", ReadToEnd(photoStream), fileName, "image/jpeg");

			onetimeClient.ExecuteAsync(request, (response) =>
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						callback(response.Content);
					}
					else
					{
						//error ocured during upload
						callback(null);
					}
				});
		}

		public static string GetImageUploadURL()
		{
			RestRequest request = new RestRequest("uploadImage", Method.GET);
			IRestResponse response = apiClient.Execute(request);
			return response.Content;
		}

       




        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

    }
}