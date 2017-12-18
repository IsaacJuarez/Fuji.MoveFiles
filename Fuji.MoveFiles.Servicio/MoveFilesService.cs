using Fuji.MoveFiles.Servicio.database;
using Fuji.MoveFiles.Servicio.Entidades;
using Fuji.MoveFiles.Servicio.Extensions;
using Fuji.MoveFiles.Servicio.Feed2Service;
using Fuji.MoveFiles.Servicio.ServicioMueveEstudio;
using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace Fuji.MoveFiles.Servicio
{
    partial class MoveFilesService : ServiceBase
    {
        private Timer MoveFileTimer = new Timer();
        private NapoleonServerDataAccess NapSerDA = new NapoleonServerDataAccess();
        public static int id_Servicio = 0;
        public static string vchClaveSitio = "";
        public static string AETitle = "";
        public static string vchPathRep = "";
        public static string path = "";
        public static Entidades.clsConfiguracion _conf;

        public MoveFilesService()
        {
            try
            {
                try
                {
                    path = ConfigurationManager.AppSettings["ConfigDirectory"] != null ? ConfigurationManager.AppSettings["ConfigDirectory"].ToString() : "";
                }
                catch (Exception ePath)
                {
                    path = "";
                    Log.EscribeLog("Error al obtener el path desde appSettings: " + ePath.Message);
                }
                if (File.Exists(path + "info.xml"))
                {
                    _conf = XMLConfigurator.getXMLfile();
                    id_Servicio = _conf.id_Sitio;
                    AETitle = _conf.vchAETitle;
                    vchPathRep = _conf.vchPathLocal;
                    vchClaveSitio = _conf.vchClaveSitio;
                }
                cargaServicio();
            }
            catch (Exception eSync)
            {
                Log.EscribeLog("Existe un error en MoveFilesService: " + eSync.Message);
            }
        }

        private void cargaServicio()
        {
            try
            {
                Console.WriteLine("Se cargó correctamente el servicio MoveFilesService. " + "[" + DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString() + "]");
                int segundosPoleo;
                try
                {
                    segundosPoleo = ConfigurationManager.AppSettings["segundosPoleo"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["segundosPoleo"].ToString()) : 1;
                }
                catch (Exception eGPOLeo)
                {
                    Log.EscribeLog("Existe un error al obtener el tiempo para el poleo del servicio: " + eGPOLeo.Message);
                    segundosPoleo = 60;
                }
                int minutos = (int)(1000 * segundosPoleo);
                MoveFileTimer.Elapsed += new System.Timers.ElapsedEventHandler(MoveFileTimer_Elapsed);
                MoveFileTimer.Interval = minutos;
                MoveFileTimer.Enabled = true;
                MoveFileTimer.Start();
                try
                {
                    NapSerDA.setService(id_Servicio, vchClaveSitio);
                }
                catch (Exception e)
                {
                    Log.EscribeLog("Existe un error en setService: " + e.Message);
                }
            }
            catch (Exception ecS)
            {
                Log.EscribeLog("Existe un error en cargaServicio MoveFilesService: " + ecS.Message);
            }
        }

        private void MoveFileTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                NapoleonServerDataAccess NSDA = new NapoleonServerDataAccess();
                ClienteF2CResponse lista_estudios = NSDA.getEstudiosTransmitir(id_Servicio, vchClaveSitio);

                foreach (var ID in lista_estudios.lstEstudio)
                {

                    //string filename = @"C:\Anonymized20171205.dcm";

                    string filename = ID.vchPathFile;

                    String strFile = System.IO.Path.GetFileName(filename);
                    FileInfo fInfo = new FileInfo(filename);
                    long numBytes = fInfo.Length;
                    double dLen = Convert.ToDouble(fInfo.Length / 1000000);

                    FileStream fStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fStream);

                    // convert the file to a byte array 
                    byte[] data = br.ReadBytes((int)numBytes);
                    br.Close();
                    ServicioMueveEstudio.Service1Client SM = new ServicioMueveEstudio.Service1Client();
                    string path_mover = id_Servicio + @"\" + DateTime.Now.ToString("ddMMyyyy") + @"\";
                    string path_mover_completo = id_Servicio + @"\" + DateTime.Now.ToString("ddMMyyyy") + @"\" + Path.GetFileName(filename);
                    CArchivo request = new CArchivo();
                    request.bytArchivo = data;
                    request.id_Sitio = id_Servicio;
                    request.vchpath = path_mover;
                    request.Token = Security.Encrypt(id_Servicio + "|" + vchClaveSitio);
                    request.vchClaveSitio = vchClaveSitio;
                    request.vchfilename = strFile;
                    string sTmp = SM.Carga_Archivo_F33D2(request); 
                    fStream.Close();
                    fStream.Dispose();

                    if (sTmp == "OK")
                    {
                        NSDA.updateEstatusTransmitido(ID.intDetEstudioID, id_Servicio, vchClaveSitio, path_mover_completo);
                    }
                    else
                    {
                        Log.EscribeLog("NO SE LOGRÓ HACER EL ENVÍO DEL ARCHIVO: " + filename + " , error: " + sTmp);
                    }

                }
            }
            catch (Exception eSYTi)
            {
                Log.EscribeLog("Existe un error en MoveFileTimer_Elapsed: " + eSYTi.Message);
            }
        }


        protected override void OnStart(string[] args)
        {
            // TODO: agregar código aquí para iniciar el servicio.
        }

        protected override void OnStop()
        {
            // TODO: agregar código aquí para realizar cualquier anulación necesaria para detener el servicio.
        }
    }
}
