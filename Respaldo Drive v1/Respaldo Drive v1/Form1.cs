using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Google.Apis.Drive.v2;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Drive.v2.Data;
using System.Collections;
using System.IO;

namespace Respaldo_Drive_v1
{
    public partial class Form1 : Form
    {
        string prueba;
        static string[] Scopes = { DriveService.Scope.DriveFile };
        static string ApplicationName = "Drive API .NET Quickstart";
        static DriveService service;
        public Form1()
        {
            InitializeComponent();
        }


        // paso 1 Carga del programa
        private void Form1_Load(object sender, EventArgs e)
        {
            login();
            //PrintFilesInFolder(service, "root");
             createDirectory(service, "sub carpeta","segunda prueba","0B7shiVIBuSJyVnNjYXQ0RDI1NGM");



        }

        // Paso 2 Inicio de sesion con las credenciales en google
        private static void login()
        {

            UserCredential credential;

            using (var stream =
                new FileStream("client_secret_167485110667-2cst7mjnk8kh43gpedpvn45jmlb5rnuf.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read))
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

            // Paso 3  Create Drive API service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }


        //Crea un directorio en la nube
        public static Google.Apis.Drive.v2.Data.File createDirectory(DriveService _service, string _title, string _description, string _parent)
        {
            Google.Apis.Drive.v2.Data.File NewDirectory = null;
            // Create metaData for a new Directory
            Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
            body.Title = _title;
            body.Description = _description;
            body.MimeType = "application/vnd.google-apps.folder";
            body.Parents = new List<ParentReference>() { new ParentReference() { Id = _parent } };

            try
            {
                FilesResource.InsertRequest request = _service.Files.Insert(body);
                NewDirectory = request.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            Console.Read();
            return NewDirectory;
        }


        //obtiene el tipo de archivo que se esta tomando
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }


        //Paso 5 Carga el o los archivos a la nube
        public static Google.Apis.Drive.v2.Data.File uploadFile(DriveService _service, string _uploadFile, string _parent)
        {
            if (System.IO.File.Exists(_uploadFile))
            {
                Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
                body.Title = System.IO.Path.GetFileName(_uploadFile);
                body.Description = "Achivo subido por un programa externo";
                body.MimeType = GetMimeType(_uploadFile);
                body.Parents = new List<ParentReference>() { new ParentReference() { Id = _parent } }; // File's content. 
                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.InsertMediaUpload request = _service.Files.Insert(body, stream, GetMimeType(_uploadFile));
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("File does not exist: " + _uploadFile);
                return null;
            }
            
        }


        //Imprime los id de los archivos en la nube
        public static void PrintFilesInFolder(DriveService service, String folderId)
        {
            ChildrenResource.ListRequest request = service.Children.List(folderId);


            do
            {

                try
                {
                    ChildList children = request.Execute();


                    foreach (ChildReference child in children.Items)
                    {
                        string prueba = ("File Id: " + child.Id);


                    }


                    request.PageToken = children.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            Console.Read();
        }

        //Solo es un pinche boton
        private void btnrespaldo_Click(object sender, EventArgs e)
        {

            listado();
       
            //PrintFilesInFolder(service, "root");
        }
         
       
        //  Paso 4 Seleccionar los archivos y realizar la carga cargar
        private void listado()
        {
            listBox1.Items.Clear();
            FolderBrowserDialog d = new FolderBrowserDialog();
            DialogResult response = d.ShowDialog();
            if (response == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(d.SelectedPath);

                ListBox items = new ListBox();
                items.Items.AddRange(files);
                foreach (string file in files)
                {
                    listBox1.Items.Add(file);
                    uploadFile(service, file, "0B7shiVIBuSJyVnNjYXQ0RDI1NGM");

                }

            }

        }


    }

    
}
