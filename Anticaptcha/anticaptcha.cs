using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Anticaptcha
{
    public class anticaptcha
    {
        public string soft_id = "e5c979564";
        public string key;
        public int phrase = 0;
        public int is_russian = 1;
        public int numeric = 0;
        public int calc = 0;
        public int regsense = 0;
        public int min_len = 0;
        public int max_len = 0;
        public string label;

        public anticaptcha(string key)
        {
            this.key = key;
        }

        #region GET/POST
        public static string GetPage(string url)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var responseReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                {
                    return responseReader.ReadToEnd();
                }
            }
        }

        public static string PostPage(string url, string postData)
        {
            byte[] postContentData = Encoding.GetEncoding("windows-1251").GetBytes(postData);
            if (postContentData.Length > 0)
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Proxy = new WebProxy();
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                using (Stream stream = req.GetRequestStream())
                {
                    stream.Write(postContentData, 0, postContentData.Length);
                    stream.Close();
                }

                using (var response = (HttpWebResponse)req.GetResponse())
                {
                    using (var responseReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                    {
                        return responseReader.ReadToEnd();
                    }
                }
            }
            return null;
        }
        #endregion

        #region Data Convertation
        public static byte[] ImageToBytes(Image imageIn, System.Drawing.Imaging.ImageFormat imgFormat)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imgFormat);
                return ms.ToArray();
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return System.Drawing.Image.FromStream(ms, true);
            }
        }

        public static string ImageToBase64(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                return Convert.ToBase64String(imageBytes);
            }
        }

        public static byte[] Base64ToBytes(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            return imageBytes;
        }

        public static System.Drawing.Image BytesToImage(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn);
            return System.Drawing.Image.FromStream(ms);
        }
        #endregion

        #region Image Format Methods
        public static string ToMIME(string extension)
        {
            switch (extension.ToLower().Replace(".", ""))
            {
                case "jpg": return "image/jpg";
                case "jpeg": return "image/jpeg";
                case "gif": return "image/gif";
                case "png": return "image/png";
                case "bmp": return "image/bmp";
                case "tiff": return "image/tiff";
                default: return extension;
            }
        }

        public static ImageFormat DetectImageFormat(string fmt)
        {
            switch (fmt.ToLower().Replace(".", ""))
            {
                case "image/jpg": return ImageFormat.Jpeg;
                case "image/jpeg": return ImageFormat.Jpeg;
                case "image/gif": return ImageFormat.Gif;
                case "image/png": return ImageFormat.Png;
                case "image/bmp": return ImageFormat.Bmp;
                case "image/tiff": return ImageFormat.Tiff;
                case "jpg": return ImageFormat.Jpeg;
                case "jpeg": return ImageFormat.Jpeg;
                case "gif": return ImageFormat.Gif;
                case "png": return ImageFormat.Png;
                case "bmp": return ImageFormat.Bmp;
                case "tiff": return ImageFormat.Tiff;
                default: return null;
            }
        }

        public static ImageFormat DetectImageFormat(Image image)
        {
            if (ImageFormat.Jpeg.Equals(image.RawFormat))
            {
                return ImageFormat.Jpeg;
            }
            else if (ImageFormat.Png.Equals(image.RawFormat))
            {
                return ImageFormat.Png;
            }
            else if (ImageFormat.Gif.Equals(image.RawFormat))
            {
                return ImageFormat.Gif;
            }
            else if (ImageFormat.Bmp.Equals(image.RawFormat))
            {
                return ImageFormat.Bmp;
            }
            else if (ImageFormat.Tiff.Equals(image.RawFormat))
            {
                return ImageFormat.Tiff;
            }
            else
                return ImageFormat.Jpeg;
        }

        public string GetExtension(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                ImageFormat fmt = DetectImageFormat(image);
                return fmt.ToString().ToLower().Replace("jpeg", "jpg");
            }
        }
        #endregion

        #region Image Downloaders
        public byte[] DownloadImage(string imgURL, out ImageFormat imageFormat)
        {
            imageFormat = null;

            var req = (HttpWebRequest)HttpWebRequest.Create(imgURL);
            req.UserAgent =
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.04506.590; .NET CLR 3.5.20706)";
            req.Accept = "*/*";
            req.Headers.Add("Accept-Language", "ru");
            req.Proxy = new WebProxy();
            req.KeepAlive = true;
            req.AllowAutoRedirect = false;
            req.Method = "GET";

            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                using (Stream stream = resp.GetResponseStream())
                {
                    if (stream != null)
                    {
                        Image image = Image.FromStream(stream);
                        imageFormat = DetectImageFormat(image);
                        return ImageToBytes(image, imageFormat);
                    }
                }
            }
            return new byte[] { };
        }

        public byte[] DownloadImage(string imgURL)
        {
            var req = (HttpWebRequest)WebRequest.Create(imgURL);
            req.Proxy = new WebProxy();
            req.AllowAutoRedirect = true;
            req.Method = "GET";
            req.UserAgent =
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.04506.590; .NET CLR 3.5.20706)";

            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                using (var stream = resp.GetResponseStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        if (stream != null) stream.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
        }
        #endregion

        #region Pixodrom Image Uploaders
        public string Upload(byte[] imgbytes)
        {
            string sBoundary = DateTime.Now.Ticks.ToString("x");

            var req = (HttpWebRequest)HttpWebRequest.Create("http://pixodrom.com/in.php");
            req.UserAgent = "Mozilla";
            req.Accept = "*/*";
            req.Headers.Add("Accept-Language", "ru");
            req.Proxy = new WebProxy();
            req.KeepAlive = true;
            req.AllowAutoRedirect = false;
            req.Method = "POST";
            req.ContentType = String.Format("multipart/form-data; boundary={0}", sBoundary);

            var sPostMultiString = new StringBuilder();
            sPostMultiString.Append(PostData.MultiFormData("method", "post", sBoundary));
            sPostMultiString.Append(PostData.MultiFormData("key", key, sBoundary));
            //sPostMultiString.Append(PostData.MultiFormData("file", filename, sBoundary));
            sPostMultiString.Append(PostData.MultiFormData("calc", calc.ToString(), sBoundary));
            sPostMultiString.Append(PostData.MultiFormData("numeric", numeric.ToString(), sBoundary));
            sPostMultiString.Append(PostData.MultiFormData("phrase", phrase.ToString(), sBoundary));
            sPostMultiString.Append(PostData.MultiFormData("minlen", min_len.ToString(), sBoundary));
            sPostMultiString.Append(PostData.MultiFormData("maxlen", max_len.ToString(), sBoundary));
            sPostMultiString.Append(PostData.MultiFormData("is_russian", is_russian.ToString(), sBoundary));
            sPostMultiString.Append(PostData.MultiFormData("label", label, sBoundary));
            sPostMultiString.Append(PostData.MultiFormData("soft_id", soft_id, sBoundary));

            string sFileContent = Encoding.Default.GetString(imgbytes);

            sPostMultiString.Append(PostData.MultiFormDataFile("file", sFileContent, "captcha", "", sBoundary));
            sPostMultiString.Append(String.Format("--{0}--\r\n\r\n", sBoundary));

            byte[] byteArray = Encoding.Default.GetBytes(sPostMultiString.ToString());
            req.ContentLength = byteArray.Length;
            req.GetRequestStream().Write(byteArray, 0, byteArray.Length);

            using (var response = (HttpWebResponse)req.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.Default))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public string UploadImage(Image image)
        {
            return Upload(ImageToBytes(image, DetectImageFormat(image)));
        }

        public string UploadFile(string filename)
        {
            return Upload(File.ReadAllBytes(filename));
        }

        public string UploadURL(string imgURL)
        {
            return Upload(DownloadImage(imgURL));
        }

        public string UploadBase64Bytes(byte[] imgbytes)
        {
            return UploadBase64(Convert.ToBase64String(imgbytes));
        }

        public string UploadBase64(string base64)
        {
            var sPostString = new StringBuilder();
            sPostString.Append("method=base64");
            sPostString.Append(String.Format("&key={0}", this.key));
            sPostString.Append(String.Format("&is_russian={0}", is_russian));
            sPostString.Append(String.Format("&phrase={0}", phrase));
            sPostString.Append(String.Format("&regsense={0}", regsense));
            sPostString.Append(String.Format("&numeric={0}", numeric));
            sPostString.Append(String.Format("&calc={0}", calc));
            sPostString.Append(String.Format("&min_len={0}", min_len));
            sPostString.Append(String.Format("&max_len={0}", max_len));
            sPostString.Append(String.Format("&soft_id={0}", soft_id));
            sPostString.Append(String.Format("&body={0}", base64));

            string postData = sPostString.ToString();

            byte[] postContentData = Encoding.GetEncoding("windows-1251").GetBytes(postData);
            if (postContentData.Length > 0)
            {
                var req = (HttpWebRequest)WebRequest.Create("http://pixodrom.com/in.php");
                req.Proxy = new WebProxy();
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                using (Stream stream = req.GetRequestStream())
                {
                    stream.Write(postContentData, 0, postContentData.Length);
                    stream.Close();
                }

                using (var response = (HttpWebResponse)req.GetResponse())
                {
                    using (var responseReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("windows-1251")))
                    {
                        return responseReader.ReadToEnd();
                    }
                }
            }
            else return "ERROR_IMAGE_UPLOADING";
        }

        public string UploadImageBase64(Image image)
        {
            return UploadBase64Bytes(ImageToBytes(image, DetectImageFormat(image)));
        }

        public string UploadFileBase64(string filename)
        {
            return UploadBase64Bytes(File.ReadAllBytes(filename));
        }

        public string UploadURLBase64(string url)
        {
            return UploadBase64Bytes(DownloadImage(url));
        }
        #endregion

        #region Pixodrom Methods
        public string GetBalance()
        {
            return GetPage(String.Format("http://pixodrom.com/res.php?key={0}&action=getbalance", key));
        }

        public string GetResult(string capid)
        {
            return GetPage(String.Format("http://pixodrom.com/res.php?key={0}&action=get&id={1}", key, capid));
        }

        public string GetResults(string ids, bool isPost = false)
        {
            if (!isPost)
                return GetPage(String.Format("http://pixodrom.com}/res.php?key={0}&action=get&ids={1}", key, ids));
            else
                return PostPage("http://pixodrom.com/res.php",
                    String.Format("key={0}&action=get&ids={1}", key, ids));
        }

        public void ReportBad(string capid)
        {
            GetPage(String.Format("http://pixodrom.com/res.php?key={0}&action=reportbad&id={1}", key, capid));
        }
        #endregion
    }

    public class PostData
    {
        private readonly string _sMethod = String.Empty;
        private readonly string _sAction = String.Empty;
        private readonly string _sParam = String.Empty;

        public string Method { get { return this._sMethod; } }
        public string Action { get { return this._sAction; } }
        public string Param { get { return this._sParam; } }

        public PostData(string sPostString)
        {
            if (sPostString != null && sPostString.IndexOf("=", System.StringComparison.Ordinal) != -1)
            {
                this._sMethod = sPostString.Substring(0, sPostString.IndexOf("=", System.StringComparison.Ordinal));
                this._sAction = sPostString.Substring(sPostString.IndexOf("=", System.StringComparison.Ordinal) + 1);
                if (this._sAction.IndexOf("!", System.StringComparison.Ordinal) != -1)
                {
                    this._sAction = _sAction.Substring(0, this._sAction.IndexOf("!", System.StringComparison.Ordinal));
                    this._sParam = sPostString.Substring(sPostString.IndexOf("!", System.StringComparison.Ordinal) + 1);
                }
            }
        }

        public static string MultiFormData(string key, string value, string boundary)
        {
            var output = new StringBuilder(String.Format("--{0}\r\n", boundary));
            output.Append(String.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n", key));
            output.Append(String.Format("{0}\r\n", value));
            return output.ToString();
        }

        public static string MultiFormDataFile(string key, string value, string fileName, string fileType, string boundary)
        {
            var output = new StringBuilder(String.Format("--{0}\r\n", boundary));
            output.Append(String.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n", key, fileName));
            output.Append(String.Format("Content-Type: {0} \r\n\r\n", fileType));
            output.Append(String.Format("{0}\r\n", value));
            return output.ToString();
        }
    }

}
