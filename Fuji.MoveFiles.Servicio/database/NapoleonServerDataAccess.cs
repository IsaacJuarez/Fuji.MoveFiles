using Fuji.MoveFiles.Servicio.Extensions;
using Fuji.MoveFiles.Servicio.Feed2Service;
using System;

namespace Fuji.MoveFiles.Servicio.database
{
    public class NapoleonServerDataAccess
    {
        public static NapoleonServiceClient NapoleonDA = new NapoleonServiceClient();

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
