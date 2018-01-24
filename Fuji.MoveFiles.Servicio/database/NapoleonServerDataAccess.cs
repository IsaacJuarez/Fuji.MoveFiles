using Fuji.MoveFiles.Servicio.Extensions;
using Fuji.MoveFiles.Servicio.Feed2Service;
using System;
using System.Linq;

namespace Fuji.MoveFiles.Servicio.database
{
    public class NapoleonServerDataAccess
    {
        public static NapoleonServiceClient NapoleonDA = new NapoleonServiceClient();

        public ClienteF2CResponse getEstudiosTransmitir(int id_Sitio, string vchClaveSitio)
        {
            Log.EscribeLog("Leyendo del Sitio: " + id_Sitio.ToString());
            ClienteF2CResponse response = new ClienteF2CResponse();

            try

            {
                ClienteF2CRequest request = new ClienteF2CRequest();
                request.id_Sitio = id_Sitio;
                //request.id_SitioSpecified = true;
                request.vchClaveSitio = vchClaveSitio;            
                request.Token = Security.Encrypt(id_Sitio + "|" + vchClaveSitio);
                response = NapoleonDA.getEstudiosTransmitir(request);
                Log.EscribeLog("Archivos a enviar: " + response.lstEstudio.Length.ToString());
            }

            catch (Exception e)

            {
                response.message = e.Message;
                response.valido = false;
                Log.EscribeLog("Existe un error en getEstudiosTransmitir: " + e.Message);
            }

            return response;
        }




        public ClienteF2CResponse updateEstatusTransmitido(int intDetEstudioID, int id_Sitio, string vchClaveSitio, string path_server)
        {
            ClienteF2CResponse response = new ClienteF2CResponse();
            try
            {
                ClienteF2CRequest request = new ClienteF2CRequest();
                request.intDetEstudioID = intDetEstudioID;
                //request.intDetEstudioIDSpecified = true;
                request.vchPathServer = path_server;
                request.Token = Security.Encrypt(id_Sitio + "|" + vchClaveSitio);
                request.id_Sitio = id_Sitio;
                //request.id_SitioSpecified = true;
                request.vchClaveSitio = vchClaveSitio;
                response = NapoleonDA.updateEstatusTransmitir(request);
            }

            catch (Exception eup)
            {
                Log.EscribeLog("Existe un error en updateEstatusTransmitido:" + eup.Message);
            }
            return response;
        }

        public void setService(int id_Sitio, string vchClaveSitio)
        {
            try
            {
                ClienteF2CRequest request = new ClienteF2CRequest();
                request.id_Sitio = id_Sitio;
                //request.id_SitioSpecified = true;
                request.vchClaveSitio = vchClaveSitio;
                request.tipoServicio = 3;
                request.Token = Security.Encrypt(id_Sitio + "|" + vchClaveSitio);
                NapoleonDA.setService(request);
            }
            catch (Exception eSS)
            {
                Log.EscribeLog("Existe un error en setService: " + eSS.Message);
                //throw eSS;
            }
        }
    }
}
