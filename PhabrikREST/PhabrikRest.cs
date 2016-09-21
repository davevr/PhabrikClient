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
    public delegate void PlanetList_callback(List<PlanetObj> theResult);
    public delegate void Player_callback(PlayerObj theResult);
    public delegate void string_callback(string theResult);
	public delegate void bool_callback(bool theResult);
	public delegate void null_callback();

    public class PhabrikServer
    {
        private static RestClient apiClient;
		private static string serverAPI = "http://phabrik-server-01.appspot.com/api/v1/";
		private static string localAPI = "http://10.0.3.2:8080/api/v1/"; // "http://127.0.0.1:8080/api/v1/";
		private static string apiPath;
        private static PlayerObj _currentUser = null;
		public static string LastError {get; set;}
		private static bool useProdServer = false;
        
        public static void InitServer(bool_callback callback)
        {
			if (useProdServer)
			{
				System.Console.WriteLine("Using Production Server");
				apiPath = serverAPI;
			}
			else {
				System.Console.WriteLine("Using Local Server");
				apiPath = localAPI;
			}
            apiClient = new RestClient(apiPath);
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