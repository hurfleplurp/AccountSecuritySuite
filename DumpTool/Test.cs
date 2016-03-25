using System;

using System.Collections.Generic;

using System.Text;

using System.Net;

using System.IO;

using System.Collections.Specialized;



class PosT

{
    public void POST()
    {

        string url = "http://testing.electroquimica.cl/login"; //mediciones/subir_archivo";

        HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(url);

        tokenRequest.CookieContainer = new CookieContainer();

        HttpWebResponse tokenResponse = (HttpWebResponse)tokenRequest.GetResponse();

        String token = tokenResponse.Cookies["csrftoken"].ToString().Split('=')[1];



        HttpWebRequest loginRequest = (HttpWebRequest)WebRequest.Create(url);

        loginRequest.Method = "post";

        loginRequest.CookieContainer = new CookieContainer();

        loginRequest.ContentType = "application/x-www-form-urlencoded";



        string tempEmail = "arojas";

        string tempPass = "andrea.rojas";



        loginRequest.CookieContainer.Add(new Uri("http://testing.electroquimica.cl"), tokenResponse.Cookies);

        //  string dd = string(new Uri("http://testing.electroquimica.cl"), tokenResponse.Cookies));

        String postData = "csrfmiddlewaretoken=" + token;

        postData += "&username=" + tempEmail;

        postData += "&password=" + tempPass;



        byte[] data = Encoding.ASCII.GetBytes(postData);

        loginRequest.ContentLength = data.Length; // +1;

        loginRequest.Timeout = 3000;



        //String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(tempEmail + ":" + tempPass));

        loginRequest.Headers.Add("Origin", "http://testing.electroquimica.cl");



        loginRequest.GetRequestStream().Write(data, 0, data.Length);

        loginRequest.PreAuthenticate = true;



        //loginRequest.Headers.Add("Accept-Language", "en-us");



        try
        {

            HttpWebResponse response = (HttpWebResponse)loginRequest.GetResponse();


            if (response.StatusCode == HttpStatusCode.OK)
            {

                string url1 = "http://testing.electroquimica.cl/mediciones/subir_archivo"; //mediciones/subir_archivo";

                HttpWebRequest tokenRequest1 = (HttpWebRequest)WebRequest.Create(url1);

                tokenRequest1.CookieContainer = new CookieContainer();

                HttpWebResponse tokenResponse1 = (HttpWebResponse)tokenRequest1.GetResponse();

                String token1 = tokenResponse1.Cookies["csrftoken"].ToString().Split('=')[1];



                HttpWebRequest subirrequest = (HttpWebRequest)WebRequest.Create(url1);

                subirrequest.Method = WebRequestMethods.Http.Post;

                subirrequest.CookieContainer = new CookieContainer();

                string boundaryString = "------WebKitFormBoundaryizR2U9c55ZBosjVC";

                subirrequest.ContentType = "multipart/form-data; boundary=" + boundaryString;

                subirrequest.Headers.Add("Origin", "http://testing.electroquimica.cl");

                subirrequest.KeepAlive = true; //false

                subirrequest.Credentials = System.Net.CredentialCache.DefaultCredentials;



                subirrequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

                subirrequest.UserAgent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.114 Safari/537.36";

                subirrequest.Headers.Add("Accept-Encoding: gzip,deflate,sdch");

                subirrequest.Headers.Add("Accept-Language: es-ES,es;q=0.8");

                subirrequest.Headers.Add("Upgrade-Insecure-Requests: 1");



                subirrequest.PreAuthenticate = true;

                //request.Credentials = CredentialCache.DefaultNetworkCredentials;

                subirrequest.CookieContainer.Add(new Uri("http://testing.electroquimica.cl/mediciones/subir_archivo"), tokenResponse1.Cookies);

                string fileUrl = @"C:\\Desarrollo\\Archivos\\Extraccion\\Datos  23-03-2016 al 24-03-2016.xlsx";



                MemoryStream postDataStream = new MemoryStream();

                StreamWriter postDataWriter = new StreamWriter(postDataStream);

                // Include the file in the post data

                postDataWriter.Write("--" + boundaryString + "");

                postDataWriter.Write("\r\nContent-Disposition: form-data;"

                                       + " name=\"{0}\"\r\n\r\n{1}",

                                       "csrfmiddlewaretoken",

                                       token1);



                postDataWriter.Write("\r\n--" + boundaryString + "");

                postDataWriter.Write("\r\nContent-Disposition: form-data;"

                                       + " name=\"{0}\";"

                                        + " filename=\"{1}\""

                                        + "\r\nContent-Type: {2}\r\n\r\n",

                                        "file",

                                        Path.GetFileName(fileUrl),

                                        Path.GetExtension(fileUrl));

                postDataWriter.Flush();
                
                //  postDataStream.WriteTo(postDataStream);

                
                FileStream fileStream = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);

                byte[] buffer = new byte[1024];

                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    postDataStream.Write(buffer, 0, bytesRead);
                }

                fileStream.Close();



                // string boundaryString = "----SomeRandomText";

                postDataWriter.Write("\r\n--" + boundaryString + "--\r\n");

                postDataWriter.Flush();



                // Set the http request body content length

                subirrequest.ContentLength = postDataStream.Length;

                //subirrequest.ContentLength = postDataStream.Length;



                // Dump the post data from the memory stream to the request stream

                using (Stream s = subirrequest.GetRequestStream())
                {
                    postDataStream.WriteTo(s);
                }

                postDataStream.Close();
            }
            else
            {

                string hh = "no conectar";

                //  serverMessenger.SendErrorMessage(0);

                //Debug.LogError("Cannot Find User. TryToLogin finished");

            }
        }
        catch (WebException e)
        {
            using (WebResponse response = e.Response)
            {

                HttpWebResponse httpResponse = (HttpWebResponse)response;

                string jaja = "Error code: " + httpResponse.StatusCode;
            }

        }
    }
}