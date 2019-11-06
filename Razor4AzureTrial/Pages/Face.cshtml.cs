using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Razor4AzureTrial.Models;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;


namespace Razor4AzureTrial.Pages
{
    public class FaceModel : PageModel
    {
        
        public string selectedImage { get; set; } = string.Empty;


        [BindProperty]
        public string base64txt { get; set; }

        
        public List<FaceId> faceIdList { get; set; }

        public static readonly string base64PrefixPNG = "data:image/png;base64,";
        private readonly int thumHeight = 75;
        private readonly int thumWidth = 75;

        public void OnGet()
        {

        }

        public void OnPost()
        {
            
            const string subscriptionKey = "0c648c1aec4b4ae087c88aa5c2b8f9e5";
            const string uriBase =
            "https://westus2.api.cognitive.microsoft.com/face/v1.0/detect";

            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
                "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";


            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            string convert = base64txt.Remove(0,base64txt.IndexOf(",")+1);

            byte[] byteData = Convert.FromBase64String(convert);


            using (ByteArrayContent content = new ByteArrayContent(byteData))            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                Task<HttpResponseMessage> postTask = client.PostAsync(uri, content);
                postTask.Wait();

                // Get the JSON response.
                response = postTask.Result;
                Task<string> contentString = response.Content.ReadAsStringAsync();

                faceIdList = JsonConvert.DeserializeObject<List<FaceId>>(contentString.Result);
                                
            }

            using (MemoryStream ms1 = new MemoryStream(byteData))
            {
                using (Bitmap bmp = new Bitmap(ms1))
                {  
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        foreach (FaceId f in faceIdList)
                        {
                            Rectangle rect = new Rectangle(f.faceRectangle.left, f.faceRectangle.top, f.faceRectangle.width, f.faceRectangle.height);
                            Bitmap bmpNew = bmp.Clone(rect, bmp.PixelFormat);
                            Image thum = bmpNew.GetThumbnailImage(thumHeight, thumWidth, null, IntPtr.Zero);
                            Bitmap bmpThum = new Bitmap(thum);
                            f.faceRectangleBase64String = bmp264string(bmpThum);
                            bmpNew.Dispose();
                            bmpThum.Dispose();
                        }

                        foreach (FaceId f in faceIdList)
                        {
                            Pen p = getPen(f);
                            g.DrawRectangle(p, f.faceRectangle.left, f.faceRectangle.top, f.faceRectangle.width, f.faceRectangle.height);
                            p.Dispose();
                        }

                        MemoryStream ms2 = new MemoryStream();
                        bmp.Save(ms2, ImageFormat.Png);

                        selectedImage = base64PrefixPNG + Convert.ToBase64String(ms2.GetBuffer());

                        ms2.Close();
                        ms2.Dispose();

                    }
                }
            }
        }

        private string bmp264string(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms,ImageFormat.Png);
            string str = base64PrefixPNG + Convert.ToBase64String(ms.GetBuffer());

            return str;
        }

        private Pen getPen(FaceId f)
        {
            double tmpd;
            int tmpi;

            tmpd = f.faceAttributes.emotion.anger;
            int boldness = getBoldness(f.faceAttributes.emotion.anger);
            Color linecolor = Color.Red;
            f.mainEmotion = "anger";

            tmpi = getBoldness(f.faceAttributes.emotion.contempt);
            if (tmpi >= boldness && tmpd < f.faceAttributes.emotion.contempt)
            {
                tmpd = f.faceAttributes.emotion.contempt;
                boldness = tmpi;
                linecolor = Color.Green;
                f.mainEmotion = "contempt";
            }
            tmpi = getBoldness(f.faceAttributes.emotion.disgust);
            if (tmpi>= boldness && tmpd < f.faceAttributes.emotion.disgust)
            {
                tmpd = f.faceAttributes.emotion.disgust;
                boldness = tmpi;
                linecolor = Color.Gray;
                f.mainEmotion = "disgust";
            }
            tmpi = getBoldness(f.faceAttributes.emotion.fear);
            if (tmpi >= boldness && tmpd < f.faceAttributes.emotion.fear)
            {
                tmpd = f.faceAttributes.emotion.fear;
                boldness = tmpi;
                linecolor = Color.PaleTurquoise;
                f.mainEmotion = "fear";
            }
            tmpi = getBoldness(f.faceAttributes.emotion.happiness);
            if (tmpi >= boldness && tmpd < f.faceAttributes.emotion.happiness)
            {
                tmpd = f.faceAttributes.emotion.happiness;
                boldness = tmpi;
                linecolor = Color.Pink;
                f.mainEmotion = "happiness";
            }
            tmpi = getBoldness(f.faceAttributes.emotion.neutral);
            if (tmpi >= boldness && tmpd < f.faceAttributes.emotion.neutral)
            {
                tmpd = f.faceAttributes.emotion.neutral;
                boldness = tmpi;
                linecolor = Color.AntiqueWhite;
                f.mainEmotion = "neutral";
            }

            tmpi = getBoldness(f.faceAttributes.emotion.sadness);
            if (tmpi >= boldness && tmpd < f.faceAttributes.emotion.sadness)
            {
                tmpd = f.faceAttributes.emotion.sadness;
                boldness = tmpi;
                linecolor = Color.Aquamarine;
                f.mainEmotion = "sadness";
            }
            tmpi = getBoldness(f.faceAttributes.emotion.surprise);
            if (tmpi >= boldness && tmpd < f.faceAttributes.emotion.surprise)
            {
                tmpd = f.faceAttributes.emotion.surprise;
                boldness = tmpi;
                linecolor = Color.DarkKhaki;
                f.mainEmotion = "surprise";
            }

            if (boldness < 1) { boldness = 1; }

            return new Pen(linecolor, boldness);
        }

        private int getBoldness(double d)
        {
            int i = Convert.ToInt32(d * 5);
            if(i==0 && d > 0) { i = 1; }
            return i;
        }
    }
}
    
