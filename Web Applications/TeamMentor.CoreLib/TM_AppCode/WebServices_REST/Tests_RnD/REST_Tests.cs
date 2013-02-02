using System;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace TeamMentor.CoreLib
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode	 = AspNetCompatibilityRequirementsMode.Allowed)]
	public class REST_Tests
	{


		[OperationContract, WebGet(UriTemplate = "hello")]
		public System.IO.Stream HelloWorld()
		{
			string result = "<h1>Hello world</h1>";
			byte[] resultBytes = Encoding.UTF8.GetBytes(result);
			WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
			return new MemoryStream(resultBytes);
		}

		[OperationContract, WebGet(UriTemplate = "html/*")]
		public System.IO.Stream HelloWorld_Html()
		{
			string result = "<h1>Hello world</h1>";
			byte[] resultBytes = Encoding.UTF8.GetBytes(result);
			WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
			return new MemoryStream(resultBytes);
		}

		//test to send out an image
		[OperationContract, WebGet]
		public Stream GetImage(int width, int height)
		{
			Bitmap bitmap = new Bitmap(width, height);
			for (int i = 0; i < bitmap.Width; i++)
			{
				for (int j = 0; j < bitmap.Height; j++)
				{
					bitmap.SetPixel(i, j, (Math.Abs(i - j) < 2) ? Color.Blue : Color.Red);
				}
			}
			MemoryStream ms = new MemoryStream();
			bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
			ms.Position = 0;
			WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpeg";
			return ms;
		}
	}
}
