namespace AppWebBibliotecaConsumo.Data
{
    public class LibrosApi
    {
        //metodo que inicializa la comunicacion con la api

        public HttpClient Inicial()
        {
            //variable para manejar el protocolo HttpClient
            var client= new HttpClient();

            //Direccion de la api
            //client.BaseAddress = new Uri("http://apiwebkeyler.somee.com/");
            //client.BaseAddress = new Uri("http://appwebseguridadc12159.somee.com/");
            client.BaseAddress = new Uri("https://localhost:7082");
            return client;
        }

    }
}
