using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Download;

namespace DriveSimple
{
    public partial class Form1 : Form
    {
        class deFile
        {
            public string Id { get; set; }
            public string name { get; set; }
            public long? size{ get; set; }
        }

        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "Drive API .NET Quickstart";
        DriveService service;
        IList<Google.Apis.Drive.v3.Data.File> files;
        deFile selectedFile = null;
        private List<deFile> deList = new List<deFile>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
           // listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name,size)";
            


            // List files.
            files = listRequest.Execute().Files;
          /*  dataGridView1.DataSource = files;
            dataGridView1.Columns["IconLink"]
            dataGridView1.Columns["AppProperties"]
            dataGridView1.Columns["Capabilities"]
            dataGridView1.Columns["ContentHints"]
            dataGridView1.Columns["CreatedTime"]
            dataGridView1.Columns["CreatedTimeRaw"]
            dataGridView1.Columns["Description"]
            dataGridView1.Columns["ETag"]
            dataGridView1.Columns["ExplicitlyTrashed"]
            dataGridView1.Columns["FileExtension"]
            dataGridView1.Columns["FolderColorRgb"]
            dataGridView1.Columns["FullFileExtension"]
            dataGridView1.Columns["HeadRevisionId"]
            dataGridView1.Columns["ImageMediaMetadata"]
            dataGridView1.Columns["Kind"]
            dataGridView1.Columns["LastModifyingUser"]
            dataGridView1.Columns["Md5Checksum"]
            dataGridView1.Columns["MimeType"]
            dataGridView1.Columns["ModifiedByMeTime"]
            dataGridView1.Columns["ModifiedByMeTimeRaw"]
            dataGridView1.Columns["ModifiedTime"]
            dataGridView1.Columns["ModifiedTimeRaw"]
            dataGridView1.Columns["OwnedByMe"]
            dataGridView1.Columns["Owners"]
            dataGridView1.Columns["Permissions"]
            dataGridView1.Columns["ModifiedByMeTimeRaw"]
            dataGridView1.Columns["ModifiedByMeTimeRaw"]
            */
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {

                    deFile df = new deFile();
                    df.Id = file.Id;
                    df.name = file.Name;
                    df.size = file.Size;
                    deList.Add(df);
                    //Console.WriteLine("{0} ({1}) ({2}) ", file.Name, file.Id ,file.Size.ToString());
                }
            }
            dataGridView1.DataSource = deList;
           /* else
            {
                Console.WriteLine("No files found.");
            }*/


            //Console.Read();
        }
        private void indir()
        {
            button2.Enabled = false;

            var fileId = selectedFile.Id;
            var request = service.Files.Get(fileId);
            var stream = new System.IO.MemoryStream();
         
            request.MediaDownloader.ProgressChanged +=
    (IDownloadProgress progress) =>
    {
        switch (progress.Status)
        {
            case DownloadStatus.Downloading:
                {
                    label1.Text = "Downloading...";
               
                    Application.DoEvents();
                    break;
                }
            case DownloadStatus.Completed:
                {
                    label1.Text = "Completed...";
                    break;
                }
            case DownloadStatus.Failed:
                {
                    label1.Text = "Download Eror...";
                    break;
                }
        }
    };

            request.Download(stream);
            FileStream fs = new FileStream(selectedFile.name, FileMode.OpenOrCreate);
            stream.WriteTo(fs);
            fs.Close();
            stream.Close();
            button2.Enabled = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
        
            label1.Text = "";

            if (selectedFile != null) { 
            Thread thread1 = new Thread(new ThreadStart(indir));
            thread1.Start();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
           selectedFile =    (deFile)dataGridView1.CurrentRow.DataBoundItem;
         
        }
    }
}
