using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CITechnicalTest.RestAPI
{
    public class RequestObject
    {
		public string url { get; set; }

		public RequestObject(string url)
		{
			this.url = url;
		}

		public string ExecAPIRequest()
		{
			string result = string.Empty;
			HttpWebRequest apiRequest = null;
			HttpWebResponse apiResponse = null;
			Stream stream = null;
			StreamReader streamReader = null;

			try
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

				apiRequest = (HttpWebRequest)WebRequest.Create(url);
				apiRequest.Method = "Get";
				apiRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				apiRequest.ContentType = "application/json";

				apiResponse = (HttpWebResponse)apiRequest.GetResponse();
				stream = apiResponse.GetResponseStream();
				streamReader = new StreamReader(stream);
				result = streamReader.ReadToEnd();
			}

			catch (Exception ex)
			{
				result = ex.Message.ToString();
			}

			finally
			{
				if (apiResponse != null)
				{
					apiResponse.Close();
					stream.Dispose();
					streamReader.Dispose();
				}
			}

			return result;
		}
	}
}
