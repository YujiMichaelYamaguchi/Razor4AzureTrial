using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Razor4AzureTrial.Models;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Razor4AzureTrial.Pages
{
    public class CVisionModel : PageModel
    {
        [BindProperty]
        public string base64txt { get; set; }

        public string selectedImage { get; set; } = string.Empty;

        public FreehandRecognitionResult recogResult { get; set; }

        public void OnGet()
        {

        }

        public void OnPost()
        {

            const string subscriptionKey = "0c648c1aec4b4ae087c88aa5c2b8f9e5";
            const string endpoint = "https://westus2.api.cognitive.microsoft.com/";

            // the Batch Read method endpoint
            const string uriBase = endpoint + "vision/v2.0/ocr";

            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            
            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?language=unk&detectOrientation=true";

            HttpResponseMessage response;
            Task<HttpResponseMessage> postTask;

            string convert = base64txt.Remove(0, base64txt.IndexOf(",") + 1);

            byte[] byteData = Convert.FromBase64String(convert);

            string contentStr;

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                postTask = client.PostAsync(uri, content);
                postTask.Wait();

                // Get the JSON response.
                response = postTask.Result;
                Task<string> contentStrTask = response.Content.ReadAsStringAsync();
                contentStr = contentStrTask.Result;

            }

            recogResult = JsonConvert.DeserializeObject<FreehandRecognitionResult>(contentStr);
            createSentence();

            drawRect(byteData);

        }

        private void drawRect(byte[] bData)
        {
            Random rnd = new System.Random();
            using (MemoryStream ms1 = new MemoryStream(bData))
            {
                using (Bitmap tmpbmp = new Bitmap(ms1))
                {
                    Bitmap bmp = new Bitmap(tmpbmp.Width, tmpbmp.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(tmpbmp, 0, 0);
                        foreach (var region in recogResult.regions)
                        {
                            foreach(var line in region.lines)
                            {
                                int R = rnd.Next(255);
                                int G = rnd.Next(255);
                                int B = rnd.Next(255);
                                Pen p = new Pen(Color.FromArgb(R, G, B));
                                string[] arr = line.boundingbox.Split(",");
                                g.DrawRectangle(p,int.Parse(arr[0]),int.Parse(arr[1]),int.Parse(arr[2]),int.Parse(arr[3]));
                                p.Dispose();
                            }   
                        }

                        MemoryStream ms2 = new MemoryStream();
                        bmp.Save(ms2, ImageFormat.Png);

                        selectedImage = FaceModel.base64PrefixPNG + Convert.ToBase64String(ms2.GetBuffer());

                        ms2.Close();
                        ms2.Dispose();

                    }
                    bmp.Dispose();
                }
            }
        }

        
        private void createSentence()
        {
            foreach(var region in recogResult.regions)
            {
                foreach(var line in region.lines)
                {
                    string str = string.Empty;
                    foreach(var word in line.words)
                    {
                        str += word.text;
                    }
                    line.sentence = str;
                }
            }
        }
    }
}